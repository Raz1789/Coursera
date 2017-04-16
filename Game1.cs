using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Window Dimensions variables
        const int WindowWidth = 800;
        const int WindowHeight = 600;

        //Game Entity Lists variables
        List<Mine> mines = new List<Mine>();
        List<TeddyBear> bears = new List<TeddyBear>();
        List<Explosion> explosions = new List<Explosion>();

        //Texture2Ds variables
        Texture2D explosionTx2D;
        Texture2D mineTx2D;
        Texture2D bearTx2D;

        //Mouse Variables
        MouseState mouse;
        ButtonState previousLeftButtonPress = ButtonState.Released;

        //Support Variables
        float timeCounter = 0f;
        int spawnTime;
        Random rand;

        #endregion

        #region In-built functions
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            rand = new Random();
            SpawnTimeRandomGen();
            IsMouseVisible = true;
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

            // TODO: use this.Content to load your game content here

            explosionTx2D = Content.Load<Texture2D>(@"graphics/explosion");
            mineTx2D = Content.Load<Texture2D>(@"graphics/mine");
            bearTx2D = Content.Load<Texture2D>(@"graphics/teddybear");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            mouse = Mouse.GetState();

            //TimeCounter for bear spawn
            timeCounter += gameTime.ElapsedGameTime.Milliseconds;

            //Mine Logic
            if (mouse.LeftButton == ButtonState.Released && previousLeftButtonPress == ButtonState.Pressed)
            {
                Mine mine = new Mine(mineTx2D, mouse.X, mouse.Y);
                mines.Add(mine);
            }

            previousLeftButtonPress = mouse.LeftButton;

            //bear Spawning
            if (timeCounter > spawnTime * 1000)
            {
                timeCounter = 0f;
                TeddyBear bear = new TeddyBear(bearTx2D, VelocityRandomGen(), WindowWidth, WindowHeight);
                bears.Add(bear);
                SpawnTimeRandomGen();  
            }

            

            //Collision Detection
            for (int i = mines.Count - 1; i >=0; i--)
            {
                if (mines[i].Active) {
                    for (int j = bears.Count - 1; j >= 0; j--)
                    {
                        if (bears[j].Active && mines[i].CollisionRectangle.Intersects(bears[j].CollisionRectangle)) {
                            Explosion explosion = new Explosion(explosionTx2D, mines[i].CollisionRectangle.Center.X, mines[i].CollisionRectangle.Center.Y);
                            explosions.Add(explosion);
                            mines[i].Active = false;
                            bears[j].Active = false;
                        }
                    }
                }
            }

            //Game Entity Removal
            //Mine Removal
            for (int i = mines.Count - 1; i >= 0; i--)
            {
                if(!mines[i].Active)
                {
                    mines.RemoveAt(i);
                }
            }

            //Bear Removal
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                {
                    bears.RemoveAt(i);
                }
            }

            //Explosion Removal
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].Playing)
                {
                    explosions.RemoveAt(i);
                }
            }

            //Game Entity Updates
            //bear update
            foreach (TeddyBear bear in bears)
            {
                bear.Update(gameTime);
            }

            //Explosion Update
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            foreach (Mine mine in mines)
            {
                mine.Draw(spriteBatch);
            }
            foreach (TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }
        #endregion

        #region Support Methods

        /// <summary>
        /// Generates Random Spawn Time
        /// </summary>
        private void SpawnTimeRandomGen()
        {
            spawnTime = rand.Next(1,4);
        }

        /// <summary>
        /// Generates Random X and Y velocities
        /// </summary>
        /// <returns>Vector2 Velocity</returns>
        private Vector2 VelocityRandomGen()
        {
            return new Vector2(rand.Next(-5,5)/10f, rand.Next(-5,5) / 10f);
        }

        #endregion
    }

}
