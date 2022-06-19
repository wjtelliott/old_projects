using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Util
{
    /// <summary>
    /// The main class for our manager to handle our splash screens.
    /// </summary>
    public sealed class SplashManager
    {
        /// <summary>
        /// All of our splash screens into a texture array.
        /// We will fade to black, then display the next until
        /// we have displayed them all or chosen to skip.
        /// </summary>
        Texture2D[] gameSplashes;

        /// <summary>
        /// Some int32 counters and markers
        /// </summary>
        int lengthPerSplash, currentSlide, counter;

        /// <summary>
        /// Are we allowed to skip these splashes?
        /// </summary>
        bool canSkip;

        /// <summary>
        /// Our alloted drawing space
        /// </summary>
        Rectangle drawSpace;

        /// <summary>
        /// Our drawing color filter
        /// </summary>
        Color drawColor;

        /// <summary>
        /// Are we finished showing all the splash screens?
        /// </summary>
        public bool IsFinished;

        /// <summary>
        /// Create our manager with default values and the given splash textures.
        /// </summary>
        /// <param name="splashes">All the displayed splash textures</param>
        /// <param name="length">The length to display them, in ms</param>
        /// <param name="skip">Allow the user to skip the splash screens. Yes = True</param>
        public SplashManager(Texture2D[] splashes, int length, bool skip)
        {

            // Allow the drawing space to consume the whole window.
            // TODO: pass the graphics device info for the window size and 
            // ensure that the splash screens encompass the entire window, no
            // matter the resolution.
            this.drawSpace = new Rectangle(0, 0, 800, 480);
            
            // Given values
            this.canSkip = skip;
            this.lengthPerSplash = length;
            this.gameSplashes = splashes;

            // Defaults
            this.IsFinished = false;
            this.currentSlide = 0;
            this.counter = 0;
        }

        /// <summary>
        /// Update the splash manager to fade and continue.
        /// </summary>
        public void Update(KeyboardState kState)
        {
            // If we press any key. Skip all splashes
            if (this.canSkip && kState.GetPressedKeys().Length > 0)
                this.IsFinished = true;

            // If we are done, or chose to skip, don't continue with any more logic.
            if (this.IsFinished) return;

            // Add to our counter, if we are at the max for the length of this slide, reset our counter
            this.counter = (this.counter + 1 > this.lengthPerSplash) ? 0 : this.counter + 1;

            // Is our counter reset? If so, check if there's a next slide after this once,  if there is, move to the next slide.
            this.currentSlide = 
                (this.counter == 0) ? 
                    (this.currentSlide + 1 > this.gameSplashes.Length - 1) ? 
                        this.currentSlide 
                        : 
                        this.currentSlide + 1 
                    : 
                    this.currentSlide;

            // Fade in from black. Clamp the int to make sure we don't go outside 0-255 for the color data.
            this.drawColor = new Color(ClampColor(counter), ClampColor(counter), ClampColor(counter), ClampColor(counter));

            // If we are on the last slide, and our counter is finished. We are done.
            if (this.currentSlide == this.gameSplashes.Length - 1 && this.counter >= this.lengthPerSplash)
                this.IsFinished = true;
        }

        /// <summary>
        /// Clamp our color counter to force it to
        /// stay within 0-255 to avoid a JIT error.
        /// </summary>
        /// <param name="counter">Our fade counter</param>
        /// <returns>The new fade amount</returns>
        int ClampColor(int counter)
        {
            // If the length is longer, multiply the speed by 2
            if (this.lengthPerSplash >= 255)
                return Math.Min(counter * 2, 255);

            // If the fade is short, go faster for more fade effect
            return Math.Min(counter * 3, 255);
        }

        /// <summary>
        /// Draw our splash screen
        /// </summary>
        /// <param name="spriteBatch">Game's sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // If we are finished showing our splashes, don't draw anything.
            if (this.IsFinished) return;

            // Draw our splash screen of our current slide.
            spriteBatch.Draw(this.gameSplashes[this.currentSlide], this.drawSpace, this.drawColor);
        }
    }
}
