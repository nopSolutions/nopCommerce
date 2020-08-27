namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents hosting configuration parameters
    /// </summary>
    public partial class HostingConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use HTTP_CLUSTER_HTTPS
        /// </summary>
        public bool UseHttpClusterHttps { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use HTTP_X_FORWARDED_PROTO
        /// </summary>
        public bool UseHttpXForwardedProto { get; set; } = false;

        /// <summary>
        /// Gets or sets custom forwarded HTTP header (e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc)
        /// </summary>
        public string ForwardedHttpHeader { get; set; } = string.Empty;
    }
}