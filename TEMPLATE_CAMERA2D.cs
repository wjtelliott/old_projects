/*
    This template was used by a few projects.
    I've cleaned it up from the specific projects and put it here.
    Comments are current
*/



//I'm not sure you need all of these imports
//Mono should work fine with the correct namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT
{
    public sealed class CAMERA
    {

        protected float _zoom;
        public float Zoom { get => _zoom; set { this._zoom = value; } }

        // This probably doesn't need public set access
        protected Matrix _transform;
        public Matrix Transform { get => _transform; set { this._transform = value; } }

        protected Vector2 _pos;
        public Vector2 Position { get => _pos; set { this._pos = value; } }

        protected Viewport _viewport;

        public Matrix InverseTransform { get => Matrix.Invert(this._transform) }

        public Camera2D(Viewport viewport)
        {
            this._zoom = 1.0f;
            this._pos = new Vector2(0, 0);

            // This might be annoying to pass down in constructor to constructor ect
            this._viewport = viewport;
        }

        public void Update(Vector2 newPosition)
        {
            /*
                If you have access to Xna's GraphicDeviceManager and the player's 2d texture
                you can use this pseudo for _pos instead:

                this._pos = -(
                    X/Y origin pos as Vector2 -
                    new Vector2 (
                        Graphics.PreferredBackBufferWidth / 2 - textureWidth / 2,
                        Graphics.PreferredBackBufferHeight / 2 - textureHeight / 2
                    )
                )
            */
            this._pos = newPosition;

            // Useful to clamp the zoom between 0-10
            this._zoom = MathHelper.Clamp(_zoom, 0.0f, 10.0f);

            // Create our transform
            this._transform = Matrix.CreateTranslation(_pos.X, _pos.Y, 0);
        }
    }
}
