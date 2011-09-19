using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Installation;
using Nop.Services.Security;
using Nop.Web.Models.Install;

namespace Nop.Web.Controllers
{
    public class InstallController : BaseNopController
    {
        #region Utilities

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        private bool sqlServerDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a database on the server.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Error</returns>
        private string createDatabase(string connectionString)
        {
            try
            {
                //parse database name
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;
                //now create connection string to 'master' dabatase. It always exists.
                builder.InitialCatalog = "master";
                var masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE [{0}] COLLATE SQL_Latin1_General_CP1_CI_AS", databaseName);

                using (var conn = new SqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();  
                    } 
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Check permissions
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="checkRead">Check read</param>
        /// <param name="checkWrite">Check write</param>
        /// <param name="checkModify">Check modify</param>
        /// <param name="checkDelete">Check delete</param>
        /// <returns>Resulr</returns>
        private bool checkPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            System.Security.AccessControl.AuthorizationRuleCollection rules = null;
            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
            }
            catch
            {
                return true;
            }
            try
            {
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!current.User.Equals(rule.IdentityReference))
                    {
                        continue;
                    }
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                        {
                            flag8 = true;
                        }
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                        {
                            flag7 = true;
                        }
                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                        {
                            flag5 = true;
                        }
                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                        {
                            flag6 = true;
                        }
                    }
                }
                foreach (IdentityReference reference in current.Groups)
                {
                    foreach (FileSystemAccessRule rule2 in rules)
                    {
                        if (!reference.Equals(rule2.IdentityReference))
                        {
                            continue;
                        }
                        if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag4 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag3 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag2 = true;
                            continue;
                        }
                        if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag8 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag7 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag5 = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag6 = true;
                        }
                    }
                }
                bool flag9 = !flag4 && flag8;
                bool flag10 = !flag3 && flag7;
                bool flag11 = !flag && flag5;
                bool flag12 = !flag2 && flag6;
                bool flag13 = true;
                if (checkRead)
                {
                    flag13 = flag13 && flag11;
                }
                if (checkWrite)
                {
                    flag13 = flag13 && flag12;
                }
                if (checkModify)
                {
                    flag13 = flag13 && flag10;
                }
                if (checkDelete)
                {
                    flag13 = flag13 && flag9;
                }
                return flag13;
            }
            catch (IOException)
            {
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
        private string createConnectionString(bool trustedConnection,
            string serverName, string databaseName, string userName, string password, int timeout = 0)
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
            if (timeout > 0)
            {
                builder.ConnectTimeout = timeout;
            }
            return builder.ConnectionString;
        }
        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new InstallModel()
            {
                AdminEmail = "admin@yourStore.com",
                //AdminPassword = "admin",
                //ConfirmPassword = "admin",
                InstallSampleData = true,
                DatabaseConnectionString = "",
                DataProvider = "sqlserver",
                SqlAuthenticationType = "sqlauthentication",
                SqlConnectionInfo = "sqlconnectioninfo_values",
                SqlServerCreateDatabase = false,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            //SQL Server
            if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
            {
                if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                {
                    //raw connection string
                    if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                        ModelState.AddModelError("", "A SQL connection string is required");

                    try
                    {
                        //try to create connection string
                        new SqlConnectionStringBuilder(model.DatabaseConnectionString);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Wrong SQL connection string format");
                    }
                }
                else
                {
                    //values
                    if (string.IsNullOrEmpty(model.SqlServerName))
                        ModelState.AddModelError("", "SQL Server name is required");
                    if (string.IsNullOrEmpty(model.SqlDatabaseName))
                        ModelState.AddModelError("", "Database name is required");

                    //authentication type
                    if (model.SqlAuthenticationType.Equals("sqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //SQL authentication
                        if (string.IsNullOrEmpty(model.SqlServerUsername))
                            ModelState.AddModelError("", "SQL Username is required");
                        if (string.IsNullOrEmpty(model.SqlServerPassword))
                            ModelState.AddModelError("", "SQL Password is required");
                    }
                }
            }


            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

            //validate permissions
            string rootDir = Server.MapPath("~/");
            var dirsToCheck = new List<string>();
            dirsToCheck.Add(rootDir);
            dirsToCheck.Add(rootDir + "App_Data");
            dirsToCheck.Add(rootDir + "bin");
            dirsToCheck.Add(rootDir + "content");
            dirsToCheck.Add(rootDir + "content\\images");
            dirsToCheck.Add(rootDir + "content\\images\\thumbs");
            dirsToCheck.Add(rootDir + "content\\files\\exportimport");
            dirsToCheck.Add(rootDir + "plugins");
            dirsToCheck.Add(rootDir + "plugins\\bin");
            foreach (string dir in dirsToCheck)
                if (!checkPermissions(dir, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, dir));

            var filesToCheck = new List<string>();
            filesToCheck.Add(rootDir + "web.config");
            filesToCheck.Add(rootDir + "App_Data\\InstalledPlugins.txt");
            filesToCheck.Add(rootDir + "App_Data\\Settings.txt");
            foreach (string file in filesToCheck)
                if (!checkPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on file '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, file));
            
            if (ModelState.IsValid)
            {
                var settingsManager = new DataSettingsManager();
                try
                {
                    string connectionString = null;
                    if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //SQL Server

                        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //raw connection string
                            connectionString = model.DatabaseConnectionString;
                        }
                        else
                        {
                            //values
                            connectionString = createConnectionString(model.SqlAuthenticationType == "windowsauthentication",
                                model.SqlServerName, model.SqlDatabaseName,
                                model.SqlServerUsername, model.SqlServerPassword);
                        }
                        
                        if (model.SqlServerCreateDatabase)
                        {
                            if (!sqlServerDatabaseExists(connectionString))
                            {
                                //create database
                                var errorCreatingDatabase = createDatabase(connectionString);
                                if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                    throw new Exception(errorCreatingDatabase);
                                else
                                {
                                    //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                    //that's just wait 3 seconds
                                    Thread.Sleep(3000);
                                }
                            }
                        }
                        else
                        {
                            //check whether database exists
                            if (!sqlServerDatabaseExists(connectionString))
                                throw new Exception("Database does not exist or you don't have permissions to connect to it");
                        }
                    }
                    else
                    {
                        //SQL CE
                        //little hack here (SQL CE 4 bug - http://www.hanselman.com/blog/PDC10BuildingABlogWithMicrosoftUnnamedPackageOfWebLove.aspx)
                        //string databasePath = HostingEnvironment.MapPath("~/App_Data/") + @"Nop.Db.sdf";
                        string databaseFileName = "Nop.Db.sdf";
                        string databasePath = @"|DataDirectory|\" + databaseFileName;
                        connectionString = "Data Source=" + databasePath + ";Persist Security Info=False";

                        //drop database if exists
                        string databaseFullPath = HostingEnvironment.MapPath("~/App_Data/") + databaseFileName;
                        if (System.IO.File.Exists(databaseFullPath))
                        {
                            System.IO.File.Delete(databaseFullPath);
                        }
                    }

                    //save settings
                    //save settings
                    var dataProvider = model.DataProvider;
                    var settings = new DataSettings()
                    {
                        DataProvider = dataProvider,
                        DataConnectionString = connectionString
                    };
                    settingsManager.SaveSettings(settings);

                    //init data provider
                    var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                    dataProviderInstance.InitDatabase();
                    
                    
                    //now resolve installation service
                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminEmail, model.AdminPassword, model.InstallSampleData);

                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //install plugins
                    PluginManager.MarkAllPluginsAsUninstalled();
                    var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
                    var plugins = pluginFinder.GetPlugins<IPlugin>(false)
                        .ToList()
                        .OrderBy(x => x.PluginDescriptor.Group)
                        .ThenBy(x => x.PluginDescriptor.DisplayOrder)
                        .ToList();
                    foreach (var plugin in plugins)
                    {
                        plugin.Install();
                    }
                    
                    //register default permissions
                    //var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
                    var permissionProviders = new List<Type>();
                    permissionProviders.Add(typeof(StandardPermissionProvider));
                    foreach (var providerType in permissionProviders)
                    {
                        dynamic provider = Activator.CreateInstance(providerType);
                        EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
                    }

                    //restart application
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    webHelper.RestartAppDomain();

                    //Redirect to home page
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception exception)
                {
                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //clear provider settings if something got wrong
                    settingsManager.SaveSettings(new DataSettings
                    {
                        DataProvider = null,
                        DataConnectionString = null
                    });
                    
                    ModelState.AddModelError("", "Setup failed: " + exception);
                }
            }
            return View(model);
        }

        public ActionResult RestartInstall()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");
            
            //restart application
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain("~/Install/Index");

            //Redirect to home page
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
