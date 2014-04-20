using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace GroundControl
{
    class Aircraft
    {
        //The maximum speed of the aircraft
        const float speed = 2f;
        //how much to multiply the texture by
        const float scale = 1f / 3;

        Texture2D texture;
        Vector2 position;
        Vector2 velocity = new Vector2();

        Airport airport;
        //direction the aircraft is facing, in radians
        float direction = 0;

        //The nodes the aircraft must visit
        Queue<TaxiNode> queue = new Queue<TaxiNode>();

        //UI things
        public bool ShowInfo = false;

        /// <summary>
        /// Constructs a new aircraft
        /// </summary>
        /// <param name="texture">The texture used to draw the aircraft</param>
        /// <param name="startAt">What node the aircraft will begin at</param>
        /// <param name="direction">What direction (radians) the aircraft will face</param>
        public Aircraft(Texture2D texture, Airport airport, TaxiNode startAt, float direction)
        {
            this.texture = texture;
            this.airport = airport;
            this.JumpTo(startAt, direction);
        }

        /// <summary>
        /// Called during the game's update event. Controls aircraft AI.
        /// </summary>
        public void Update(float dt)
        {
            
        }

        /// <summary>
        /// Called during the game's draw event. Draws the airplane
        /// </summary>
        public void Draw()
        {

            if (ShowInfo)
            {
                List<TaxiNode> nodes = queue.ToList();
                //Draw the labels
                foreach (Tuple<Vector2, string> label in airport.PathLabels(nodes))
                    Display.DrawText(label.Item2, label.Item1,Color.Red);

                //Draw the path lines
                VertexPositionColor[] lines = new VertexPositionColor[nodes.Count+1];
                lines[0] = new VertexPositionColor(new Vector3(position, 0), Color.Red);
                for (int i = 0; i < nodes.Count; i++)
                    lines[i + 1] = new VertexPositionColor(new Vector3(nodes[i].position, 0), Color.Red);
                Display.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,lines,0,nodes.Count);

                //Draw info about the plane
                if (nodes[nodes.Count - 1].nodeType == NodeType.Gate)
                    Display.DrawText("Dest: " + nodes[nodes.Count - 1].GateTag, new Vector2(0,texture.Height * scale)+ position, Color.Red);
                else if (nodes[nodes.Count - 1].nodeType == NodeType.Runway)
                    Display.DrawText("Dest: " + nodes[nodes.Count - 1].GateTag, new Vector2(0, texture.Height * scale) + position, Color.Red);
                else
                     Display.DrawText("Dest: TWY" + airport.taxiways.GetTag(nodes[nodes.Count-2],nodes[nodes.Count - 1]), new Vector2(0, texture.Height * scale) + position, Color.Red);
            }
            Display.SpriteBatch.Draw(texture, new Rectangle((int)Display.WorldToScreen(position).X, (int)Display.WorldToScreen(position).Y, (int)(texture.Width * Display.WorldToScreenXScale*scale), (int)(texture.Height * Display.WorldToScreenYScale*scale)), new Rectangle(0, 0, texture.Width, texture.Height), Color.White, direction, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Provides a bulk interface to Queue(Taxinode tn) - lets you put in a lot of them at once
        /// </summary>
        /// <param name="tnList"></param>
        public void Queue(IEnumerable<TaxiNode> tnList)
        {
            foreach (TaxiNode tn in tnList)
                this.Queue(tn);
        }

        /// <summary>
        /// Adds a destination to the queue of points the AI aircraft will visit
        /// </summary>
        /// <param name="tn">The node to visit</param>
        public void Queue(TaxiNode tn)
        {
            if (queue.Count == 0 || queue.Peek() != tn)
                queue.Enqueue(tn);
        }

        /// <summary>
        /// Makes the aircraft forget its instructions and teleport to a place on the taxiway
        /// </summary>
        /// <param name="tn">The node to teleport to</param>
        /// <param name="direction">The direction (radians) to face</param>
        public void JumpTo(TaxiNode tn, float direction)
        {
            this.position = tn.position;
            this.direction = direction;
            queue = new Queue<TaxiNode>();//flush the queue
        }

        public bool Intersects(Vector2 v)
        {
            return
                (v.X > position.X - (texture.Width * scale) / 2 && v.X < position.X + (texture.Width * scale) / 2
                && v.Y > position.Y - (texture.Height * scale) / 2 && v.Y < position.Y + (texture.Height * scale) / 2);
        }
    }
}