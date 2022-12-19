using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api
{
    /// <summary>
    /// Represents base request object
    /// </summary>
    public abstract class ApiRequest : IApiRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        [JsonIgnore]
        public abstract string BaseUrl { get; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public abstract string Path { get; }

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public abstract string Method { get; }
    }
}