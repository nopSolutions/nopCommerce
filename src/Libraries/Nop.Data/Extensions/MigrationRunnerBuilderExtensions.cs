using System;
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

        #endregion
    }
}
