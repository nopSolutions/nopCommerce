using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Nop.Core.Http
{
    /// <summary>
    /// Represents middleware that adds Etag to resources
    /// <see cref="https://madskristensen.net/blog/send-etag-headers-in-aspnet-core/"/>
    /// </summary>
    public class ETagMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public ETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;

            using (var ms = new MemoryStream())
            {
                response.Body = ms;

                await _next(context);

                if (IsEtagSupported(response))
                {
                    string checksum = CalculateChecksum(ms);

                    response.Headers[HeaderNames.ETag] = checksum;

                    if (context.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag) && checksum == etag)
                    {
                        response.StatusCode = StatusCodes.Status304NotModified;
                        response.Headers[HeaderNames.ContentLength] = "0";
                        response.ContentLength = 0L;
                        response.ContentType = null;
                        response.Body = null;
                        return;
                    }
                }

                ms.Position = 0;
                await ms.CopyToAsync(originalStream);
            }
        }

        #endregion

        private static bool IsEtagSupported(HttpResponse response)
        {
            if (response.StatusCode != StatusCodes.Status200OK)
                return false;

            // The 50kb length limit is not based in science. Feel free to change
            if (response.Body.Length > 50 * 1024)
                return false;

            if (response.Headers.ContainsKey(HeaderNames.ETag))
                return false;

            return true;
        }

        private static string CalculateChecksum(MemoryStream ms)
        {
            string checksum = "";

            using (var algo = SHA1.Create())
            {
                ms.Position = 0;
                byte[] bytes = algo.ComputeHash(ms);
                checksum = $"\"{WebEncoders.Base64UrlEncode(bytes)}\"";
            }

            return checksum;
        }
    }
}
