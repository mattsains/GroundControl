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

        public static vpcind Triangulate(List<Vector2> vectors, Color colour)
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
            
            return new vpcind(vpc,sIndices);
        }
    }
    //A useful class that holds a tuple of VertexPositionColor[] and short[] for the graphics processor
    class vpcind : Tuple<VertexPositionColor[], short[]>
    {
        public vpcind(VertexPositionColor[] v, short[] i) : base(v, i) { }
        public VertexPositionColor[] vertices { get { return base.Item1; } }
        public short[] indices { get { return base.Item2; } }
    }
}
