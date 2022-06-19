using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.Entities
{
    public class ServerPlayer
    {
        Vector2 position;
        Vector2 velocity;
        readonly string playerName;
        readonly long userIndentifier;

        const float acceleration = 2f;
        const float friction = 0.6f;

        public Vector2 Position { get => this.position; }
        public long UserID { get => this.userIndentifier; }
        public String Name { get => this.playerName; }

        public ServerPlayer(string pName, long uid)
        {
            this.position = Vector2.Zero;
            this.velocity = Vector2.Zero;
            this.playerName = pName;
            this.userIndentifier = uid;
        }

        public void AddVelocityFromClientKeys(byte[] byteKeys)
        {
            this.velocity += TokenizeVelocity(byteKeys);
        }

        public void Update()
        {
            if (friction > 0f)
                AddFriction(friction);
            NormalizeAndAddVelocity();
        }

        void AddFriction(float amount)
        {
            Vector2 AbsVelocity = new Vector2
                (
                    Math.Abs(this.velocity.X),
                    Math.Abs(this.velocity.Y)
                );

            {
                float amountX = amount;
                if (AbsVelocity.X - amountX < 0)
                    amountX = AbsVelocity.X;

                this.velocity.X = (this.velocity.X < 0) ?
                    this.velocity.X + amountX :
                    (this.velocity.X > 0) ?
                    this.velocity.X - amountX :
                    this.velocity.X;
            }

            {
                float amountY = amount;
                if (AbsVelocity.Y - amountY < 0)
                    amountY = AbsVelocity.Y;

                this.velocity.Y = (this.velocity.Y < 0) ?
                    this.velocity.Y + amountY :
                    (this.velocity.Y > 0) ?
                    this.velocity.Y - amountY :
                    this.velocity.Y;
            }
        }

        void NormalizeAndAddVelocity()
        {
            this.position += this.velocity;
        }

        Vector2 TokenizeVelocity(byte[] keys)
        {
            Vector2 newVelocity = Vector2.Zero;

            foreach (byte b in keys)
                switch (b)
                {
                    case 0:
                        newVelocity.X -= acceleration;
                        break;
                    case 1:
                        newVelocity.Y -= acceleration;
                        break;
                    case 2:
                        newVelocity.Y += acceleration;
                        break;
                    case 3:
                        newVelocity.X += acceleration;
                        break;
                }

            return newVelocity;
        }
    }
}
