using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Secure
{
    /// <summary>
    /// Represents request to get the information about a merchant's account
    /// </summary>
    public class GetAccountInfoRequest : SecureApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "api/resources/organizations/self";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}