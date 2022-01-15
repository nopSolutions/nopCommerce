using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents method for the multi-factor authentication
    /// </summary>
    public partial interface IMultiFactorAuthenticationMethod : IPlugin
    {
        #region Methods

        /// <summary>
        ///  Gets a multi-factor authentication type
        /// </summary>
        MultiFactorAuthenticationType Type { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="ViewComponent"/> for displaying plugin in public store
        /// </summary>
        /// <returns>The <see cref="Type"/> of the <see cref="ViewComponent"/>.</returns>
        Type GetPublicViewComponentType();

        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="ViewComponent"/> for displaying verification page
        /// </summary>
        /// <returns>The <see cref="Type"/> of the <see cref="ViewComponent"/>.</returns>
        Type GetVerificationViewComponentType();

        /// <summary>
        /// Gets a multi-factor authentication method description that will be displayed on customer info pages in the public store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<string> GetDescriptionAsync();

        #endregion
    }
}
