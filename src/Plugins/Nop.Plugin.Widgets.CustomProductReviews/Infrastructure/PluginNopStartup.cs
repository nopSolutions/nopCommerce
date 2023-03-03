using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomProductReviews.Services;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widgets.CustomProductReviews.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            //register services and interfaces
            //services.AddScoped<CustomModelFactory, ICustomerModelFactory>();
            services.AddScoped<IVideoService,VideoService>();
            services.AddScoped<ICustomProductReviewMappingService, CustomProductReviewMappingService>();
            services.AddSingleton<IBackgroundQueue,BackgroundQueue>();
            services.AddHostedService<QueueService>();
            services.AddScoped<IPluginService, PluginService>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}