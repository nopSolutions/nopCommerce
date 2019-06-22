using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Nop.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents the data settings manager
    /// </summary>
    public partial class DataSettingsManager
    {
        private static bool? _databaseIsInstalled;

        private static readonly string[] _tableNamesToValidate = new string[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };

        private static async Task<bool> IsDatabaseInitialized(string connectionString, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new EmptyConnectionStringException();

            logger?.LogInformation(DataLoggingEvents.DataConnectionStringEmpty, "ConnectionString is not empty");

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (string.IsNullOrEmpty(builder.InitialCatalog))
                    throw new ConnectionStringBadFormatException(connectionString);

                logger?.LogInformation(DataLoggingEvents.DataConnectionStringWellFormatted, "ConnectionString is well formatted");

                var tables = new List<string>();

                //just try to connect
                using (var conn = new SqlConnection(connectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT TABLE_NAME
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = @PARAM_CATALOG";
                        cmd.Parameters.AddWithValue("@PARAM_CATALOG", builder.InitialCatalog);

                        await cmd.Connection.OpenAsync();

                        logger?.LogInformation(DataLoggingEvents.DataConnectionOpenned, "Connection openned to check tables");

                        using (var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                        {
                            logger?.LogInformation(DataLoggingEvents.DataConnectionReaderOpenned, "DataReader openned");

                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    tables.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                }
                bool intersect = _tableNamesToValidate.Intersect(tables, StringComparer.InvariantCultureIgnoreCase).Any();
                logger?.LogInformation(DataLoggingEvents.DataConnectionTablesIntersect, "Tables intersect {intersect}", intersect);
                return intersect;
            }
            catch (Exception ex)
            {
                logger?.LogError(DataLoggingEvents.DataConnection, ex, "Erro while attempting to check data connection");
                return false;
            }
        }

        /// <summary>
        /// Reset "database is installed" cached information
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }

        /// <summary>
        /// Gets a value indicating whether database is already installed
        /// </summary>
        [Obsolete("Use method version instead")]
        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    var dbConfig = EngineContext.Current.Resolve<IOptions<DataSettings>>() ?? throw new NopDbConfigOptionNotConfiguredException();

                    _databaseIsInstalled = IsDatabaseInitialized(dbConfig.Value.DataConnectionString).GetAwaiter().GetResult();
                }
                //_databaseIsInstalled = !string.IsNullOrEmpty(LoadSettings(reloadSettings: true)?.DataConnectionString);

                return _databaseIsInstalled.Value;
            }
        }

        public static bool GetDatabaseIsInstalled(bool force = false)
        {
            if (force || !_databaseIsInstalled.HasValue)
            {
                var dbConfig = EngineContext.Current.Resolve<IOptions<DataSettings>>();
                return GetDatabaseIsInstalled(dbConfig.Value, force: force);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(IServiceProvider serviceProvider, bool force = false)
        {
            if (force || !_databaseIsInstalled.HasValue)
            {
                var dbConfig = (IOptions<DataSettings>)serviceProvider.GetService(typeof(IOptions<DataSettings>));
                var logger = (ILogger)serviceProvider.GetService(typeof(ILogger));

                return GetDatabaseIsInstalled(dbConfig.Value, logger: logger, force: force);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(IServiceProvider serviceProvider, ILogger logger, bool force = false)
        {
            if (force || !_databaseIsInstalled.HasValue)
            {
                var dbConfig = (IOptions<DataSettings>)serviceProvider.GetService(typeof(IOptions<DataSettings>));

                return GetDatabaseIsInstalled(dbConfig.Value, logger, force);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(DataSettings dbConfig, ILogger logger = null, bool force = false)
        {
            if (force || !_databaseIsInstalled.HasValue)
            {
                _databaseIsInstalled = IsDatabaseInitialized(dbConfig.DataConnectionString, logger).GetAwaiter().GetResult();
            }

            return _databaseIsInstalled.Value;
        }
    }
}