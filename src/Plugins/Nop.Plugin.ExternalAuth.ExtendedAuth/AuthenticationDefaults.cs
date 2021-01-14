namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication
{
    /// <summary>
    /// Default values used by the authentication middleware
    /// </summary>
    public class AuthenticationDefaults
    {
        public const string PluginSystemName = "ExternalAuth";      
        
        public const string TwitterAuthenticationScheme = "Twitter";
        public const string TwitterCallbackPath = "signin-Twitter";

        public const string FacebookAuthenticationScheme = "Facebook";
        public const string FacebookCallbackPath = "signin-facebook";

        public const string GoogleAuthenticationScheme = "Google";
        public const string GoogleCallbackPath = "signin-Google";

        public const string MicrosoftAuthenticationScheme = "Microsoft";
        public const string MicrosoftCallbackPath = "signin-microsoft";

        public const string LinkedInAuthenticationScheme = "LinkedIn";
        public const string LinkedInCallbackPath = "signin-LinkedIn";

        public const string YahooAuthenticationScheme = "Yahoo";
        public const string AmazonAuthenticationScheme = "Amazon";
                
        public static string AvatarClaimType => "picture";
        public static string ErrorCallback = "ErrorCallback";

        public const string PluginPath = "/Plugins/ExternalAuth.ExtendedAuth";
    }
     
    // convert the response to property
    public class SocialUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string age_range { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }
        public string picture { get; set; }

        // linked in 
        public string ID { get; set; }
        public string profilePicture { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string location { get; set; }
        public string positions { get; set; }
        public string preferredLocale { get; set; }
        public string headline { get; set; }
        public string title { get; set; }
        public string emailAddress { get; set; }
        public string nameidentifier { get; set; }
        public string formattedName { get; set; }
        public string pictureUrl { get; set; }

        // Google
        public string given_name { get; set; } // firstname
        public string family_name { get; set; } // lastname
        public string email_verified { get; set; }

        // Twitter ref. https://developer.twitter.com/en/docs/tweets/data-dictionary/overview/intro-to-tweet-json
        public string id_str { get; set; }
        public string screen_name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string place { get; set; }
        public string entities { get; set; }
        public string urls { get; set; }
        public string user_mentions { get; set; }
        public string created_at { get; set; }
    }
}
