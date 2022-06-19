using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Util
{
    /// <summary>
    /// Our texture resource provider
    /// </summary>
    public sealed class TextureManager
    {
        /// <summary>
        /// These serve as our one image for our tileset (from pipeline)
        /// and our texture array gained from splitting the textures into
        /// smaller tiles.
        /// </summary>
        public Texture2D[] Game_Tiles;
        public Texture2D Game_Tileset;

        public Texture2D InventoryTexture;

        public Texture2D[] PlayerAnimations;

        /// <summary>
        /// Used as a form of progress
        /// </summary>
        int amountOfTexturesLoaded;

        /// <summary>
        /// Tile sprite parameters
        /// </summary>
        int tileWidth, tileHeight, tileSetAmountX, tileSetAmountY;

        /// <summary>
        /// How many textures have we loaded vs. amount of textures left to load.
        /// </summary>
        /// <returns>Progress of textures loaded</returns>
        public string Progress()
        {
            return this.amountOfTexturesLoaded.ToString();
        }

        public TextureManager(ContentManager content, GraphicsDevice gfx, int defaultTileWidth = 16, int defaultTileHeight = 16)
        {
            // Make this the game's tileset from Content pipeline
            this.Game_Tileset = content.Load<Texture2D>("Assets/Debug/debug_tileset_dev1");

            // Load inventory debug texture
            this.InventoryTexture = content.Load<Texture2D>("Assets/Debug/inven_basic");

            // Load new player animations, screw the debug DOT
            this.PlayerAnimations = new Texture2D[]
            {
                content.Load<Texture2D>("Assets/Debug/player_idle"),
                content.Load<Texture2D>("Assets/Debug/player_pistol_idle")
            };

            // Haven't loaded any textures yet
            this.amountOfTexturesLoaded = 0;
            
            // Set our tile sprite's default values
            this.tileWidth = defaultTileWidth;
            this.tileHeight = defaultTileHeight;

            // Get our amount of tiles on the X and Y plane based upon the
            // tileset width and height vs the tile's width and height.
            this.tileSetAmountX = this.Game_Tileset.Width / defaultTileWidth;
            this.tileSetAmountY = this.Game_Tileset.Height / defaultTileHeight;

            // Check how many tiles are in this tileset and
            // initialize correctly.
            {
                int amountOfTiles =
                    (this.tileSetAmountX) *
                    (this.tileSetAmountY);
                this.Game_Tiles = new Texture2D[amountOfTiles];
            }


            // Loop through our X and Y plane of the tileset and create
            // our tile textures from source targets of our tileset image.
            {
                // Init our X/Y plane
                int x = 0, y = 0;

                // Loop through until we have all tiles
                for (int i = 0; i < this.Game_Tiles.Length; i++)
                {
                    // Create a new texture with the same height and width of our
                    // default tile values. Use the graphics device info to create the texture.
                    this.Game_Tiles[i] = new Texture2D(gfx, this.tileWidth, this.tileHeight);

                    
                    {
                        // Grab our location of the texture for the tile using the x/y plane we created
                        // and our tile's values
                        Rectangle textureLocation = new Rectangle
                            (
                                x * this.tileWidth,
                                y * this.tileHeight,
                                this.tileWidth,
                                this.tileHeight
                            );

                        // Set the Color data (pixels) to the texture from the source rectangle on the tileset
                        this.Game_Tiles[i].SetData<Color>(GetTextureFromTileset(textureLocation));

                        // We loaded +1 textures
                        this.amountOfTexturesLoaded++;

                        // Comment out this, but save it to simulate long load times.
                        //System.Threading.Thread.Sleep(3);
                    }

                    // Advance our X/Y plane to the right.
                    // If we can't, we go down one row.
                    // If we can't, we've finished our loop
                    x = (x + 1 > this.tileSetAmountX) ? 0 : x + 1;
                    y = (x == 0) ? y + 1 : y;
                }
            }

            // Append a tag to all textures, giving them a name for later
            for (int i = 0; i < this.textureNames.Length; i++)
                this.Game_Tiles[i].Tag = this.textureNames[i];
        }

        /// <summary>
        /// Get a texture from memory by name from the tag on the texture 2d object.
        /// </summary>
        /// <param name="textureName">The name of the texture we wish to retrieve</param>
        /// <returns>The first Texture2D object from the name given</returns>
        public Texture2D GetTextureByName(string textureName)
        {
            // Loop through each texture in our tiles
            foreach (Texture2D text in this.Game_Tiles)

                // If we find a texture that doesn't have a null tag, and the tag
                // matches the string we are given
                if (text.Tag != null && text.Tag.ToString() == textureName)

                    // return that texture
                    return text;

            // We didn't find that texture
            return null;
        }

        /// <summary>
        /// Get texture color[] data from a source rectangle on that texture.
        /// </summary>
        /// <param name="textureLocation">The location of the color[] data to grab</param>
        /// <returns>A color array of data from the source rectangle of the original texture</returns>
        Color[] GetTextureFromTileset(Rectangle textureLocation)
        {
            // TODO: We are calling and creating a new array every single time we need
            // to create one new texture. This is virtualy redo-ing the tileset EVERY
            // texture. We could shave off a few seconds of load time by only doing this once?

            // Create a color array to match the tileset size
            Color[] sourceData = new Color[this.Game_Tileset.Width * this.Game_Tileset.Height];

            // Get all color data from the tileset
            this.Game_Tileset.GetData<Color>(sourceData);

            // Create new Data for our texture we are grabbing
            Color[] data = new Color[textureLocation.Width * textureLocation.Height];

            // Loop through our source data and transfer data in the texture location
            // to the new data array
            for (int x = 0; x < textureLocation.Width; x++)
                for (int y = 0; y < textureLocation.Height; y++)
                    data[x + y * textureLocation.Width] = sourceData[x + textureLocation.X + (y + textureLocation.Y) * this.Game_Tileset.Width];

            // return the new data array to be used to create a new texture
            return data;
        }

        /// <summary>
        /// The names of all of our textures in the tilemap.
        /// Make sure to keep this updated.
        /// </summary>
        string[] textureNames = new string[]
        {
            "grass_left",
            "grass_right",
            "player_debug"
        };
    }
}

