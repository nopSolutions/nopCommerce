using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Sendinblue.Services;
using Nop.Services.Messages;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Misc.Sendinblue.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<MarketingAutomationHttpClient>().WithProxy();

            //register custom services
            services.AddScoped<SendinblueManager>();
            services.AddScoped<MarketingAutomationManager>();

            //override services
            services.AddScoped<IWorkflowMessageService, SendinblueMessageService>();
            services.AddScoped<IEmailSender, SendinblueEmailSender>();
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
        public int Order => 3000;
    }
}