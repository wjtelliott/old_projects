using GearedUpEngine.Assets.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Menus
{

    /// <summary>
    /// Our menu button's main class. We use this to get user confirmation of action
    /// on a certain menu.
    /// </summary>
    public sealed class MenuButton
    {

        /// <summary>
        /// The menu that this object should be on
        /// </summary>
        public CurrentMenu Menu { get; set; }

        /// <summary>
        /// The position and sizing to draw this object
        /// </summary>
        Rectangle drawSpace;

        /// <summary>
        /// The old and current mouse states
        /// </summary>
        MouseState mState, mStateOld;

        /// <summary>
        /// The texture to draw for the menu button
        /// </summary>
        readonly Texture2D texture;

        /// <summary>
        /// Is this object pressed?
        /// </summary>
        bool pressed;

        /// <summary>
        /// Do we perform this buttons action? This is taken care of by the menu manager
        /// </summary>
        public bool DoAction { get; set; }

        /// <summary>
        /// What is our menu action to perform?
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Construct our MenuButton on a given menu
        /// </summary>
        /// <param name="newTexture">The texture for the menu button</param>
        /// <param name="size">The drawing space for the menu button</param>
        /// <param name="action">The menu action when this button is clicked</param>
        /// <param name="ontoMenu">The menu this button will be on</param>
        public MenuButton(Texture2D newTexture, Rectangle size, string action, CurrentMenu ontoMenu)
        {
            
            // Init our fields with the given args
            this.texture = newTexture;
            this.drawSpace = size;

            // Our first mouse states will not matter, just grab the current frame's state
            this.mState = this.mStateOld = Mouse.GetState();

            this.pressed = false;
            this.DoAction = false;
            this.Tag = action;
            this.Menu = ontoMenu;
        }

        /// <summary>
        /// Update our menu button. Call this only once per frame, and only on the menu that
        /// this button belongs to.
        /// </summary>
        /// <param name="newState">The current frame's mouse state</param>
        public void Update(MouseState newState)
        {

            // TODO: why do we need a mouse state AND an old state?
            // this.mState is OBSOLETE
            this.mState = newState;

            // This block will decide if we are above the object and if we clicked this object
            {

                // Get the mouse position for easy-ness
                Vector2 mousePos = new Vector2(this.mState.X, this.mState.Y);

                // Is our mouse button released last frame, but this frame we pressed it?
                // And we are in the bounds of the object?
                // OR we are already being pressed, but we haven't left the bounds of the object?
                if (this.mStateOld.LeftButton == ButtonState.Released &&
                    this.mState.LeftButton == ButtonState.Pressed &&
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
                    this.mState.LeftButton == ButtonState.Released &&
                    this.drawSpace.Contains(mousePos))

                    // We've been clicked, tell the menu manager we need to 
                    // call our menu action
                    this.DoAction = true;

                // If we are pressed and the mouse isn't pressed this frame, reset
                if (this.pressed && this.mState.LeftButton == ButtonState.Released)
                    this.pressed = false;
            }

            // We are done with the mouse state for the update/frame.
            // Move the current state to the old state
            this.mStateOld = this.mState;
        }

        /// <summary>
        /// Draw our menu button. Call this only once per frame, and only when the menu being displayed
        /// is being drawn.
        /// </summary>
        /// <param name="spriteBatch">The game's sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // Check if our texture exists.
            if (this.texture == null)
            {
                
                // Our texture is nulled, report this to the developer console
                DeveloperDetails.Details += "Button tried drawing without a texture!;";

                // Don't attempt to draw a nulled texture
                return;
            }

            // Are we being pressed? Doesn't mean we've been clicked on
            if (this.pressed)
            {

                // Create a new drawing space, and slightly alter it to show user feedback
                Rectangle newDraw = this.drawSpace;

                // Move the X/Y slightly
                newDraw.X += 3;
                newDraw.Y += 3;

                // Draw in a different color. Maybe customize this color more?
                spriteBatch.Draw(this.texture, newDraw, Color.White);
            }

            // We aren't being pressed, draw this object normally
            else spriteBatch.Draw(this.texture, this.drawSpace, Color.DarkRed);
        }
    }
}
