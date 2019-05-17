using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Themes;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents the theme context implementation
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IWorkContext _workContext;
        private readonly StoreInformationSettings _storeInformationSettings;

        private string _cachedThemeName;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="themeProvider">Theme provider</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeInformationSettings">Store information settings</param>
        public ThemeContext(IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IThemeProvider themeProvider,
            IWorkContext workContext,
            StoreInformationSettings storeInformationSettings)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _themeProvider = themeProvider;
            _workContext = workContext;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedThemeName))
                    return _cachedThemeName;

                var themeName = string.Empty;

                //whether customers are allowed to select a theme
                if (_storeInformationSettings.AllowCustomerToSelectTheme && _workContext.CurrentCustomer != null)
                {
                    themeName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                        NopCustomerDefaults.WorkingThemeNameAttribute, _storeContext.CurrentStore.Id);
                }

                //if not, try to get default store theme
                if (string.IsNullOrEmpty(themeName))
                    themeName = _storeInformationSettings.DefaultStoreTheme;

                //ensure that this theme exists
                if (!_themeProvider.ThemeExists(themeName))
                {
                    //if it does not exist, try to get the first one
                    themeName = _themeProvider.GetThemes().FirstOrDefault()?.SystemName
                        ?? throw new Exception("No theme could be loaded");
                }

                //cache theme system name
                _cachedThemeName = themeName;

                return themeName;
            }
            set
            {
                //whether customers are allowed to select a theme
                if (!_storeInformationSettings.AllowCustomerToSelectTheme || _workContext.CurrentCustomer == null)
                    return;

                //save selected by customer theme system name
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    NopCustomerDefaults.WorkingThemeNameAttribute, value, _storeContext.CurrentStore.Id);

                //clear cache
                _cachedThemeName = null;
            }
        }

        #endregion
    }
}