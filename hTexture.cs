using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank
{
    /// <summary>
    /// Provides advanced texture functionality
    /// </summary>
    class hTexture : IDisposable
    {
        /// <summary>
        /// The base texture
        /// </summary>
        public Texture2D Base { get; private set; }

        /// <summary>
        /// Width of this texture
        /// </summary>
        public int Width
        {
            get
            {
                if (this.IsAtlas)
                    return (int)AtlasTile.X;
                else
                    return Base.Width;
            }
        }

        /// <summary>
        /// Height of this texture
        /// </summary>
        public int Height
        {
            get
            {
                if (this.IsAtlas)
                    return (int)AtlasTile.Y;
                else
                    return Base.Height;
            }
        }


        /// <summary>
        /// Indicates that this texture is a atlas. Check out the constructors to see how to create an atlas
        /// </summary>
        public bool IsAtlas { get; private set; }

        /// <summary>
        /// Size of a tile in this atlas
        /// </summary>
        public Vector2 AtlasTile { get; private set; }

        /// <summary>
        /// Total number of atlas frames on this texture
        /// </summary>
        public int AtlasFrameCount { get; private set; }

        /// <summary>
        /// ID of the currently visible atlas frame
        /// </summary>
        public int AtlasFrame { get; set; }


        /// <summary>
        /// Indicates that this texture is a animation. Check out the constructors to see how to create an atlas
        /// </summary>
        public bool IsAnimation { get; private set; }

        /// <summary>
        /// Frames per msecond for this animation (play speed)
        /// </summary>
        public float AnimationFPS { get; set; }

        /// <summary>
        /// Indicates if the animation is running. Use the start/pause/stop methods to control it
        /// </summary>
        public bool AnimationRunning 
        { 
            get
            {
                return _AnimationRunning;
            }
            private set
            {
                var oldValue = _AnimationRunning;
                _AnimationRunning = value;
                if (oldValue && !value && AnimationStopped != null)
                    AnimationStopped(this);
            }
        }
        private bool _AnimationRunning = false;

        /// <summary>
        /// Repeat animation when done
        /// </summary>
        private bool _repeat;

        /// <summary>
        /// Repeat animation until counter in _repeatTimes is 0
        /// </summary>
        private bool _repeatUntil;

        /// <summary>
        /// Repeat animation x times
        /// </summary>
        private int _repeatTimes = -1;

        /// <summary>
        /// Contains sequences for this animation
        /// </summary>
        public Dictionary<string, int[]> AnimationSequences { get; private set; }


        /// <summary>
        /// Time that indicates how long the animation was running
        /// </summary>
        private float _AnimTime;

        /// <summary>
        /// Custom animation sequence
        /// </summary>
        private int[] _Sequence;

        /// <summary>
        /// Animation stopped delegate
        /// </summary>
        public delegate void AnimationStoppedHandler(hTexture sender);

        /// <summary>
        /// Occurs when [animation switched from playing to not playing].
        /// </summary>
        public event AnimationStoppedHandler AnimationStopped; 

        /// <summary>
        /// Initializes a new instance of the <see cref="hTexture"/> class.
        /// </summary>
        /// <param name="Texture">The texture.</param>
        public hTexture(Texture2D Texture)
        {
            this.Base = Texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="hTexture" /> class.
        /// </summary>
        /// <param name="Texture">The texture.</param>
        /// <param name="AtlasTile">The atlas tile.</param>
        /// <param name="AtlasFrameCount">The atlas frame count.</param>
        public hTexture(Texture2D Texture, Vector2 AtlasTile, int AtlasFrameCount)
        {
            this.Base = Texture;

            this.IsAtlas = true;
            this.AtlasTile = AtlasTile;
            this.AtlasFrameCount = AtlasFrameCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="hTexture"/> class.
        /// </summary>
        /// <param name="Texture">The texture.</param>
        /// <param name="AtlasTile">The atlas tile.</param>
        /// <param name="AtlasFrameCount">The atlas frame count.</param>
        /// <param name="AnimationFPS">The animations FPS.</param>
        public hTexture(Texture2D Texture, Vector2 AtlasTile, int AtlasFrameCount, float AnimationFPS)
        {
            this.Base = Texture;

            this.IsAtlas = true;
            this.AtlasTile = AtlasTile;
            this.AtlasFrameCount = AtlasFrameCount;

            this.IsAnimation = true;
            this.AnimationFPS = AnimationFPS;
            this.AnimationSequences = new Dictionary<string, int[]>();
        }

        /// <summary>
        /// Returns the source rectangle for the current settings on this texture
        /// </summary>
        public Rectangle GetSourceRectangle()
        {
            if (IsAtlas)
            {
                var xAT = (int)(AtlasTile.X);
                var yAT = (int)(AtlasTile.Y);

                var hAtlas = (int)(Base.Width / xAT);
                var yA = (int)(AtlasFrame / hAtlas);
                var xA = (int)(AtlasFrame - yA * hAtlas);

                return new Rectangle((int)(xAT * xA), (int)(yAT * yA), xAT, yAT);
            }
            else
                return Base.Bounds;
        }

        /// <summary>
        /// Plays this animation
        /// </summary>
        /// <param name="Repeat">if set to <c>true</c> [repeat].</param>
        /// <param name="Sequence">Key for a custom animation sequence (create new one at property AnimationSequences). Will go through every frame if empty.</param>
        /// <param name="Times">Amount of times to repeat (-1 = unlimited)</param>
        public void Play(bool Repeat = true, string Sequence = "", int RepeatTimes = -1)
        {
            if(this.IsAnimation)
            {
                this._repeat = Repeat;
                this._repeatTimes = RepeatTimes;
                this._repeatUntil = RepeatTimes >= 0;
                this.AnimationRunning = true;
                this.AtlasFrame = 0;
                this._AnimTime = 0;
                if (AnimationSequences != null && AnimationSequences.ContainsKey(Sequence))
                    this._Sequence = AnimationSequences[Sequence];
                else
                    this._Sequence = null;
            }
        }

        /// <summary>
        /// Stops this animation.
        /// </summary>
        public void Stop()
        {
            this.AnimationRunning = false;
        }

        /// <summary>
        /// Draws this texture in the current state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, Rectangle Destination, Color Color, float Rotation, Vector2 Origin, SpriteEffects Effect, int DrawOrder)
        {
            if(this.IsAnimation && this.AnimationRunning)
            {
                this._AnimTime += gameTime.ElapsedGameTime.Milliseconds;

                var mFrames = AtlasFrameCount;
                if (_Sequence != null) mFrames = _Sequence.Length;

                var sTime = this._AnimTime / 1000;
                var cFrame = sTime * AnimationFPS;

                bool AnimStop = false;
                if ((int)(cFrame / mFrames) > 0)
                {
                    AnimStop = true;
                    cFrame = mFrames-1; // Make sure last frame will be last
                }
                
                if(this.AnimationRunning)
                {
                    var cFrameIndex = (int)((cFrame / mFrames - (int)(cFrame / mFrames)) * mFrames);

                    if (_Sequence != null)
                        AtlasFrame = _Sequence[cFrameIndex];
                    else
                        AtlasFrame = cFrameIndex;
                }

                if(AnimStop)
                    if (!_repeat)
                    {
                        this.AnimationRunning = false;
                    }
                    else if (this._repeatUntil)
                    {
                        _repeatTimes--;
                        this._AnimTime = 0;
                        if (_repeatTimes < 1)
                            _repeat = false;
                    }
            }

            SceneManager.Game.SpriteBatch.Draw(this.Base, Destination, this.GetSourceRectangle(), Color, Rotation, Origin, Effect, DrawOrder);
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>Exact copy of this instance</returns>
        public hTexture Copy()
        {
            if (this.IsAnimation)
                return new hTexture(this.Base, this.AtlasTile, this.AtlasFrameCount, this.AnimationFPS)
                {
                    AnimationSequences = this.AnimationSequences
                };

            if(this.IsAtlas)
                return new hTexture(this.Base, this.AtlasTile, this.AtlasFrameCount);

            return new hTexture(this.Base);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Base.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(bool DisposeBase = true)
        {
            if (DisposeBase)
                this.Base.Dispose();
        }
    }
}
