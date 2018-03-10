using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Nop.Core;

namespace Nop.Web.Framework.Compression
{
    /// <summary>
    /// Workaround for ResponseCompression
    /// </summary>
    public partial class ResponseCompressionVaryWorkaroundMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next">Next</param>
        public ResponseCompressionVaryWorkaroundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Task</returns>
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebHelper webHelper)
        {
            //TODO remove this code once upgraded to the latest version of Microsoft.AspNetCore.ResponseCompression (already fixed there)

            // If the Accept-Encoding header is present, always add the Vary header
            // This will be added as a feature in the next release of the middleware.
            // https://github.com/aspnet/BasicMiddleware/issues/187

            //find more at https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression

            var accept = context.Request.Headers[HeaderNames.AcceptEncoding];
            if (!StringValues.IsNullOrEmpty(accept))
            {
                context.Response.Headers.Append(HeaderNames.Vary, HeaderNames.AcceptEncoding);
            }

            await _next(context);
        }

        #endregion
    }
}