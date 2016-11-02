using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ODBank
{
    class Camera2D
    {
        public virtual Viewport Viewport { get; set; }

        public Vector2 position;
        public Vector2 topleft
        {
            get
            {
                return position - new Vector2(Viewport.Width, Viewport.Height) / 2;
            }
        }
        public float rotation;
        public float zoom;
        public Vector2 origin;

        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (value < 0)
                {
                    zoom = 0;
                }
                else
                {
                    zoom = value;
                }
            }
        }

        public Matrix View
        {
            get
            {
                return
                    // Flip all pixels vertically
                    // Matrix.CreateScale(-zoom) * 

                    // Step 4
                    // Translate EVERY sprite to the position that can be changed by the user. Doing this user-based movement LAST allows the matrix to scale and rotate first,
                    // and THEN apply the final movement. This is of course optional, as you may now want the matrix to scale and rotate last, in which case you would just
                    // move this matrix down to Step 2.
                    Matrix.CreateTranslation(new Vector3(-position, 0.0f)) *
                    // Step 3
                    // Rotate EVERY sprite by the specified rotation. This is done after the translation for the same reason as the scale matrix. Since rotation and scale matrices
                    // don't change the origin (or focus point) and instead simply rotate or scale the center point, the rotation and scale matrice order can be swaped.
                    Matrix.CreateRotationZ(rotation) *
                    // Step 2
                    // Scale EVERY sprite by the specified zoom level on the X and Y axis, Z is ignored again. This is done second to allow the matrix to first translate to the "new"
                    // point of origin. The matrix can then be scaled based on this new point. If the matrix is scaled first, and THEN moved, the matrix will scale based on the old
                    // point instead of the origin that we want. 
                    // If this was done, this would give an effect of scaling AND translating at the same time because of the old origin point.
                    Matrix.CreateScale(zoom, zoom, 0) *
                    // Step 1, Matrices are done in reverse.
                    // Translate EVERY sprite to origin which is located half of the screen's width on the X axis and half of the screen's height on the Y axis (center point).
                    // The box will still be located at (0,0) in the game world, the camera will simply be centered on that point. Z axis is ignored since we are looking at
                    // a 2D world.
                    Matrix.CreateTranslation(new Vector3(origin, 0));
            }
        }

        /// <summary>
        /// Visible area on the screen in world coordinates
        /// </summary>
        public Rectangle Visible
        {
            get
            {
                var inverseViewMatrix = Matrix.Invert(
                    Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                        Matrix.CreateRotationZ(rotation) *
                        Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                        Matrix.CreateTranslation(new Vector3(origin, 0)));

                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(Viewport.Width, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, Viewport.Height), inverseViewMatrix);
                var br = Vector2.Transform(new Vector2(Viewport.Width, Viewport.Height), inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))), 
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))), 
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

                /*var negZoom = ((Zoom * -1 + 2));
                negZoom = (float)Math.Pow((double)negZoom, (double)negZoom);

                return new Rectangle(
                    (int)((this.position.X - (this.viewport.Width / 2 * negZoom))),
                    (int)((this.position.Y - (this.viewport.Height / 2 * negZoom))),
                    (int)((this.viewport.Width) * negZoom),
                    (int)((this.viewport.Height) * negZoom));*/
            }
        }

        public Camera2D(Viewport inputViewport)
        {
            Viewport = inputViewport;

            position = Vector2.Zero;
            rotation = 0;
            zoom = 1;
            Refresh();
        }

        public void Refresh()
        {
            origin = new Vector2(Viewport.Width / 2, Viewport.Height / 2);
        }

        public Vector2 ToWorld(Vector2 Point)
        {
            var inverseViewMatrix = Matrix.Invert(
                Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                    Matrix.CreateRotationZ(rotation) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(origin, 0)));
            return Vector2.Transform(Point, inverseViewMatrix);
        }

        public Vector2 ToScreen(Vector2 Point)
        {
            var ViewMatrix = Matrix.Invert(
                Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                    Matrix.CreateRotationZ(rotation) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(origin, 0)));
            return Vector2.Transform(Point, ViewMatrix);
        }
    }
}

/*
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monogame2D
{
    class Camera2D
    {
        public Vector3 cameraPosition;
        public Vector3 cameraTarget;
        public Game1 gameObject;

        public Camera2D(Game1 inputGameObject)
        {
            gameObject = inputGameObject;
            cameraPosition = new Vector3(gameObject.centerRectangle.Center.ToVector2(), 50);
            cameraTarget = new Vector3(gameObject.centerRectangle.Center.ToVector2(), 0);
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }
    }
}
*/


/* Old
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monogame2D
{
    class Camera2D
    {
        public Viewport viewport;

        public Vector2 position;
        public float rotation;
        public float zoom;
        public Vector2 origin;

        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (value < 0)
                {
                    zoom = 0;
                }
                else
                {
                    zoom = value;
                }
            }
        }

        public Camera2D(Viewport inputViewport)
        {
            viewport = inputViewport;

            position = Vector2.Zero;
            rotation = 0;
            zoom = 1;
            origin = new Vector2(viewport.Width / 2, viewport.Height / 2);
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(zoom, zoom, 1) *
                Matrix.CreateTranslation(new Vector3(origin, 0.0f));
        }
    }
}
*/
