using GearedUpEngine.Assets.Entities;
using GearedUpEngine.Assets.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.Menus
{
    public class Inventory
    {
        readonly Texture2D texture;
        public Boolean IsVisible { get; set; }
        KeyboardState kStateOld;
        MouseState mStateOld;
        InventoryTile[] inventoryTiles;
        InventoryItem inHand = null;
        readonly Rectangle bounds;
        public Inventory(TextureManager textureManager)
        {
            this.texture = textureManager.InventoryTexture;
            this.kStateOld = Keyboard.GetState();
            this.mStateOld = Mouse.GetState();
            this.IsVisible = false;

            this.bounds = new Rectangle(0, 0, this.texture.Width, this.texture.Height);

            this.inventoryTiles = new InventoryTile[6 * 11];

            int index = 0;
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 11; y++)
                {
                    this.inventoryTiles[index] = new InventoryTile(x, y, textureManager.GetTextureByName("grass_left"));
                    index++;
                }
            this.inventoryTiles[4].Occupied = true;
            this.inventoryTiles[4].ItemInPosition = new InventoryItem("Item: gun1\n it shoots \n weight 4kg", textureManager.GetTextureByName("grass_right"));
            this.inventoryTiles[14].Occupied = true;
            this.inventoryTiles[14].ItemInPosition = new InventoryItem("Item: bread\n you eat it dummy \n weight 8kg", textureManager.GetTextureByName("grass_right"));
        }
        public void Update(KeyboardState kState, MouseState mState)
        {
            // Set up our character to toggle inventory
            char inventoryToggle = 'i';

            // Check to see if we should toggle the inventory on this frame.
            // (If the inventory key is pressed this frame, but wasn't last frame)
            this.IsVisible = 
                (TypeManager.GetKeys(kState, this.kStateOld).Contains<char>(inventoryToggle))
                
                ? !this.IsVisible : this.IsVisible;

            {
                char debugAddItemCharacter = 'k';
                if (TypeManager.GetKeys(kState, this.kStateOld).Contains<char>(debugAddItemCharacter))
                {
                    if (AddDebugItemToInventory(this.texture))
                    {
                        //we got an item
                    }
                }
            }

            this.kStateOld = kState;
            
            if (this.inHand != null)
                this.inHand.Update(mState);


            DropItemNotVisible(mState);


            if (!this.IsVisible)
                return;

            foreach (InventoryTile iTile in this.inventoryTiles)
                iTile.Update(mState, ref this.inHand);
            

            DropItem(mState);
        }

        public void Draw(SpriteBatch spriteBatch, FontManager fontManager)
        {
            if (this.IsVisible)
            {
                spriteBatch.Draw(this.texture, new Vector2(0, 0), Color.White);
                foreach (InventoryTile iTile in this.inventoryTiles)
                    iTile.Draw(spriteBatch);
                foreach (InventoryTile iTile in this.inventoryTiles)
                    if (iTile.ItemInPosition != null)
                        iTile.DrawDetails(spriteBatch, fontManager);
            }
            if (this.inHand != null)
                this.inHand.Draw(spriteBatch, fontManager);
        }

        bool AddDebugItemToInventory(Texture2D txm)
        {
            foreach (InventoryTile iT in this.inventoryTiles)
            {
                if (!iT.Occupied)
                {
                    iT.Occupied = true;
                    iT.ItemInPosition = new InventoryItem("New Item", txm);
                    return true;
                }
            }
            return false;
        }

        void DropItemNotVisible(MouseState mState)
        {
            // TODO: Tell server we dropped the item
            if (mState.LeftButton == ButtonState.Pressed &&
                !this.IsVisible &&
                this.inHand != null)
                this.inHand = null;
        }

        void DropItem(MouseState mState)
        {
            Vector2 mousePos = new Vector2(
                mState.X,
                mState.Y);

            // TODO: Tell server we dropped the item
            if (mState.LeftButton == ButtonState.Pressed &&
                this.inHand != null &&
                !this.bounds.Contains(mousePos))
            {
                this.inHand = null;
            }
        }
    }

    public class InventoryTile
    {
        Texture2D texture;
        public Boolean Occupied { get; set; }
        public Vector2 PositionInTilemap { get; set; }
        public Vector2 Position { get; set; }
        Rectangle drawSpace;
        public InventoryItem ItemInPosition { get; set; }
        MouseState mStateOld;
        bool pressed, hover;

        public InventoryTile(int x, int y, Texture2D t)
        {
            this.texture = t;
            this.PositionInTilemap = new Vector2(x, y);
            this.Position = new Vector2(0, 0);
            this.Occupied = false;
            this.ItemInPosition = null;
            this.mStateOld = Mouse.GetState();
            this.drawSpace = new Rectangle(x * 47 + 24, y * 44 + 12, 16, 16);
            this.pressed = false;
            this.hover = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.drawSpace, (this.Occupied) ? Color.White : Color.Green);
        }
        public void DrawDetails(SpriteBatch spriteBatch, FontManager fontManager)
        {
            if (this.hover && this.ItemInPosition != null)
            {
                // draw some extra details
                spriteBatch.DrawString(fontManager.GetFontByName("menu"), this.ItemInPosition.Details, new Vector2(Mouse.GetState().X + 15, Mouse.GetState().Y + 15), Color.Red);
            }
        }

        public void Update(MouseState mState, ref InventoryItem item)
        {
            // This block will decide if we are above the object and if we clicked this object
            {

                // Get the mouse position for easy-ness
                Vector2 mousePos = new Vector2(mState.X, mState.Y);

                // Is our mouse button released last frame, but this frame we pressed it?
                // And we are in the bounds of the object?
                // OR we are already being pressed, but we haven't left the bounds of the object?
                if (this.mStateOld.LeftButton == ButtonState.Released &&
                    mState.LeftButton == ButtonState.Pressed &&
                    this.drawSpace.Contains(mousePos)

                    || this.pressed && this.drawSpace.Contains(mousePos))
                {

                    // We have been pressed
                    this.pressed = true;
                }

                // The above is not true, or we left the bounds of the object while still holding our mouse
                // button down.
                else this.pressed = false;

                // Are we already pressed and this frame we released our mouse button?
                // And we are still in the bounds of the object?
                // This object has been clicked.
                if (this.pressed &&
                    mState.LeftButton == ButtonState.Released &&
                    this.drawSpace.Contains(mousePos))
                {
                    if (item != null && !this.Occupied)
                    {
                        this.Occupied = true;
                        this.ItemInPosition = item;
                        item = null;
                    }
                    else if (item == null && this.Occupied)
                    {
                        this.Occupied = false;
                        item = this.ItemInPosition;
                        this.ItemInPosition = null;
                    }
                }
                    // We've been clicked, set the object in the inventory to this object's occupied spot
                    //if (item == null)
                    //    item = this.ItemInPosition;

                // If we are pressed and the mouse isn't pressed this frame, reset
                if (this.pressed && mState.LeftButton == ButtonState.Released)
                    this.pressed = false;
            }

            // In this block we check to see if we are hovering overtop the item
            {

                // Get the mouse position for easy-ness
                Vector2 mousePos = new Vector2(mState.X, mState.Y);

                // Check to see if we are hovering the item
                this.hover =
                    (this.drawSpace.Contains(mousePos));

            }

            this.mStateOld = mState;
        }
    }
}
