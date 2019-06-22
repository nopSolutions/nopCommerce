using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Start of ConfigureServices");

            services.AddApplicationInsightsTelemetry();

            var svcs = services.ConfigureApplicationServices(_configuration, _hostingEnvironment, _logger);
            
            _logger.LogInformation("End of ConfigureServices");

            return svcs;
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            _logger.LogInformation("Start of Configure");

            application.ConfigureRequestPipeline();

            _logger.LogInformation("End of Configure");
        }
    }
}