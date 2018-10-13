using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Data.MySQL.Controllers;
using Nop.Plugin.Data.MySQL.Data;
using Nop.Web.Controllers;

namespace Nop.Plugin.Data.MySQL.Infrastructure
{
    public class Startup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<InstallController, InstallWithMySQLController>();

            services.AddScoped<IDbContextOptionsBuilderHelper, MySQLDbContextOptionsBuilderHelper>();

            //add EF services
            services.AddEntityFrameworkMySql();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// After the NopDbStartup startup.
        /// </summary>
        public int Order => 11;
    }
}
