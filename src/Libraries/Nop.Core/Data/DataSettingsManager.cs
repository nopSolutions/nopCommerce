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

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents the data settings manager
    /// </summary>
    public partial class DataSettingsManager
    {
        private static bool? _databaseIsInstalled;

        private static readonly string[] _tableNamesToValidate = new string[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };

        private static async Task<bool> IsDatabaseInitialized(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new EmptyConnectionStringException();

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (string.IsNullOrEmpty(builder.InitialCatalog))
                    throw new ConnectionStringBadFormatException(connectionString);

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

                        using (var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                        {
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
                return _tableNamesToValidate.Intersect(tables, StringComparer.InvariantCultureIgnoreCase).Any();
            }
            catch
            {
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

        public static bool GetDatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var dbConfig = EngineContext.Current.Resolve<IOptions<DataSettings>>();
                return GetDatabaseIsInstalled(dbConfig.Value);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(IServiceProvider serviceProvider)
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var dbConfig = (IOptions<DataSettings>)serviceProvider.GetService(typeof(IOptions<DataSettings>));

                return GetDatabaseIsInstalled(dbConfig.Value);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(DataSettings dbConfig)
        {
            if (!_databaseIsInstalled.HasValue)
            {
                _databaseIsInstalled = IsDatabaseInitialized(dbConfig.DataConnectionString).GetAwaiter().GetResult();
            }

            return _databaseIsInstalled.Value;
        }
    }
}