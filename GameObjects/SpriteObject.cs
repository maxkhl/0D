using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    /// <summary>
    /// Drawable GameObject
    /// </summary>
    class SpriteObject : DrawableObject, IDrawable
    {
        /// <summary>
        /// The gameobjects Position
        /// </summary>
        public virtual Vector2 Position { get; set; }

        /// <summary>
        /// The gameobjects Rotation
        /// </summary>
        public virtual float Rotation { get; set; }

        /// <summary>
        /// The gameobjects Size
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Destination rectangle of this gameobject
        /// </summary>
        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                    (int)Position.X, (int)Position.Y,
                    (int)Size.X, (int)Size.Y);
            }
        }

        /// <summary>
        /// Source rectangle of this gameobject
        /// </summary>
        public Rectangle Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source = value;
            }
        }
        private Rectangle _Source = Rectangle.Empty;

        /// <summary>
        /// Origin of the objects rotation
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// The gameobjects Color
        /// </summary>
        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }
        private Color _Color = Color.White;

        /// <summary>
        /// Animates the spriteColor
        /// </summary>
        public Tools.Animation ColorAnimation { get; private set; }

        /// <summary>
        /// The gameobjects sprite effects
        /// </summary>
        public SpriteEffects Effects
        {
            get
            {
                return _Effects;
            }
            set
            {
                _Effects = value;
            }
        }
        private SpriteEffects _Effects = SpriteEffects.None;

        /// <summary>
        /// The gameobjects Texture
        /// </summary>
        public virtual hTexture Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value;
                Source = _Texture.Base.Bounds;
                Size = new Vector2(_Texture.Width, _Texture.Height);
                Origin = new Vector2((float)_Texture.Width / 2, (float)_Texture.Height / 2);
            }
        }
        private hTexture _Texture = null;

        /// <summary>
        /// Used to implement multiple draw-commands inside one sprite object
        /// </summary>
        public List<TextureOverlay> TextureOverlays { get; set; }
        public struct TextureOverlay
        {
            public hTexture Texture;
            public Vector2 Offset;
            public bool Enabled;
            public int AtlasFrame;
            public bool UseLifetime;
            public double Lifetime;
        }

        /// <summary>
        /// Texture width
        /// </summary>
        public override float Width
        {
            get
            {
                return Texture.Width;
            }
        }

        /// <summary>
        /// Texture height
        /// </summary>
        public override float Height
        {
            get
            {
                return Texture.Height;
            }
        }

        /// <summary>
        /// Constructor (Registers component automatically)
        /// </summary>
        /// <param name="GameScene">GameScene, this module should be registered to</param>
        public SpriteObject(Scene GameScene)
            : base(GameScene)
        {
            TextureOverlays = new List<TextureOverlay>();
            ColorAnimation = new Tools.Animation(this.GetType().GetProperty("Color"), this);
        }

        /// <summary>
        /// GameObject Update
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            ColorAnimation.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// GameObject Draw
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(this.Texture != null)
                this.Texture.Draw(gameTime, Destination, Color, Rotation, Origin, Effects, DrawOrder);

            for (int i = 0; i < TextureOverlays.Count; i++)
                if (TextureOverlays[i].Texture != null && TextureOverlays[i].Enabled)
                {
                    var tOverlay = TextureOverlays[i];

                    int oldAtlasFrame = tOverlay.Texture.AtlasFrame;

                    if (tOverlay.Texture.IsAtlas)
                        tOverlay.Texture.AtlasFrame = tOverlay.AtlasFrame;

                    tOverlay.Texture.Draw(gameTime, new Rectangle(tOverlay.Offset.ToPoint() + Destination.Location, Destination.Size), Color, Rotation, Origin, Effects, DrawOrder);

                    if (tOverlay.Texture.IsAtlas)
                        tOverlay.Texture.AtlasFrame = oldAtlasFrame;

                    if(tOverlay.UseLifetime)
                    {
                        if (tOverlay.Lifetime < 0)
                            tOverlay.Enabled = false;
                        else
                            tOverlay.Lifetime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                        TextureOverlays[i] = tOverlay;
                    }
                }
            
            //SceneManager.Game.SpriteBatch.Draw(this.Texture, this.Destination, this.Source, this.Color, this.Rotation, this.Origin, this.Effects, this.DrawOrder);
        }

        /// <summary>
        /// Dispose drawable object
        /// </summary>
        public override void Dispose()
        {
            this.Visible = false;
            base.Dispose();
        }
    }
}
