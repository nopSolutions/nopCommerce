using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// IDbContextOptionsBuilderHelper
    /// </summary>
    public interface IDbContextOptionsBuilderHelper
    {
        /// <summary>
        /// Get or set the data provider type.
        /// </summary>
        DataProviderType DataProvider { get; }

        /// <summary>
        /// Configure db context options.
        /// </summary>
        /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
        /// <param name="services">IServiceCollection</param>
        /// <param name="nopConfig">NopConfig</param>
        /// <param name="dataSettings">DataSettings</param>
        void Configure(DbContextOptionsBuilder optionsBuilder, IServiceCollection services, NopConfig nopConfig, DataSettings dataSettings);
    }
}
