using Nop.Plugin.ExternalAuth.ExtendedAuthentication;
using System.Collections.Generic;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Domain
{
    public class SocialMediaList
    {
        public SocialMediaList()
        {
            SocialMedias = new List<SocialMedia> {
            new SocialMedia { Name = AuthenticationDefaults.FacebookAuthenticationScheme, CallBackPath = AuthenticationDefaults.FacebookCallbackPath },
            new SocialMedia { Name = AuthenticationDefaults.GoogleAuthenticationScheme, CallBackPath = AuthenticationDefaults.GoogleCallbackPath },
            new SocialMedia { Name = AuthenticationDefaults.LinkedInAuthenticationScheme, CallBackPath = AuthenticationDefaults.LinkedInCallbackPath },
            new SocialMedia { Name = AuthenticationDefaults.MicrosoftAuthenticationScheme, CallBackPath = AuthenticationDefaults.MicrosoftCallbackPath },
            new SocialMedia { Name = AuthenticationDefaults.TwitterAuthenticationScheme, CallBackPath = AuthenticationDefaults.TwitterCallbackPath }
            };
        }

        public List<SocialMedia> SocialMedias { get; set; }
        public class SocialMedia
        {
            public string Name { get; set; }
            public string CallBackPath { get; set; }
        }
    }
}



