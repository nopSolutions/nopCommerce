using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System.IO;
using Nop.Services.Plugins;

namespace D0b0.Plugin.Analytics.ApplicationInsights.Infrastructure
{
	/// <summary>
	/// Represents object for the configuring plugin on application startup
	/// </summary>
	public class PluginStartup : BasePlugin, INopStartup
    {
		/// <summary>
		/// Add and configure any of the middleware
		/// </summary>
		/// <param name="services">Collection of service descriptors</param>
		/// <param name="configuration">Configuration of the application</param>
		public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(GetConfigDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.Build();
			var instrumentationKey = config["ApplicationInsights:InstrumentationKey"];

			if (!string.IsNullOrEmpty(instrumentationKey))
			{
				services.AddApplicationInsightsTelemetry(instrumentationKey);
			}
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
		public int Order => 11;

        private string GetConfigDirectory()
		{
			string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(PluginStartup)).Location;
			string pluginPath = Path.Combine(assemblyPath, @"..\..\Analytics.ApplicationInsights");
			return Path.GetFullPath(pluginPath);
		}
	}
}