using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Hosting;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Installation;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
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
        private bool SqlServerDatabaseExists(string connectionString)
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
        /// Check permissions
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="checkRead">Check read</param>
        /// <param name="checkWrite">Check write</param>
        /// <param name="checkModify">Check modify</param>
        /// <param name="checkDelete">Check delete</param>
        /// <returns>Resulr</returns>
        private bool CheckPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
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
        private string CreateConnectionString(bool trustedConnection,
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
            if (SettingsHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            //TODO Allow store owner to enter database name, username, password (for SQL Server)
            var model = new InstallModel()
            {
                AdminEmail = "admin@yourStore.com",
                //AdminPassword = "admin",
                //ConfirmPassword = "admin",
                InstallSampleData = true,
                DatabaseConnectionString = "",
                DataProvider = "sqlce",
                SqlAuthenticationType = "sqlauthentication",
                SqlConnectionInfo = "sqlconnectioninfo_values"
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model)
        {
            if (SettingsHelper.DatabaseIsInstalled())
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
                    catch (Exception exc)
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

            //validate permissions
            string rootDir = Server.MapPath("~/");
            var dirsToCheck = new List<string>();
            dirsToCheck.Add(rootDir);
            dirsToCheck.Add(rootDir + "App_Data");
            dirsToCheck.Add(rootDir + "App_Data\\InstalledPlugins.txt");
            dirsToCheck.Add(rootDir + "App_Data\\Settings.txt");
            dirsToCheck.Add(rootDir + "content");
            dirsToCheck.Add(rootDir + "content\\images");
            dirsToCheck.Add(rootDir + "content\\images\\thumbs");
            dirsToCheck.Add(rootDir + "content\files\\exportimport");
            dirsToCheck.Add(rootDir + "plugins\bin");
            foreach (string dir in dirsToCheck)
                if (!CheckPermissions(dir, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, dir));
                

            var filesToCheck = new List<string>();
            filesToCheck.Add(rootDir + "web.config");
            filesToCheck.Add(rootDir + "bin\\" + "Settings.txt");
            foreach (string file in filesToCheck)
                if (!CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on file '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, file));
            
            if (ModelState.IsValid)
            {
                var settingsManager = new SettingsManager();
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
                            connectionString = CreateConnectionString(model.SqlAuthenticationType == "windowsauthentication",
                                model.SqlServerName, model.SqlDatabaseName,
                                model.SqlServerUsername, model.SqlServerPassword);
                        }
                        
                        //check whether database exists
                        if (!SqlServerDatabaseExists(connectionString))
                            throw new Exception("Database does not exist or you don't have permissions to connect to it");
                    }
                    else
                    {
                        //SQL CE
                        //little hack here (SQL CE 4 bug - http://www.hanselman.com/blog/PDC10BuildingABlogWithMicrosoftUnnamedPackageOfWebLove.aspx)
                        string databasePath = HostingEnvironment.MapPath("~/App_Data/") + @"Nop.Db.sdf";
                        connectionString = "Data Source=" + databasePath + ";Persist Security Info=False";

                        //drop database if exists
                        if (System.IO.File.Exists(databasePath))
                        {
                            System.IO.File.Delete(databasePath);
                        }
                    }

                    //save settings
                    //save settings
                    var dataProvider = model.DataProvider;
                    var settings = new Settings()
                    {
                        DataProvider = dataProvider,
                        DataConnectionString = connectionString
                    };
                    settingsManager.SaveSettings(settings);

                    //init data provider
                    var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                    dataProviderInstance.InitDatabase();
                    
                    
                    //only now resolve installation service
                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminEmail, model.AdminPassword, model.InstallSampleData);

                    //reset cache
                    SettingsHelper.ResetCache();

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
                    var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
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
                    //clear provider settings if something got wrong
                    settingsManager.SaveSettings(new Settings
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
            if (SettingsHelper.DatabaseIsInstalled())
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
