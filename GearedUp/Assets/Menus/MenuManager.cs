using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
    /// Our menus in our game
    /// </summary>
    public enum CurrentMenu
    {
        Main,
        Settings,
        Play,
        Login,
        ServerSettings
    }

    /// <summary>
    /// Our menu manager's main class. We use this to
    /// draw and update all main menus for the game.
    /// </summary>
    public sealed class MenuManager
    {

        /// <summary>
        /// All of our menu buttons for all menus
        /// </summary>
        MenuButton[] buttons;

        /// <summary>
        /// All of our menu texts for all menus
        /// </summary>
        MenuText[] texts;

        /// <summary>
        /// All of our menu text boxes for all menus
        /// </summary>
        MenuTextbox[] textBoxes;

        /// <summary>
        /// The current menu that we are displaying to the user
        /// </summary>
        public CurrentMenu CurrentMenu { get; set; }

        public Boolean StartListenServer { get; set; }
        public Boolean StartDedicatedServer { get; set; }

        /// <summary>
        /// Construct our menu manager and load our menus into memory.
        /// </summary>
        /// <param name="buttonTexture">The texture to use for the menu buttons</param>
        public MenuManager(Texture2D buttonTexture)
        {

            // Set up our default values and load our menus
            this.CurrentMenu = CurrentMenu.Main;
            this.buttons = new MenuButton[0];
            this.texts = new MenuText[0];
            this.textBoxes = new MenuTextbox[0];
            this.StartListenServer = false;
            this.StartDedicatedServer = false;

            // Load our menus in our arrays.
            LoadMainMenu(buttonTexture);
            LoadLoginMenu(buttonTexture);
            LoadServerMenu(buttonTexture);
        }

        /// <summary>
        /// Grab our user input from a textbox from a menu from the provided name.
        /// </summary>
        /// <param name="textBoxName">The name of the text box</param>
        /// <returns></returns>
        public string GetUserInputString(string textBoxName)
        {

            // Loop through each text box
            foreach (MenuTextbox tb in this.textBoxes)

                // Check if our name matches the given
                if (tb.Name == textBoxName)

                    // Return the user input on that text box
                    return tb.Input;

            // We found no textbox with that name, return nil
            return "nil";
        }

        /// <summary>
        /// Update our menu manager, and thereof the menu objects with it.
        /// </summary>
        /// <param name="mState">The current mouse state for this frame</param>
        /// <param name="kState">The current keyboard state for this frame</param>
        public void Update(MouseState mState, KeyboardState kState)
        {

            // Loop through each button
            foreach (MenuButton btn in this.buttons)

                // If it is part of the current menu, update it
                if (btn.Menu == this.CurrentMenu)
                    btn.Update(mState);

            // Loop through each button - TODO: Combine these?
            foreach (MenuButton btn in this.buttons)

                // If it is a part of the current menu
                if (btn.Menu == this.CurrentMenu)

                    // If the button was clicked, perform its action
                    if (btn.DoAction)

                        // Check the action of the button
                        switch (btn.Tag)
                        {

                            // We clicked on submit to multiplayer
                            case "play":

                                // Change to the play setting, the game lauch will update accordingly
                                // to connect, display the map, etc
                                // Reset the button
                                this.CurrentMenu = CurrentMenu.Play;
                                btn.DoAction = false;
                                break;

                            // We clicked to exit game
                            case "nil":
                                Environment.Exit(0);
                                break;

                            // We clicked to go to the main menu
                            case "pop":

                                // Change to the main menu, reset action
                                this.CurrentMenu = CurrentMenu.Main;
                                btn.DoAction = false;
                                break;

                            // We clicked to go to the login to multiplayer screen
                            case "push_login":

                                // Change to the login menu, reset action
                                this.CurrentMenu = CurrentMenu.Login;
                                btn.DoAction = false;
                                break;

                            case "push_server":

                                // Change menu to the local listen server/ dedicated server
                                this.CurrentMenu = CurrentMenu.ServerSettings;
                                btn.DoAction = false;
                                break;

                            case "startServerListen":

                                // Tell the game launch to start the listen server
                                this.StartListenServer = true;
                                btn.DoAction = false;
                                break;

                            case "startServerDedicated":

                                // Tell the game launch to start the dedicated server
                                this.StartDedicatedServer = true;
                                btn.DoAction = false;
                                break;
                        }

            // Loop through our text boxes
            foreach (MenuTextbox tb in this.textBoxes)
            {

                // If it is on the menu we are current viewing
                if (this.CurrentMenu == tb.Menu)
                {

                    // If the object is not focused
                    if (!tb.IsFocused)
                    {

                        // Update our object
                        tb.Update(kState, mState);

                        // If we are now focused, remove focus of all other text boxes
                        if (tb.IsFocused)
                        {

                            // Loop through all text boxes
                            foreach (MenuTextbox tb2 in this.textBoxes)

                                // If it is not this one
                                if (tb2.Name != tb.Name)

                                    // Remove focus
                                    tb2.IsFocused = false;

                            // Break from our loop, we don't
                            // need to update our other text boxes if this one was focused
                            break;
                        }
                    }

                    // This one is already focused, just update it
                    else tb.Update(kState, mState);
                }
            }
        }

        /// <summary>
        /// Draw our currently displayed menu
        /// </summary>
        /// <param name="spriteBatch">The game's sprite batch</param>
        /// <param name="menuFont">The font used for this menu</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
        {

            // Loop through each button
            foreach (MenuButton btn in this.buttons)

                // If it is on the menu we are currently viewing, draw it
                if (this.CurrentMenu == btn.Menu)
                    btn.Draw(spriteBatch);

            // Loop through each text
            foreach (MenuText txt in this.texts)

                // If it is on the menu we are currently viewing, draw it
                if (this.CurrentMenu == txt.Menu)
                    txt.Draw(spriteBatch, menuFont);

            // Loop through each text box
            foreach (MenuTextbox tb in this.textBoxes)

                // If it is on the menu we are currently viewing, draw it
                if (this.CurrentMenu == tb.Menu)
                    tb.Draw(spriteBatch, menuFont);
        }

        /// <summary>
        /// Load our login to multiplayer menu into memory
        /// </summary>
        /// <param name="btnTexture">The menu's button texture to use</param>
        void LoadServerMenu(Texture2D btnTexture)
        {
            // TODO: Change all menu's object positions to dynamic positions based upon resolution (canvas size)

            // Create our buttons
            MenuButton[] newButtons = new MenuButton[]
            {

                // button ( texture, position as Rect, action as string, menu )
                new MenuButton(btnTexture, new Rectangle(650, 400, 50, 35), "startServerListen", CurrentMenu.ServerSettings),
                new MenuButton(btnTexture, new Rectangle(150, 400, 50, 35), "startServerDedicated", CurrentMenu.ServerSettings),
                new MenuButton(btnTexture, new Rectangle(10, 400, 50, 35), "pop", CurrentMenu.ServerSettings)
            };

            // Create our basic text
            MenuText[] newTexts = new MenuText[]
            {

                // text ( position as vector2, text as string, menu )
                new MenuText(new Vector2(150, 10), "Click to start listen or dedicated server", CurrentMenu.ServerSettings),
                new MenuText(new Vector2(650, 450), "Start Listen Server", CurrentMenu.ServerSettings),
                new MenuText(new Vector2(150, 450), "Start Dedicated Server", CurrentMenu.ServerSettings),
                new MenuText(new Vector2(10, 450), "BACK", CurrentMenu.ServerSettings),
            };


            // Create our text boxes, must have unique names for each one, including other menus
            MenuTextbox[] newTextBoxes = new MenuTextbox[]
            {
                
            };
            

            // Append our menu to the array in our class
            AppendToArray<MenuButton>(ref this.buttons, newButtons);
            AppendToArray<MenuText>(ref this.texts, newTexts);
            AppendToArray<MenuTextbox>(ref this.textBoxes, newTextBoxes);
        }

        /// <summary>
        /// Load our login to multiplayer menu into memory
        /// </summary>
        /// <param name="btnTexture">The menu's button texture to use</param>
        void LoadLoginMenu(Texture2D btnTexture)
        {
            // TODO: Change all menu's object positions to dynamic positions based upon resolution (canvas size)

            // Create our buttons
            MenuButton[] newButtons = new MenuButton[]
            {

                // button ( texture, position as Rect, action as string, menu )
                new MenuButton(btnTexture, new Rectangle(650, 400, 50, 35), "play", CurrentMenu.Login),
                new MenuButton(btnTexture, new Rectangle(10, 400, 50, 35), "pop", CurrentMenu.Login)
            };

            // Create our basic text
            MenuText[] newTexts = new MenuText[]
            {

                // text ( position as vector2, text as string, menu )
                new MenuText(new Vector2(150, 10), "Login to server:", CurrentMenu.Login),
                new MenuText(new Vector2(650, 450), "SUBMIT", CurrentMenu.Login),
                new MenuText(new Vector2(10, 450), "BACK", CurrentMenu.Login),

                new MenuText(new Vector2(10, 150), "USERNAME", CurrentMenu.Login),
                new MenuText(new Vector2(10, 200), "PASSWORD", CurrentMenu.Login),
                new MenuText(new Vector2(10, 250), "SERVER PASSWORD", CurrentMenu.Login),
                new MenuText(new Vector2(10, 275), "(if applicable)", CurrentMenu.Login),
            };

            
            // Create our text boxes, must have unique names for each one, including other menus
            MenuTextbox[] newTextBoxes = new MenuTextbox[]
            {

                // text box ( unique name as string, position as Rect, menu )
                new MenuTextbox("usernameTextBox", new Rectangle(250, 150, 250, 50), CurrentMenu.Login),
                new MenuTextbox("passwordTextBox", new Rectangle(250, 200, 250, 50), CurrentMenu.Login),
                new MenuTextbox("serverTextBox", new Rectangle(400, 10, 250, 50), CurrentMenu.Login)
            };
            

            // Password protect our password text box
            newTextBoxes[1].PasswordProtect = true;

            // Append our menu to the array in our class
            AppendToArray<MenuButton>(ref this.buttons, newButtons);
            AppendToArray<MenuText>(ref this.texts, newTexts);
            AppendToArray<MenuTextbox>(ref this.textBoxes, newTextBoxes);
        }

        /// <summary>
        /// Load our main menu objects in memory to our array
        /// </summary>
        /// <param name="btnTexture">The texture for the buttons on this menu</param>
        void LoadMainMenu(Texture2D btnTexture)
        {

            // Create our buttons
            MenuButton[] newButtons = new MenuButton[]
            {

                // button ( texture, position as Rect, action as string, menu )
                new MenuButton(btnTexture, new Rectangle(90, 150, 50, 35), "push_login", CurrentMenu.Main),
                new MenuButton(btnTexture, new Rectangle(90, 200, 50, 35), "nil", CurrentMenu.Main),
                new MenuButton(btnTexture, new Rectangle(320, 250, 50, 35), "push_server", CurrentMenu.Main)
            };

            // Create our basic text
            MenuText[] newTexts = new MenuText[]
            {

                // text ( position as vector2, text as string, menu )
                new MenuText(new Vector2(300, 10), "ENDURE", CurrentMenu.Main),
                new MenuText(new Vector2(10, 150), "PLAY", CurrentMenu.Main),
                new MenuText(new Vector2(10, 250), "SERVER SETTINGS", CurrentMenu.Main),
                new MenuText(new Vector2(10, 200), "QUIT", CurrentMenu.Main)
            };
            
            // Append our objects to the class array
            AppendToArray<MenuButton>(ref this.buttons, newButtons);
            AppendToArray<MenuText>(ref this.texts, newTexts);
        }

        /// <summary>
        /// Append an object array to the end of another object array.
        /// We use this in our MenuManager class to append our menu objects from one
        /// menu array in a func to the class array for later use in the game.
        /// </summary>
        /// <typeparam name="T">The type of object array ( MenuButton, MenuText, MenuTextbox )</typeparam>
        /// <param name="oldObjects">The array to append to</param>
        /// <param name="newObjects">The object(s) to append to the end of the array</param>
        void AppendToArray<T>(ref T[] oldObjects, T[] newObjects)
        {

            // Resize our referenced array to include the length of the new objects
            Array.Resize<T>(ref oldObjects, oldObjects.Length + newObjects.Length);

            // Init a variable for indexing
            int index = 0;

            // Loop through the null added objects in the array resizing
            for (int i = oldObjects.Length - newObjects.Length; i < oldObjects.Length; i++)
            {
                
                // Add data to the nulled objects by index of the new objects
                oldObjects[i] = newObjects[index];

                // Increase our index to loop through all new objects
                index++;
            }
        }
    }
}
