using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Settings;
using Nop.Admin.Models.Stores;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI.Captcha;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public partial class SettingController : BaseNopController
	{
		#region Fields

        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IOrderService _orderService;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;


        private TaxSettings _taxSettings;
        private CatalogSettings _catalogSettings;
        private RewardPointsSettings _rewardPointsSettings;
        private readonly CurrencySettings _currencySettings;
        private OrderSettings _orderSettings;
        private ShoppingCartSettings _shoppingCartSettings;
        private MediaSettings _mediaSettings;
        private CustomerSettings _customerSettings;
        private AddressSettings _addressSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly SeoSettings _seoSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly PdfSettings _pdfSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
	    private readonly CommonSettings _commonSettings;

		#endregion

		#region Constructors

        public SettingController(ISettingService settingService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IAddressService addressService, ITaxCategoryService taxCategoryService,
            ICurrencyService currencyService, IPictureService pictureService, 
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IOrderService orderService, IEncryptionService encryptionService,
            IThemeProvider themeProvider, ICustomerService customerService, 
            ICustomerActivityService customerActivityService, IPermissionService permissionService,
            IWebHelper webHelper, IFulltextService fulltextService, 
            IMaintenanceService maintenanceService, IStoreService storeService,
            IWorkContext workContext, IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            CatalogSettings catalogSettings, RewardPointsSettings rewardPointsSettings,
            CurrencySettings currencySettings, OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings, MediaSettings mediaSettings,
            CustomerSettings customerSettings, AddressSettings addressSettings,
            DateTimeSettings dateTimeSettings, StoreInformationSettings storeInformationSettings,
            SeoSettings seoSettings,SecuritySettings securitySettings, PdfSettings pdfSettings,
            LocalizationSettings localizationSettings, CaptchaSettings captchaSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            CommonSettings commonSettings)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._taxCategoryService = taxCategoryService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._orderService = orderService;
            this._encryptionService = encryptionService;
            this._themeProvider = themeProvider;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._webHelper = webHelper;
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;

            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._currencySettings = currencySettings;
            this._orderSettings = orderSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._mediaSettings = mediaSettings;
            this._customerSettings = customerSettings;
            this._addressSettings = addressSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._seoSettings = seoSettings;
            this._securitySettings = securitySettings;
            this._pdfSettings = pdfSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._commonSettings = commonSettings;
        }

		#endregion 
        
        #region Methods
        
        [NonAction]
        private int GetActiveStoreScopeConfiguration()
        {
            //ensure that we have 2 (or more) stores
            if (_storeService.GetAllStores().Count < 2)
                return 0;


            var storeId = _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = _storeService.GetStoreById(storeId);
            return store != null ? store.Id : 0;
        }
        [ChildActionOnly]
        public ActionResult StoreScopeConfiguration()
        {
            var allStores = _storeService.GetAllStores();
            if (allStores.Count < 2)
                return Content("");

            var model = new StoreScopeConfigurationModel();
            foreach (var s in allStores)
            {
                model.Stores.Add(new StoreModel()
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
            model.StoreId = GetActiveStoreScopeConfiguration();

            return PartialView(model);
        }
        public ActionResult ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
        {
            var store = _storeService.GetStoreById(storeid);
            if (store != null || storeid == 0)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration, storeid);
            }
            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = _webHelper.GetUrlReferrer();
            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home");
            return Redirect(returnUrl);
        }

        public ActionResult Blog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var blogSettings = _settingService.LoadSetting<BlogSettings>(storeScope);
            var model = blogSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.Enabled, storeScope);
                model.PostsPageSize_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.PostsPageSize, storeScope);
                model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);
                model.NotifyAboutNewBlogComments_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.NotifyAboutNewBlogComments, storeScope);
                model.NumberOfTags_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.NumberOfTags, storeScope);
                model.ShowHeaderRssUrl_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.ShowHeaderRssUrl, storeScope);
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Blog(BlogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var blogSettings = _settingService.LoadSetting<BlogSettings>(storeScope);
            blogSettings = model.ToEntity(blogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.Enabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.Enabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.Enabled, storeScope);
            
            if (model.PostsPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.PostsPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.PostsPageSize, storeScope);
            
            if (model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);
            
            if (model.NotifyAboutNewBlogComments_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.NotifyAboutNewBlogComments, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.NotifyAboutNewBlogComments, storeScope);
            
            if (model.NumberOfTags_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.NumberOfTags, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.NumberOfTags, storeScope);
            
            if (model.ShowHeaderRssUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(blogSettings, x => x.ShowHeaderRssUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(blogSettings, x => x.ShowHeaderRssUrl, storeScope);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Blog");
        }




        public ActionResult Forum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            var model = forumSettings.ToModel();
            model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ForumsEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumsEnabled, storeScope);
                model.RelativeDateTimeFormattingEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeScope);
                model.ShowCustomersPostCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ShowCustomersPostCount, storeScope);
                model.AllowGuestsToCreatePosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreatePosts, storeScope);
                model.AllowGuestsToCreateTopics_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreateTopics, storeScope);
                model.AllowCustomersToEditPosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToEditPosts, storeScope);
                model.AllowCustomersToDeletePosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToDeletePosts, storeScope);
                model.AllowCustomersToManageSubscriptions_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeScope);
                model.TopicsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.TopicsPageSize, storeScope);
                model.PostsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.PostsPageSize, storeScope);
                model.ForumEditor_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumEditor, storeScope);
                model.SignaturesEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.SignaturesEnabled, storeScope);
                model.AllowPrivateMessages_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowPrivateMessages, storeScope);
                model.ShowAlertForPM_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ShowAlertForPM, storeScope);
                model.NotifyAboutPrivateMessages_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.NotifyAboutPrivateMessages, storeScope);
                model.ActiveDiscussionsFeedEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeScope);
                model.ActiveDiscussionsFeedCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedCount, storeScope);
                model.ForumFeedsEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumFeedsEnabled, storeScope);
                model.ForumFeedCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumFeedCount, storeScope);
                model.SearchResultsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.SearchResultsPageSize, storeScope);
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Forum(ForumSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            forumSettings = model.ToEntity(forumSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.ForumsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ForumsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ForumsEnabled, storeScope);
            
            if (model.RelativeDateTimeFormattingEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeScope);
            
            if (model.ShowCustomersPostCount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ShowCustomersPostCount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ShowCustomersPostCount, storeScope);
            
            if (model.AllowGuestsToCreatePosts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowGuestsToCreatePosts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowGuestsToCreatePosts, storeScope);
            
            if (model.AllowGuestsToCreateTopics_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowGuestsToCreateTopics, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowGuestsToCreateTopics, storeScope);
            
            if (model.AllowCustomersToEditPosts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowCustomersToEditPosts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowCustomersToEditPosts, storeScope);
            
            if (model.AllowCustomersToDeletePosts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowCustomersToDeletePosts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowCustomersToDeletePosts, storeScope);
            
            if (model.AllowCustomersToManageSubscriptions_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeScope);
            
            if (model.TopicsPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.TopicsPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.TopicsPageSize, storeScope);
            
            if (model.PostsPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.PostsPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.PostsPageSize, storeScope);
            
            if (model.ForumEditor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ForumEditor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ForumEditor, storeScope);
            
            if (model.SignaturesEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.SignaturesEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.SignaturesEnabled, storeScope);
            
            if (model.SignaturesEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.SignaturesEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.SignaturesEnabled, storeScope);
            
            if (model.AllowPrivateMessages_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.AllowPrivateMessages, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.AllowPrivateMessages, storeScope);
            
            if (model.ShowAlertForPM_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ShowAlertForPM, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ShowAlertForPM, storeScope);
            
            if (model.NotifyAboutPrivateMessages_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.NotifyAboutPrivateMessages, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.NotifyAboutPrivateMessages, storeScope);
            
            if (model.ActiveDiscussionsFeedEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeScope);
            
            if (model.ActiveDiscussionsFeedCount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ActiveDiscussionsFeedCount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ActiveDiscussionsFeedCount, storeScope);
            
            if (model.ForumFeedsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ForumFeedsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ForumFeedsEnabled, storeScope);
            
            if (model.ForumFeedCount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ForumFeedCount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ForumFeedCount, storeScope);
            
            if (model.SearchResultsPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.SearchResultsPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.SearchResultsPageSize, storeScope);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Forum");
        }




        public ActionResult News()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
            var model = newsSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.Enabled, storeScope);
                model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);
                model.NotifyAboutNewNewsComments_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.NotifyAboutNewNewsComments, storeScope);
                model.ShowNewsOnMainPage_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.ShowNewsOnMainPage, storeScope);
                model.MainPageNewsCount_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.MainPageNewsCount, storeScope);
                model.NewsArchivePageSize_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.NewsArchivePageSize, storeScope);
                model.ShowHeaderRssUrl_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.ShowHeaderRssUrl, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult News(NewsSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
            newsSettings = model.ToEntity(newsSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.Enabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.Enabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.Enabled, storeScope);

            if (model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);

            if (model.NotifyAboutNewNewsComments_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.NotifyAboutNewNewsComments, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.NotifyAboutNewNewsComments, storeScope);

            if (model.ShowNewsOnMainPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.ShowNewsOnMainPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.ShowNewsOnMainPage, storeScope);

            if (model.MainPageNewsCount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.MainPageNewsCount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.MainPageNewsCount, storeScope);

            if (model.NewsArchivePageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.NewsArchivePageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.NewsArchivePageSize, storeScope);

            if (model.ShowHeaderRssUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(newsSettings, x => x.ShowHeaderRssUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(newsSettings, x => x.ShowHeaderRssUrl, storeScope);

            //now clear settings cache
            _settingService.ClearCache();


            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("News");
        }




        public ActionResult Shipping()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(storeScope);
            var model = shippingSettings.ToModel();//shipping origin
            var originAddress = shippingSettings.ShippingOriginAddressId > 0
                                     ? _addressService.GetAddressById(shippingSettings.ShippingOriginAddressId)
                                     : null;
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel();
            else
                model.ShippingOriginAddress = new AddressModel();

            model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (originAddress != null && c.Id == originAddress.CountryId) });

            var states = originAddress != null && originAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(originAddress.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == originAddress.StateProvinceId) });
            }
            else
                model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.ShippingOriginAddress.CountryEnabled = true;
            model.ShippingOriginAddress.StateProvinceEnabled = true;
            model.ShippingOriginAddress.ZipPostalCodeEnabled = true;
            model.ShippingOriginAddress.ZipPostalCodeRequired = true;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.FreeShippingOverXEnabled_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXEnabled, storeScope);
                model.FreeShippingOverXValue_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXValue, storeScope);
                model.FreeShippingOverXIncludingTax_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeScope);
                model.EstimateShippingEnabled_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.EstimateShippingEnabled, storeScope);
                model.DisplayShipmentEventsToCustomers_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeScope);
                model.ShippingOriginAddress_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.ShippingOriginAddressId, storeScope);
            }
            
            return View(model);
        }
        [HttpPost]
        public ActionResult Shipping(ShippingSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration();
            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(storeScope);
            shippingSettings = model.ToEntity(shippingSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            if (model.FreeShippingOverXEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.FreeShippingOverXEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.FreeShippingOverXEnabled, storeScope);

            if (model.FreeShippingOverXValue_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.FreeShippingOverXValue, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.FreeShippingOverXValue, storeScope);

            if (model.FreeShippingOverXIncludingTax_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeScope);

            if (model.EstimateShippingEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.EstimateShippingEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.EstimateShippingEnabled, storeScope);

            if (model.DisplayShipmentEventsToCustomers_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeScope);

            if (model.ShippingOriginAddress_OverrideForStore || storeScope == 0)
            {
                //update address
                var addressId = _settingService.SettingExists(shippingSettings, x => x.ShippingOriginAddressId, storeScope) ?
                    shippingSettings.ShippingOriginAddressId : 0;
                var originAddress = _addressService.GetAddressById(addressId) ??
                    new Core.Domain.Common.Address()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                //update ID manually (in case we're in multi-store configuration mode it'll be set to the shared one)
                model.ShippingOriginAddress.Id = addressId;
                originAddress = model.ShippingOriginAddress.ToEntity(originAddress);
                if (originAddress.Id > 0)
                    _addressService.UpdateAddress(originAddress);
                else
                    _addressService.InsertAddress(originAddress);
                shippingSettings.ShippingOriginAddressId = originAddress.Id;

                _settingService.SaveSetting(shippingSettings, x => x.ShippingOriginAddressId, storeScope, false);
            }
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.ShippingOriginAddressId, storeScope);


            //now clear settings cache
            _settingService.ClearCache();


            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Shipping");
        }




        public ActionResult Tax()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _taxSettings.ToModel();
            model.TaxBasedOnValues = _taxSettings.TaxBasedOn.ToSelectList();
            model.TaxDisplayTypeValues = _taxSettings.TaxDisplayType.ToSelectList();

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.ShippingTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.ShippingTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == _taxSettings.ShippingTaxClassId });
            model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == _taxSettings.PaymentMethodAdditionalFeeTaxClassId });

            //EU VAT countries
            model.EuVatShopCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.EuVatShopCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id == _taxSettings.EuVatShopCountryId });

            //default tax address
            var defaultAddress = _taxSettings.DefaultTaxAddressId > 0
                                     ? _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId)
                                     : null;
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel();
            else
                model.DefaultTaxAddress = new AddressModel();

            model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (defaultAddress != null && c.Id == defaultAddress.CountryId) });

            var states = defaultAddress != null && defaultAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(defaultAddress.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == defaultAddress.StateProvinceId) });
            }
            else
                model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.DefaultTaxAddress.CountryEnabled = true;
            model.DefaultTaxAddress.StateProvinceEnabled = true;
            model.DefaultTaxAddress.ZipPostalCodeEnabled = true;
            model.DefaultTaxAddress.ZipPostalCodeRequired = true;

            return View(model);
        }
        [HttpPost]
        public ActionResult Tax(TaxSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _taxSettings = model.ToEntity(_taxSettings);

            var defaultAddress = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId) ??
                                         new Core.Domain.Common.Address()
                                         {
                                             CreatedOnUtc = DateTime.UtcNow,
                                         };
            defaultAddress = model.DefaultTaxAddress.ToEntity(defaultAddress);
            if (defaultAddress.Id > 0)
                _addressService.UpdateAddress(defaultAddress);
            else
                _addressService.InsertAddress(defaultAddress);

            _taxSettings.DefaultTaxAddressId = defaultAddress.Id;
            _settingService.SaveSetting(_taxSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Tax");
        }




        public ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _catalogSettings.ToModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _catalogSettings = model.ToEntity(_catalogSettings);
            _settingService.SaveSetting(_catalogSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Catalog");
        }



        public ActionResult RewardPoints()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _rewardPointsSettings.ToModel();
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            return View(model);
        }
        [HttpPost]
        public ActionResult RewardPoints(RewardPointsSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            if (ModelState.IsValid)
            {
                _rewardPointsSettings = model.ToEntity(_rewardPointsSettings);
                _settingService.SaveSetting(_rewardPointsSettings);

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));
                
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"), false);
            }
            
            return View(model);
        }




        public ActionResult Order()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _orderSettings.ToModel();
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //gift card activation/deactivation
            model.GiftCards_Activated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Activated_OrderStatuses.Insert(0, new SelectListItem() { Text = "---", Value = "0" });
            model.GiftCards_Deactivated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Deactivated_OrderStatuses.Insert(0, new SelectListItem() { Text = "---", Value = "0" });


            //parse return request actions
            for (int i = 0; i < _orderSettings.ReturnRequestActions.Count; i++)
            {
                model.ReturnRequestActionsParsed += _orderSettings.ReturnRequestActions[i];
                if (i != _orderSettings.ReturnRequestActions.Count - 1)
                    model.ReturnRequestActionsParsed += ",";
            }
            //parse return request reasons
            for (int i = 0; i < _orderSettings.ReturnRequestReasons.Count; i++)
            {
                model.ReturnRequestReasonsParsed += _orderSettings.ReturnRequestReasons[i];
                if (i != _orderSettings.ReturnRequestReasons.Count - 1)
                    model.ReturnRequestReasonsParsed += ",";
            }

            //order ident
            model.OrderIdent = _maintenanceService.GetTableIdent<Order>();

            return View(model);
        }
        [HttpPost]
        public ActionResult Order(OrderSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
                _orderSettings = model.ToEntity(_orderSettings);

                //parse return request actions
                _orderSettings.ReturnRequestActions.Clear();
                foreach (var returnAction in model.ReturnRequestActionsParsed.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    _orderSettings.ReturnRequestActions.Add(returnAction);
                //parse return request reasons
                _orderSettings.ReturnRequestReasons.Clear();
                foreach (var returnReason in model.ReturnRequestReasonsParsed.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    _orderSettings.ReturnRequestReasons.Add(returnReason);

                _settingService.SaveSetting(_orderSettings);

                //order ident
                if (model.OrderIdent.HasValue)
                {
                    try
                    {
                        _maintenanceService.SetTableIdent<Order>(model.OrderIdent.Value);
                    }
                    catch (Exception exc)
                    {
                        ErrorNotification(exc.Message);
                    }
                }

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        ErrorNotification(error.ErrorMessage);
            }
            return RedirectToAction("Order");
        }




        public ActionResult ShoppingCart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _shoppingCartSettings.ToModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ShoppingCart(ShoppingCartSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _shoppingCartSettings = model.ToEntity(_shoppingCartSettings);
            _settingService.SaveSetting(_shoppingCartSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("ShoppingCart");
        }




        public ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = _mediaSettings.ToModel();
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _mediaSettings = model.ToEntity(_mediaSettings);
            _settingService.SaveSetting(_mediaSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public ActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }



        public ActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //merge settings
            var model = new CustomerUserSettingsModel();
            model.CustomerSettings = _customerSettings.ToModel();
            model.AddressSettings = _addressSettings.ToModel();

            model.DateTimeSettings.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem()
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = _externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _customerSettings = model.CustomerSettings.ToEntity(_customerSettings);
            _settingService.SaveSetting(_customerSettings);

            _addressSettings = model.AddressSettings.ToEntity(_addressSettings);
            _settingService.SaveSetting(_addressSettings);

            _dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            _dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(_dateTimeSettings);

            _externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(_externalAuthenticationSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("CustomerUser");
        }






        public ActionResult GeneralCommon(string selectedTab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            //store information
            var model = new GeneralCommonSettingsModel();
            model.StoreInformationSettings.StoreClosed = _storeInformationSettings.StoreClosed;
            model.StoreInformationSettings.StoreClosedAllowForAdmins = _storeInformationSettings.StoreClosedAllowForAdmins;
            //desktop themes
            model.StoreInformationSettings.DefaultStoreThemeForDesktops = _storeInformationSettings.DefaultStoreThemeForDesktops;
            model.StoreInformationSettings.AvailableStoreThemesForDesktops = _themeProvider
                .GetThemeConfigurations()
                .Where(x => !x.MobileTheme)  //do not display themes for mobile devices
                .Select(x =>
                {
                    return new SelectListItem()
                    {
                        Text = x.ThemeTitle,
                        Value = x.ThemeName,
                        Selected = x.ThemeName.Equals(_storeInformationSettings.DefaultStoreThemeForDesktops, StringComparison.InvariantCultureIgnoreCase)
                    };
                })
                .ToList();
            model.StoreInformationSettings.AllowCustomerToSelectTheme = _storeInformationSettings.AllowCustomerToSelectTheme;
            model.StoreInformationSettings.MobileDevicesSupported = _storeInformationSettings.MobileDevicesSupported;
            //mobile device themes
            model.StoreInformationSettings.DefaultStoreThemeForMobileDevices = _storeInformationSettings.DefaultStoreThemeForMobileDevices;
            model.StoreInformationSettings.AvailableStoreThemesForMobileDevices = _themeProvider
                .GetThemeConfigurations()
                .Where(x => x.MobileTheme)  //do not display themes for desktops
                .Select(x =>
                {
                    return new SelectListItem()
                    {
                        Text = x.ThemeTitle,
                        Value = x.ThemeName,
                        Selected = x.ThemeName.Equals(_storeInformationSettings.DefaultStoreThemeForMobileDevices, StringComparison.InvariantCultureIgnoreCase)
                    };
                })
                .ToList();
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = _storeInformationSettings.DisplayEuCookieLawWarning;

            //seo settings
            model.SeoSettings.PageTitleSeparator = _seoSettings.PageTitleSeparator;
            model.SeoSettings.DefaultTitle = _seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = _seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = _seoSettings.DefaultMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = _seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = _seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.PageTitleSeoAdjustmentValues = _seoSettings.PageTitleSeoAdjustment.ToSelectList();
            
            //security settings
            model.SecuritySettings.EncryptionKey = _securitySettings.EncryptionKey;
            if (_securitySettings.AdminAreaAllowedIpAddresses!=null)
                for (int i=0;i<_securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += _securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != _securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions = _securitySettings.HideAdminMenuItemsBasedOnPermissions;
            model.SecuritySettings.CaptchaEnabled = _captchaSettings.Enabled;
            model.SecuritySettings.CaptchaShowOnLoginPage = _captchaSettings.ShowOnLoginPage;
            model.SecuritySettings.CaptchaShowOnRegistrationPage = _captchaSettings.ShowOnRegistrationPage;
            model.SecuritySettings.CaptchaShowOnContactUsPage = _captchaSettings.ShowOnContactUsPage;
            model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage = _captchaSettings.ShowOnEmailWishlistToFriendPage;
            model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage = _captchaSettings.ShowOnEmailProductToFriendPage;
            model.SecuritySettings.CaptchaShowOnBlogCommentPage = _captchaSettings.ShowOnBlogCommentPage;
            model.SecuritySettings.CaptchaShowOnNewsCommentPage = _captchaSettings.ShowOnNewsCommentPage;
            model.SecuritySettings.CaptchaShowOnProductReviewPage = _captchaSettings.ShowOnProductReviewPage;
            model.SecuritySettings.ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey;
            model.SecuritySettings.ReCaptchaPrivateKey = _captchaSettings.ReCaptchaPrivateKey;

            //PDF settings
            model.PdfSettings.Enabled = _pdfSettings.Enabled;
            model.PdfSettings.LetterPageSizeEnabled = _pdfSettings.LetterPageSizeEnabled;
            model.PdfSettings.LogoPictureId = _pdfSettings.LogoPictureId;

            //localization
            model.LocalizationSettings.UseImagesForLanguageSelection = _localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;

            //full-text support
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = _commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchModeValues = _commonSettings.FullTextMode.ToSelectList();


            ViewData["selectedTab"] = selectedTab;
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            _storeInformationSettings.StoreClosedAllowForAdmins = model.StoreInformationSettings.StoreClosedAllowForAdmins;
            _storeInformationSettings.DefaultStoreThemeForDesktops = model.StoreInformationSettings.DefaultStoreThemeForDesktops;
            _storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            //store whether MobileDevicesSupported setting has been changed (requires application restart)
            bool mobileDevicesSupportedChanged = _storeInformationSettings.MobileDevicesSupported !=
                                                 model.StoreInformationSettings.MobileDevicesSupported;
            _storeInformationSettings.MobileDevicesSupported = model.StoreInformationSettings.MobileDevicesSupported;
            _storeInformationSettings.DefaultStoreThemeForMobileDevices = model.StoreInformationSettings.DefaultStoreThemeForMobileDevices;
            //EU Cookie law
            _storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            _settingService.SaveSetting(_storeInformationSettings);



            //seo settings
            _seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            _seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            _seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            _seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            _seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            _seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            _seoSettings.PageTitleSeoAdjustment = model.SeoSettings.PageTitleSeoAdjustment;
            _settingService.SaveSetting(_seoSettings);



            //security settings
            if (_securitySettings.AdminAreaAllowedIpAddresses == null)
                _securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            _securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        _securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            _securitySettings.HideAdminMenuItemsBasedOnPermissions = model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions;
            _settingService.SaveSetting(_securitySettings);
            _captchaSettings.Enabled = model.SecuritySettings.CaptchaEnabled;
            _captchaSettings.ShowOnLoginPage = model.SecuritySettings.CaptchaShowOnLoginPage;
            _captchaSettings.ShowOnRegistrationPage = model.SecuritySettings.CaptchaShowOnRegistrationPage;
            _captchaSettings.ShowOnContactUsPage = model.SecuritySettings.CaptchaShowOnContactUsPage;
            _captchaSettings.ShowOnEmailWishlistToFriendPage = model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage;
            _captchaSettings.ShowOnEmailProductToFriendPage = model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage;
            _captchaSettings.ShowOnBlogCommentPage = model.SecuritySettings.CaptchaShowOnBlogCommentPage;
            _captchaSettings.ShowOnNewsCommentPage = model.SecuritySettings.CaptchaShowOnNewsCommentPage;
            _captchaSettings.ShowOnProductReviewPage = model.SecuritySettings.CaptchaShowOnProductReviewPage;
            _captchaSettings.ReCaptchaPublicKey = model.SecuritySettings.ReCaptchaPublicKey;
            _captchaSettings.ReCaptchaPrivateKey = model.SecuritySettings.ReCaptchaPrivateKey;
            _settingService.SaveSetting(_captchaSettings);
            if (_captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(_captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(_captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification("Captcha is enabled but the appropriate keys are not entered");
            }

            //PDF settings
            _pdfSettings.Enabled = model.PdfSettings.Enabled;
            _pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            _pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
            _settingService.SaveSetting(_pdfSettings);


            //localization settings
            _localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                //clear cached values of routes
                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
            }
            _settingService.SaveSetting(_localizationSettings);

            //full-text
            _commonSettings.FullTextMode = model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(_commonSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            if (mobileDevicesSupportedChanged)
            {
                //MobileDevicesSupported setting has been changed
                //restart application
                _webHelper.RestartAppDomain();
            }

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public ActionResult ChangeEnryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = _securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update encrypted order info
                var orders = _orderService.SearchOrders(0, 0, null, null, null, null, null, null, null, 0, int.MaxValue);
                foreach (var order in orders)
                {
                    string decryptedCardType = _encryptionService.DecryptText(order.CardType, oldEncryptionPrivateKey);
                    string decryptedCardName = _encryptionService.DecryptText(order.CardName, oldEncryptionPrivateKey);
                    string decryptedCardNumber = _encryptionService.DecryptText(order.CardNumber, oldEncryptionPrivateKey);
                    string decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(order.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                    string decryptedCardCvv2 = _encryptionService.DecryptText(order.CardCvv2, oldEncryptionPrivateKey);
                    string decryptedCardExpirationMonth = _encryptionService.DecryptText(order.CardExpirationMonth, oldEncryptionPrivateKey);
                    string decryptedCardExpirationYear = _encryptionService.DecryptText(order.CardExpirationYear, oldEncryptionPrivateKey);

                    string encryptedCardType = _encryptionService.EncryptText(decryptedCardType, newEncryptionPrivateKey);
                    string encryptedCardName = _encryptionService.EncryptText(decryptedCardName, newEncryptionPrivateKey);
                    string encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, newEncryptionPrivateKey);
                    string encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                    string encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, newEncryptionPrivateKey);
                    string encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                    string encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, newEncryptionPrivateKey);

                    order.CardType = encryptedCardType;
                    order.CardName = encryptedCardName;
                    order.CardNumber = encryptedCardNumber;
                    order.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
                    order.CardCvv2 = encryptedCardCvv2;
                    order.CardExpirationMonth = encryptedCardExpirationMonth;
                    order.CardExpirationYear = encryptedCardExpirationYear;
                    _orderService.UpdateOrder(order);
                }

                //update user information
                //optimization - load only users with PasswordFormat.Encrypted
                var customers = _customerService.GetAllCustomersByPasswordFormat(PasswordFormat.Encrypted);
                foreach (var customer in customers)
                {
                    string decryptedPassword = _encryptionService.DecryptText(customer.Password, oldEncryptionPrivateKey);
                    string encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    customer.Password = encryptedPassword;
                    _customerService.UpdateCustomer(customer);
                }

                _securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(_securitySettings);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            return RedirectToAction("GeneralCommon", new { selectedTab = "security" });
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            try
            {
                if (! _fulltextService.IsFullTextSupported())
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                if (_commonSettings.UseFullTextSearch)
                {
                    _fulltextService.DisableFullText();

                    _commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(_commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                {
                    _fulltextService.EnableFullText();

                    _commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(_commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            return RedirectToAction("GeneralCommon", new { selectedTab = "fulltext" });
        }




        //all settings
        public ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AllSettings(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService
                .GetAllSettings()
                .Select(x =>
                            {
                                string storeName = "";
                                if (x.StoreId == 0)
                                {
                                    storeName = _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                                }
                                else
                                {
                                    var store = _storeService.GetStoreById(x.StoreId);
                                    storeName = store != null ? store.Name : "Unknown";
                                }
                                var settingModel = new SettingModel()
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Value = x.Value,
                                    Store = storeName,
                                    StoreId = x.StoreId
                                };
                                return settingModel;
                            })
                .ForCommand(command)
                .ToList();
            
            var model = new GridModel<SettingModel>
            {
                Data = settings.PagedForCommand(command),
                Total = settings.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult SettingUpdate(SettingModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            var storeId = Int32.Parse(model.Store); //use Store property (not StoreId) because appropriate property is stored in it

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeId)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return AllSettings(command);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            var storeId = Int32.Parse(model.Store); //use Store property (not StoreId) because appropriate property is stored in it
            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return AllSettings(command);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult SettingDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return AllSettings(command);
        }

        #endregion
    }
}
