using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ODBank
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class MainGame : Game
    {
        /// <summary>
        /// Main graphics device manager
        /// </summary>
        public GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Spritebatch
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Scene manager (set property ActiveScene to change the scene)
        /// </summary>
        public SceneManager SceneManager { get; private set; }

        /// <summary>
        /// Input manager. Used to assign keys to certain actions.
        /// </summary>
        public InputManager InputManager { get; private set; }

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            // For starters only, make dynamic later using interface.
            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 600;

            this.Window.AllowAltF4 = true; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            // Setup the SceneManager
            SceneManager = new SceneManager(this);

            // Setup the InputManager
            InputManager = new InputManager();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            Graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Load first scene
            SceneManager.ActiveScene = new Scenes.TestScene();

            // Load initial keyboard layout (TODO - agent you probably need to implement that into the options menu)
            InputManager.LoadDefault();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
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
            //Update the input manager
            InputManager.Update(gameTime);

            //Update the scene
            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Draw the scene
            SceneManager.Draw(gameTime);
        }

        /// <summary>
        /// Draws the components.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void DrawComponents(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
