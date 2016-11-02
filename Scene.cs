using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank
{
    abstract partial class Scene : IDisposable
    {
        /// <summary>
        /// Gets the currently used game
        /// </summary>
        public MainGame Game
        {
            get
            {
                return SceneManager.Game;
            }
        }

        /// <summary>
        /// The maps background
        /// </summary>
        public GameObjects.Background Background { get; protected set; }

        /// <summary>
        /// Camera for this map
        /// </summary>
        public GameObjects.GameCam Camera { get; private set; }

        /// <summary>
        /// Default shader
        /// </summary>
        public Effect Shader { get; set; }

        /// <summary>
        /// Game Components that should be drawn in front of everything
        /// </summary>
        public List<IDrawable> OverlayLayer { get; set; }

        public Scene()
        {
            this.SamplerState = SamplerState.LinearClamp; //everything shall be smooth
            ClearColor = Color.Black;
            this.OverlayLayer = new List<IDrawable>();
            this.Camera = new GameObjects.GameCam(this.Game);
        }

        /// <summary>
        /// Default font used by this scene.
        /// </summary>
        public SpriteFont DefaultFont { get; set; }

        /// <summary>
        /// Will be called by the scene manager during activation of this scene
        /// </summary>
        public virtual void LoadContent()
        {
            CreateRenderTarget(
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            Game.Window.ClientSizeChanged += Window_ClientSizeChanged;
            Shader = SceneManager.Game.Content.Load<Effect>("Shader/Default");
            PostShader = SceneManager.Game.Content.Load<Effect>("Shader/PostProcessing");
            DefaultFont = Game.Content.Load<SpriteFont>("Fonts/Default");
        }


        /// <summary>
        /// Will be called by the scene manager during update loop if this scene is active
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Gets or sets the clearcolor.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Gets or sets the default samplerState.
        /// </summary>
        public SamplerState SamplerState { get; set; }

        /// <summary>
        /// Will be called by the scene manager during draw loop if this scene is active
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Draw(GameTime gameTime)
        {
            SceneManager.Game.GraphicsDevice.SetRenderTarget(RenderTarget);
            SceneManager.Game.GraphicsDevice.Clear(ClearColor);

            //Background-Drawcall
            if (Background != null)
                Background.Draw(gameTime);


            //Main-Drawcalls
            Game.SpriteBatch.Begin(transformMatrix: Camera.View, effect: Shader, samplerState: this.SamplerState);
            SceneManager.Game.DrawComponents(gameTime);
            SceneManager.Game.SpriteBatch.End();

            //Overlay-Drawcalls
            SceneManager.Game.SpriteBatch.Begin(transformMatrix: Matrix.Identity, effect: Shader);
            foreach (var dComponent in OverlayLayer)
                dComponent.Draw(gameTime);
            SceneManager.Game.SpriteBatch.End();

            SceneManager.Game.GraphicsDevice.SetRenderTarget(null);

            //Post processing
            SceneManager.Game.SpriteBatch.Begin(effect: PostShader);

            //Set frame parameters
            SetPostShaderParameters();

            //Draw rendertarget
            SceneManager.Game.SpriteBatch.Draw((Texture2D)RenderTarget,
                new Vector2(0, 0),
                new Rectangle(0, 0, RenderTarget.Width, RenderTarget.Height),
                Color.White);

            SceneManager.Game.SpriteBatch.End();
        }

        /// <summary>
        /// Dispose method. Will be called if this scene is released from the scene manager.
        /// !This will automatically remove all components from the game! Override and remove "base.Dispose()" to disable this
        /// </summary>
        public virtual void Dispose()
        {
            //Get a copy of all components
            var components = SceneManager.Game.Components.ToList();

            //Undock all components from the game
            SceneManager.Game.Components.Clear();

            //Dispose all disposable components
            foreach (var component in components)
                if (component.GetType().IsAssignableFrom(typeof(IDisposable)))
                    ((IDisposable)component).Dispose();
        }

        protected void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            CreateRenderTarget(
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            if (Camera != null)
                Camera.Refresh();
        }
    }
}
