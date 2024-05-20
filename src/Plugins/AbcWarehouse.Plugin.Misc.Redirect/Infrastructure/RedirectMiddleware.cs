using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;

namespace AbcWarehouse.Plugin.Misc.Redirect.Infrastructure
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var host = context.Request.Host.Value;
            if (host == "clearance.abcwarehouse.com")
            {
                context.Response.Redirect("https://www.abcwarehouse.com/clearance");
            }

            await _next(context);
        }
    }
}