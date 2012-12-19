﻿using System;
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
        public static vpcInd Triangulate(List<Vector2> vectors, Color colour)
        {
            Vector2[] v = vectors.ToArray();
            int[] indices;
            Triangulator.Triangulator.Triangulate(v, Triangulator.WindingOrder.CounterClockwise, out v, out indices);

            short[] sIndices=new short[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                sIndices[i] = (short)indices[i];

            VertexPositionColor[] vpc=new VertexPositionColor[v.Length];
            for (int i = 0; i < v.Length; i++)
                vpc[i] = new VertexPositionColor(new Vector3(v[i], 0), colour);
            
            return new vpcInd(vpc,sIndices);
        }
        public static Vector2 WorldToScreen(Vector2 orig)
        {
            Vector3 v3=GraphicsDevice.Viewport.Project(new Vector3(orig, 0), basicEffect.Projection, basicEffect.View, basicEffect.World);
            return new Vector2(v3.X, v3.Y);
        }
        public static Vector2 ScreenToWorld(Vector2 orig)
        {
            Vector3 v3 = GraphicsDevice.Viewport.Unproject(new Vector3(orig, 0), basicEffect.Projection, basicEffect.View, basicEffect.World);
            return new Vector2(v3.X, v3.Y);
        }

        public static void DrawText(string text, Vector2 position,Color color=default(Color))
        {
            Vector2 offset = font.MeasureString(text) / 2;
            Display.SpriteBatch.DrawString(font, text, position-offset, color);
        }
        public static float ScreenToWorldXScale
        { get { return 1 / WorldToScreenXScale; } }
        public static float WorldToScreenXScale
        { get { return WorldToScreen(new Vector2(1,0)).X-WorldToScreen(new Vector2(0,0)).X; } }

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
            int Scrolldiff = mouseState.ScrollWheelValue - lastScrollValue;

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

                    left -= ScreenToWorldXScale*Xdiff;
                    right -= ScreenToWorldXScale*Xdiff;

                    top -= ScreenToWorldYScale*Ydiff;
                    bottom -= ScreenToWorldYScale*Ydiff;

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
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (left, right,
                bottom, top,
                0, 1);       // near, far plane
        }
        public static bool GotMouse
        { get {
                MouseState m = Mouse.GetState();
                return (m.RightButton==ButtonState.Pressed && lastRightValue) || (m.X < GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width && m.X > GraphicsDevice.Viewport.X &&
                       m.Y < GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height && m.Y > GraphicsDevice.Viewport.Y);
        } }
    }
    //A useful class that holds a tuple of VertexPositionColor[] and short[] for the graphics processor
    class vpcInd : Tuple<VertexPositionColor[], short[]>
    {
        public vpcInd(VertexPositionColor[] v, short[] i) : base(v, i) { }
        public VertexPositionColor[] vertices { get { return base.Item1; } }
        public short[] indices { get { return base.Item2; } }
    }
}
