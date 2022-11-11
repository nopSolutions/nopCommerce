using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components
{
    /// <summary>
    /// Represents view component for verification GoogleAuthenticator
    /// </summary>
    public class GAVerificationViewComponent : NopViewComponent
    {
        #region Fields

        #endregion

        #region Ctor

        public GAVerificationViewComponent()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        ///  Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var model = new TokenModel();
            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAVefification.cshtml", model);
        }

        #endregion
    }
}
