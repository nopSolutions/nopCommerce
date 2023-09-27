using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher
{
    /// <summary>
    /// Represents request to get the webhook subscriptions
    /// </summary>
    public class GetSubscriptionsRequest : PusherApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "organizations/self/subscriptions";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}