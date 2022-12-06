using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to get all tax rates
    /// </summary>
    public class GetTaxRatesRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "v1/taxes";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}