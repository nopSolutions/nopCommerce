namespace Nop.Plugin.Api.IdentityServer.Endpoints
{
    using System;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Hosting;
    using IdentityServer4.ResponseHandling;
    using Microsoft.AspNetCore.Http;
    using Nop.Plugin.Api.IdentityServer.Extensions;
    using Nop.Plugin.Api.IdentityServer.Infrastructure;

    public class TokenErrorResult : IEndpointResult
    {
        public TokenErrorResponse Response { get; }

        public TokenErrorResult(TokenErrorResponse error)
        {
            if (error.Error.IsMissing()) throw new ArgumentNullException("Error must be set", nameof(error.Error));

            Response = error;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                error = Response.Error,
                error_description = Response.ErrorDescription
            };

            if (Response.Custom.IsNullOrEmpty())
            {
                await context.Response.WriteJsonAsync(dto);
            }
            else
            {
                var jobject = ObjectSerializer.ToJObject(dto);
                jobject.AddDictionary(Response.Custom);

                await context.Response.WriteJsonAsync(jobject);
            }
        }

        internal class ResultDto
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
    }
}