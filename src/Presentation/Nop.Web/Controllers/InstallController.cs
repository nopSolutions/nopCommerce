using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Installation;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;

namespace Nop.Web.Controllers
{
    public partial class InstallController : Controller
    {
        #region Fields

        private readonly IInstallationLocalizationService _locService;
        private readonly INopFileProvider _fileProvider;
        private readonly NopConfig _config;

        #endregion

        #region Ctor

        public InstallController(IInstallationLocalizationService locService,
            INopFileProvider fileProvider,
            NopConfig config)
        {
            _locService = locService;
            _fileProvider = fileProvider;
            _config = config;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// A value indicating whether we use MARS (Multiple Active Result Sets)
        /// </summary>
        protected virtual bool UseMars => false;

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        protected virtual bool SqlServerDatabaseExists(string connectionString)
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
        /// <param name="collation">Server collation; the default one will be used if not specified</param>
        /// <param name="triesToConnect">
        /// Number of times to try to connect to database. 
        /// If connection cannot be open, then error will be returned. 
        /// Pass 0 to skip this validation.
        /// </param>
        /// <returns>Error</returns>
        protected virtual string CreateDatabase(string connectionString, string collation, int triesToConnect = 10)
        {
            try
            {
                //parse database name
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;
                //now create connection string to 'master' dabatase. It always exists.
                builder.InitialCatalog = "master";
                var masterCatalogConnectionString = builder.ToString();
                var query = $"CREATE DATABASE [{databaseName}]";
                if (!string.IsNullOrWhiteSpace(collation))
                    query = $"{query} COLLATE {collation}";
                using (var conn = new SqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                //try connect
                if (triesToConnect <= 0) 
                    return string.Empty;

                //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
                //but we have already started creation of tables and sample data.
                //as a result there is an exception thrown and the installation process cannot continue.
                //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
                for (var i = 0; i <= triesToConnect; i++)
                {
                    if (i == triesToConnect)
                        throw new Exception("Unable to connect to the new database. Please try one more time");

                    if (!SqlServerDatabaseExists(connectionString))
                        Thread.Sleep(1000);
                    else
                        break;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format(_locService.GetResource("DatabaseCreationError"), ex.Message);
            }
        }

        /// <summary>
        /// Create contents of connection strings used by the SqlConnection class
        /// </summary>
        /// <param name="trustedConnection">Avalue that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="username">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <param name="timeout">The connection timeout</param>
        /// <returns>Connection string</returns>
        protected virtual string CreateConnectionString(bool trustedConnection,
            string serverName, string databaseName,
            string username, string password, int timeout = 0)
        {
            var builder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = trustedConnection,
                DataSource = serverName,
                InitialCatalog = databaseName
            };

            if (!trustedConnection)
            {
                builder.UserID = username;
                builder.Password = password;
            }

            builder.PersistSecurityInfo = false;
            if (UseMars)
            {
                builder.MultipleActiveResultSets = true;
            }

            if (timeout > 0)
            {
                builder.ConnectTimeout = timeout;
            }

            return builder.ConnectionString;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("Homepage");

            var model = new InstallModel
            {
                AdminEmail = "admin@yourStore.com",
                InstallSampleData = false,
                DatabaseConnectionString = string.Empty,
                DataProvider = DataProviderType.SqlServer,
                //fast installation service does not support SQL compact
                DisableSampleDataOption = _config.DisableSampleDataDuringInstallation,
                SqlAuthenticationType = "sqlauthentication",
                SqlConnectionInfo = "sqlconnectioninfo_values",
                SqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            foreach (var lang in _locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code
                });
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Index(InstallModel model)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("Homepage");

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            //prepare language list
            foreach (var lang in _locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code
                });
            }

            model.DisableSampleDataOption = _config.DisableSampleDataDuringInstallation;

            //SQL Server
            if (model.DataProvider == DataProviderType.SqlServer)
            {
                if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                {
                    //raw connection string
                    if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                        ModelState.AddModelError(string.Empty, _locService.GetResource("ConnectionStringRequired"));

                    try
                    {
                        //try to create connection string
                        var unused = new SqlConnectionStringBuilder(model.DatabaseConnectionString);    
                    }
                    catch
                    {
                        ModelState.AddModelError(string.Empty, _locService.GetResource("ConnectionStringWrongFormat"));
                    }
                }
                else
                {
                    //values
                    if (string.IsNullOrEmpty(model.SqlServerName))
                        ModelState.AddModelError(string.Empty, _locService.GetResource("SqlServerNameRequired"));
                    if (string.IsNullOrEmpty(model.SqlDatabaseName))
                        ModelState.AddModelError(string.Empty, _locService.GetResource("DatabaseNameRequired"));

                    //authentication type
                    if (model.SqlAuthenticationType.Equals("sqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //SQL authentication
                        if (string.IsNullOrEmpty(model.SqlServerUsername))
                            ModelState.AddModelError(string.Empty, _locService.GetResource("SqlServerUsernameRequired"));
                        if (string.IsNullOrEmpty(model.SqlServerPassword))
                            ModelState.AddModelError(string.Empty, _locService.GetResource("SqlServerPasswordRequired"));
                    }
                }
            }

            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //validate permissions
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.GetResource("ConfigureDirectoryPermissions"), CurrentOSUser.FullName, dir));

            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (!_fileProvider.FileExists(file))
                    continue;

                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.GetResource("ConfigureFilePermissions"), CurrentOSUser.FullName, file));
            }

            if (!ModelState.IsValid) 
                return View(model);

            try
            {
                var connectionString = string.Empty;
                if (model.DataProvider == DataProviderType.SqlServer)
                {
                    //SQL Server

                    if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //raw connection string

                        //we know that MARS option is required when using Entity Framework
                        //let's ensure that it's specified
                        var sqlCsb = new SqlConnectionStringBuilder(model.DatabaseConnectionString);
                        if (UseMars)
                            sqlCsb.MultipleActiveResultSets = true;

                        connectionString = sqlCsb.ToString();
                    }
                    else
                    {
                        //values
                        connectionString = CreateConnectionString(model.SqlAuthenticationType == "windowsauthentication",
                            model.SqlServerName, model.SqlDatabaseName,
                            model.SqlServerUsername, model.SqlServerPassword);
                    }

                    if (model.SqlServerCreateDatabase)
                    {
                        if (!SqlServerDatabaseExists(connectionString))
                        {
                            //create database
                            var collation = model.UseCustomCollation ? model.Collation : string.Empty;
                            var errorCreatingDatabase = CreateDatabase(connectionString, collation);
                            if (!string.IsNullOrEmpty(errorCreatingDatabase))
                                throw new Exception(errorCreatingDatabase);
                        }
                    }
                    else
                    {
                        //check whether database exists
                        if (!SqlServerDatabaseExists(connectionString))
                            throw new Exception(_locService.GetResource("DatabaseNotExists"));
                    }
                }

                //save settings
                DataSettingsManager.SaveSettings(new DataSettings
                {
                    DataProvider = model.DataProvider,
                    DataConnectionString = connectionString
                }, _fileProvider);

                //initialize database
                EngineContext.Current.Resolve<IDataProvider>().InitializeDatabase();

                //now resolve installation service
                var installationService = EngineContext.Current.Resolve<IInstallationService>();
                installationService.InstallRequiredData(model.AdminEmail, model.AdminPassword);

                if (model.InstallSampleData)
                    installationService.InstallSampleData(model.AdminEmail);

                //reset cache
                DataSettingsManager.ResetCache();

                //prepare plugins to install
                var pluginService = EngineContext.Current.Resolve<IPluginService>();
                pluginService.ClearInstalledPluginsList();

                var pluginsIgnoredDuringInstallation = new List<string>();
                if (!string.IsNullOrEmpty(_config.PluginsIgnoredDuringInstallation))
                {
                    pluginsIgnoredDuringInstallation = _config.PluginsIgnoredDuringInstallation
                        .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pluginName => pluginName.Trim()).ToList();
                }

                var plugins = pluginService.GetPluginDescriptors<IPlugin>(LoadPluginsMode.All)
                    .Where(pluginDescriptor => !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                    .OrderBy(pluginDescriptor => pluginDescriptor.Group).ThenBy(pluginDescriptor => pluginDescriptor.DisplayOrder)
                    .ToList();

                foreach (var plugin in plugins)
                {
                    pluginService.PreparePluginToInstall(plugin.SystemName, checkDependencies: false);
                }

                //register default permissions
                //var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
                var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
                foreach (var providerType in permissionProviders)
                {
                    var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                    EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
                }

                //restart application
                webHelper.RestartAppDomain();

                //Redirect to home page
                return RedirectToRoute("Homepage");
            }
            catch (Exception exception)
            {
                //reset cache
                DataSettingsManager.ResetCache();

                var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
                cacheManager.Clear();

                //clear provider settings if something got wrong
                DataSettingsManager.SaveSettings(new DataSettings(), _fileProvider);

                ModelState.AddModelError(string.Empty, string.Format(_locService.GetResource("SetupFailed"), exception.Message));
            }

            return View(model);
        }

        public virtual IActionResult ChangeLanguage(string language)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("Homepage");

            _locService.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        [HttpPost]
        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("Homepage");

            //restart application
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain();

            //Redirect to home page
            return RedirectToRoute("Homepage");
        }

        #endregion
    }
}