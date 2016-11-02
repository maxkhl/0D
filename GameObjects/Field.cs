using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    class Field : DrawableObject
    {
        /// <summary>
        /// Size of the entire field
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Position of the entire field
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Size of a single tile in the field
        /// </summary>
        public Vector2 TileSize { get; set; }

        private hTexture _blockBackground;

        public Field(Scene Scene, Vector2 Size, Vector2 TileSize)
            : base(Scene)
        {
            this.Size = Size;
            this.TileSize = TileSize;
            this.Position = (Size / 2) * -1;

            _blockBackground = new hTexture(Game.Content.Load<Texture2D>("Images/Block"),
                new Vector2(50, 50),
                25,
                25);
        }

        public override void Draw(GameTime gameTime)
        {
            for(int x = 0; x < Size.X / TileSize.X; x++)
                for(int y = 0; y < Size.Y / TileSize.Y; y++)
                {
                    int r = 50 + (int)(3 * y),
                        g = 50 + (int)(3 * y),
                        b = 50 + (int)(3 * y);

                    _blockBackground.Draw(
                        gameTime,
                        new Rectangle((int)(x * TileSize.X + Position.X), (int)(y * TileSize.Y + Position.Y), (int)TileSize.X, (int)TileSize.Y),
                        new Color(r, g, b),
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0);
                }
            
            base.Draw(gameTime);
        }
    }
}
