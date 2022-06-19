/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2019 by: William E
/// Do not modify.
/// </summary>
namespace GearedUpEngine.Assets.Entities
{
    /// <summary>
    /// This is the base class for all Static objects
    /// </summary>
    public class StaticEntity : Drawable
    {
        /// <summary>
        /// This entity does not update, and only uses this void to hide it's inherited Update function.
        /// </summary>
        public void Update() { return; }
    }
}
