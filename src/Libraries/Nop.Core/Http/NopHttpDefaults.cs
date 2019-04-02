
namespace Nop.Core.Http
{
    /// <summary>
    /// Represents default values related to HTTP features
    /// </summary>
    public static partial class NopHttpDefaults
    {
        /// <summary>
        /// Gets the name of the default HTTP client
        /// </summary>
        public static string DefaultHttpClient => "default";

        /// <summary>
        /// Gets the name of a request item that stores the value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public static string IsPostBeingDoneRequestItem => "nop.IsPOSTBeingDone";

        /// <summary>
        /// Gets the name of HTTP_CLUSTER_HTTPS header
        /// </summary>
        public static string HttpClusterHttpsHeader => "HTTP_CLUSTER_HTTPS";

        /// <summary>
        /// Gets the name of HTTP_X_FORWARDED_PROTO header
        /// </summary>
        public static string HttpXForwardedProtoHeader => "X-Forwarded-Proto";

        /// <summary>
        /// Gets the name of X-FORWARDED-FOR header
        /// </summary>
        public static string XForwardedForHeader => "X-FORWARDED-FOR";
    }
}