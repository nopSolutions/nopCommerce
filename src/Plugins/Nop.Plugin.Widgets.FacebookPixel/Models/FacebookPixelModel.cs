using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a Facebook Pixel model
    /// </summary>
    public record FacebookPixelModel : BaseNopEntityModel
    {
        #region Ctor

        public FacebookPixelModel()
        {
            AvailableStores = new List<SelectListItem>();
            CustomEventSearchModel = new CustomEventSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId")]
        public string PixelId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.DisableForUsersNotAcceptingCookieConsent")]
        public bool DisableForUsersNotAcceptingCookieConsent { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.Store")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public bool HideStoresList { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching")]
        public bool UseAdvancedMatching { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties")]
        public bool PassUserProperties { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPageView")]
        public bool TrackPageView { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToCart")]
        public bool TrackAddToCart { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPurchase")]
        public bool TrackPurchase { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackViewContent")]
        public bool TrackViewContent { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToWishlist")]
        public bool TrackAddToWishlist { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackInitiateCheckout")]
        public bool TrackInitiateCheckout { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackSearch")]
        public bool TrackSearch { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackContact")]
        public bool TrackContact { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackCompleteRegistration")]
        public bool TrackCompleteRegistration { get; set; }

        public bool HideCustomEventsSearch { get; set; }

        public CustomEventSearchModel CustomEventSearchModel { get; set; }

        #endregion
    }
}