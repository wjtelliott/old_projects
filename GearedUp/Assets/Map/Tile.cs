using GearedUpEngine.Assets.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Map
{

    /// <summary>
    /// Our tile's main class
    /// </summary>
    public sealed class Tile
    {
        /// <summary>
        /// How big are our tile's size going to be? Not related to texture size,
        /// but should be the same as the texture size
        /// </summary>
        const int tileSize = 16;

        /// <summary>
        /// Can we walk through this tile?
        /// </summary>
        public bool IsSolid { get; set; }

        /// <summary>
        /// The tile's texture. We ignore XML serialization here, since we can't serialize
        /// a texture anyway
        /// </summary>
        [XmlIgnoreAttribute]
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The tile's heightmap. We will draw this after we draw other world entities.
        /// This will give a 'height' to the world. Ignore XML also
        /// </summary>
        [XmlIgnoreAttribute]
        public Texture2D[] Heightmap { get; set; }

        /// <summary>
        /// Since we can't serialize a texture, we can with a string and call the texture
        /// manager after initialization with this string as the texture name, to get our
        /// texture. It's sloppy, but it works.
        /// </summary>
        public string TextureSerialization { get; set; }

        /// <summary>
        /// The same as the texture serialzation, but for the height map. Use an array to grab multiple textures
        /// </summary>
        public string[] HeightmapSerialization { get; set; }

        /// <summary>
        /// The position in the world using X/Y.
        /// </summary>
        public Rectangle WorldPosition { get; set; }

        /// <summary>
        /// The position in the tilemap using X/Y, IE 0,0 - 0,1 - 0,2 etc
        /// </summary>
        public Vector2 PositionInTilemap { get; set; }

        /// <summary>
        /// The name of tile, we can use this to add/remove tiles or manipulate them later in the game.
        /// Any tile with the name 'tile.essential' is a tile is a core map tile and shouldn't be manipulated
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The filter color to draw with the tile's texture. White -> no filter
        /// </summary>
        public Color FilterColor { get; set; }

        /// <summary>
        /// Construct a tile. No args, so we can serialize and deserialize with XML
        /// </summary>
        public Tile()
        {

            // Init our default values
            this.IsSolid = false;
            this.TextureSerialization = "grass_left";
            this.HeightmapSerialization = new string[] { };
            this.WorldPosition = new Rectangle(0, 0, 16, 16);
            this.PositionInTilemap = new Vector2(0, 0);
            this.Name = "tile.essential";
            this.FilterColor = Color.White;
        }

        /// <summary>
        /// Change the location of our in the tilemap, not the world position.
        /// The world position will be updated afterwards.
        /// </summary>
        /// <param name="x">New tilemap position X</param>
        /// <param name="y">New tilemap position Y</param>
        public void SetLocation(int x, int y)
        {
            
            // Change the tile's position in the tilemap
            this.PositionInTilemap = new Vector2(x, y);

            // Update our world position to reflect the new tilemap position
            this.WorldPosition =
                (y % 2 == 0) ?
                new Rectangle(x * tileSize, y * (tileSize / 2), tileSize, tileSize) :
                new Rectangle((x * tileSize), y * (tileSize / 2), tileSize, tileSize);
        }

        /// <summary>
        /// Straight override our texture with a new Texture2D object
        /// </summary>
        /// <param name="newTexture">The new texture to use</param>
        public void OverrideTexture(Texture2D newTexture)
        { this.Texture = newTexture; }

        /// <summary>
        /// Straight override the heightmap with a new texture array
        /// </summary>
        /// <param name="newHeightmap">The new textures to use</param>
        public void OverrideHeightmap(Texture2D[] newHeightmap)
        { this.Heightmap = newHeightmap; }

        /// <summary>
        /// Change if this tile is solid, can an Actor pass over it?
        /// </summary>
        /// <param name="newS">True = sold, cannot pass. False = Actors may pass through</param>
        public void SetSolid(bool newS)
        { this.IsSolid = newS; }

        /// <summary>
        /// Does this tile contain a Vector2 location?
        /// </summary>
        /// <param name="position">The Vector2 location to check</param>
        /// <returns>Yes = true, No = false</returns>
        public bool Contains(Vector2 position)
        { return this.WorldPosition.Contains(position) ? true : false; }

        /// <summary>
        /// Serialize our texture with our serialization string in this object
        /// </summary>
        /// <param name="txm">The game's texture manager</param>
        public void SerializeTexture(TextureManager txm)
        { this.Texture = txm.GetTextureByName(this.TextureSerialization); }

        /// <summary>
        /// Serialize our heightmap with our heightmap serialization string in this object
        /// </summary>
        /// <param name="txm">The game's texture manager</param>
        public void SerializeHeightmap(TextureManager txm)
        {

            // Init our heightmap with the same length as the serialization length
            this.Heightmap = new Texture2D[this.HeightmapSerialization.Length];

            // Loop through all heightmap index's
            for (int i = 0; i < this.Heightmap.Length; i++)

                // Get the texture from name of the serialization from texture manager
                this.Heightmap[i] = txm.GetTextureByName(this.HeightmapSerialization[i]);
        }

        /// <summary>
        /// Draw our tile to screen. Only do this once per frame, and only while the game world is being drawn.
        /// </summary>
        /// <param name="spriteBatch">The game's sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // Check if our texture is not serialized
            if (this.Texture == null)
            {

                // We have a null texture, tell the developer console, and do not attempt to draw.
                DeveloperDetails.Details += "Tile tried to draw without a texture!;";
                return;
            }

            // Draw our texture normally
            spriteBatch.Draw(this.Texture, this.WorldPosition, this.FilterColor);
        }

        /// <summary>
        /// Draw our tile's heightmap to screen. Only draw the once per frame, after all entities have been drawn, and only while the game
        /// world is being drawn.
        /// </summary>
        /// <param name="spriteBatch">The game's sprite batch</param>
        public void DrawHeightmap(SpriteBatch spriteBatch)
        {

            // Loop through all heightmap textures
            for (int i = 0; i < this.Heightmap.Length; i++)
            {

                // Check to make sure our texture isn't null, and has been serialized
                if (this.Heightmap[i] == null)
                {

                    // We have a null heightmap, don't draw this texture, attempt to finish the array though
                    DeveloperDetails.Details += "Tile tried to draw heightmap without a texture!;";
                    continue;
                }

                // Create a new drawing position = the tile's world position
                Rectangle drawPosition = this.WorldPosition;

                // Offset the drawing position by the tile's texture size and the index of the texture
                drawPosition.Y -= (i * tileSize) - tileSize;

                // Draw the heightmap
                spriteBatch.Draw(this.Heightmap[i], drawPosition, this.FilterColor);
            }
        }
    }
}
