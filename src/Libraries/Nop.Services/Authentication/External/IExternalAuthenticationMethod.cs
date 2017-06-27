using Nop.Core.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents method for the external authentication
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
        /// <summary>
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        /// <param name="viewComponentArguments">View component arguments</param>
        void GetPublicViewComponent(out string viewComponentName, out object viewComponentArguments);
    }
}
