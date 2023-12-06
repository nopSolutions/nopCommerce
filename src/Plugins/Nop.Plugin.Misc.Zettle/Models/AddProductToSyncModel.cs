using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a product model to add for synchronization
    /// </summary>
    public record AddProductToSyncModel : BaseNopModel
    {
        #region Ctor

        public AddProductToSyncModel()
        {
            SelectedProductIds = new List<int>();
        }

        #endregion

        #region Properties

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}