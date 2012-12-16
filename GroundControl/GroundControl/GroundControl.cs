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

        BasicEffect basicEffect;
        Airport t;
        Aircraft a;
        Texture2D tex;
        Vector2 dotpos;

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
            // TODO: Add your initialization logic here
            // I want to draw lines. Therefore start a 3D projection. lol wut
            
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
            Display.Initialise(spriteBatch, GraphicsDevice);
            // TODO: use this.Content to load your game content here
            t = new Airport("airports/fape");
            tex = Content.Load<Texture2D>("dot");
            a=new Aircraft(Content.Load<Texture2D>("dumbplane"),new Vector2());
            a.Queue(t.taxiways.Vertices[0].position);

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
            TaxiNode mint=t.taxiways.Vertices[0];
            foreach (TaxiNode tn in t.taxiways.Vertices)
            {
                int dsq=(int)Vector2.DistanceSquared(Display.SpaceCoords(tn.position),new Vector2(Mouse.GetState().X,Mouse.GetState().Y));
                if (min > dsq && tn.canHold)
                {
                    min = dsq;
                    mint = tn;
                }
            }
            dotpos = mint.position;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !lastMouse)
            {
                lastMouse = true;
                a.Queue(dotpos);
                
            }
            else if (Mouse.GetState().LeftButton==ButtonState.Released)
            {
                lastMouse = false;
            }
            dotpos.X -= 8;
            dotpos.Y -= 8;

            a.update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            Rectangle final = new Rectangle((int)dotpos.X, (int)dotpos.Y, 16, 16);
            

            Display.basicEffect.CurrentTechnique.Passes[0].Apply(); //apparently does pixel shaders and stuff haha
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            t.Draw();
            spriteBatch.Begin();
            spriteBatch.Draw(tex, Display.SpaceCoords(dotpos), Color.White);
            a.draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
