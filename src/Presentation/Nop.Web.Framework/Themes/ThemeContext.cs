using System;
using System.Linq;
using System.Threading.Tasks;
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

        protected IGenericAttributeService GenericAttributeService { get; }
        protected IStoreContext StoreContext { get; }
        protected IThemeProvider ThemeProvider { get; }
        protected IWorkContext WorkContext { get; }
        protected StoreInformationSettings StoreInformationSettings { get; }

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
            GenericAttributeService = genericAttributeService;
            StoreContext = storeContext;
            ThemeProvider = themeProvider;
            WorkContext = workContext;
            StoreInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<string> GetWorkingThemeNameAsync()
        {
            if (!string.IsNullOrEmpty(_cachedThemeName))
                return _cachedThemeName;

            var themeName = string.Empty;

            //whether customers are allowed to select a theme
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (StoreInformationSettings.AllowCustomerToSelectTheme &&
                customer != null)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                themeName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.WorkingThemeNameAttribute, store.Id);
            }

            //if not, try to get default store theme
            if (string.IsNullOrEmpty(themeName))
                themeName = StoreInformationSettings.DefaultStoreTheme;

            //ensure that this theme exists
            if (!await ThemeProvider.ThemeExistsAsync(themeName))
            {
                //if it does not exist, try to get the first one
                themeName = (await ThemeProvider.GetThemesAsync()).FirstOrDefault()?.SystemName
                            ?? throw new Exception("No theme could be loaded");
            }

            //cache theme system name
            _cachedThemeName = themeName;

            return themeName;
        }

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetWorkingThemeNameAsync(string workingThemeName)
        {
            //whether customers are allowed to select a theme
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!StoreInformationSettings.AllowCustomerToSelectTheme ||
                customer == null)
                return;

            //save selected by customer theme system name
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.WorkingThemeNameAttribute, workingThemeName,
                store.Id);

            //clear cache
            _cachedThemeName = null;
        }

        #endregion
    }
}