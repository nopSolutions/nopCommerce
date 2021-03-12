using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private AppSettings _appSettings;
        private IEngine _engine;

        #endregion

        #region Ctor

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            (_engine, _appSettings) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
        }

        /// <summary>
        /// Configure the DI container 
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public void ConfigureContainer(IServiceCollection services)
        {
            _engine.RegisterDependencies(services, _appSettings);
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
            application.StartEngine();
        }
    }
}