using System.Linq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _desktopThemeIsCached;
        private string _cachedDesktopThemeName;

        private bool _mobileThemeIsCached;
        private string _cachedMobileThemeName;

        public ThemeContext(IWorkContext workContext, IGenericAttributeService genericAttributeService,
            StoreInformationSettings storeInformationSettings, IThemeProvider themeProvider)
        {
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._storeInformationSettings = storeInformationSettings;
            this._themeProvider = themeProvider;
        }

        /// <summary>
        /// Get or set current theme for desktops (e.g. darkOrange)
        /// </summary>
        public string WorkingDesktopTheme
        {
            get
            {
                if (_desktopThemeIsCached)
                    return _cachedDesktopThemeName;

                string theme = "";
                if (_storeInformationSettings.AllowCustomerToSelectTheme)
                {
                    if (_workContext.CurrentCustomer != null)
                        theme = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.WorkingDesktopThemeName);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _storeInformationSettings.DefaultStoreThemeForDesktops;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => !x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;
                
                //cache theme
                this._cachedDesktopThemeName = theme;
                this._desktopThemeIsCached = true;
                return theme;
            }
            set
            {
                if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                    return;

                if (_workContext.CurrentCustomer == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.WorkingDesktopThemeName, value);

                //clear cache
                this._desktopThemeIsCached = false;
            }
        }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        public string WorkingMobileTheme
        {
            get
            {
                if (_mobileThemeIsCached)
                    return _cachedMobileThemeName;

                //default store theme
                string theme = _storeInformationSettings.DefaultStoreThemeForMobileDevices;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;

                //cache theme
                this._cachedMobileThemeName = theme;
                this._mobileThemeIsCached = true;
                return theme;
            }
        }
    }
}
