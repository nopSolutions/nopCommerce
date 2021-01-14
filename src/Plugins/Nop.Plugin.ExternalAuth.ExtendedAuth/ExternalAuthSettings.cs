using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication
{
    public class ExternalAuthSettings : ISettings
    {
        public string FacebookClientId { get; set; }
        public string FacebookClientSecret { get; set; }
        public bool FacebookEnable { get; set; }

        public string TwitterClientId { get; set; }
        public string TwitterClientSecret { get; set; }
        public bool TwitterEnable { get; set; }

        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public bool GoogleEnable { get; set; }
        public bool GoogleEmailDomainWhitelistingEnabled { get; set; }
        public List<string> WhitelistedEmailDomains { get; set; }

        public string MicrosoftClientId { get; set; }
        public string MicrosoftClientSecret { get; set; }
        public bool MicrosoftEnable { get; set; }

        public string LinkedInClientId { get; set; }
        public string LinkedInClientSecret { get; set; }
        public bool LinkedInEnable { get; set; }
    }
}
