//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Installation
{
    /// <summary>
    /// Represents an installer helper
    /// </summary>
    public partial class InstallerHelper
    {
        /// <summary>
        /// Checks if the connection string is set
        /// </summary>
        /// <returns></returns>
        public static bool ConnectionStringIsSet()
        {
            return !String.IsNullOrEmpty(NopConfig.ConnectionString);
        }

        /// <summary>
        /// Redirects user to the installation page
        /// </summary>
        public static void RedirectToInstallationPage()
        {
            string thisPage = CommonHelper.GetThisPageUrl(false);
            if (!thisPage.ToLower().Contains("install/install.aspx"))
            {
                string fileExtension = Path.GetExtension(thisPage);
                if (!String.IsNullOrEmpty(fileExtension) && fileExtension.ToLower() == ".aspx")
                {
                    if (HttpContext.Current != null)
                    {
                        string installPath = CommonHelper.GetStoreLocation() + "install/install.aspx";
                        HttpContext.Current.Response.Redirect(installPath);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the given connection parameters
        /// </summary>
        /// <param name="trustedConnection">Avalue that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <returns>Returns true if an attempt to open the database by using the connection succeeds.</returns>
        public static string TestConnection(bool trustedConnection, string serverName, string databaseName, string userName, string password)
        {
            try
            {
                string connectionString = CreateConnectionString(trustedConnection, serverName, databaseName, userName, password, 10);
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.Close();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets a current version of installed application
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Current version</returns>
        public static string GetCurrentVersion(string connectionString)
        {
            string version = string.Empty;
            try
            {
                string query = "SELECT * FROM [Nop_Setting] where [name]='Common.CurrentVersion'";

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    var rdr = command.ExecuteReader();
                    if (rdr.Read())
                    {
                        if (rdr["value"] != null && rdr["value"] != DBNull.Value)
                            version = rdr["value"].ToString();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
            return version;
        }
        
        /// <summary>
        /// Sets a version of installed application
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="version">Version</param>
        /// <returns>Error</returns>
        public static string SetCurrentVersion(string connectionString, string version )
        {
            try
            {
                string query = string.Format("Update [Nop_Setting] SET [Value]='{0}' WHERE [name]='Common.CurrentVersion'", version);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }               
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Creates a database on the server.
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Error</returns>
        public static string CreateDatabase(string databaseName, string connectionString)
        {
            try
            {
                string query = string.Format("CREATE DATABASE [{0}] COLLATE SQL_Latin1_General_CP1_CI_AS", databaseName);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }               
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="trustedConnection">A value that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <returns>Returns true if the database exists.</returns>
        public static bool DatabaseExists(bool trustedConnection,
            string serverName, string databaseName, string userName, string password)
        {
            // Prepare the query
            string connectionString = CreateConnectionString(trustedConnection, serverName, "master", userName, password, 120);
            string query = "SELECT Count(name) FROM sysdatabases WHERE name = '" + databaseName.Replace("'", "''") + "'";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var command = new SqlCommand(query, conn);
                object obj1 = command.ExecuteScalar();
                if (obj1 != null && obj1 != DBNull.Value)
                    return Convert.ToInt32(obj1) > 0;
                conn.Close();
            }  

            return false;
        }

        /// <summary>
        /// Create contents of connection strings used by the SqlConnection class
        /// </summary>
        /// <param name="trustedConnection">Avalue that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <param name="timeout">The connection timeout</param>
        /// <returns>Connection string</returns>
        public static string CreateConnectionString(bool trustedConnection,
            string serverName, string databaseName, string userName, string password, int timeout)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = trustedConnection;
            builder.DataSource = serverName;
            builder.InitialCatalog = databaseName;
            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }
            builder.PersistSecurityInfo = false;
            builder.MultipleActiveResultSets = true;
            builder.ConnectTimeout = timeout;
            return builder.ConnectionString;
        }

        /// <summary>
        /// Sets or adds the specified connection string in the ConnectionStrings section
        /// </summary>
        /// <param name="name">ConnectionString name</param>
        /// <param name="connectionString">Connection string</param>
        public static bool SaveConnectionString(string name, string connectionString)
        {
            try
            {
                var config = WebConfigurationManager.OpenWebConfiguration("~");

                if (config.ConnectionStrings.ConnectionStrings[name] != null)
                    config.ConnectionStrings.ConnectionStrings[name].ConnectionString = connectionString;
                else
                    config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString));
               
                config.Save(ConfigurationSaveMode.Modified);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Backup database
        /// </summary>
        /// <param name="fileName">Destination file name</param>
        public static void Backup(string fileName)
        {
            //TODO SQL Server only now
            string sqlConnectionString = NopSqlDataHelper.GetConnectionString("NopSqlConnection");
            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                string dbName = NopSqlDataHelper.GetDatabaseName(sqlConnectionString);
                string commandText = string.Format(
                    "BACKUP DATABASE [{0}] TO DISK = '{1}' WITH FORMAT",
                    dbName,
                    fileName);

                DbCommand dbCommand = new SqlCommand(commandText, conn);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="fileName">Target file name</param>
        public static void RestoreBackup(string fileName)
        {
            //TODO SQL Server only now

            string sqlConnectionString = NopSqlDataHelper.GetConnectionString("NopSqlConnection");
            string masterConnectionString = NopSqlDataHelper.GetMasterConnectionString(sqlConnectionString);
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                string dbName = NopSqlDataHelper.GetDatabaseName(sqlConnectionString);
                string commandText = string.Format(
                    "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
                    "BEGIN TRY\n" +
                        "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
                    "END TRY\n" +
                    "BEGIN CATCH\n" +
                        "SET @ErrorMessage = ERROR_MESSAGE()\n" +
                    "END CATCH\n" +
                    "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
                    "IF (@ErrorMessage is not NULL)\n" +
                    "BEGIN\n" +
                        "RAISERROR (@ErrorMessage, 16, 1)\n" +
                    "END",
                    dbName,
                    fileName);

                DbCommand dbCommand = new SqlCommand(commandText, conn);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                dbCommand.ExecuteNonQuery();
            }

            //clear all pools
            SqlConnection.ClearAllPools();
        }
    }
}
