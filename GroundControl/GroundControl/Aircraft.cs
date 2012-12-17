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
        const float speed = 2f;
        Texture2D texture;
        Vector2 position;
        Vector2 velocity = new Vector2();
        float direction = 0;
        float prevdot=float.MaxValue;

        public TaxiNode destination;
        Queue<TaxiNode> queue = new Queue<TaxiNode>();

        public Aircraft(Texture2D texture, TaxiNode startAt)
        {
            this.texture = texture;
            this.JumpTo(startAt);
        }
        public void update()
        {
            if (queue.Count > 0)
            {
                Vector2 temp = queue.Peek().position - position;// vector to our destination
                Vector2 vecDirection = Vector2.Normalize(velocity);
                if (Math.Abs(Vector2.Dot(vecDirection, Vector2.Normalize(queue.Peek().position - position)) + 1) < 0.1f || (temp.X==0 && temp.Y==0))
                {
                    // We are either at our destination, or past it
                    queue.Dequeue();
                    velocity = new Vector2();
                }
                else if (velocity.X == 0 && velocity.Y == 0)
                {
                    temp.Normalize();
                    this.velocity = new Vector2(temp.X * speed, temp.Y * speed);
                    direction = (float)Math.PI / 2 - (float)Math.Atan(-velocity.Y / velocity.X);
                    if (velocity.X < 0 && velocity.Y > 0)
                        //special case :(
                        direction += (float)Math.PI;
                    if (velocity.X < 0 && velocity.Y < 0)
                        //special case #2 :((
                        direction += (float)Math.PI * (3 / 2);
                    prevdot = Vector2.Dot(position, queue.Peek().position);
                }

            }
            else { velocity = new Vector2(); }
            this.position += velocity;
        }
        public void draw()
        {
            Display.SpriteBatch.Draw(texture, new Rectangle((int)Display.WorldToScreen(position).X, (int)Display.WorldToScreen(position).Y, 32, 32), new Rectangle(0, 0, 32, 32), Color.White, direction, new Vector2(16, 16), SpriteEffects.None, 0);
        }
        //some cool overloads
        public void Queue(IEnumerable<TaxiNode> tnList)
        {
            foreach (TaxiNode tn in tnList)
                this.Queue(tn);
        }
        public void Queue(TaxiNode tn)
        {
            queue.Enqueue(tn);
            destination = tn;
            Debug.Print("New order: {0}", tn.id);
        }

        public void JumpTo(TaxiNode tn)
        {
            this.position = tn.position;
            queue = new Queue<TaxiNode>();//flush the queue
            destination = tn; //and the destination
        }
    }
}