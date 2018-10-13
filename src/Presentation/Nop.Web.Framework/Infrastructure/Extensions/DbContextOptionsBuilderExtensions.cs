using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Data;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of DbContextOptionsBuilder
    /// </summary>
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// SQL Server specific extension method for Microsoft.EntityFrameworkCore.DbContextOptionsBuilder
        /// </summary>
        /// <param name="optionsBuilder">Database context options builder</param>
        /// <param name="services">Collection of service descriptors</param>
        public static void UseSqlServerWithLazyLoading(this DbContextOptionsBuilder optionsBuilder, IServiceCollection services)
        {
            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            var provider = services.BuildServiceProvider();
            var nopConfig = provider.GetRequiredService<NopConfig>();

            var optionsBuilderHelpers = provider.GetServices<IDbContextOptionsBuilderHelper>();
            var optionsBuilderHelper = optionsBuilderHelpers?.FirstOrDefault(h => h.DataProvider == dataSettings.DataProvider);
            if (optionsBuilderHelper != null)
            {
                optionsBuilderHelper.Configure(optionsBuilder, services, nopConfig, dataSettings);
                return;
            }

            var dbContextOptionsBuilder = optionsBuilder.UseLazyLoadingProxies();

            if (nopConfig.UseRowNumberForPaging)
                dbContextOptionsBuilder.UseSqlServer(dataSettings.DataConnectionString, option => option.UseRowNumberForPaging());
            else
                dbContextOptionsBuilder.UseSqlServer(dataSettings.DataConnectionString);
        }
    }
}
