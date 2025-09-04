// Nop.Web/Infrastructure/CustomMiddlewareStartup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;
using System.Linq;

namespace Nop.Web.Infrastructure
{
    public class CustomMiddlewareStartup : INopStartup
    {
        // Run early in the pipeline
        public int Order => 1;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // No services to configure
        }

        public void Configure(IApplicationBuilder application)
        {
            
            application.Use(async (context, next) =>
            {
                // Log to console to confirm middleware runs
                Console.WriteLine($"Custom middleware checking path: {context.Request.Path.Value}");

                var goneUrls = new[]
                {
                    "/water-heaters-delivered-installed-within-24hours"
                };

                // Normalize path: lowercase + no trailing slash
                var requestPath = context.Request.Path.Value?.TrimEnd('/').ToLowerInvariant();

                if (goneUrls.Any(u => string.Equals(u, requestPath, StringComparison.OrdinalIgnoreCase)))
{
    context.Response.StatusCode = StatusCodes.Status410Gone;
    context.Response.ContentType = "text/html";

    await context.Response.WriteAsync(@"
        <html>
            <head>
                <title>410 Gone</title>
                <meta http-equiv='refresh' content='5;url=https://www.abcwarehouse.com/' />
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                        background-color: #ffffff;
                        color: #333333;
                        text-align: center;
                        margin: 0;
                        padding: 0;
                    }
                    .container {
                        margin: 8% auto;
                        padding: 40px;
                        max-width: 500px;
                        border: 1px solid #e5e5e5;
                        border-radius: 20px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.08);
                    }
                    .logo {
                        margin-bottom: 25px;
                    }
                    h1 {
                        font-size: 28px;
                        color: #e32228;
                        margin-bottom: 20px;
                    }
                    p {
                        font-size: 16px;
                        color: #555555;
                        margin-bottom: 30px;
                    }
                    a {
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #e32228;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 12px;
                        font-weight: 500;
                        transition: background-color 0.2s ease;
                    }
                    a:hover {
                        background-color: #c51d22;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='logo'>
                        <img alt='ABC Warehouse' src='https://www.abcwarehouse.com/images/thumbs/164/1641055_abc_web-logo_60th.png' />
                    </div>
                    <h1>410 - Page Removed</h1>
                    <p>This page is no longer available.<br/>You will be redirected shortly.</p>
                    <a href='https://www.abcwarehouse.com/'>Go to Homepage</a>
                </div>
            </body>
        </html>
    ");
    return;
}



                await next();
            });
        }
    }
}
