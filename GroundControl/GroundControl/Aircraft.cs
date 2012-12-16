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
        const float speed = 10f;
        Texture2D texture;
        Vector2 position;
        Vector2 velocity = new Vector2();

        Queue<Vector2> queue=new Queue<Vector2>();
        public Aircraft(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }
        public void update()
        {
            const int threash = 20;
            if (queue.Count > 0)
            {
                Rectangle search;
                if (velocity.X > 0)
                {
                    if (velocity.Y > 0) search = new Rectangle((int)position.X, (int)position.Y, (int)velocity.X*threash, (int)velocity.Y*threash);
                    else search = new Rectangle((int)position.X, (int)(position.Y + velocity.X), (int)velocity.X*threash, -(int)velocity.Y*threash);
                }
                else
                {
                    if (velocity.Y > 0) search = new Rectangle((int)(position.X + velocity.X), (int)position.Y, -(int)velocity.X * threash, (int)velocity.Y * threash);
                    else search = new Rectangle((int)(position.X + velocity.X), (int)(position.Y + velocity.X), -(int)velocity.X * threash, -(int)velocity.Y * threash);
                }
                Debug.Print("new rectangle: {0}", search.ToString());
                if (search.Intersects(new Rectangle((int)queue.Peek().X, (int)queue.Peek().Y, 5, 5)))
                {
                    queue.Dequeue();
                    velocity = new Vector2();
                }
                else if (velocity.LengthSquared()==0)
                {
                    Vector2 temp = queue.Peek() - position;
                    temp.Normalize();
                    this.velocity = new Vector2(temp.X * speed, temp.Y * speed);
                }

            }
            else { velocity = new Vector2(); }
            this.position += velocity;
        }
        public void draw()
        {
            Display.SpriteBatch.Draw(texture, Display.SpaceCoords(position), Color.White);
        }
        public void Queue(Vector2 position)
        {
            queue.Enqueue(new Vector2(position.X,position.Y));
            Debug.Print("New Order: {0}", position.ToString());
        }
    }
}
