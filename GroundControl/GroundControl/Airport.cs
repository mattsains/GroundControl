﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Media;

namespace GroundControl
{
    /// <summary>
    /// Holds a taxiway, which is defined as many taxiway segments
    /// </summary>
    class Airport
    {
        public string icao; // ICAO code of airport, eg., FAPE for PE

        List<Tuple<List<Vector2>, Color>> aprons = new List<Tuple<List<Vector2>, Color>>();//I call graphics polygons aprons
        public Graph<TaxiNode> taxiways = new Graph<TaxiNode>(); //graph of points for pathfinding

        //cache of graphics properties
        private List<vpcInd> graphicsPolys = new List<vpcInd>();
        private Color background = Color.CornflowerBlue;
        public int width, height;

        public Airport(string file)
        {
            XmlTextReader XMLAsset = new XmlTextReader(string.Format("Content/{0}.xml", file));
            while (XMLAsset.Read())
            {
                if (XMLAsset.NodeType == XmlNodeType.Element)
                {
                    if (XMLAsset.Name == "Airport")
                    {
                        this.icao = XMLAsset.GetAttribute("Icao");
                        this.width = int.Parse(XMLAsset.GetAttribute("width"));
                        this.height = int.Parse(XMLAsset.GetAttribute("height"));
                        try
                        {
                            int c = Convert.ToInt32(XMLAsset.GetAttribute("color").Substring(1), 16);
                            background.R = (byte)((c & 0xFF0000) >> 16);
                            background.G = (byte)((c & 0x00FF00) >> 8);
                            background.B = (byte)(c & 0x0000FF);
                        }
                        catch (Exception) { }
                        break;
                    }
                }
            }

            while (XMLAsset.Read())
            {
                if (XMLAsset.NodeType == XmlNodeType.Element && XMLAsset.Name == "Apron")
                {
                    List<Vector2> apron = new List<Vector2>();
                    Color color = Color.Gray;
                    try
                    {
                        int c = Convert.ToInt32(XMLAsset.GetAttribute("color").Substring(1), 16);
                        color.R = (byte)((c & 0xFF0000) >> 16);
                        color.G = (byte)((c & 0x00FF00) >> 8);
                        color.B = (byte)(c & 0x0000FF);
                    }
                    catch (Exception) { }
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
                    aprons.Add(new Tuple<List<Vector2>, Color>(apron, color));
                    continue;
                }


                if (XMLAsset.NodeType == XmlNodeType.Element && XMLAsset.Name == "Nodes")
                {

                    while (XMLAsset.Read())
                    {
                        if (XMLAsset.NodeType == XmlNodeType.EndElement) { break; }
                        if (XMLAsset.NodeType != XmlNodeType.Element) { continue; }
                        NodeType nodeType;
                        switch (XMLAsset.Name)
                        {
                            case "Runway": nodeType = NodeType.Runway; break;
                            case "Gate": nodeType = NodeType.Gate; break;
                            case "Taxiway":
                            default: nodeType = NodeType.Taxiway; break;
                        }
                        if (nodeType == NodeType.Gate)
                            taxiways.Vertices.Add(new TaxiNode(XMLAsset.GetAttribute("id"),//the gate's ID
                                                new Vector2(float.Parse(XMLAsset.GetAttribute("x")), float.Parse(XMLAsset.GetAttribute("y"))),//the position
                                                nodeType, //it's a gate
                                                XMLAsset.GetAttribute("tag")));
                        else if (nodeType == NodeType.Runway)
                            taxiways.Vertices.Add(new TaxiNode(XMLAsset.GetAttribute("id"),//the gate's ID
                                                new Vector2(float.Parse(XMLAsset.GetAttribute("x")), float.Parse(XMLAsset.GetAttribute("y"))),//the position
                                                nodeType, //it's a runway
                                                bool.Parse(XMLAsset.GetAttribute("canhold")),
                                                XMLAsset.GetAttribute("tag")));
                        else taxiways.Vertices.Add(new TaxiNode(XMLAsset.GetAttribute("id"), //the node's ID
                                                new Vector2(float.Parse(XMLAsset.GetAttribute("x")), float.Parse(XMLAsset.GetAttribute("y"))), //the position
                                                nodeType, //type
                                                bool.Parse(XMLAsset.GetAttribute("canhold"))));//whether you can hold at this point
                        //wow.
                    }
                    continue;
                }
                if (XMLAsset.NodeType == XmlNodeType.Element && XMLAsset.Name == "Edges")
                {
                    while (XMLAsset.Read())
                    {
                        if (XMLAsset.NodeType == XmlNodeType.EndElement) { break; }

                        if (XMLAsset.NodeType == XmlNodeType.Element)
                        {
                            //parse a connection
                            //we have a ton of trivial graphs which we'll slowly string together
                            string fromid = XMLAsset.GetAttribute("from");
                            string toid = XMLAsset.GetAttribute("to");
                            string tag = XMLAsset.GetAttribute("tag");

                            TaxiNode fromNode = null;
                            TaxiNode toNode = null;

                            //search for the trivial (or maybe not by this stage) graph
                            foreach (TaxiNode node in taxiways.Vertices)
                            {
                                if (node.id == fromid) fromNode = node;
                                if (node.id == toid) toNode = node;
                                if (fromNode != null && toNode != null) break; //we're done searching
                            }
                            if (fromNode.nodeType == NodeType.Runway && toNode.nodeType == NodeType.Runway)
                                taxiways.Connect(fromNode, toNode, tag, (int)((fromNode.position - toNode.position).Length() * 1.1)); //square roots are expensive!
                            else taxiways.Connect(fromNode, toNode, tag, (int)(fromNode.position - toNode.position).Length()); //square roots are expensive!
                        }

                    }
                }
            }
            //Add to the polygon cache
            foreach (Tuple<List<Vector2>, Color> apron in aprons)
                graphicsPolys.Add(Display.Triangulate(apron.Item1, apron.Item2));
        }
        
        /// <summary>
        /// Draws the entire airport environment. Draw everything else after this.
        /// </summary>
        public void Draw()
        {
            Display.GraphicsDevice.Clear(background);
            foreach (vpcInd graphicsPoly in graphicsPolys)
                Display.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, graphicsPoly.vertices, 0, graphicsPoly.vertices.Length, graphicsPoly.indices, 0, graphicsPoly.indices.Length / 3);
        }

        /// <summary>
        /// Calculates the positions of labels for any path around the airport
        /// </summary>
        /// <param name="points">the points in the route</param>
        /// <returns>Tuples representing each label and where it should be</returns>
        public List<Tuple<Vector2, string>> PathLabels(List<TaxiNode> nodes)
        {
            List<Tuple<Vector2, string>> uniqueLabels = new List<Tuple<Vector2, string>>();

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                if (uniqueLabels.Count == 0 || !uniqueLabels[uniqueLabels.Count - 1].Item2.Equals(taxiways.GetTag(nodes[i], nodes[i+1])))
                    uniqueLabels.Add(new Tuple<Vector2, string>((nodes[i].position + nodes[i+1].position)/2, taxiways.GetTag(nodes[i], nodes[i + 1])));
            }
            return uniqueLabels;
        }

        /// <summary>
        /// Returns the node closest to a point on the screen
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TaxiNode ClosestToScreenPos(int x, int y)
        {
            Vector2 worldpoint = Display.ScreenToWorld(new Vector2(x, y));

            return ClosestToWorldPoint(worldpoint);
        }
        public TaxiNode ClosestToWorldPoint(Vector2 v)
        {
            TaxiNode closest = taxiways.Vertices[0];
            float mindistance=float.MaxValue;

            foreach (TaxiNode t in taxiways.Vertices)
            {
                float distance=(t.position - v).LengthSquared();
                if (distance < mindistance)
                {
                    mindistance = distance;
                    closest = t;
                }
            }
            return closest;
        }
        /// <summary>
        /// A useful enclosure for edge labels on a graph
        /// </summary>
        class LabelInfo : IComparable<LabelInfo>
        {
            public int Weight { get; set; }
            public Vector2 Pos { get; set; }
            public string Tag { get; set; }

            public LabelInfo(int weight, Vector2 pos, string tag)
            {
                this.Weight = weight;
                this.Pos = pos;
                this.Tag = tag;
            }

            public int CompareTo(LabelInfo that)
            {
                return this.Weight - that.Weight;
            }
        }
    }
}