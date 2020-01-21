using FluentMigrator.Runner;

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
        public static IMigrationRunnerBuilder SetServers(this IMigrationRunnerBuilder builder)
        {   
            return builder
                .AddSqlServer()
                .AddMySql5();
        }

        #endregion
    }
}
