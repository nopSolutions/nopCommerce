using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Twitter
{
    public class TwitterExternalAuthSettings : ISettings
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
}
