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
    public class Item
    {
    }
    public class InventoryItem
    {
        public string Details { get; private set; }
        readonly Texture2D texture;

        // We use this to draw on the mouse
        Rectangle drawPosition;

        public InventoryItem(string newDetails, Texture2D newTexture)
        {
            this.Details = newDetails;
            this.texture = newTexture;
            this.drawPosition = new Rectangle(0, 0, this.texture.Width, this.texture.Height);
        }

        public void Update(MouseState mState)
        {
            this.drawPosition.X = mState.X - this.drawPosition.Width / 2;
            this.drawPosition.Y = mState.Y - this.drawPosition.Height / 2;
        }

        public void Draw(SpriteBatch spriteBatch, FontManager fontManager)
        {
            spriteBatch.Draw(this.texture, this.drawPosition, Color.White);
        }
    }
}
