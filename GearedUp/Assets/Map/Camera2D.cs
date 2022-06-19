using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2019 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Map
{
    public class Camera2D
    {

        protected float _zoom;
        protected Matrix _transform;
        protected Vector2 _pos;
        protected Viewport _viewport;
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public Matrix InverseTransform
        {
            get { return Matrix.Invert(_transform); }
        }

        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public Camera2D(Viewport viewport)
        {
            _zoom = 1.0f;
            _pos = new Vector2(0, 0);
            _viewport = viewport;
        }
        public void Update(Vector2 PlayerPosition)//, Texture2D PlayerTexture, GraphicsDeviceManager Graphics)
        {
            _zoom = MathHelper.Clamp(_zoom, 0.0f, 10.0f);
            this._pos = PlayerPosition;//-(PlayerPosition - new Vector2((Graphics.PreferredBackBufferWidth / 2) - (PlayerTexture.Width / 2), (Graphics.PreferredBackBufferHeight / 2) - (PlayerTexture.Height / 2)));
            _transform = Matrix.CreateTranslation(_pos.X, _pos.Y, 0);
        }
    }
}
