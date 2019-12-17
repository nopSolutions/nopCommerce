namespace Nop.Plugin.Api.IdentityServer.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.Primitives;

    public class IdentityServerScopeParameterMiddleware
    {
        private readonly RequestDelegate _next;

        public IdentityServerScopeParameterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/connect/authorize", StringComparison.InvariantCultureIgnoreCase) ||
                context.Request.Path.Value.Equals("/oauth/authorize", StringComparison.InvariantCultureIgnoreCase))
            {
                // Make sure we have "nop_api" and "offline_access" scope

                var queryValues = new Dictionary<string, StringValues>();

                foreach (var item in context.Request.Query)
                {
                    if (item.Key == "scope")
                    {
                        string scopeValue = item.Value;

                        if (!scopeValue.Contains("nop_api offline_access"))
                        {
                            // add our scope instead since we don't support other scopes
                            queryValues.Add(item.Key, "nop_api offline_access");
                            continue;
                        }
                    }

                    queryValues.Add(item.Key, item.Value);
                }

                if (!queryValues.ContainsKey("scope"))
                {
                    // if no scope is specified we add it
                    queryValues.Add("scope", "nop_api offline_access");
                }

                var newQueryCollection = new QueryCollection(queryValues);

                context.Request.Query = newQueryCollection;

            }

            await _next(context);
        }
    }
}