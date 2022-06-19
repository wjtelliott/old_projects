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
namespace GearedUpEngine.Assets.Map
{

    /// <summary>
    /// Our main tile map class used to draw the game world
    /// </summary>
    public sealed class Tilemap
    {

        /// <summary>
        /// Our tiles as an array
        /// </summary>
        public Tile[] Tiles { get; set; }

        /// <summary>
        /// The width and height of our tile map
        /// </summary>
        int width, height;
        public Tilemap()
        {

            // Init our defaults, setup the map
            this.width = 1; this.height = 1;
            this.SetupMap();
        }

        /// <summary>
        /// Change our map to 'flatgrass' with the given width and height
        /// </summary>
        /// <param name="newWidth">The new map width</param>
        /// <param name="newHeight">The new map height</param>
        public void ChangeMap_Flatgrass(int newWidth, int newHeight)
        {

            // Set our width and height to the new values
            this.width = newWidth;
            this.height = newHeight;

            // Setup our map's textures
            this.SetupMap();
        }


        /// <summary>
        /// Redo our map's textures, as well as set our tile locations to the new width and height
        /// </summary>
        void SetupMap()
        {
            // Init our index variable
            int index = 0;

            // Init our tiles with a new length
            this.Tiles = new Tile[width * height];

            // Loop through the X plane
            for (int x = 0; x < this.width; x++)

                // Loop through the Y plane
                for (int y = 0; y < this.height; y++)
                {

                    // Initialize the tile with it's new index
                    this.Tiles[index] = new Tile();

                    // Setup the tile's location
                    this.Tiles[index].SetLocation(x, y);

                    // Serialize the texture (flip flop the textures)
                    this.Tiles[index].TextureSerialization = (index % 2 == 0) ? "grass_left" : "grass_right";

                    // Increase our texture's loop
                    index++;
                }
        }

        /// <summary>
        /// Draw our game world. Draw this only once per frame, and only while the world is being drawn
        /// </summary>
        /// <param name="spriteBatch">The game's spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            
            // Loop through all tiles
            foreach (Tile t in this.Tiles)

                // Draw the tile
                t.Draw(spriteBatch);
        }
    }
}
