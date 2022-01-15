using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents method for the external authentication
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="ViewComponent"/> for displaying plugin in public store
        /// </summary>
        /// <returns>The <see cref="Type"/> of the <see cref="ViewComponent"/>.</returns>
        Type GetPublicViewComponentType();
    }
}
