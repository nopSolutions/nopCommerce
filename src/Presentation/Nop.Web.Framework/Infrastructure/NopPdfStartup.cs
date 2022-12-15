using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data;
using QuestPDF.Drawing;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopPdfStartup : INopStartup
    {
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            var fontPaths = fileProvider.EnumerateFiles(fileProvider.MapPath("~/App_Data/Pdf/"), "*.ttf") ?? Enumerable.Empty<string>();

            foreach (var fp in fontPaths)
            {
                FontManager.RegisterFont(File.OpenRead(fp));
            }
        }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 101;
    }
}