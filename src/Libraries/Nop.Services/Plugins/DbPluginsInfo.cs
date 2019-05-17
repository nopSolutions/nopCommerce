using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Core.Infrastructure;

namespace Nop.Services.Plugins
{
    public class DbPluginsInfo : PluginsInfo
    {
        private static bool s_databaseVerified;

        public DbPluginsInfo(INopFileProvider fileProvider) : base(fileProvider)
        {
        }

        /// <summary>
        ///     Save plugins info to the DB
        /// </summary>
        public override void Save()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                EnsurePluginsTable();

                var text = JsonConvert.SerializeObject(this, Formatting.Indented);

                using (var cmd = GetDbCommand($@"   TRUNCATE TABLE [dbo].[Plugins]; 
                                                    INSERT INTO [dbo].[Plugins] VALUES('{text}')"))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        ///     Get plugins info
        /// </summary>
        /// <returns>True if data are loaded, otherwise False</returns>
        public override bool LoadPluginInfo()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                EnsurePluginsTable();

                // Try to get plugin info from the DB
                using (var cmd = GetDbCommand("SELECT TOP 1 [Json] FROM [dbo].[Plugins]"))
                {
                    var serializedItem = cmd.ExecuteScalar() as string;

                    return (!string.IsNullOrEmpty(serializedItem) && DeserializePluginInfo(serializedItem));
                }
            }

            return false;
        }

        private static void EnsurePluginsTable()
        {
            if (!s_databaseVerified)
            {
                using (var cmd = GetDbCommand(@"
                                            SET ANSI_NULLS ON;
                                            SET QUOTED_IDENTIFIER ON;
                                            IF (NOT EXISTS (SELECT 1 
                                                        FROM INFORMATION_SCHEMA.TABLES 
                                                        WHERE TABLE_SCHEMA = 'dbo' 
                                                        AND  TABLE_NAME = 'Plugins'))
                                            BEGIN
	                                            CREATE TABLE [dbo].[Plugins](
		                                            [Json] [nvarchar](max) NULL
	                                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
                                            END"))
                {
                    cmd.ExecuteNonQuery();
                    s_databaseVerified = true;
                }
            }
        }

        /// <summary>
        /// We do not have DI available to us at this point.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static IDbCommand GetDbCommand(string sql)
        {
            var connection = new SqlConnection(DataSettingsManager.LoadSettings().DataConnectionString);
            connection.Open();
            
            return new SqlCommand(sql, connection);
        }
    }
}