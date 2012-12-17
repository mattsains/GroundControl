using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GroundControl
{
    static class Display
    {
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static BasicEffect basicEffect;
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
        public static void Initialise(SpriteBatch SpriteBatch, GraphicsDevice GraphicsDevice, int width, int height)
        {
            Display.SpriteBatch = SpriteBatch;
            Display.GraphicsDevice = GraphicsDevice;
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, width,     // left, right
                height, 0,    // bottom, top
                0, 1);                                         // near, far plane

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
