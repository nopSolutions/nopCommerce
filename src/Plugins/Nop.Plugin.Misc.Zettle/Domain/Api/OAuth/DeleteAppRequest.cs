using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth
{
    /// <summary>
    /// Represents request to delete app connections associated with user data access
    /// </summary>
    public class DeleteAppRequest : OAuthApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "application-connections/self";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}