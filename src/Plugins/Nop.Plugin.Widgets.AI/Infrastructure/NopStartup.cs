using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.AI.Configurations;

namespace Nop.Plugin.Widgets.AI.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 100;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AIConfiguration>(opts =>
            {
                opts.InstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            });
        }
    }
}
