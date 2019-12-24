using System;
using FluentMigrator.Runner;
using Nop.Core.Infrastructure;

namespace Nop.Data.Extensions
{
    public static partial class MigrationRunnerBuilderExtensions
    {
        public static IMigrationRunnerBuilder SetServer(this IMigrationRunnerBuilder builder, DataSettings dataSettings)
        {
            if (dataSettings is null)
                throw new ArgumentNullException(nameof(dataSettings));

            switch (dataSettings.DataProvider)
            {
                case DataProviderType.SqlServer:
                    builder.AddSqlServer();
                    break;
                case DataProviderType.MySql:
                    builder.AddMySql5();
                    break;
            }

            return builder;
        }
    }
}
