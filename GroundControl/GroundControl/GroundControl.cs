using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Triangulator;
using System.Diagnostics;

namespace GroundControl
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GroundControl : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Airport airport;
        Aircraft airplane;

        Edge<TaxiNode> lastEdge = null;
        Stack<TaxiNode> path;
        bool lastMouse = false;
        public GroundControl()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Window.AllowUserResizing = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            airport = new Airport("airports/fape");//TODO: this should be dynamic from whatever is in /airports

            Display.Initialise(spriteBatch, GraphicsDevice,airport.width,airport.height); //Display helps us with drawing. It's static

            airplane = new Aircraft(Content.Load<Texture2D>("dumbplane"), airport.taxiways.Vertices[0]); //just start anywhere to test
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            int min = int.MaxValue;
            TaxiNode mint=airport.taxiways.Vertices[0];
            foreach (TaxiNode tn in airport.taxiways.Vertices)
            {
                int dsq=(int)Vector2.DistanceSquared(Display.WorldToScreen(tn.position),new Vector2(Mouse.GetState().X,Mouse.GetState().Y));
                if (min > dsq /*&& tn.canHold*/)
                {
                    min = dsq;
                    mint = tn;
                }
            }
            //mint is now the closest to the mouse
            path = airport.taxiways.Dijkstra(airplane.destination, mint,airplane.pendest);
            path.Push(airplane.destination);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !lastMouse)
            {
                lastMouse = true;
                Debug.Print("Clicked at: {0},{1}", Mouse.GetState().X, Mouse.GetState().Y);
                airplane.Queue(path);
            }
            else if (Mouse.GetState().LeftButton==ButtonState.Released)
            {
                lastMouse = false;
            }

            airplane.update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Display.basicEffect.CurrentTechnique.Passes[0].Apply(); //apparently does pixel shaders and stuff haha
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            airport.Draw();

            if (path.Count > 1)
            {
                VertexPositionColor[] lines = new VertexPositionColor[path.Count];
                while (path.Count > 0)
                    lines[lines.Length - path.Count] = new VertexPositionColor(new Vector3(path.Pop().position, 0), Color.Red);

                Display.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, lines, 0, lines.Length - 1);
            }
            spriteBatch.Begin();
            airplane.draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
