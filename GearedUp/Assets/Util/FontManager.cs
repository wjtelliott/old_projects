using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Util
{
    /// <summary>
    /// Our font manager's main class
    /// </summary>
    public sealed class FontManager
    {
        /// <summary>
        /// Our game fonts as an array
        /// </summary>
        readonly SpriteFont[] fonts;

        /// <summary>
        /// Our font tags for naming them
        /// </summary>
        readonly string[] tags;

        /// <summary>
        /// Create our font manager with pipeline fonts and name values
        /// </summary>
        /// <param name="newFonts">Fonts from content pipeline</param>
        /// <param name="newTags">Name of fonts in string array</param>
        public FontManager(SpriteFont[] newFonts, string[] newTags)
        {
            // Set our values
            this.fonts = newFonts;
            this.tags = newTags;
        }

        /// <summary>
        /// Grab a font reference from a tagged name on the font manager
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <returns>The font reference as a SpriteFontDescription</returns>
        public SpriteFont GetFontByName(string name)
        {
            // Loop through all our tags and find the matching string name
            for (int i = 0; i < this.tags.Length; i++)
                if (this.tags[i] == name)

                    // Return our font with the correct name
                    return this.fonts[i];

            // We couldn't find the font. Now just return the first indexed font instead
            return this.fonts[0];
        }
    }
}
