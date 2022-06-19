using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GearedUpEngine.Assets.Util;
using System.Xml.Serialization;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2019 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Entities
{
    /// <summary>
    /// This is the base class for all Drawable objects in the Geared Up Engine
    /// </summary>
    public class Drawable
    {
        /// <summary>
        /// This is the rectangle in which we draw our object.
        /// If the texture for the object is not the same dimensions,
        /// use this.StretchTexture to fit inside the drawing space.
        /// </summary>
        public Rectangle DrawRectangle { get; set; }

        /// <summary>
        /// Our entity's texture
        /// </summary>
        [XmlIgnoreAttribute]
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Do we want to draw this entity? It will still update.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// This is the color applied to the texture atop the
        /// texture's colors. White = No filter
        /// </summary>
        public Color FilterColor { get; set; }

        /// <summary>
        /// This is the bounding box used by other game logic,
        /// hitboxes, walking boxes, etc
        /// </summary>
        public Rectangle BoundingBox { get; set; }

        /// <summary>
        /// Do we want to stretch our texture?
        /// </summary>
        public bool StretchTexture { get; set; }

        /// <summary>
        /// Custom X/Y with fields. If changing an entity's position, use this
        /// </summary>
        public DrawingPosition RawPosition { get; set; }

        /// <summary>
        /// An int used to identify an entity in a list
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Do we update our entity?
        /// </summary>
        public bool SkipLogic { get; set; }

        /// <summary>
        /// Our entity's name. Whitespace by default
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// New Drawable object with all default values.
        /// </summary>
        public Drawable()
        {
            this.Init();
        }

        /// <summary>
        /// Run this only on an object's creation. Sets all properties to
        /// default values.
        /// </summary>
        void Init()
        {
            this.DrawRectangle = new Rectangle(0, 0, 16, 16);
            //TEXTURE INIT ELSEWHERE
            this.IsVisible = false; //no texture
            this.FilterColor = Color.White;
            this.BoundingBox = new Rectangle(0, 0, 16, 16);
            this.StretchTexture = true;
            this.RawPosition = new DrawingPosition();
            this.EntityId = 0;
            this.SkipLogic = false;
            this.Name = "";
        }

        /// <summary>
        /// This sets the new width and height that we draw a texture.
        /// This does not change whether we stretch the texture or not.
        /// </summary>
        /// <param name="width">New draw width</param>
        /// <param name="height">New draw height</param>
        public void SetDrawSpace(int width, int height)
        {
            this.DrawRectangle = new Rectangle
                (
                    this.RawPosition.X,
                    this.RawPosition.Y,
                    width,
                    height
                );
        }

        /// <summary>
        /// Set our new bounding box's size.
        /// The X/Y will stay the same and the size will grow
        /// or shrink from the origin. (Top-left corner)
        /// </summary>
        /// <param name="width">The new width</param>
        /// <param name="height">The new height</param>
        public void SetBoundingBox(int width, int height)
        {
            this.BoundingBox = new Rectangle
                (
                    this.RawPosition.X,
                    this.RawPosition.Y,
                    width,
                    height
                );
        }

        /// <summary>
        /// Set our object's new location
        /// </summary>
        /// <param name="x">New X value</param>
        /// <param name="y">New Y value</param>
        public void SetLocation(int x, int y)
        {

            // Set our raw position to new value
            this.RawPosition.X = x;
            this.RawPosition.Y = y;

            // Move our drawing space to new position
            this.DrawRectangle = new Rectangle
                (
                    x, y,
                    this.DrawRectangle.Width,
                    this.DrawRectangle.Height
                );
        }

        /// <summary>
        /// Update our entity's logic
        /// </summary>
        public void Update()
        {
            //Is this object skipping logic?
            if (this.SkipLogic) return;

            //Update our draw space based upon our position
            this.DrawRectangle = new Rectangle
                (
                    this.RawPosition.X,
                    this.RawPosition.Y,
                    this.DrawRectangle.Width, // these values persist
                    this.DrawRectangle.Height
                );

            //Update our bounding box based upon our position
            this.BoundingBox = new Rectangle
                (
                    this.RawPosition.X,
                    this.RawPosition.Y,
                    this.BoundingBox.Width, // these values persist
                    this.BoundingBox.Height
                );
            
        }

        /// <summary>
        /// Draw the object.
        /// </summary>
        /// <param name="spriteBatch">Our Game's sprite batch object</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Are we visible? No? Return
            if (!this.IsVisible) return;

            //Is our texture initialised? No? Return, send debug warning
            if (this.Texture == null)
            {
                DeveloperDetails.Details += string.Format("Texture from entity {0} is nulled and tried to draw.;", this.EntityId.ToString());
                return;
            }

            //Do we want to stretch our texture across the drawspace?
            if (this.StretchTexture)
                spriteBatch.Draw(this.Texture, this.DrawRectangle, this.FilterColor);
            else //use the texture's width and height instead
                spriteBatch.Draw(this.Texture, 
                    new Rectangle(this.DrawRectangle.X, this.DrawRectangle.Y, this.Texture.Width, this.Texture.Height), 
                    this.FilterColor);
        }
    }

    public sealed class DrawingPosition
    {
        //My manager says he doesn't care about encapulation right now
        public int X { get; set; }
        public int Y { get; set; }
        public DrawingPosition() { this.X = this.Y = 0; }
        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }
    }
}
