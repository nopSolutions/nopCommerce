using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components
{
    /// <summary>
    /// Represents view component for setting GoogleAuthenticator
    /// </summary>
    [ViewComponent(Name = GoogleAuthenticatorDefaults.VIEW_COMPONENT_NAME)]
    public class GAAuthenticationViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AuthenticationModelFactory _authenticationModelFactory;

        #endregion

        #region Ctor

        public GAAuthenticationViewComponent(AuthenticationModelFactory authenticationModelFactory)
        {
            _authenticationModelFactory = authenticationModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new AuthModel();
            model = await _authenticationModelFactory.PrepareAuthModel(model);

            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAAuthentication.cshtml", model);
        }

        #endregion
    }
}
