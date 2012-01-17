using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ExternalAuth.Twitter.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Twitter.ConsumerKey")]
        public string ConsumerKey { get; set; }
        [NopResourceDisplayName("Plugins.ExternalAuth.Twitter.ConsumerSecret")]
        public string ConsumerSecret { get; set; }
    }
}