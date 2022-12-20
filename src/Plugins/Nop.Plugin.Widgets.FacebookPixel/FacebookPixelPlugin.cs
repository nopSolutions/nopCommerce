using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.FacebookPixel.Components;
using Nop.Plugin.Widgets.FacebookPixel.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.FacebookPixel
{
    /// <summary>
    /// Represents Facebook Pixel plugin
    /// </summary>
    public class FacebookPixelPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly FacebookPixelService _facebookPixelService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public FacebookPixelPlugin(FacebookPixelService facebookPixelService,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            WidgetSettings widgetSettings)
        {
            _facebookPixelService = facebookPixelService;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(FacebookPixelDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            var widgetZones = new List<string> { PublicWidgetZones.HeadHtmlTag };
            widgetZones.AddRange(await _facebookPixelService.GetCustomEventsWidgetZonesAsync());
            
            return widgetZones;
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            return typeof(FacebookPixelViewComponent);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.FacebookPixel.Configuration"] = "Configuration",
                ["Plugins.Widgets.FacebookPixel.Configuration.CookieSettingsWarning"] = "It looks like you have <a href=\"{0}\" target=\"_blank\">DisplayEuCookieLawWarning</a> setting disabled.",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents"] = "Configure custom events",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.SaveBeforeEdit"] = "You need to save the configuration before edit custom events.",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.EventName"] = "Event name",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.EventName.Hint"] = "Enter a name of the custom event (e.g. BlogView).",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.WidgetZones"] = "Widget zones",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.WidgetZones.Hint"] = "Choose widget zones in which the custom event will be tracked (e.g. blogpost_page_top).",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Search.WidgetZone"] = "Widget zone",
                ["Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Search.WidgetZone.Hint"] = "Search custom events by the widget zone.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken"] = "Access token",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Hint"] = "Enter the Facebook Conversions API access token.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"] = "Access token is required",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.DisableForUsersNotAcceptingCookieConsent"] = "Disable for users not accepting Cookie Consent",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.DisableForUsersNotAcceptingCookieConsent.Hint"] = "Check to disable the Facebook Pixel for users not accepting Cookie Consent. You may want this if you conduct business in countries that are subject to General Data Protection Regulation (GDPR). You also need to activate the \"DisplayEuCookieLawWarning\" setting on the General settings page in order to display Cookie Consent for users.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled"] = "Pixel enabled",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled.Hint"] = "Toggle to enable/disable Facebook Pixel for this configuration.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled"] = "Conversions API enabled",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled.Hint"] = "Toggle to enable/disable Facebook Conversions API for this configuration.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId"] = "Pixel ID",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId.Hint"] = "Enter the Facebook Pixel ID.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId.Required"] = "Pixel ID is required",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties"] = "Include User properties",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties.Hint"] = "Check to include User properties, data about the User, in a pixel. Then you can view User properties in the Facebook Analytics dashboard under People > User Properties.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties.Forbidden"] = "User Properties cannot be used together with Advanced Matching",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.Store"] = "Store",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.Store.Hint"] = "Choose a store in which the Facebook Pixel is used.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToCart"] = "Track \"AddToCart\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToCart.Hint"] = "Check to enable tracking standard event, when a product is added to the shopping cart.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToWishlist"] = "Track \"AddToWishlist\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToWishlist.Hint"] = "Check to enable tracking standard event, when a product is added to the wishlist.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackCompleteRegistration"] = "Track \"CompleteRegistration\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackCompleteRegistration.Hint"] = "Check to enable tracking standard event, when a registration form is completed.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackContact"] = "Track \"Contact\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackContact.Hint"] = "Check to enable tracking standard event, when a person person submits a question via contact us form.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackInitiateCheckout"] = "Track \"InitiateCheckout\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackInitiateCheckout.Hint"] = "Check to enable tracking standard event, when a person enters the checkout flow prior to completing the checkout flow.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPageView"] = "Track \"PageView\" event ",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPageView.Hint"] = "Check to enable tracking standard event, when a person lands on the website pages.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPurchase"] = "Track \"Purchase\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPurchase.Hint"] = "Check to enable tracking standard event, when an order is placed.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackSearch"] = "Track \"Search\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackSearch.Hint"] = "Check to enable tracking standard event, when a search is made.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackViewContent"] = "Track \"ViewContent\" event",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackViewContent.Hint"] = "Check to enable tracking standard event, when a person lands on a product details page.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching"] = "Advanced Matching",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching.Hint"] = "Check to enable Advanced Matching for tracked conversion events. In this case, some of the visitor's data (in the hashed format) will be collected by the Facebook Pixel. If you automatically implement advanced matching using the Events Manager, uncheck this setting.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching.Forbidden"] = "Advanced Matching cannot be used together with User Properties",
                ["Plugins.Widgets.FacebookPixel.Configuration.Search.Store"] = "Store",
                ["Plugins.Widgets.FacebookPixel.Configuration.Search.Store.Hint"] = "Search configuration by the store."
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(FacebookPixelDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.FacebookPixel");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}