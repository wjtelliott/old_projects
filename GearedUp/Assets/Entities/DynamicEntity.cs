using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2019 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Entities
{
    /// <summary>
    /// This is the Dynamic entity base for all objects
    /// </summary>
    public class DynamicEntity : Drawable
    {
        /// <summary>
        /// Private field for our velocity, with a property for public use
        /// </summary>
        Vector2 _velocity;
        public Vector2 Velocity { get { return this._velocity; } set { this._velocity = value; } }

        /// <summary>
        /// Our friction value for this object
        /// </summary>
        public float Friction { get; set; }

        /// <summary>
        /// Create a dynamic Object with default values
        /// </summary>
        public DynamicEntity()
        {
            this._velocity = Vector2.Zero;
            this.Friction = 0.6f;
        }

        /// <summary>
        /// Add acceleration to this object
        /// </summary>
        /// <param name="accel">The amount as a Vector2</param>
        public void AddAcceleration(Vector2 accel)
        {
            this._velocity += accel;
        }

        /// <summary>
        /// Update our object, adding velocity to position and friction.
        /// </summary>
        public new void Update()
        {
            if (this.Friction < 0f) this.Friction = 0f;
            AddFriction(this.Friction);
            NormalizeAndAddVelocity();
            base.Update();
        }

        /// <summary>
        /// Normalize our velocity to an INT for raw positioning
        /// </summary>
        void NormalizeAndAddVelocity()
        {
            if (Math.Abs(this._velocity.X) > 1)
                this.RawPosition.X += (int)this._velocity.X;
            if (Math.Abs(this._velocity.Y) > 1)
                this.RawPosition.Y += (int)this._velocity.Y;
        }

        void AddFriction(float amount)
        {
            Vector2 AbsVelocity = new Vector2
                (
                    Math.Abs(this._velocity.X),
                    Math.Abs(this._velocity.Y)
                );

            {
                float amountX = amount;
                if (AbsVelocity.X - amountX < 0)
                    amountX = AbsVelocity.X;

                this._velocity.X = (this._velocity.X < 0) ?
                    this._velocity.X + amountX :
                    (this._velocity.X > 0) ?
                    this._velocity.X - amountX :
                    this._velocity.X;
            }

            {
                float amountY = amount;
                if (AbsVelocity.Y - amountY < 0)
                    amountY = AbsVelocity.Y;

                this._velocity.Y = (this._velocity.Y < 0) ?
                    this._velocity.Y + amountY :
                    (this._velocity.Y > 0) ?
                    this._velocity.Y - amountY :
                    this._velocity.Y;
            }
        }
    }
}
