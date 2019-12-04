using FluentMigrator.Runner;
using Nop.Core.Infrastructure;

namespace Nop.Data.Extensions
{
    public static partial class MigrationRunnerBuilderExtensions
    {
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
    }
}
