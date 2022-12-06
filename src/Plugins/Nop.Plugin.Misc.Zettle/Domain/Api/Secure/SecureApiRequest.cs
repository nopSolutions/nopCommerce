namespace Nop.Plugin.Misc.Zettle.Domain.Api.Secure
{
    /// <summary>
    /// Represents base request to Secure API
    /// </summary>
    public abstract class SecureApiRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        public override string BaseUrl => "https://secure.izettle.com/";
    }
}