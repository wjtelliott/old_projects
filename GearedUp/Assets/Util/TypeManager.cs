using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Util
{
    /// <summary>
    /// The main type for our static Utility typing manager
    /// </summary>
    public static class TypeManager
    {

        /// <summary>
        /// Get a key array of newly pressed keys from the prior frame. This key array is automatically converted
        /// into a char array for use in a textbox. This should not be used as a system for key bindings.
        /// </summary>
        /// <param name="kState">The Current keyboard state</param>
        /// <param name="kStateOld">The prior keyboard state</param>
        /// <returns></returns>
        public static char[] GetKeys(KeyboardState kState, KeyboardState kStateOld)
        {
            // Initialize our char array
            char[] keysToReturn = new char[0];
            
            // Are we using capitalized letters?
            bool shiftPressed = 
                (kState.IsKeyDown(Keys.LeftShift) || kState.IsKeyDown(Keys.RightShift) || kState.CapsLock) ? 
                true : false;

            // Loop through each key that is pressed down, if it
            // wasn't in the last update, add it to a list of newly pressed keys.
            // IMPORTANT: Make sure to tokenize the key to a char for later use in strings
            // for text box's, game chats, etc.
            foreach (Keys k in kState.GetPressedKeys())
                if (!kStateOld.GetPressedKeys().Contains<Keys>(k))
                    AppendToArray<char>(ref keysToReturn, TokenizeKey(k, shiftPressed));
       

            //No new Characters found
            return keysToReturn;
        }
        
        /// <summary>
        /// Append a new object to the end of an array
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="array">The array to resize and append to</param>
        /// <param name="addition">The new addition to the array</param>
        static void AppendToArray<T>(ref T[] array, T addition)
        {
            // Resize our array to the new length. (1 index longer)
            Array.Resize<T>(ref array, array.Length + 1);
            // Add the addition onto the last index of the array.
            array[array.Length - 1] = addition;
        }

        /// <summary>
        /// Tokenize the Key input as a char for use in a string.
        /// This is helpful for text boxes, and chats
        /// </summary>
        /// <param name="input">The key to tokenize</param>
        /// <param name="shift">Are we using shift or caps lock? True = yes</param>
        /// <returns></returns>
        static char TokenizeKey(Keys input, bool shift)
        {
            // All basic characters
            if (input.ToString().Length < 2)
                return (shift) ?
                    input.ToString().ToUpper().ToCharArray()[0]
                    :
                    input.ToString().ToLower().ToCharArray()[0];

            // All numbers excluding numpad numbers
            else if (input.ToString().Length == 2 &&
                input.ToString().Substring(0, 1) == "D")
                return input.ToString().ToCharArray()[1];

            // Extras
            switch (input)
            {
                default:
                    return char.MinValue;
                case Keys.Space:
                    return ' ';
                case Keys.Back:
                    return char.MaxValue;
                case Keys.OemPeriod:
                    return '.';
            }
        }
    }
}