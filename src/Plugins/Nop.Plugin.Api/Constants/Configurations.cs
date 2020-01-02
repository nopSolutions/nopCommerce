namespace Nop.Plugin.Api.Constants
{
    public class Configurations
    {
        // time is in seconds (10 years = 315360000 seconds) and should not exceed 2038 year
        // https://stackoverflow.com/questions/43593074/jwt-validation-fails/43605820
        public const int DefaultAccessTokenExpiration = 315360000;
        public const int DefaultRefreshTokenExpiration = int.MaxValue;
        public const int DefaultLimit = 50;
        public const int DefaultPageValue = 1;
        public const int DefaultSinceId = 0;
        public const int DefaultCustomerId = 0;
        public const string DefaultOrder = "Id";
        public const int MaxLimit = 250;
        public const int MinLimit = 1;
        public const string PublishedStatus = "published";
        public const string UnpublishedStatus = "unpublished";
        public const string AnyStatus = "any";
        public const string JsonTypeMapsPattern = "json.maps";

        public const string NEWSLETTER_SUBSCRIBERS_KEY = "Nop.api.newslettersubscribers";
    }
}