using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.plugin.IsEnabled")]
        public bool IsActive { get; set; }
        public bool IsActive_OverrideForStore { get; set; }
        public int ActiveStoreScopeConfiguration { get; set; }

        // facebook
        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.IsEnabled")]
        public bool FacebookBtnIsDisplay { get; set; } = true;
        public bool FacebookBtnIsDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier")]
        public string FacebookClientId { get; set; }
        public bool FacebookClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientSecret")]
        public string FacebookClientSecret { get; set; }
        public bool FacebookClientSecret_OverrideForStore { get; set; }

        // Twitter
        [NopResourceDisplayName("Plugins.ExternalAuth.Twitter.IsEnabled")]
        public bool TwitterBtnIsDisplay { get; set; } = true;
        public bool TwitterBtnIsDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Twitter.ConsumerKey")]
        public string TwitterClientId { get; set; }
        public bool TwitterClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Twitter.ConsumerSecret")]
        public string TwitterClientSecret { get; set; }
        public bool TwitterClientSecret_OverrideForStore { get; set; }

        // Google
        [NopResourceDisplayName("Plugins.ExternalAuth.Google.IsEnabled")]
        public bool GmailBtnIsDisplay { get; set; } = true;
        public bool GmailBtnIsDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Google.ClientId")]
        public string GoogleClientId { get; set; }
        public bool GoogleClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Google.ClientSecret")]
        public string GoogleClientSecret { get; set; }
        public bool GoogleClientSecret_OverrideForStore { get; set; }

        // Microsoft
        [NopResourceDisplayName("Plugins.ExternalAuth.Microsoft.IsEnabled")]
        public bool MicrosoftBtnIsDisplay { get; set; } = true;
        public bool MicrosoftBtnIsDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Microsoft.ClientId")]
        public string MicrosoftClientId { get; set; }
        public bool MicrosoftClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Microsoft.ClientSecret")]
        public string MicrosoftClientSecret { get; set; }
        public bool MicrosoftClientSecret_OverrideForStore { get; set; }
                
        // LinkedIn        
        [NopResourceDisplayName("Plugins.ExternalAuth.LinkedIn.IsEnabled")]
        public bool LinkedInBtnIsDisplay { get; set; } = true;
        public bool LinkedInBtnIsDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.LinkedIn.ClientId")]
        public string LinkedInClientId { get; set; }
        public bool LinkedInClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.LinkedIn.ClientSecret")]
        public string LinkedInClientSecret { get; set; }
        public bool LinkedInClientSecret_OverrideForStore { get; set; }               
    }
}