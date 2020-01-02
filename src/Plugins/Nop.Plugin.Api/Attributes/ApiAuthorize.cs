using Nop.Core.Infrastructure;
using Nop.Services.Plugins;

namespace Nop.Plugin.Api.Attributes
{
    using Microsoft.AspNetCore.Authorization;

    // We need the ApiAuthorize attribute because when the api plugin assembly is loaded in memory by PluginManager 
    // all of its attributes are being initialized by the .NetFramework.
    // The authorize attribute of the api plugin is marked with the Bearer authentication scheme, but the scheme is registered in the ApiStartup class,
    // which is called on plugin install. 
    // If the plugin is not installed the authorize attribute will still be initialized when the assembly is loaded in memory, but the scheme won't be registered,
    // which will cause an exception.
    // That is why we need to make sure that the plugin is installed before setting the scheme.
    public class ApiAuthorize : AuthorizeAttribute
    {
        public new string Policy
        {
            get => base.AuthenticationSchemes;
            set => base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
        }

        public new string AuthenticationSchemes
        {
            get => base.AuthenticationSchemes;
            set => base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
        }
        
        private static string GetAuthenticationSchemeName(string value)
        {
            var pluginService = EngineContext.Current.Resolve<IPluginService>();
            var pluginDescriptor = pluginService.GetPluginDescriptorBySystemName<IPlugin>(Nop.Plugin.Api.Constants.Plugin.SystemName);
            bool pluginInstalled = pluginDescriptor != null && pluginDescriptor.Installed;

            if (pluginInstalled)
            {
                return value;
            }

            return default(string);
        }
    }
}