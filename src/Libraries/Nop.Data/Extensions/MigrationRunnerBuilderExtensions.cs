using FluentMigrator.Runner;
using Nop.Data.Data;

namespace Nop.Data.Extensions
{
    public static partial class MigrationRunnerBuilderExtensions
    {
        public static IMigrationRunnerBuilder SetServer(this IMigrationRunnerBuilder builder, DataSettings dataSettings)
        {
            switch (dataSettings.DataProvider)
            {
                case DataProviderType.SqlServer:
                    builder.AddSqlServer()
                        // set the connection string
                        .WithGlobalConnectionString(dataSettings.DataConnectionString);
                    break;
            }

            return builder;
        }
    }
}
