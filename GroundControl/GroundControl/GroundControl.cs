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
        AircraftGroup aircraft = new AircraftGroup();


        List<Aircraft> selectedAircraft=new List<Aircraft>();

        List<TaxiNode> planned = null;
        public GroundControl()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Window.AllowUserResizing = true;
            graphics.PreferMultiSampling = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            airport = new Airport("airports/fape");//TODO: this should be dynamic from whatever is in /airports

            Display.Initialise(spriteBatch, GraphicsDevice, Content.Load<SpriteFont>("sego"), airport.width, airport.height); //Display helps us with drawing. It's static

            aircraft.Add(new Aircraft(Content.Load<Texture2D>("aircraft/737"), airport, airport.taxiways.Vertices[0], 0));
            List<TaxiNode> path = airport.taxiways.Dijkstra(airport.taxiways.Vertices[0], airport.taxiways.Vertices[31],null).ToList();

            aircraft[0].Queue(path);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
             || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            planned = null;

            foreach (Aircraft a in aircraft)
                if (a.Intersects(Display.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y))))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        if (selectedAircraft.Contains(a))
                            selectedAircraft.Remove(a);
                        else selectedAircraft.Add(a);

                    a.ShowInfoColor = Color.Red;
                }
                else a.ShowInfoColor = Color.Transparent;


            aircraft.Update(gameTime.ElapsedGameTime.Ticks);

            foreach (Aircraft a in selectedAircraft)
            {
                a.ShowInfoColor = Color.Green;
                TaxiNode destination = airport.ClosestToScreenPos(Mouse.GetState().X, Mouse.GetState().Y);
                TaxiNode start;
                if (a.isBusy)
                    start=a.destination;
                else
                    start=airport.ClosestToWorldPoint(a.position);

                planned = airport.taxiways.Dijkstra(start, destination,a.pendestination).ToList();
            }

            

            Display.UpdateProjection(Mouse.GetState());
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            Display.basicEffect.CurrentTechnique.Passes[0].Apply(); //apparently does pixel shaders and stuff haha
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            airport.Draw();
            aircraft.Draw();
            if (planned!=null)
                Display.DrawRoute(planned, airport, Color.Blue);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}