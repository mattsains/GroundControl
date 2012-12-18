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
        Aircraft airplaneL;
        Aircraft airplaneR;

        Stack<TaxiNode> pathL;
        Stack<TaxiNode> pathR;

        List<Tuple<string,Vector2>> labelL;
        List<Tuple<string,Vector2>> labelR;

        bool lastMouseL = false;
        bool lastMouseR = false;
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

            Display.Initialise(spriteBatch, GraphicsDevice, Content.Load<SpriteFont>("sego"), airport.width, airport.height); //Display helps us with drawing. It's static

            airplaneL = new Aircraft(Content.Load<Texture2D>("dumbplane"), airport.taxiways.Vertices[0]); //just start anywhere to test
            airplaneR = new Aircraft(Content.Load<Texture2D>("dumbplane2"), airport.taxiways.Vertices[1]);
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
            //instant debugging!
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                int i = 0;
                int j = 5 / i;
            }
            // TODO: Add your update logic here
            int min = int.MaxValue;
            TaxiNode mint = airport.taxiways.Vertices[0];
            foreach (TaxiNode tn in airport.taxiways.Vertices)
            {
                int dsq = (int)Vector2.DistanceSquared(Display.WorldToScreen(tn.position), new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                if (min > dsq && (tn.canHold || tn.nodeType == NodeType.Gate))
                {
                    min = dsq;
                    mint = tn;
                }
            }
            //mint is now the closest to the mouse
            pathL = airport.taxiways.Dijkstra(airplaneL.destination, mint, airplaneL.pendest);
            pathR = airport.taxiways.Dijkstra(airplaneR.destination, mint, airplaneR.pendest);

            pathL.Push(airplaneL.destination);
            pathR.Push(airplaneR.destination);

            labelL = airport.LabelPath(pathL);
            labelR = airport.LabelPath(pathR);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !lastMouseL)
            {
                lastMouseL = true;
                Debug.Print("Clicked at: {0},{1}", Mouse.GetState().X, Mouse.GetState().Y);
                airplaneL.Queue(pathL);
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                lastMouseL = false;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !lastMouseR)
            {
                lastMouseR = true;
                Debug.Print("RClicked at: {0},{1}", Mouse.GetState().X, Mouse.GetState().Y);
                airplaneR.Queue(pathR);
            }
            else if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                lastMouseR = false;
            }

            airplaneL.update();
            airplaneR.update();
            
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
            spriteBatch.Begin();
            if (pathL.Count > 1)
            {
                VertexPositionColor[] lines = new VertexPositionColor[pathL.Count];
                while (pathL.Count > 0)
                    lines[lines.Length - pathL.Count] = new VertexPositionColor(new Vector3(pathL.Pop().position, 0), Color.Red);
                Display.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, lines, 0, lines.Length - 1);
            }
            if (pathR.Count > 1)
            {
                VertexPositionColor[] lines = new VertexPositionColor[pathR.Count];
                while (pathR.Count > 0)
                    lines[lines.Length - pathR.Count] = new VertexPositionColor(new Vector3(pathR.Pop().position, 0), Color.Blue);
                Display.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, lines, 0, lines.Length - 1);
            }

            foreach (Tuple<string, Vector2> l in labelL)
                Display.DrawText(l.Item1, Display.WorldToScreen(l.Item2), Color.Red);
            foreach (Tuple<string, Vector2> l in labelR)
                Display.DrawText(l.Item1, Display.WorldToScreen(l.Item2), Color.Blue);

            airplaneL.draw();
            airplaneR.draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}