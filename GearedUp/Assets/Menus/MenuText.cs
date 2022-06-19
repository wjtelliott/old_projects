using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    /// This class/object is used to just draw text to a menu screen.
    /// We need only a position and font, plus the menu this is
    /// attached to.
    /// </summary>
    public sealed class MenuText
    {
        /// <summary>
        /// What will our text say for this menu?
        /// </summary>
        readonly string text;

        /// <summary>
        /// Where will we draw this text?
        /// </summary>
        Vector2 position;

        /// <summary>
        /// What menu are we using this text for?
        /// </summary>
        public CurrentMenu Menu { get; set; }

        /// <summary>
        /// Construct our text object
        /// </summary>
        /// <param name="pos">The position for this object</param>
        /// <param name="txt">The text to display for this object</param>
        /// <param name="ontoMenu">The menu this object is on</param>
        public MenuText(Vector2 pos, string txt, CurrentMenu ontoMenu)
        {

            // Set up our fields from the given
            this.text = txt;
            this.position = pos;
            this.Menu = ontoMenu;
        }

        /// <summary>
        /// Draw our object. Only call this once per frame and only
        /// when drawing the menu for this object.
        /// </summary>
        /// <param name="spriteBatch">The game's sprite batch</param>
        /// <param name="font">The font used to draw this object</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

            // Draw our string in the position given.
            // TODO: Allow for colors?
            spriteBatch.DrawString(font, this.text, this.position, Color.DarkRed);
        }
    }
}
