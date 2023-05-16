using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    /// <summary>
    /// Represents JSON-LD model created event
    /// </summary>
    public class JsonLdCreatedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model">Created model</param>
        public JsonLdCreatedEvent(BaseNopModel model)
        {
            Model = model;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Created model
        /// </summary>
        public BaseNopModel Model { get; }

        #endregion
    }
}