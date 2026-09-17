using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.AIRecommendation.GoogleAI.Services;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.AIRecommendation.GoogleAI;

public class GoogleAiPlugin : BasePlugin, IMiscPlugin, IAiRecommendationPlugin
{
    #region Fields

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly GoogleAiService _googleAiService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public GoogleAiPlugin(ArtificialIntelligenceSettings artificialIntelligenceSettings,
        GoogleAiService googleAiService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _googleAiService = googleAiService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get products identifiers by the specified keywords
    /// </summary>
    /// <param name="keywords">Keywords</param>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="filteredSpecOptions">Filtered specification options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains product identifiers
    /// </returns>
    public async Task<List<int>> SearchProductsAsync(string keywords, IList<int> categoryIds = null,
        IList<int> manufacturerIds = null, int productTagId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null)
    {
        var googleAiSettings = await _settingService.LoadSettingAsync<GoogleAiSettings>();

        if (!googleAiSettings.SearchAllowed || !googleAiSettings.Enabled)
            return [];

        return await _googleAiService.SearchProductsAsync(keywords, categoryIds, manufacturerIds, productTagId, filteredSpecOptions);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(GoogleAiDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        await _settingService.SetSettingAsync($"{nameof(MediaSettings)}.{nameof(MediaSettings.UseAbsoluteImagePath)}", true, clearCache: false);

        //settings
        var googleAiSettings = new GoogleAiSettings
        {
            Enabled = false,
            LocationId = "global",
            CatalogId = "default_catalog",
            BranchId = "default_branch",
            ProjectId = string.Empty,
            LogRequests = false,
            SearchAllowed = true,
            SyncAllowed = true
        };

        //settings
        if (string.IsNullOrEmpty(_artificialIntelligenceSettings.ActiveAIRecommendationProviderSystemName))
        {
            googleAiSettings.Enabled = true;

            _artificialIntelligenceSettings.ActiveAIRecommendationProviderSystemName = GoogleAiDefaults.SystemName;
            await _settingService.SaveSettingAsync(_artificialIntelligenceSettings);
        }

        await _settingService.SaveSettingAsync(googleAiSettings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugin.AIRecommendation.GoogleAI.Title"] = "Google AI powered recommendation",
            ["Plugin.AIRecommendation.GoogleAI.Description"] = "<p>Authorization for Google Cloud is primarily handled by <a href=\"https://docs.cloud.google.com/iam/docs/overview\" target=\"_blank\">Identity and Access Management (IAM)</a>. IAM offers granular control by principal and by resource.</p>\r\n<p>This plugin support <a href=\"https://docs.cloud.google.com/docs/authentication/application-default-credentials\" target=\"_blank\">Application Default Credentials (ADC)</a>. When you use ADC, your code can run in either a development or production environment without changing how your application authenticates to Google Cloud services and APIs.</p>\r\n<p>The most sutebal way to configure ADC for this integration is configire with credentials from a service account by using <a href=\"https://docs.cloud.google.com/docs/authentication#service-accounts\" target=\"_blank\">service account</a> impersonation or by using a service account key.</p>\r\n\r\n<h3>Step‑by‑step instructions</h3>\r\n\r\n<p><strong>Creating a service account and a key</strong></p>\r\n\r\n<ul>\r\n\t<li><strong>Open the Google Cloud CLI:</strong> go to the <a href=\"https://console.cloud.google.com/iam-admin/serviceaccounts\" target=\"_blank\">IAM & Admin > Service Accounts</a> section in Google Cloud.</li>\r\n\t<li><strong>Create an account:</strong> click Create Service Account, enter a name, and then click Create and Continue.</li>\r\n\t<li><strong>Assign a role:</strong> select the role required to work with the Retail API (for example, Retail Editor or Retail Viewer, depending on your tasks). Click Done.</li>\r\n\t<li><strong>Download the JSON key:</strong> find the created account in the list, go to the Keys tab, click Add Key > Create new key, and select the JSON format. The key file will be automatically downloaded to your computer.</li>\r\n</ul>\r\n\r\n<p><strong>Setting up the environment on your server</strong></p>\r\n<p>Transfer the downloaded JSON file to your server (for example, to the /etc/gcloud/ directory). Then configure the environment variable so that the plugin can locate this file.</p>\r\n<p>For Linux (in the terminal or in the .bashrc startup script): export GOOGLE_APPLICATION_CREDENTIALS=\"/etc/gcloud/your-service-account-key.json\"</p>\r\n<p>For Windows (in PowerShell): [System.Environment]::SetEnvironmentVariable('GOOGLE_APPLICATION_CREDENTIALS', 'C:\\gcloud\\your-service-account-key.json', 'Machine')</p>\r\n<p>Via Docker (if the application is containerized): Pass the variable and mount the key file via a volume: docker run -e GOOGLE_APPLICATION_CREDENTIALS=/app/key.json -v /etc/gcloud/your-service-account-key.json:/app/key.json my-retail-app</p><p><strong>Warning!</strong> To work this plugin properly you need to install and configure the \"Google Analytics\" plugin from nopCommerce team.</p>",
            ["Plugin.AIRecommendation.GoogleAI.Enabled"] = "Enabled",
            ["Plugin.AIRecommendation.GoogleAI.Enabled.Hint"] = "Check the box to enable the Google AI powered recommendation plugin.",
            ["Plugin.AIRecommendation.GoogleAI.ProjectId"] = "Project ID",
            ["Plugin.AIRecommendation.GoogleAI.ProjectId.Hint"] = "Enter the Google Cloud Project ID.",
            ["Plugin.AIRecommendation.GoogleAI.ProjectId.Required"] = "Project ID is required.",
            ["Plugin.AIRecommendation.GoogleAI.LocationId"] = "Location ID",
            ["Plugin.AIRecommendation.GoogleAI.LocationId.Hint"] = "Enter the Google Cloud Location ID.",
            ["Plugin.AIRecommendation.GoogleAI.LocationId.Required"] = "Location ID is required.",
            ["Plugin.AIRecommendation.GoogleAI.CatalogId"] = "Catalog ID",
            ["Plugin.AIRecommendation.GoogleAI.CatalogId.Hint"] = "Enter the Google Cloud Catalog ID.",
            ["Plugin.AIRecommendation.GoogleAI.CatalogId.Required"] = "Catalog ID is required.",
            ["Plugin.AIRecommendation.GoogleAI.BranchId"] = "Branch ID",
            ["Plugin.AIRecommendation.GoogleAI.BranchId.Hint"] = "Enter the Google Cloud Branch ID.",
            ["Plugin.AIRecommendation.GoogleAI.BranchId.Required"] = "Branch ID is required.",
            ["Plugin.AIRecommendation.GoogleAI.LogRequests"] = "Log Requests",
            ["Plugin.AIRecommendation.GoogleAI.LogRequests.Hint"] = "Check the box to log requests sent to Google AI.",
            ["Plugin.AIRecommendation.GoogleAI.SyncAllowed"] = "Sync Allowed",
            ["Plugin.AIRecommendation.GoogleAI.SyncAllowed.Hint"] = "Check the box to allow synchronization of product data with Google AI. You may no need this function if you use another method (for example a bigquery).",
            ["Plugin.AIRecommendation.GoogleAI.SearchAllowed"] = "Search Allowed",
            ["Plugin.AIRecommendation.GoogleAI.SearchAllowed.Hint"] = "Check the box to allow searching by Google AI",
            ["Plugin.AIRecommendation.GoogleAI.SyncCatalog"] = "Sync catalog now",
            ["Plugin.AIRecommendation.GoogleAI.CatalogImportedSuccessfully"] = "Catalog imported successfully (successfully imported {0} products, failed to import {1} products)",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<GoogleAiSettings>();

        if (_artificialIntelligenceSettings.ActiveAIRecommendationProviderSystemName == GoogleAiDefaults.SystemName)
        {
            _artificialIntelligenceSettings.ActiveAIRecommendationProviderSystemName = string.Empty;
            await _settingService.SaveSettingAsync(_artificialIntelligenceSettings);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugin.AIRecommendation.GoogleAI");

        await base.UninstallAsync();
    }

    #endregion
}