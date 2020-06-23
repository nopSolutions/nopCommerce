using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Authentication.External;
using Nop.Services.Caching;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Plugins.Marketplace;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the plugin model factory implementation
    /// </summary>
    public partial class PluginModelFactory : IPluginModelFactory
    {
        #region Fields

        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IPluginService _pluginService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly OfficialFeedManager _officialFeedManager;

        #endregion

        #region Ctor

        public PluginModelFactory(IAclSupportedModelFactory aclSupportedModelFactory,
            IAuthenticationPluginManager authenticationPluginManager,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICacheKeyService cacheKeyService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IPaymentPluginManager paymentPluginManager,
            IPickupPluginManager pickupPluginManager,
            IPluginService pluginService,
            IShippingPluginManager shippingPluginManager,
            IStaticCacheManager staticCacheManager,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ITaxPluginManager taxPluginManager,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            OfficialFeedManager officialFeedManager)
        {
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _authenticationPluginManager = authenticationPluginManager;
            _baseAdminModelFactory = baseAdminModelFactory;
            _cacheKeyService = cacheKeyService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _pickupPluginManager = pickupPluginManager;
            _pluginService = pluginService;
            _shippingPluginManager = shippingPluginManager;
            _staticCacheManager = staticCacheManager;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _taxPluginManager = taxPluginManager;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
            _officialFeedManager = officialFeedManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare plugin model properties of the installed plugin
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="plugin">Plugin</param>
        protected virtual void PrepareInstalledPluginModel(PluginModel model, IPlugin plugin)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            //prepare configuration URL
            model.ConfigurationUrl = plugin.GetConfigurationPageUrl();

            //prepare enabled/disabled (only for some plugin types)
            model.CanChangeEnabled = true;
            switch (plugin)
            {
                case IPaymentMethod paymentMethod:
                    model.IsEnabled = _paymentPluginManager.IsPluginActive(paymentMethod);
                    break;

                case IShippingRateComputationMethod shippingRateComputationMethod:
                    model.IsEnabled = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                    break;

                case IPickupPointProvider pickupPointProvider:
                    model.IsEnabled = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                    break;

                case ITaxProvider taxProvider:
                    model.IsEnabled = _taxPluginManager.IsPluginActive(taxProvider);
                    break;

                case IExternalAuthenticationMethod externalAuthenticationMethod:
                    model.IsEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                    break;

                case IWidgetPlugin widgetPlugin:
                    model.IsEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
                    break;

                default:
                    model.CanChangeEnabled = false;
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare plugin search model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>Plugin search model</returns>
        public virtual PluginSearchModel PreparePluginSearchModel(PluginSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available load plugin modes
            _baseAdminModelFactory.PrepareLoadPluginModes(searchModel.AvailableLoadModes, false);

            //prepare available groups
            _baseAdminModelFactory.PreparePluginGroups(searchModel.AvailableGroups);

            //prepare page parameters
            searchModel.SetGridPageSize();

            searchModel.NeedToRestart = _pluginService.IsRestartRequired();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged plugin list model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>Plugin list model</returns>
        public virtual PluginListModel PreparePluginListModel(PluginSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter plugins
            var group = string.IsNullOrEmpty(searchModel.SearchGroup) || searchModel.SearchGroup.Equals("0") ? null : searchModel.SearchGroup;
            var loadMode = (LoadPluginsMode)searchModel.SearchLoadModeId;
            var friendlyName = string.IsNullOrEmpty(searchModel.SearchFriendlyName) ? null : searchModel.SearchFriendlyName;
            var author = string.IsNullOrEmpty(searchModel.SearchAuthor) ? null : searchModel.SearchAuthor;

            //filter visible plugins
            var plugins = _pluginService.GetPluginDescriptors<IPlugin>(group: group, loadMode: loadMode, friendlyName: friendlyName, author: author)
                .Where(p => p.ShowInPluginsList)
                .OrderBy(plugin => plugin.Group).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new PluginListModel().PrepareToGrid(searchModel, plugins, () =>
            {
                return plugins.Select(pluginDescriptor =>
                {
                    //fill in model values from the entity
                    var pluginModel = pluginDescriptor.ToPluginModel<PluginModel>();

                    //fill in additional values (not existing in the entity)
                    pluginModel.LogoUrl = _pluginService.GetPluginLogoUrl(pluginDescriptor);
                    if (pluginDescriptor.Installed)
                        PrepareInstalledPluginModel(pluginModel, pluginDescriptor.Instance<IPlugin>());

                    return pluginModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare plugin model
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Plugin model</returns>
        public virtual PluginModel PreparePluginModel(PluginModel model, PluginDescriptor pluginDescriptor, bool excludeProperties = false)
        {
            Action<PluginLocalizedModel, int> localizedModelConfiguration = null;

            if (pluginDescriptor != null)
            {
                //fill in model values from the entity
                model ??= pluginDescriptor.ToPluginModel(model);

                model.LogoUrl = _pluginService.GetPluginLogoUrl(pluginDescriptor);
                model.SelectedStoreIds = pluginDescriptor.LimitedToStores;
                model.SelectedCustomerRoleIds = pluginDescriptor.LimitedToCustomerRoles;
                var plugin = pluginDescriptor.Instance<IPlugin>();
                if (pluginDescriptor.Installed)
                    PrepareInstalledPluginModel(model, plugin);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.FriendlyName = _localizationService.GetLocalizedFriendlyName(plugin, languageId, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model);

            return model;
        }

        /// <summary>
        /// Prepare search model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>Search model of plugins of the official feed</returns>
        public virtual OfficialFeedPluginSearchModel PrepareOfficialFeedPluginSearchModel(OfficialFeedPluginSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available versions
            var pluginVersions = _officialFeedManager.GetVersions();
            searchModel.AvailableVersions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var version in pluginVersions)
                searchModel.AvailableVersions.Add(new SelectListItem { Text = version.Name, Value = version.Id.ToString() });

            //pre-select current version
            //current version name and named on official site do not match. that's why we use "Contains"
            var currentVersionItem = searchModel.AvailableVersions.FirstOrDefault(x => x.Text.Contains(NopVersion.CurrentVersion));
            if (currentVersionItem != null)
            {
                searchModel.SearchVersionId = int.Parse(currentVersionItem.Value);
                currentVersionItem.Selected = true;
            }

            //prepare available plugin categories
            var pluginCategories = _officialFeedManager.GetCategories();
            searchModel.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var pluginCategory in pluginCategories)
            {
                var pluginCategoryNames = new List<string>();
                var tmpCategory = pluginCategory;
                while (tmpCategory != null)
                {
                    pluginCategoryNames.Add(tmpCategory.Name);
                    tmpCategory = pluginCategories.FirstOrDefault(category => category.Id == tmpCategory.ParentCategoryId);
                }

                pluginCategoryNames.Reverse();

                searchModel.AvailableCategories.Add(new SelectListItem
                {
                    Value = pluginCategory.Id.ToString(),
                    Text = string.Join(" >> ", pluginCategoryNames)
                });
            }

            //prepare available prices
            searchModel.AvailablePrices.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Common.All")
            });
            searchModel.AvailablePrices.Add(new SelectListItem
            {
                Value = "10",
                Text = _localizationService.GetResource("Admin.Configuration.Plugins.OfficialFeed.Price.Free")
            });
            searchModel.AvailablePrices.Add(new SelectListItem
            {
                Value = "20",
                Text = _localizationService.GetResource("Admin.Configuration.Plugins.OfficialFeed.Price.Commercial")
            });

            //prepare page parameters
            searchModel.SetGridPageSize(15, "15");

            return searchModel;
        }

        /// <summary>
        /// Prepare paged list model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>List model of plugins of the official feed</returns>
        public virtual OfficialFeedPluginListModel PrepareOfficialFeedPluginListModel(OfficialFeedPluginSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get plugins
            var plugins = _officialFeedManager.GetAllPlugins(categoryId: searchModel.SearchCategoryId,
                versionId: searchModel.SearchVersionId,
                price: searchModel.SearchPriceId,
                searchTerm: searchModel.SearchName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new OfficialFeedPluginListModel().PrepareToGrid(searchModel, plugins, () =>
            {
                //fill in model values from the entity
                return plugins?.Select(plugin => new OfficialFeedPluginModel
                {
                    Url = plugin.Url,
                    Name = plugin.Name,
                    CategoryName = plugin.Category,
                    SupportedVersions = plugin.SupportedVersions,
                    PictureUrl = plugin.PictureUrl,
                    Price = plugin.Price
                }) ?? new List<OfficialFeedPluginModel>();
            });

            return model;
        }

        /// <summary>
        /// Prepare plugins configuration model
        /// </summary>
        /// <param name="pluginsConfigurationModel">Plugins configuration model</param>
        /// <returns>Plugins configuration model</returns>
        public virtual PluginsConfigurationModel PreparePluginsConfigurationModel(PluginsConfigurationModel pluginsConfigurationModel)
        {
            if (pluginsConfigurationModel == null)
                throw new ArgumentNullException(nameof(pluginsConfigurationModel));

            //prepare nested search models
            PreparePluginSearchModel(pluginsConfigurationModel.PluginsLocal);
            PrepareOfficialFeedPluginSearchModel(pluginsConfigurationModel.AllPluginsAndThemes);

            return pluginsConfigurationModel;
        }

        /// <summary>
        /// Prepare plugin models for admin navigation
        /// </summary>
        /// <returns>List of models</returns>
        public virtual IList<AdminNavigationPluginModel> PrepareAdminNavigationPluginModels()
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopPluginDefaults.AdminNavigationPluginsCacheKey, _workContext.CurrentCustomer);
            return _staticCacheManager.Get(cacheKey, () =>
            {
                //get installed plugins
                return _pluginService.GetPluginDescriptors<IPlugin>(LoadPluginsMode.InstalledOnly, _workContext.CurrentCustomer)
                    .Where(plugin => plugin.ShowInPluginsList)
                    .Select(plugin => new AdminNavigationPluginModel
                    {
                        FriendlyName = plugin.FriendlyName,
                        ConfigurationUrl = plugin.Instance<IPlugin>().GetConfigurationPageUrl()
                    }).Where(model => !string.IsNullOrEmpty(model.ConfigurationUrl)).ToList();
            });
        }

        #endregion
    }
}