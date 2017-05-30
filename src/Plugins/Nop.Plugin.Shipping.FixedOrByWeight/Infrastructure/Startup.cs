using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Extensions;
using Nop.Plugin.Shipping.FixedOrByWeight.Components;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring web services and middleware on application startup
    /// </summary>
    public class Startup : IStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(new EmbeddedFileProvider(typeof(FixedOrByWeightConfigureViewComponent).GetTypeInfo().Assembly, "Nop.Plugin.Shipping.FixedOrByWeight.Components"));
            });
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            get { return 1001; }
        }
    }
}
