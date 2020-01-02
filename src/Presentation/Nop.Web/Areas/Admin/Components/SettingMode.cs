using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the setting mode
    /// </summary>
    public class SettingModeViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ISettingModelFactory _settingModelFactory;

        #endregion

        #region Ctor

        public SettingModeViewComponent(ISettingModelFactory settingModelFactory)
        {
            _settingModelFactory = settingModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="modeName">Setting mode name</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string modeName = "settings-advanced-mode")
        {
            //prepare model
            var model = _settingModelFactory.PrepareSettingModeModel(modeName);

            return View(model);
        }

        #endregion
    }
}