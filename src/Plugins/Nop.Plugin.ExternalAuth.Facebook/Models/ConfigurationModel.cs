using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ExternalAuth.Facebook.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }
        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}