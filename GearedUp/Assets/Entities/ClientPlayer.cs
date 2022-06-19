using GearedUpEngine.Assets.Map;
using GearedUpEngine.Assets.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.Entities
{
    public sealed class ClientPlayer : Actor 
    {
        readonly long uniqueID;
        public long UniqueUserID { get => uniqueID; }

        public float Rotation { get; set; }

        public ClientPlayer(long uid)
        {
            this.uniqueID = uid;
            this.Rotation = 0f;
            this.SetDrawSpace(64, 64);
        }
        public new void Update()
        {



            base.Update();
        }

        /*
         * Copy/Pasta
         * Added support for rotation Maybe add this to base drawable?
         */
        public new void Draw(SpriteBatch spriteBatch)
        {

            //Are we visible? No? Return
            if (!this.IsVisible) return;

            //Is our texture initialised? No? Return, send debug warning
            if (this.Texture == null)
            {
                DeveloperDetails.Details += string.Format("Texture from entity {0} is nulled and tried to draw.;", this.EntityId.ToString());
                return;
            }

            //Do we want to stretch our texture across the drawspace?
            if (this.StretchTexture)
                spriteBatch.Draw(this.Texture, null, this.DrawRectangle, null, origin: new Vector2(this.Texture.Width / 2, this.Texture.Height / 2), rotation: this.Rotation, scale: null, color: this.FilterColor, effects: SpriteEffects.None, layerDepth: 0f);
            //spriteBatch.Draw(this.Texture, this.DrawRectangle, this.FilterColor);
            else //use the texture's width and height instead
                spriteBatch.Draw(this.Texture,
                    new Rectangle(this.DrawRectangle.X, this.DrawRectangle.Y, this.Texture.Width, this.Texture.Height),
                    this.FilterColor);

        }

        public void UpdateSelf(MouseState mState, Camera2D camera)
        {
            // Look at mouse
            Vector2 mPos = new Vector2(mState.X - camera.Position.X, mState.Y - camera.Position.Y);
            Vector2 direction = mPos - this.RawPosition.ToVector2();
            this.Rotation = (float)Math.Atan2(direction.Y, direction.X) - 1.55f/*Texture isn't orientated correctly*/;
            base.Update();
        }

        public Vector2 GetCameraAngle()
        {
            int a = 16, b = 16;
            
            return 
                -(
                    this.RawPosition.ToVector2() -
                    new Vector2
                    (
                        800 / 2 - a/*this.Texture.Width*/ / 2,
                        480 / 2 - b/*this.Texture.Height*/ / 2
                    )
                );
        }
    }
}
