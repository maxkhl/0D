using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    class GameCam : Camera2D, IGameComponent, IUpdateable
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
        /// Use this to specify an area the camera is allowed to show
        /// </summary>
        public Rectangle AllowedArea { get; set; }

        public Tools.Animation ZoomAnimation { get; private set; }

        public override Viewport Viewport
        {
            get
            {
                if (Game != null)
                    return Game.GraphicsDevice.Viewport;
                else
                    return base.Viewport;
            }
        }

        public MainGame Game { get; set; }
        public GameCam(MainGame Game)
            : base(Game.GraphicsDevice.Viewport)
        {
            this.Game = Game;
            EaseFunction = Tools.Easing.EaseFunction.Linear;
            Game.Components.Add(this); //Register camera

            this.UpdateOrder = 5000;

            this.ZoomAnimation = new Tools.Animation(this.GetType().GetProperty("Zoom"), this);
        }

        /// <summary>
        /// Sets the GameObject the camera should follow
        /// </summary>
        public GameObjects.SpriteObject FollowGO { get; set; }

        /// <summary>
        /// Gets or sets the ease function.
        /// </summary>
        public Tools.Easing.EaseFunction EaseFunction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether velocity is used while following a gameobject
        /// </summary>
        public bool FollowUseVelocity { get; set; }

        /// <summary>
        /// Initialization of the Camera
        /// </summary>
        public void Initialize()
        {
            FollowUseVelocity = true;
        }

        /// <summary>
        /// Camera Update
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            this.ZoomAnimation.Update(gameTime);

            if(FollowGO != null)
            {
                var Target = this.FollowGO.Position;
                /*if (typeof(BodyObject).IsAssignableFrom(FollowGO.GetType()))
                {
                    var Velocity = FarseerPhysics.ConvertUnits.ToDisplayUnits(((BodyObject)FollowGO).Body.LinearVelocity);
                    if (FollowUseVelocity)
                     Target += Velocity * 3;

                    var Speed = Velocity.Length() > 0 ? Velocity.Length() : Velocity.Length() < 0 ? Velocity.Length() * -1 : 0;
                    Speed /= 1000;
                    Speed = Speed * -1 + 1;
                    Speed = MathHelper.Clamp(Speed, 0.2f, 1);
                    //Speed /= 50;
                    var TargetZoom = Speed > 0 ? Speed : 1;
                    Zoom += (TargetZoom - Zoom) / 50;
                }*/
                //this.position.X = Tools.Easing.Ease(EaseFunction, 0, Target.X, this.position.X, 200);
                //this.position.Y = Tools.Easing.Ease(EaseFunction, 0, Target.Y, this.position.Y, 200);        
                this.position += (Target - position) / 50;

                var VisibleArea = Visible;
                if(AllowedArea != Rectangle.Empty && !AllowedArea.Contains(VisibleArea))
                {
                    if (VisibleArea.X < AllowedArea.X)
                        this.position.X += AllowedArea.X - VisibleArea.X;
                    if (VisibleArea.Y < AllowedArea.Y)
                        this.position.Y += AllowedArea.Y - VisibleArea.Y;

                    if (VisibleArea.X + VisibleArea.Width > AllowedArea.X + AllowedArea.Width)
                        this.position.X -= (VisibleArea.X + VisibleArea.Width) - (AllowedArea.X + AllowedArea.Width);
                    if (VisibleArea.Y + VisibleArea.Height > AllowedArea.Y + AllowedArea.Height)
                        this.position.Y -= (VisibleArea.Y + VisibleArea.Height) - (AllowedArea.Y + AllowedArea.Height);
                }
            }
        }
    }
}
