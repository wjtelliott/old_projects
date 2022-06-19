using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using GearedUpEngine.Assets.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Menus
    
{
    /// <summary>
    /// Our main class for a text box on our main menu, settings menu,
    /// etc.
    /// </summary>
    public sealed class MenuTextbox
    {
        /// <summary>
        /// Bool for 'Is this object in focus?'
        /// If it is focused, capture key presses for text
        /// </summary>
        bool focus;

        /// <summary>
        /// Our key presses that we have captured. We will keep these in a field
        /// with a readonly property to access from other classes and objects.
        /// </summary>
        string userInput;

        /// <summary>
        /// Our maximum characters allowed to the user in this menu text box.
        /// </summary>
        readonly int charMax;

        /// <summary>
        /// The name of our text box. This will be used to differenciate 
        /// between the multiple text boxes in a given menu
        /// </summary>
        readonly string textboxName;

        /// <summary>
        /// This bool is used for whether or not the user has pressed on our object.
        /// If we have, we need to make sure the user releases the mouse click at a later frame.
        /// </summary>
        bool pressed;

        /// <summary>
        /// This bool is used to differenciate how to draw this object whether the client has their cursor
        /// hovering the object or not. We use this to draw different text/color/position to show to the
        /// user they are atop the object to click it to gain focus for the object. IE change the text box
        /// text to "Click here to type...".
        /// 
        /// We ignore this if we already have user focus.
        /// </summary>
        bool hover;

        /// <summary>
        /// This is the menu that the object is attached to. We only draw and update objects
        /// on a given menu if they are assigned to that menu.
        /// </summary>
        public CurrentMenu Menu { get; set; }

        /// <summary>
        /// If this is true. All text on the userinput field will be replaced with '*' characters.
        /// The user input will still return what the user originally presses, but this will
        /// hide the content displayed.
        /// </summary>
        public bool PasswordProtect { get; set; }

        /// <summary>
        /// Our keyboard/mouse state
        /// oldState is last frame's keyboard state.
        /// mStateOld is last frame's mouse state.
        /// </summary>
        KeyboardState oldState;
        MouseState mStateOld;

        /// <summary>
        /// This is where we draw our text on the screen.
        /// </summary>
        Rectangle drawSpace;
        
        /// <summary>
        /// Our public property for focus on our object. Read/Write to our field
        /// </summary>
        public bool IsFocused { get { return this.focus; } set { this.focus = value; } }

        /// <summary>
        /// Our public propert for our object's name. Read only, write only at constructor.
        /// </summary>
        public string Name { get { return this.textboxName; } }

        /// <summary>
        /// Initialize and construct our Menu Text box.
        /// </summary>
        /// <param name="newName">The unique name to associate this menu object</param>
        /// <param name="bounds">The drawing position of this object. Only X/Y are taken to account for positioning</param>
        /// <param name="ontoMenu">The menu that this object will be associated with</param>
        public MenuTextbox(string newName, Rectangle bounds, CurrentMenu ontoMenu)
        {
            // Set up our default and given values
            this.focus = false;
            this.pressed = false;
            this.userInput = "";
            this.textboxName = newName;

            // Since the first frame won't make a difference, grab our old state from 
            // the current frame
            this.oldState = Keyboard.GetState();
            this.mStateOld = Mouse.GetState();

            this.drawSpace = bounds;

            // TODO: make this dynamic at init?
            charMax = 15;
            this.hover = false;

            this.Menu = ontoMenu;
            this.PasswordProtect = false;
        }

        /// <summary>
        /// Return the input given to us by a user from this menu textbox at
        /// the current frame.
        /// </summary>
        public string Input { get => this.userInput; }

        /// <summary>
        /// Clear the textbox of it's user input.
        /// </summary>
        public void Clear() { this.userInput = ""; }

        /// <summary>
        /// Update the textbox. Here we figure out if the user wants to gain focus,
        /// if we are hovering above this object, if we are typing and need to capture
        /// keypresses, etc. Only update once per frame, and on the menu attached to this
        /// object.
        /// </summary>
        /// <param name="kState">The current keyboard state for this frame</param>
        /// <param name="mState">The current mouse state for this frame</param>
        public void Update(KeyboardState kState, MouseState mState)
        {
            if (this.focus)
            {
                // Loop through each key pressed in this frame & filter through keys that
                // were already pressed last frame, and ignore those.
                // Type manager shorthands this for us for other objects as well as this one.
                foreach (char c in TypeManager.GetKeys(kState, oldState))
                {
                    // Use our tokenized key to determine to add it to our user input,
                    // remove the last most character in the string, or ignore this key 
                    // (error/ invalid key press/ reached character limit)
                    if (this.userInput.Length < this.charMax)
                        this.userInput =
                            (c == char.MinValue) ?
                                this.userInput
                                :
                                (c == char.MaxValue) ?
                                    (this.userInput.Length > 0) ?
                                        this.userInput.Substring(0, this.userInput.Length - 1)
                                        :
                                        ""
                                    :
                                    this.userInput += c.ToString();

                    // We reached character maximum limit, don't add the key. For some reason
                    // I have to remove it? Maybe I should clean this up a bit and figure out why it adds
                    // too many keys to the input at times.
                    else if (this.userInput.Length >= this.charMax)
                        this.userInput = this.userInput.Substring(0, this.charMax - 1);
                    else break;

                    // Future Bill: I figured it out, but at the moment I don't want to fix it.

                    // this.userInput.Length < this.charMax should => check if we are trying to
                    // backspace, otherwise we can't redo
                }

            }

            // This block is used to figure out if the object is being pressed/ focused
            {
                // Get our mouse position as a vector for easy-ness
                Vector2 mousePos = new Vector2(mState.X, mState.Y);

                // If we are pressing our mouse in, but last frame we were not, and
                // we are inside the bounds width/height, this object is pressed.
                if (this.mStateOld.LeftButton == ButtonState.Released &&
                    mState.LeftButton == ButtonState.Pressed &&
                    this.drawSpace.Contains(mousePos)

                    // If we are already pressed (Who can realisticaly press a mouse click
                    // in only 2 frames?!) and still are inside the bounds of the object, stay
                    // pressed
                    || this.pressed && this.drawSpace.Contains(mousePos))
                {
                    this.pressed = true;
                }
                // We are no longer pressed. User has moved their mouse outside of the bounds while holding,
                // or the above was false.
                else this.pressed = false;

                // If we are pressed, but this frame we released our mouse (clicked),
                // then we have been clicked on, set us to focused.
                if (this.pressed &&
                    mState.LeftButton == ButtonState.Released &&
                    this.drawSpace.Contains(mousePos))
                    this.focus = true;

                // If the mouse is releaed but somehow we are being pressed, reset
                if (this.pressed && mState.LeftButton == ButtonState.Released)
                    this.pressed = false;
            }

            // This block just determines if we are hovering atop the object.
            // If we are, set our hovering to true.
            {
                // Get our mouse position as a vector2 for easy-ness
                Vector2 mousePos = new Vector2(mState.X, mState.Y);

                // Make sure we are not focused. If we aren't, but we are atop this object, we are hovering.
                this.hover = (this.drawSpace.Contains(mousePos) && !this.focus) ? true : false;
            }

            // Set our old states as the current state, we are finished using the old state.
            this.mStateOld = mState;
            this.oldState = kState;
        }

        /// <summary>
        /// Draw our menu textbox objec to the frame. Only call this once per frame, and only
        /// when drawing the menu associated with this object.
        /// </summary>
        /// <param name="spriteBatch">The game's spritebatch</param>
        /// <param name="font">The font used to draw this menu/object</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            //Get our position as a vector2 for easy-ness
            Vector2 pos = new Vector2(this.drawSpace.X, this.drawSpace.Y);

            // Are we hovering, but not focused?
            if (!this.focus && this.hover)

                // Draw something to show the user that they can focus this object by clicking.
                spriteBatch.DrawString(font, "Click here to type...", pos, Color.DarkRed);

            else
            {
                // Is this a hidden textbox?
                if (this.PasswordProtect)
                {

                    // Create a new string that will be drawn instead of the
                    // use input.
                    string protect = "";

                    // Enumerate through each character and populate based upon how
                    // many characters are in the user input
                    foreach (char c in this.userInput)

                        //Add our password character
                        protect += "*";

                    // Draw our protected string instead of our input
                    spriteBatch.DrawString(font, protect, pos, Color.DarkRed);
                }

                // This is not password protected, so just draw our input
                else spriteBatch.DrawString(font, this.userInput, pos, Color.DarkRed);
            }
        }
    }
}
