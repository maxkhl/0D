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
        /// Post processing shader
        /// </summary>
        public Effect PostShader { get; set; }

        /// <summary>
        /// Main rendertarget
        /// </summary>
        private RenderTarget2D RenderTarget { get; set; }

        /// <summary>
        /// Render techniques for post processing shader
        /// </summary>
        public enum RenderTechniques
        {
            Default,

            /// <summary>
            /// Able to blur the screen. See BlurAmount and BlurIntensity
            /// </summary>
            Blur
        }

        /// <summary>
        /// The current rendertechnique
        /// </summary>
        public RenderTechniques RenderTechnique
        {
            get
            {
                return _RenderTechnique;
            }
            set
            {
                _RenderTechnique = value;
            }
        }
        private RenderTechniques _RenderTechnique = RenderTechniques.Default;

        /// <summary>
        /// Amount of the blur effect. This will affect the spread of the blur
        /// !Needs the Blur RenderTechnique to be active!
        /// </summary>
        public float BlurAmount { get; set; }

        /// <summary>
        /// Intensity of the blur effect. This will affect the brightness of the blur effect.
        /// Choose a value between 0 and 0.25f to get a good result (0.25f is max intensity 0 is invisible)
        /// !Needs the Blur RenderTechnique to be active!
        /// </summary>
        public float BlurIntensity
        {
            get
            {
                return _BlurIntensity;
            }
            set
            {
                _BlurIntensity = value;
            }
        }
        private float _BlurIntensity = 0.25f;

        /// <summary>
        /// Gets or sets the picture brightness. Default 0
        /// </summary>
        public float Brightness { get; set; }

        /// <summary>
        /// Gets or sets the picture contrast. Default 1
        /// </summary>
        public float Contrast
        {
            get
            {
                return _Contrast;
            }
            set
            {
                _Contrast = value;
            }
        }
        private float _Contrast = 1;

        /// <summary>
        /// Gets or sets the color for the picture.
        /// </summary>
        public Vector4 ScreenColor
        {
            get
            {
                return _ScreenColor;
            }
            set
            {
                _ScreenColor = value;
            }
        }
        private Vector4 _ScreenColor = new Vector4(1);

        /// <summary>
        /// Gets or sets the width of the rendertarget.
        /// </summary>
        public int ResWidth
        {
            get
            {
                return RenderTarget.Width;
            }
            set
            {
                this.RenderTarget = new RenderTarget2D(
                    Game.GraphicsDevice,
                    value,
                    ResHeight);
            }
        }

        /// <summary>
        /// Gets or sets the height of the rendertarget.
        /// </summary>
        public int ResHeight
        {
            get
            {
                return RenderTarget.Height;
            }
            set
            {
                this.RenderTarget = new RenderTarget2D(
                    Game.GraphicsDevice,
                    ResWidth,
                    value);
            }
        }

        public void CreateRenderTarget(int Width, int Height)
        {
            this.RenderTarget = new RenderTarget2D(
                Game.GraphicsDevice,
                Width,
                Height);
        }

        /// <summary>
        /// Sets the post shader parameters.
        /// </summary>
        private void SetPostShaderParameters()
        {
            if (PostShader.Techniques[RenderTechnique.ToString()] != null)
                PostShader.CurrentTechnique = PostShader.Techniques[RenderTechnique.ToString()];

            if (PostShader.Parameters["Width"] != null)
                PostShader.Parameters["Width"].SetValue((float)this.RenderTarget.Width);
            if (PostShader.Parameters["Height"] != null)
                PostShader.Parameters["Height"].SetValue((float)this.RenderTarget.Height);

            if (PostShader.Parameters["Brightness"] != null)
                PostShader.Parameters["Brightness"].SetValue(Brightness);
            if (PostShader.Parameters["Contrast"] != null)
                PostShader.Parameters["Contrast"].SetValue(Contrast);
            if (PostShader.Parameters["Color"] != null)
                PostShader.Parameters["Color"].SetValue(ScreenColor);



            if (PostShader.Parameters["gaussAmount"] != null)
                PostShader.Parameters["gaussAmount"].SetValue(BlurAmount);
            if (PostShader.Parameters["gaussIntensity"] != null)
                PostShader.Parameters["gaussIntensity"].SetValue(BlurIntensity);


        }
    }
}
