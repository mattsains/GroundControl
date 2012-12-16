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
        float direction = 0;

        Queue<Vector2> queue=new Queue<Vector2>();
        public Aircraft(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }
        public void update()
        {
            const int threash = 10;
            if (queue.Count > 0)
            {
                Rectangle search= new Rectangle((int)position.X-16, (int)position.Y-16, (int)threash+16, (int)threash+16);

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
                    direction = (float)Math.PI/2 - (float)Math.Atan(-velocity.Y / velocity.X);
                    if (velocity.X < 0 && velocity.Y > 0)
                        //special case :(
                        direction += (float)Math.PI;
                }

            }
            else { velocity = new Vector2(); }
            this.position += velocity;
        }
        public void draw()
        {
            Display.SpriteBatch.Draw(texture, new Rectangle((int)Display.WorldToScreen(position).X, (int)Display.WorldToScreen(position).Y,32,32),new Rectangle(0,0,32,32),Color.White,direction,new Vector2(16,16),SpriteEffects.None,0);
        }
        public void Queue(Vector2 position)
        {
            queue.Enqueue(new Vector2(position.X,position.Y));
            Debug.Print("New Order: {0}", position.ToString());
        }
    }
}
