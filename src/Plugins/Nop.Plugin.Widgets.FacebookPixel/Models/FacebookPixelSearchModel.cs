using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a Facebook Pixel search model
    /// </summary>
    public record FacebookPixelSearchModel : BaseSearchModel
    {
        #region Ctor

        public FacebookPixelSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Search.Store")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }

        public bool HideSearchBlock { get; set; }

        #endregion
    }
}