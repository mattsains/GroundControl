using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace GroundControl
{
    /// <summary>
    /// Holds a taxiway, which is defined as many taxiway segments
    /// </summary>
    class Airport
    {
        public string icao; // taxiway number (actually it's usually a letter)
        List<List<Vector2>> aprons=new List<List<Vector2>>();

        //cache of graphics properties
        private List<vpcind> graphicsPolys=new List<vpcind>();

        public Airport(string file)
        {
            //initialize points here
            XmlTextReader XMLAsset = new XmlTextReader(string.Format("Content/{0}.xml", file));
            while (XMLAsset.Read())
            {
                if (XMLAsset.NodeType == XmlNodeType.Element)
                {
                    if (XMLAsset.Name == "Airport")
                    {
                        this.icao = XMLAsset.GetAttribute("Icao");
                        break;
                    }
                }
            }
            while (XMLAsset.Read())
            {
                if (XMLAsset.NodeType == XmlNodeType.Element && XMLAsset.Name == "Apron")
                {
                    List<Vector2> apron=new List<Vector2>();
                    while (XMLAsset.Read())
                    {
                        if (XMLAsset.NodeType == XmlNodeType.EndElement) { break; }

                        if (XMLAsset.NodeType == XmlNodeType.Element && XMLAsset.Name == "Vertex")
                        {
                            int x = int.MaxValue;
                            int y = int.MaxValue;
                            while (XMLAsset.MoveToNextAttribute())
                            {
                                if (XMLAsset.Name == "x") { x = int.Parse(XMLAsset.Value); continue; }
                                if (XMLAsset.Name == "y") { y = int.Parse(XMLAsset.Value); continue; }
                            }
                            if (x == int.MaxValue || y == int.MaxValue) { throw (new Exception("x and y values of vertex are required")); }

                            apron.Add(new Vector2((float)x, (float)y));
                        }
                    }
                    aprons.Add(apron);
                }
            }
            //</initialize>
            foreach (List<Vector2> apron in aprons)
                graphicsPolys.Add(Display.Triangulate(apron, Color.Gray));
        }
        public void Draw()
        {
            foreach (vpcind graphicsPoly in graphicsPolys)
                Display.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, graphicsPoly.vertices, 0, graphicsPoly.vertices.Length, graphicsPoly.indices, 0, graphicsPoly.indices.Length / 3);
        }
    }
}
