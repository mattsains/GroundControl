using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
namespace GroundControl
{
    static class Display
    {
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static BasicEffect basicEffect;
        public static SpriteFont font;

        public static float left = 0;
        public static float top = 0;
        public static float bottom;
        public static float right;
        /// <summary>
        /// Sets up a world-screen projection and initializes some other stuff
        /// </summary>
        /// <param name="SpriteBatch">A SpriteBatch object to draw sprites with</param>
        /// <param name="GraphicsDevice">A GraphicsDevice object to draw primatives with</param>
        /// <param name="font">The font to write text with</param>
        /// <param name="width">The initial world width to fit in the display</param>
        /// <param name="height">The initial world width to fit in the display</param>
        public static void Initialise(SpriteBatch SpriteBatch, GraphicsDevice GraphicsDevice, SpriteFont font, float width, float height)
        {
            Display.SpriteBatch = SpriteBatch;
            Display.GraphicsDevice = GraphicsDevice;
            Display.basicEffect = new BasicEffect(GraphicsDevice);
            Display.font = font;

            basicEffect.VertexColorEnabled = true;

            bottom = height;
            right = width;
            UpdateProjection();

        }
        /// <summary>
        /// Technical function, used for turning polygons into objects the graphics card can render
        /// </summary>
        /// <param name="vectors">vectors definining the polygon</param>
        /// <param name="colour">the colour of the polygon</param>
        /// <returns>An ordered set of VertexPositionColour objects</returns>
        public static vpcInd Triangulate(List<Vector2> vectors, Color colour)
        {
            Vector2[] v = vectors.ToArray();
            int[] indices;
            Triangulator.Triangulator.Triangulate(v, Triangulator.WindingOrder.CounterClockwise, out v, out indices);

            short[] sIndices = new short[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                sIndices[i] = (short)indices[i];

            VertexPositionColor[] vpc = new VertexPositionColor[v.Length];
            for (int i = 0; i < v.Length; i++)
                vpc[i] = new VertexPositionColor(new Vector3(v[i], 0), colour);

            return new vpcInd(vpc, sIndices);
        }
        /// <summary>
        /// Translates a vector in the world to a vector on the screen
        /// </summary>
        /// <param name="orig">The vector in the world</param>
        /// <returns>The corresponding vector in the screen</returns>
        public static Vector2 WorldToScreen(Vector2 orig)
        {
            Vector3 v3 = GraphicsDevice.Viewport.Project(new Vector3(orig, 0), basicEffect.Projection, basicEffect.View, basicEffect.World);
            return new Vector2(v3.X, v3.Y);
        }
        /// <summary>
        /// Translates a vector on the screen to a vector in the world
        /// </summary>
        /// <param name="orig">The vector on the screen</param>
        /// <returns>the corresponding vector in the world</returns>
        public static Vector2 ScreenToWorld(Vector2 orig)
        {
            Vector3 v3 = GraphicsDevice.Viewport.Unproject(new Vector3(orig, 0), basicEffect.Projection, basicEffect.View, basicEffect.World);
            return new Vector2(v3.X, v3.Y);
        }

        public static void DrawText(string text, Vector2 position, Color color = default(Color))
        {
            Vector2 offset = font.MeasureString(text) / 2;
            Display.SpriteBatch.DrawString(font, text, WorldToScreen(position) - offset, color);
        }
        public static float ScreenToWorldXScale
        { get { return 1 / WorldToScreenXScale; } }
        public static float WorldToScreenXScale
        { get { return WorldToScreen(new Vector2(1, 0)).X - WorldToScreen(new Vector2(0, 0)).X; } }

        public static float ScreenToWorldYScale
        { get { return 1 / WorldToScreenYScale; } }
        public static float WorldToScreenYScale
        { get { return WorldToScreen(new Vector2(0, 1)).Y - WorldToScreen(new Vector2(0, 0)).Y; } }

        private static int lastScrollValue = 0;
        private static bool lastRightValue = false;
        private static int lastX;
        private static int lastY;

        public static void UpdateProjection(MouseState mouseState)
        {
            int Scrolldiff = lastScrollValue - mouseState.ScrollWheelValue;

            if (mouseState.RightButton == ButtonState.Pressed && GotMouse)
            {
                //panning
                if (lastX == 0 && lastY == 0)
                {
                    lastX = mouseState.X;
                    lastY = mouseState.Y;
                    lastRightValue = true;
                }
                else
                {
                    int Xdiff = mouseState.X - lastX;
                    int Ydiff = mouseState.Y - lastY;

                    left -= ScreenToWorldXScale * Xdiff;
                    right -= ScreenToWorldXScale * Xdiff;

                    top -= ScreenToWorldYScale * Ydiff;
                    bottom -= ScreenToWorldYScale * Ydiff;

                    lastX = mouseState.X;
                    lastY = mouseState.Y;
                }
            }
            else
            {
                lastX = lastY = 0;
            }
            if (Scrolldiff != 0)
            {
                top -= (float)(Scrolldiff * GraphicsDevice.Viewport.Height * ScreenToWorldYScale * 0.001);
                bottom += (float)(Scrolldiff * GraphicsDevice.Viewport.Height * ScreenToWorldYScale * 0.001);
                left -= (float)(Scrolldiff * GraphicsDevice.Viewport.Width * ScreenToWorldXScale * 0.001);
                right += (float)(Scrolldiff * GraphicsDevice.Viewport.Width * ScreenToWorldXScale * 0.001);
                lastScrollValue = mouseState.ScrollWheelValue;
            }
            if (mouseState.RightButton != ButtonState.Pressed)
                lastRightValue = false;
            UpdateProjection();
        }
        public static void UpdateProjection()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            float screenRatio = ((float)screenWidth) / screenHeight;

            if ((right - left) / screenRatio < (bottom - top))
            {
                int difference = (int)(screenRatio * (bottom - top) - (right - left));
                left -= difference / 2;
                right += difference / 2;
            }
            else if ((right - left) / screenRatio > (bottom - top))
            {
                int difference = (int)(screenRatio * (bottom - top) - (right - left));
                left -= difference / 2;
                right += difference / 2;
            }

            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (left, right,
                bottom, top,
                0, 1);       // near, far plane
        }
        public static bool GotMouse
        {
            get
            {
                MouseState m = Mouse.GetState();
                return (m.RightButton == ButtonState.Pressed && lastRightValue) || (m.X < GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width && m.X > GraphicsDevice.Viewport.X &&
                       m.Y < GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height && m.Y > GraphicsDevice.Viewport.Y);
            }
        }
    }
    //A useful class that holds a tuple of VertexPositionColor[] and short[] for the graphics processor
    class vpcInd : Tuple<VertexPositionColor[], short[]>
    {
        public vpcInd(VertexPositionColor[] v, short[] i) : base(v, i) { }
        public VertexPositionColor[] vertices { get { return base.Item1; } }
        public short[] indices { get { return base.Item2; } }
    }
}