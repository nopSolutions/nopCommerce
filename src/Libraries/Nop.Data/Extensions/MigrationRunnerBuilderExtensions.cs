using FluentMigrator.Runner;
using Nop.Core.Infrastructure;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// IMigrationRunnerBuilder extensions
    /// </summary>
    public static partial class MigrationRunnerBuilderExtensions
    {
        #region Methods

        /// <summary>
        /// Configure the database server
        /// </summary>
        /// <param name="builder">Configuring migration runner services</param>
        /// <returns></returns>
        public static IMigrationRunnerBuilder SetServer(this IMigrationRunnerBuilder builder)
        {
            var dataSettings = Singleton<DataSettings>.Instance;

            switch (dataSettings.DataProvider)
            {
                case DataProviderType.SqlServer:
                    builder.AddSqlServer()
                        // set the connection string
                        .WithGlobalConnectionString(dataSettings.ConnectionString);
                    break;
            }

            return builder;
        }

        #endregion
    }
}
