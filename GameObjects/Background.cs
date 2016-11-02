using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    /// <summary>
    /// Provides a tiled-unlimited background of a texture
    /// !Requires power of 2 texture!
    /// </summary>
    class Background : DrawableObject
    {
        /// <summary>
        /// Used background texture
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the background offset.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Defines how fast the background texture is moving (compared to camera movement so 0.5f = half the cameras speed)
        /// </summary>
        public float BackgroundSpeedFactor { get; set; }

        private GameCam _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        /// <param name="GameScene">The GameScene.</param>
        /// <param name="Camera">The camera.</param>
        /// <param name="BackgroundTexture">The background texture. Make sure this is a power of 2!!</param>
        /// <exception cref="Exception">Background texture is not power of 2!</exception>
        public Background(Scene GameScene, GameCam Camera, string BackgroundTexture)
            : base(GameScene)
        {
            Game.Components.Remove(this);
            Offset = Vector2.Zero;
            this.Texture = SceneManager.Game.Content.Load<Texture2D>(BackgroundTexture);
            if (!(((Texture.Width & (Texture.Width - 1)) == 0) || (Texture.Width & (Texture.Width - 1)) == 0))
                throw new Exception("Background texture is not power of 2!");
            _camera = Camera;
            this.DrawOrder = -9999; //Draw it very far back
            BackgroundSpeedFactor = 0.2f;
        }

        public override void Draw(GameTime gameTime)
        {
            var visible =  _camera.Visible;

            // Calculate local camera world offset
            Vector2 ScreenSize = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            Offset = (_camera.position / ScreenSize);
            Offset = new Vector2((int)Offset.X, (int)Offset.Y);
            Offset *= ScreenSize;
            Offset = _camera.position - Offset;

            // Destination rectangle on the wrapped texture
            var Destination = new Rectangle((int)(_camera.position.X * BackgroundSpeedFactor), (int)(_camera.position.Y * BackgroundSpeedFactor), (int)(ScreenSize.X), (int)(ScreenSize.Y));

            Game.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null); //Wrapping mode to wrap and no matrix
            Game.SpriteBatch.Draw(this.Texture, Game.GraphicsDevice.Viewport.Bounds, Destination, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            Game.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
