using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a custom event search model
    /// </summary>
    public partial record CustomEventSearchModel : BaseSearchModel
    {
        #region Ctor

        public CustomEventSearchModel()
        {
            AddCustomEvent = new CustomEventModel();
        }

        #endregion

        #region Properties

        public int ConfigurationId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Search.WidgetZone")]
        public string WidgetZone { get; set; }

        public CustomEventModel AddCustomEvent { get; set; }

        #endregion
    }
}