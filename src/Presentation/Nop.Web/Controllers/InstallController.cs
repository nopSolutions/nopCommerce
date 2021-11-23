using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class InstallController : Controller
    {
        #region Fields

        protected AppSettings AppSettings { get; }
        protected Lazy<IInstallationLocalizationService> LocService { get; }
        protected Lazy<IInstallationService> InstallationService { get; }
        protected INopFileProvider FileProvider { get; }
        protected Lazy<IPermissionService> PermissionService { get; }
        protected Lazy<IPluginService> PluginService { get; }
        protected Lazy<IStaticCacheManager> StaticCacheManager { get; }
        protected Lazy<IUploadService> UploadService { get; }
        protected Lazy<IWebHelper> WebHelper { get; }
        protected Lazy<NopHttpClient> NopHttpClient { get; }

        #endregion

        #region Ctor

        public InstallController(AppSettings appSettings,
            Lazy<IInstallationLocalizationService> locService,
            Lazy<IInstallationService> installationService,
            INopFileProvider fileProvider,
            Lazy<IPermissionService> permissionService,
            Lazy<IPluginService> pluginService,
            Lazy<IStaticCacheManager> staticCacheManager,
            Lazy<IUploadService> uploadService,
            Lazy<IWebHelper> webHelper,
            Lazy<NopHttpClient> nopHttpClient)
        {
            AppSettings = appSettings;
            LocService = locService;
            InstallationService = installationService;
            FileProvider = fileProvider;
            PermissionService = permissionService;
            PluginService = pluginService;
            StaticCacheManager = staticCacheManager;
            UploadService = uploadService;
            WebHelper = webHelper;
            NopHttpClient = nopHttpClient;
        }

        #endregion

        #region Utilites

        private InstallModel PrepareCountryList(InstallModel model)
        {
            if (!model.InstallRegionalResources)
                return model;

            var browserCulture = LocService.Value.GetBrowserCulture();
            var countries = new List<SelectListItem>
            {
                //This item was added in case it was not possible to automatically determine the country by culture
                new SelectListItem { Value = string.Empty, Text = LocService.Value.GetResource("CountrySelect") }
            };
            countries.AddRange(from country in ISO3166.GetCollection()
                               from localization in ISO3166.GetLocalizationInfo(country.Alpha2)
                               let lang = ISO3166.GetLocalizationInfo(country.Alpha2).Count() > 1 ? $" [{localization.Language} language]" : string.Empty
                               let item = new SelectListItem
                               {
                                   Value = $"{country.Alpha2}-{localization.Culture}",
                                   Text = $"{country.Name}{lang}",
                                   Selected = (localization.Culture == browserCulture) && browserCulture[^2..] == country.Alpha2
                               }
                               select item);
            model.AvailableCountries.AddRange(countries);

            return model;
        }

        private InstallModel PrepareLanguageList(InstallModel model)
        {
            foreach (var lang in LocService.Value.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = LocService.Value.GetCurrentLanguage().Code == lang.Code
                });
            }

            return model;
        }

        private InstallModel PrepareAvailableDataProviders(InstallModel model)
        {
            model.AvailableDataProviders.AddRange(
                LocService.Value.GetAvailableProviderTypes()
                .OrderBy(v => v.Value)
                .Select(pt => new SelectListItem
                {
                    Value = pt.Key.ToString(),
                    Text = pt.Value
                }));

            return model;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            var model = new InstallModel
            {
                AdminEmail = "admin@yourStore.com",
                InstallSampleData = false,
                InstallRegionalResources = AppSettings.Get<InstallationConfig>().InstallRegionalResources,
                DisableSampleDataOption = AppSettings.Get<InstallationConfig>().DisableSampleData,
                CreateDatabaseIfNotExists = false,
                ConnectionStringRaw = false,
                DataProvider = DataProviderType.SqlServer
            };

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(InstallModel model)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            model.DisableSampleDataOption = AppSettings.Get<InstallationConfig>().DisableSampleData;
            model.InstallRegionalResources = AppSettings.Get<InstallationConfig>().InstallRegionalResources;

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

            //validate permissions
            var dirsToCheck = FileProvider.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!FileProvider.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError(string.Empty, string.Format(LocService.Value.GetResource("ConfigureDirectoryPermissions"), CurrentOSUser.FullName, dir));

            var filesToCheck = FileProvider.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (!FileProvider.FileExists(file))
                    continue;

                if (!FileProvider.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError(string.Empty, string.Format(LocService.Value.GetResource("ConfigureFilePermissions"), CurrentOSUser.FullName, file));
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dataProvider = DataProviderManager.GetDataProvider(model.DataProvider);

                var connectionString = model.ConnectionStringRaw ? model.ConnectionString : dataProvider.BuildConnectionString(model);

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception(LocService.Value.GetResource("ConnectionStringWrongFormat"));

                DataSettingsManager.SaveSettings(new DataConfig
                {
                    DataProvider = model.DataProvider,
                    ConnectionString = connectionString
                }, FileProvider);

                if (model.CreateDatabaseIfNotExists)
                {
                    try
                    {
                        dataProvider.CreateDatabase(model.Collation);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(LocService.Value.GetResource("DatabaseCreationError"), ex.Message));
                    }
                }
                else
                {
                    //check whether database exists
                    if (!await dataProvider.DatabaseExistsAsync())
                        throw new Exception(LocService.Value.GetResource("DatabaseNotExists"));
                }

                dataProvider.InitializeDatabase();

                var cultureInfo = new CultureInfo(NopCommonDefaults.DefaultLanguageCulture);
                var regionInfo = new RegionInfo(NopCommonDefaults.DefaultLanguageCulture);

                var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);
                if (model.InstallRegionalResources)
                {
                    //try to get CultureInfo and RegionInfo
                    try
                    {
                        cultureInfo = new CultureInfo(model.Country[3..]);
                        regionInfo = new RegionInfo(model.Country[3..]);
                    }
                    catch { }

                    //get URL to download language pack
                    if (cultureInfo.Name != NopCommonDefaults.DefaultLanguageCulture)
                    {
                        try
                        {
                            var languageCode = LocService.Value.GetCurrentLanguage().Code[0..2];
                            var resultString = await NopHttpClient.Value.InstallationCompletedAsync(model.AdminEmail, languageCode, cultureInfo.Name);
                            var result = JsonConvert.DeserializeAnonymousType(resultString,
                                new { Message = string.Empty, LanguagePack = new { Culture = string.Empty, Progress = 0, DownloadLink = string.Empty } });
                            if (result.LanguagePack.Progress > NopCommonDefaults.LanguagePackMinTranslationProgressToInstall)
                            {
                                languagePackInfo.DownloadUrl = result.LanguagePack.DownloadLink;
                                languagePackInfo.Progress = result.LanguagePack.Progress;
                            }

                        }
                        catch { }
                    }

                    //upload CLDR
                    UploadService.Value.UploadLocalePattern(cultureInfo);
                }

                //now resolve installation service
                await InstallationService.Value.InstallRequiredDataAsync(model.AdminEmail, model.AdminPassword, languagePackInfo, regionInfo, cultureInfo);

                if (model.InstallSampleData)
                    await InstallationService.Value.InstallSampleDataAsync(model.AdminEmail);

                //prepare plugins to install
                PluginService.Value.ClearInstalledPluginsList();

                var pluginsIgnoredDuringInstallation = new List<string>();
                if (!string.IsNullOrEmpty(AppSettings.Get<InstallationConfig>().DisabledPlugins))
                {
                    pluginsIgnoredDuringInstallation = AppSettings.Get<InstallationConfig>().DisabledPlugins
                        .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pluginName => pluginName.Trim()).ToList();
                }

                var plugins = (await PluginService.Value.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
                    .Where(pluginDescriptor => !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                    .OrderBy(pluginDescriptor => pluginDescriptor.Group).ThenBy(pluginDescriptor => pluginDescriptor.DisplayOrder)
                    .ToList();

                foreach (var plugin in plugins)
                {
                    await PluginService.Value.PreparePluginToInstallAsync(plugin.SystemName, checkDependencies: false);
                }

                //register default permissions
                var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
                foreach (var providerType in permissionProviders)
                {
                    var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                    await PermissionService.Value.InstallPermissionsAsync(provider);
                }

                return View(new InstallModel { RestartUrl = Url.RouteUrl("Homepage") });

            }
            catch (Exception exception)
            {
                await StaticCacheManager.Value.ClearAsync();

                //clear provider settings if something got wrong
                DataSettingsManager.SaveSettings(new DataConfig(), FileProvider);

                ModelState.AddModelError(string.Empty, string.Format(LocService.Value.GetResource("SetupFailed"), exception.Message));
            }

            return View(model);
        }

        public virtual IActionResult ChangeLanguage(string language)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            LocService.Value.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        [HttpPost]
        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            return View("Index", new InstallModel { RestartUrl = Url.Action("Index", "Install") });
        }

        public virtual IActionResult RestartApplication()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            //restart application
            WebHelper.Value.RestartAppDomain();

            return new EmptyResult();
        }

        #endregion
    }
}