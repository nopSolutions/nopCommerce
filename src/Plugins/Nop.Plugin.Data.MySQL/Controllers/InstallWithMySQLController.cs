using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Installation;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;

namespace Nop.Plugin.Data.MySQL.Controllers
{
    public class InstallWithMySQLController : InstallController
    {
        #region Fields

        private readonly IInstallationLocalizationService _locService;
        private readonly INopFileProvider _fileProvider;
        private readonly NopConfig _config;

        #endregion

        #region Ctor

        public InstallWithMySQLController(IInstallationLocalizationService locService,
            INopFileProvider fileProvider,
            NopConfig config)
            : base(locService, 
                fileProvider, 
                config)
        {
            _locService = locService;
            _fileProvider = fileProvider;
            _config = config;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        protected virtual bool MySQLDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new MySqlConnection(connectionString))
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
        protected virtual string CreateMySQLDatabase(string connectionString, string collation, int triesToConnect = 10)
        {
            try
            {
                //parse database name
                var builder = new MySqlConnectionStringBuilder(connectionString);
                var databaseName = builder.Database;
                //now create connection string to 'mysql' dabatase. It always exists.
                builder.Database = "mysql";
                var masterCatalogConnectionString = builder.ToString();
                var query = $"CREATE DATABASE {databaseName}";
                if (!string.IsNullOrWhiteSpace(collation))
                    query = $"{query} COLLATE {collation}";
                using (var conn = new MySqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new MySqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                //try connect
                if (triesToConnect > 0)
                {
                    //Sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
                    //But we have already started creation of tables and sample data.
                    //As a result there is an exception thrown and the installation process cannot continue.
                    //That's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
                    for (var i = 0; i <= triesToConnect; i++)
                    {
                        if (i == triesToConnect)
                            throw new Exception("Unable to connect to the new database. Please try one more time");

                        if (!this.MySQLDatabaseExists(connectionString))
                            Thread.Sleep(1000);
                        else
                            break;
                    }
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
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <param name="timeout">The connection timeout</param>
        /// <returns>Connection string</returns>
        protected virtual string CreateMySQLConnectionString(bool trustedConnection,
            string serverName, string databaseName,
            string userName, string password, int timeout = 0)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = serverName,
                Database = databaseName
            };
            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }
            builder.PersistSecurityInfo = false;
            if (timeout > 0)
            {
                builder.ConnectionTimeout = (uint)timeout;
            }
            return builder.ConnectionString;
        }

        #endregion

        #region Methods

        public override IActionResult Index()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("HomePage");

            var model = new InstallModel
            {
                AdminEmail = "admin@yourStore.com",
                InstallSampleData = false,
                DatabaseConnectionString = "",
                DataProvider = DataProviderType.MySQL,
                //fast installation service does not support SQL compact
                DisableSampleDataOption = _config.DisableSampleDataDuringInstallation,
                SqlAuthenticationType = "mysqlauthentication",
                SqlConnectionInfo = "mysqlconnectioninfo_values",
                SqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "utf8_unicode_ci",
            };

            foreach (var lang in _locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code,
                });
            }

            return View("~/Plugins/Data.MySQL/Views/Index.cshtml", model);
        }

        [HttpPost]
        public override IActionResult Index(InstallModel model)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("HomePage");

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            //prepare language list
            foreach (var lang in _locService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code,
                });
            }

            model.DisableSampleDataOption = _config.DisableSampleDataDuringInstallation;

            if (model.DataProvider == DataProviderType.SqlServer)
            {
                return base.Index(model);
            }
            else if (model.DataProvider == DataProviderType.MySQL)
            {
                if (model.SqlConnectionInfo.Equals("mysqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                {
                    //raw connection string
                    if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                        ModelState.AddModelError("", _locService.GetResource("ConnectionStringRequired"));

                    try
                    {
                        //try to create connection string
                        new MySqlConnectionStringBuilder(model.DatabaseConnectionString);
                    }
                    catch
                    {
                        ModelState.AddModelError("", _locService.GetResource("ConnectionStringWrongFormat"));
                    }
                }
                else
                {
                    //values
                    if (string.IsNullOrEmpty(model.SqlServerName))
                        ModelState.AddModelError("", _locService.GetResource("SqlServerNameRequired"));
                    if (string.IsNullOrEmpty(model.SqlDatabaseName))
                        ModelState.AddModelError("", _locService.GetResource("DatabaseNameRequired"));

                    //authentication type
                    if (model.SqlAuthenticationType.Equals("mysqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //MySQL authentication
                        if (string.IsNullOrEmpty(model.SqlServerUsername))
                            ModelState.AddModelError("", _locService.GetResource("SqlServerUsernameRequired"));
                        if (string.IsNullOrEmpty(model.SqlServerPassword))
                            ModelState.AddModelError("", _locService.GetResource("SqlServerPasswordRequired"));
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
            
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString;

                    if (model.SqlConnectionInfo.Equals("mysqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //raw connection string

                        //we know that MARS option is required when using Entity Framework
                        //let's ensure that it's specified
                        var sqlCsb = new MySqlConnectionStringBuilder(model.DatabaseConnectionString);
                        connectionString = sqlCsb.ToString();
                    }
                    else
                    {
                        //values
                        connectionString = CreateMySQLConnectionString(false, model.SqlServerName, model.SqlDatabaseName,
                            model.SqlServerUsername, model.SqlServerPassword);
                    }

                    if (model.SqlServerCreateDatabase)
                    {
                        if (!MySQLDatabaseExists(connectionString))
                        {
                            //create database
                            var collation = model.UseCustomCollation ? model.Collation : "";
                            var errorCreatingDatabase = CreateMySQLDatabase(connectionString, collation);
                            if (!string.IsNullOrEmpty(errorCreatingDatabase))
                                throw new Exception(errorCreatingDatabase);
                        }
                    }
                    else
                    {
                        //check whether database exists
                        if (!MySQLDatabaseExists(connectionString))
                            throw new Exception(_locService.GetResource("DatabaseNotExists"));
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
                    installationService.InstallData(model.AdminEmail, model.AdminPassword, model.InstallSampleData);

                    //reset cache
                    DataSettingsManager.ResetCache();

                    //install plugins
                    PluginManager.MarkAllPluginsAsUninstalled();
                    var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
                    var plugins = pluginFinder.GetPlugins<IPlugin>(LoadPluginsMode.All)
                        .ToList()
                        .OrderBy(x => x.PluginDescriptor.Group)
                        .ThenBy(x => x.PluginDescriptor.DisplayOrder)
                        .ToList();
                    var pluginsIgnoredDuringInstallation = string.IsNullOrEmpty(_config.PluginsIgnoredDuringInstallation) ?
                        new List<string>() :
                        _config.PluginsIgnoredDuringInstallation
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                    foreach (var plugin in plugins)
                    {
                        if (pluginsIgnoredDuringInstallation.Contains(plugin.PluginDescriptor.SystemName))
                            continue;

                        plugin.Install();
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
                    return RedirectToRoute("HomePage");
                }
                catch (Exception exception)
                {
                    //reset cache
                    DataSettingsManager.ResetCache();

                    var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
                    cacheManager.Clear();

                    //clear provider settings if something got wrong
                    DataSettingsManager.SaveSettings(new DataSettings(), _fileProvider);

                    ModelState.AddModelError("", string.Format(_locService.GetResource("SetupFailed"), exception.Message));
                }
            }

            return View("~/Plugins/Data.MySQL/Views/Index.cshtml", model);
        }

        #endregion
    }
}
