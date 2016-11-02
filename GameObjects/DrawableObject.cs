using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    /// <summary>
    /// Drawable GameObject. !!!Make sure base.Draw is called before your own draw so it can set the shader parameters!!!
    /// </summary>
    class DrawableObject : GameObject, IDrawable
    {
        #region Properties_IDrawable
        /// <summary>
        /// Enables/disables Updating
        /// </summary>
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                _Visible = value;
                if (VisibleChanged != null)
                    VisibleChanged(this, new EventArgs());
            }
        }
        private bool _Visible = true;

        /// <summary>
        /// Triggered when object is enabled/disabled
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;

        /// <summary>
        /// Update order
        /// </summary>
        public int DrawOrder
        {
            get
            {
                return _DrawOrder;
            }
            set
            {
                _DrawOrder = value;
                if (DrawOrderChanged != null)
                    DrawOrderChanged(this, new EventArgs());
            }
        }
        private int _DrawOrder = 0;

        /// <summary>
        /// Triggered when the UpdateOrder changed
        /// </summary>
        public event EventHandler<EventArgs> DrawOrderChanged;
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DrawableObject"/> is on the overlay layer.
        /// </summary>
        public virtual bool Overlay 
        { 
            get
            {
                return !this.Visible && GameScene.OverlayLayer.Contains(this);
            }
            set
            {
                if (value)
                {
                    if (!Overlay)
                    {
                        this.Visible = false;
                        GameScene.OverlayLayer.Add(this);
                    }
                }
                else
                {
                    if (GameScene.OverlayLayer.Contains(this))
                    {
                        GameScene.OverlayLayer.Remove(this);
                        this.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// GameScene this object is bound to
        /// </summary>
        public Scene GameScene { get; set; }

        /// <summary>
        /// Gets the width of this object.
        /// </summary>
        public virtual float Width 
        { 
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the height of this object.
        /// </summary>
        public virtual float Height 
        { 
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Constructor (Registers component automatically)
        /// </summary>
        /// <param name="GameScene">GameScene this object is used in</param>
        public DrawableObject(Scene GameScene)
            : base(GameScene.Game)
        {
            this.GameScene = GameScene;
        }

        /// <summary>
        /// GameObject Draw
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Draw(GameTime gameTime)
        {
            //GameScene.Shader.Parameters["Brightness"].SetValue(Brightness);
            if (GameScene.Shader.Parameters["Width"] != null)
                GameScene.Shader.Parameters["Width"].SetValue(this.Width);

            if (GameScene.Shader.Parameters["Height"] != null)
                GameScene.Shader.Parameters["Height"].SetValue(this.Height);
        }
    }
}
