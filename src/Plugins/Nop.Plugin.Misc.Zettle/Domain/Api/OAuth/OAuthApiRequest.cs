namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth
{
    /// <summary>
    /// Represents base request to OAuth API
    /// </summary>
    public abstract class OAuthApiRequest : ApiRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        public override string BaseUrl => "https://oauth.zettle.com/";
    }
}