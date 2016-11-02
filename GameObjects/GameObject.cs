using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ODBank.GameObjects
{
    /// <summary>
    /// Base GameObject. This is the fundament of every object in the game
    /// </summary>
    class GameObject : IGameComponent, IUpdateable, IDisposable
    {
        #region Properties_IUpdateable
        /// <summary>
        /// Enables/disables Updating
        /// </summary>
        public bool Enabled 
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
                if (EnabledChanged != null)
                    EnabledChanged(this, new EventArgs());
            }
        }
        private bool _Enabled = true;

        /// <summary>
        /// Triggered when object is enabled/disabled
        /// </summary>
        public event EventHandler<EventArgs> EnabledChanged;

        /// <summary>
        /// Update order
        /// </summary>
        public int UpdateOrder
        {
            get
            {
                return _UpdateOrder;
            }
            set
            {
                _UpdateOrder = value;
                if (UpdateOrderChanged != null)
                    UpdateOrderChanged(this, new EventArgs());
            }
        }
        private int _UpdateOrder = 0;

        /// <summary>
        /// Triggered when the UpdateOrder changed
        /// </summary>
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion
        
        /// <summary>
        /// Game, this component is attached to
        /// </summary>
        public MainGame Game { get; private set; }

        /// <summary>
        /// Constructor (Registers component automatically)
        /// </summary>
        /// <param name="Game">Game, this module should be registered to</param>
        public GameObject(MainGame Game)
        {
            Initialize();

            // Register this
            this.Game = Game;
            Game.Components.Add(this);

            this.Enabled = true;
        }

        /// <summary>
        /// Initialization of the GameObject (Construction)
        /// </summary>
        public void Initialize()
        { }

        /// <summary>
        /// GameObject Update
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GameObject"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// GameObject Dispose
        /// </summary>
        public virtual void Dispose()
        {
            this.Enabled = false;
            this.Disposed = true;
            this.Game.Components.Remove(this);
        }
    }
}
