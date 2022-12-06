namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher
{
    /// <summary>
    /// Represents base request to Pusher API
    /// </summary>
    public abstract class PusherApiRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        public override string BaseUrl => "https://pusher.izettle.com/";
    }
}