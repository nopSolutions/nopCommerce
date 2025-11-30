using Nop.Core.Caching;

namespace Nop.Plugin.Api.Infrastructure
{
    public static class Constants
    {
        public static class Roles
        {
            public const string ApiRoleSystemName = "ApiUserRole";

            public const string ApiRoleName = "Api Users";
        }

        public static class ViewNames
        {
            public const string AdminLayout = "_AdminLayout";
        }

        public static class Configurations
        {
            public const int DefaultAccessTokenExpirationInDays = 365; // 1 year

            // time is in seconds (10 years = 315360000 seconds) and should not exceed 2038 year
            // https://stackoverflow.com/questions/43593074/jwt-validation-fails/43605820
            //public const int DefaultAccessTokenExpiration = 315360000;
            //public const int DefaultRefreshTokenExpiration = int.MaxValue;
            public const int DefaultLimit = 50;
            public const int DefaultPageValue = 1;
            public const int DefaultSinceId = 0;
            public const int DefaultCustomerId = 0;
            public const string DefaultOrder = "Id";
            public const int MaxLimit = 250;

            public const int MinLimit = 1;

            //public const string PublishedStatus = "published";
            //public const string UnpublishedStatus = "unpublished";
            //public const string AnyStatus = "any";
            public static CacheKey JsonTypeMapsPattern => new CacheKey("json.maps");

            public static CacheKey NEWSLETTER_SUBSCRIBERS_KEY = new CacheKey("Nop.api.newslettersubscribers");
        }
    }
}
