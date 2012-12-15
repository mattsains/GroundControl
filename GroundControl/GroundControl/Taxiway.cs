using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GroundControl
{
    /// <summary>
    /// Holds a taxiway, which is defined as many taxiway segments
    /// </summary>
    class Taxiway
    {
        public string tag; // taxiway number (actually it's usually a letter)
        List<Vector2> points=new List<Vector2>();

        //cache of graphics properties
        private vpcind graphicsPoly;

        public Taxiway(string tag)
        {
            //initialize points here
            points.Add(new Vector2(123, 65));
            points.Add(new Vector2(178, 128));
            points.Add(new Vector2(121, 211));
            points.Add(new Vector2(52, 20));
            //</initialize>
            graphicsPoly = Display.Triangulate(points, Color.Gray);
            this.tag = tag;
        }
        public void Draw()
        {
            if (graphicsPoly!=null)
                Display.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, graphicsPoly.vertices, 0, graphicsPoly.vertices.Length, graphicsPoly.indices, 0, graphicsPoly.indices.Length / 3);
        }
    }
}
