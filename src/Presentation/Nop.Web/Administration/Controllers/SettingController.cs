using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Settings;
using Nop.Admin.Models.Stores;
using Nop.Core;
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
using Nop.Core.Domain.Vendors;
using Nop.Services;
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
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;

namespace Nop.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
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
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion

        #region Constructors

        public SettingController(ISettingService settingService,
            ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            IAddressService addressService, 
            ITaxCategoryService taxCategoryService,
            ICurrencyService currencyService,
            IPictureService pictureService, 
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IOrderService orderService,
            IEncryptionService encryptionService,
            IThemeProvider themeProvider,
            ICustomerService customerService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IFulltextService fulltextService, 
            IMaintenanceService maintenanceService,
            IStoreService storeService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService,
            IReturnRequestService returnRequestService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
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
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._returnRequestService = returnRequestService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(ReturnRequestReason rrr, ReturnRequestReasonModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rrr,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(ReturnRequestAction rra, ReturnRequestActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rra,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Mode(string modeName = "settings-advanced-mode")
        {
            var model = new ModeModel()
            {
                ModeName = modeName,
                Enabled = _workContext.CurrentCustomer.GetAttribute<bool>(modeName)
            };
            return PartialView(model);
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
                model.Stores.Add(new StoreModel
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
            model.StoreId = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

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

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

        public ActionResult Blog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
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

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var blogSettings = _settingService.LoadSetting<BlogSettings>(storeScope);
            blogSettings = model.ToEntity(blogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.NotifyAboutNewBlogComments, model.NotifyAboutNewBlogComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.NumberOfTags, model.NumberOfTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Blog");
        }




        public ActionResult Vendor()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var vendorSettings = _settingService.LoadSetting<VendorSettings>(storeScope);
            var model = vendorSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.VendorsBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeScope);
                model.ShowVendorOnProductDetailsPage_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeScope);
                model.AllowCustomersToContactVendors_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.AllowCustomersToContactVendors, storeScope);
                model.AllowCustomersToApplyForVendorAccount_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeScope);
                model.AllowSearchByVendor_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.AllowSearchByVendor, storeScope);
                model.AllowVendorsToEditInfo_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.AllowVendorsToEditInfo, storeScope);
                model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeScope);
                model.MaximumProductNumber_OverrideForStore = _settingService.SettingExists(vendorSettings, x => x.MaximumProductNumber, storeScope);
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Vendor(VendorSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var vendorSettings = _settingService.LoadSetting<VendorSettings>(storeScope);
            vendorSettings = model.ToEntity(vendorSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.VendorsBlockItemsToDisplay, model.VendorsBlockItemsToDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.ShowVendorOnProductDetailsPage, model.ShowVendorOnProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.AllowCustomersToContactVendors, model.AllowCustomersToContactVendors_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, model.AllowCustomersToApplyForVendorAccount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.AllowSearchByVendor, model.AllowSearchByVendor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.AllowVendorsToEditInfo, model.AllowVendorsToEditInfo_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(vendorSettings, x => x.MaximumProductNumber, model.MaximumProductNumber_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Vendor");
        }




        public ActionResult Forum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            var model = forumSettings.ToModel();
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
                model.AllowPostVoting_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowPostVoting, storeScope);
                model.MaxVotesPerDay_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.MaxVotesPerDay, storeScope);
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
                model.ActiveDiscussionsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsPageSize, storeScope);
            }
            model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();

            return View(model);
        }
        [HttpPost]
        public ActionResult Forum(ForumSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            forumSettings = model.ToEntity(forumSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumsEnabled, model.ForumsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.RelativeDateTimeFormattingEnabled, model.RelativeDateTimeFormattingEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ShowCustomersPostCount, model.ShowCustomersPostCount_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowGuestsToCreatePosts, model.AllowGuestsToCreatePosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowGuestsToCreateTopics, model.AllowGuestsToCreateTopics_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToEditPosts, model.AllowCustomersToEditPosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToDeletePosts, model.AllowCustomersToDeletePosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowPostVoting, model.AllowPostVoting_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.MaxVotesPerDay, model.MaxVotesPerDay_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToManageSubscriptions, model.AllowCustomersToManageSubscriptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.TopicsPageSize, model.TopicsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumEditor, model.ForumEditor_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.SignaturesEnabled, model.SignaturesEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowPrivateMessages, model.AllowPrivateMessages_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ShowAlertForPM, model.ShowAlertForPM_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.NotifyAboutPrivateMessages, model.NotifyAboutPrivateMessages_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsFeedEnabled, model.ActiveDiscussionsFeedEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsFeedCount, model.ActiveDiscussionsFeedCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumFeedsEnabled, model.ForumFeedsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumFeedCount, model.ForumFeedCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.SearchResultsPageSize, model.SearchResultsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsPageSize, model.ActiveDiscussionsPageSize_OverrideForStore, storeScope, false);
            
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

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
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

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
            newsSettings = model.ToEntity(newsSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.NotifyAboutNewNewsComments, model.NotifyAboutNewNewsComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.ShowNewsOnMainPage, model.ShowNewsOnMainPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.MainPageNewsCount, model.MainPageNewsCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.NewsArchivePageSize, model.NewsArchivePageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
            
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

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(storeScope);
            var model = shippingSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ShipToSameAddress_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.ShipToSameAddress, storeScope);
                model.AllowPickUpInStore_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.AllowPickUpInStore, storeScope);
                model.DisplayPickupPointsOnMap_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.DisplayPickupPointsOnMap, storeScope);
                model.GoogleMapsApiKey_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.GoogleMapsApiKey, storeScope);
                model.UseWarehouseLocation_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.UseWarehouseLocation, storeScope);
                model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeScope);
                model.FreeShippingOverXEnabled_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXEnabled, storeScope);
                model.FreeShippingOverXValue_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXValue, storeScope);
                model.FreeShippingOverXIncludingTax_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeScope);
                model.EstimateShippingEnabled_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.EstimateShippingEnabled, storeScope);
                model.DisplayShipmentEventsToCustomers_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeScope);
                model.DisplayShipmentEventsToStoreOwner_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeScope);
                model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeScope);
                model.ShippingOriginAddress_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.ShippingOriginAddressId, storeScope);
            }
            //shipping origin
            var originAddress = shippingSettings.ShippingOriginAddressId > 0
                                     ? _addressService.GetAddressById(shippingSettings.ShippingOriginAddressId)
                                     : null;
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel();
            else
                model.ShippingOriginAddress = new AddressModel();

            model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (originAddress != null && c.Id == originAddress.CountryId) });

            var states = originAddress != null && originAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(originAddress.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == originAddress.StateProvinceId) });
            }
            else
                model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.ShippingOriginAddress.CountryEnabled = true;
            model.ShippingOriginAddress.StateProvinceEnabled = true;
            model.ShippingOriginAddress.CityEnabled = true;
            model.ShippingOriginAddress.StreetAddressEnabled = true;
            model.ShippingOriginAddress.ZipPostalCodeEnabled = true;
            model.ShippingOriginAddress.ZipPostalCodeRequired = true;
            
            return View(model);
        }
        [HttpPost]
        public ActionResult Shipping(ShippingSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(storeScope);
            shippingSettings = model.ToEntity(shippingSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.ShipToSameAddress, model.ShipToSameAddress_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.AllowPickUpInStore, model.AllowPickUpInStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.DisplayPickupPointsOnMap, model.DisplayPickupPointsOnMap_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.GoogleMapsApiKey, model.GoogleMapsApiKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.UseWarehouseLocation, model.UseWarehouseLocation_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.FreeShippingOverXEnabled, model.FreeShippingOverXEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.FreeShippingOverXValue, model.FreeShippingOverXValue_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.FreeShippingOverXIncludingTax, model.FreeShippingOverXIncludingTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.EstimateShippingEnabled, model.EstimateShippingEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.DisplayShipmentEventsToCustomers, model.DisplayShipmentEventsToCustomers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, model.DisplayShipmentEventsToStoreOwner_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, storeScope, false);
            
            if (model.ShippingOriginAddress_OverrideForStore || storeScope == 0)
            {
                //update address
                var addressId = _settingService.SettingExists(shippingSettings, x => x.ShippingOriginAddressId, storeScope) ?
                    shippingSettings.ShippingOriginAddressId : 0;
                var originAddress = _addressService.GetAddressById(addressId) ??
                    new Address
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


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var taxSettings = _settingService.LoadSetting<TaxSettings>(storeScope);
            var model = taxSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.PricesIncludeTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PricesIncludeTax, storeScope);
                model.AllowCustomersToSelectTaxDisplayType_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeScope);
                model.TaxDisplayType_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.TaxDisplayType, storeScope);
                model.DisplayTaxSuffix_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DisplayTaxSuffix, storeScope);
                model.DisplayTaxRates_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DisplayTaxRates, storeScope);
                model.HideZeroTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.HideZeroTax, storeScope);
                model.HideTaxInOrderSummary_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.HideTaxInOrderSummary, storeScope);
                model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeScope);
                model.TaxBasedOn_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.TaxBasedOn, storeScope);
                model.DefaultTaxAddress_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DefaultTaxAddressId, storeScope);
                model.ShippingIsTaxable_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingIsTaxable, storeScope);
                model.ShippingPriceIncludesTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingPriceIncludesTax, storeScope);
                model.ShippingTaxClassId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingTaxClassId, storeScope);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeScope);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeScope);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeScope);
                model.EuVatEnabled_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatEnabled, storeScope);
                model.EuVatShopCountryId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatShopCountryId, storeScope);
                model.EuVatAllowVatExemption_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatAllowVatExemption, storeScope);
                model.EuVatUseWebService_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatUseWebService, storeScope);
                model.EuVatAssumeValid_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatAssumeValid, storeScope);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeScope);
            }

            model.TaxBasedOnValues = taxSettings.TaxBasedOn.ToSelectList();
            model.TaxDisplayTypeValues = taxSettings.TaxDisplayType.ToSelectList();

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.ShippingTaxCategories.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.ShippingTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == taxSettings.ShippingTaxClassId });
            model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == taxSettings.PaymentMethodAdditionalFeeTaxClassId });

            //EU VAT countries
            model.EuVatShopCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.EuVatShopCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id == taxSettings.EuVatShopCountryId });

            //default tax address
            var defaultAddress = taxSettings.DefaultTaxAddressId > 0
                                     ? _addressService.GetAddressById(taxSettings.DefaultTaxAddressId)
                                     : null;
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel();
            else
                model.DefaultTaxAddress = new AddressModel();

            model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (defaultAddress != null && c.Id == defaultAddress.CountryId) });

            var states = defaultAddress != null && defaultAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(defaultAddress.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == defaultAddress.StateProvinceId) });
            }
            else
                model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
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


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var taxSettings = _settingService.LoadSetting<TaxSettings>(storeScope);
            taxSettings = model.ToEntity(taxSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PricesIncludeTax, model.PricesIncludeTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, model.AllowCustomersToSelectTaxDisplayType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.TaxDisplayType, model.TaxDisplayType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.DisplayTaxSuffix, model.DisplayTaxSuffix_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.DisplayTaxRates, model.DisplayTaxRates_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.HideZeroTax, model.HideZeroTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.HideTaxInOrderSummary, model.HideTaxInOrderSummary_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.TaxBasedOn, model.TaxBasedOn_OverrideForStore, storeScope, false);


            if (model.DefaultTaxAddress_OverrideForStore || storeScope == 0)
            {
                //update address
                var addressId = _settingService.SettingExists(taxSettings, x => x.DefaultTaxAddressId, storeScope) ?
                    taxSettings.DefaultTaxAddressId : 0;
                var originAddress = _addressService.GetAddressById(addressId) ??
                    new Address
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                //update ID manually (in case we're in multi-store configuration mode it'll be set to the shared one)
                model.DefaultTaxAddress.Id = addressId;
                originAddress = model.DefaultTaxAddress.ToEntity(originAddress);
                if (originAddress.Id > 0)
                    _addressService.UpdateAddress(originAddress);
                else
                    _addressService.InsertAddress(originAddress);
                taxSettings.DefaultTaxAddressId = originAddress.Id;

                _settingService.SaveSetting(taxSettings, x => x.DefaultTaxAddressId, storeScope, false);
            }
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.DefaultTaxAddressId, storeScope);

            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingIsTaxable, model.ShippingIsTaxable_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingPriceIncludesTax, model.ShippingPriceIncludesTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingTaxClassId, model.ShippingTaxClassId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatEnabled, model.EuVatEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatShopCountryId, model.EuVatShopCountryId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatAllowVatExemption, model.EuVatAllowVatExemption_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatUseWebService, model.EuVatUseWebService_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatAssumeValid, model.EuVatAssumeValid_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Tax");
        }




        public ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            var model = catalogSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllowViewUnpublishedProductPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeScope);
                model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeScope);
                model.ShowProductSku_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductSku, storeScope);
                model.ShowManufacturerPartNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowManufacturerPartNumber, storeScope);
                model.ShowGtin_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowGtin, storeScope);
                model.ShowFreeShippingNotification_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowFreeShippingNotification, storeScope);
                model.AllowProductSorting_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowProductSorting, storeScope);
                model.AllowProductViewModeChanging_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowProductViewModeChanging, storeScope);
                model.ShowProductsFromSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductsFromSubcategories, storeScope);
                model.ShowCategoryProductNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumber, storeScope);
                model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeScope);
                model.CategoryBreadcrumbEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeScope);
                model.ShowShareButton_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowShareButton, storeScope);
                model.PageShareCode_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PageShareCode, storeScope);
                model.ProductReviewsMustBeApproved_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductReviewsMustBeApproved, storeScope);
                model.AllowAnonymousUsersToReviewProduct_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeScope);
                model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeScope);
                model.EmailAFriendEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.EmailAFriendEnabled, storeScope);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeScope);
                model.RecentlyViewedProductsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsNumber, storeScope);
                model.RecentlyViewedProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeScope);
                model.NewProductsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewProductsNumber, storeScope);
                model.NewProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewProductsEnabled, storeScope);
                model.CompareProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CompareProductsEnabled, storeScope);
                model.ShowBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowBestsellersOnHomepage, storeScope);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeScope);
                model.SearchPageProductsPerPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageProductsPerPage, storeScope);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeScope);
                model.SearchPagePageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPagePageSizeOptions, storeScope);
                model.ProductSearchAutoCompleteEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeScope);
                model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeScope);
                model.ShowProductImagesInSearchAutoComplete_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeScope);
                model.ProductSearchTermMinimumLength_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchTermMinimumLength, storeScope);
                model.ProductsAlsoPurchasedEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeScope);
                model.ProductsAlsoPurchasedNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeScope);
                model.NumberOfProductTags_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfProductTags, storeScope);
                model.ProductsByTagPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSize, storeScope);
                model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeScope);
                model.ProductsByTagPageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeScope);
                model.IncludeShortDescriptionInCompareProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeScope);
                model.IncludeFullDescriptionInCompareProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeScope);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeScope);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeScope);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeScope);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeScope);
                model.ShowProductReviewsPerStore_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsPerStore, storeScope);
                model.ShowProductReviewsOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeScope);
                model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x=> x.ProductReviewsPageSizeOnAccountPage, storeScope);
                model.ExportImportProductAttributes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ExportImportProductAttributes, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            catalogSettings = model.ToEntity(catalogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowViewUnpublishedProductPage, model.AllowViewUnpublishedProductPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductSku, model.ShowProductSku_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowManufacturerPartNumber, model.ShowManufacturerPartNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowGtin, model.ShowGtin_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowFreeShippingNotification, model.ShowFreeShippingNotification_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowProductSorting, model.AllowProductSorting_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowProductViewModeChanging, model.AllowProductViewModeChanging_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductsFromSubcategories, model.ShowProductsFromSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryProductNumber, model.ShowCategoryProductNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CategoryBreadcrumbEnabled, model.CategoryBreadcrumbEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowShareButton, model.ShowShareButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.PageShareCode, model.PageShareCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductReviewsMustBeApproved, model.ProductReviewsMustBeApproved_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, model.AllowAnonymousUsersToReviewProduct_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.EmailAFriendEnabled, model.EmailAFriendEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedProductsNumber, model.RecentlyViewedProductsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedProductsEnabled, model.RecentlyViewedProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewProductsNumber, model.NewProductsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewProductsEnabled, model.NewProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CompareProductsEnabled, model.CompareProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowBestsellersOnHomepage, model.ShowBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfBestsellersOnHomepage, model.NumberOfBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageProductsPerPage, model.SearchPageProductsPerPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPagePageSizeOptions, model.SearchPagePageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, model.ProductSearchAutoCompleteEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, model.ShowProductImagesInSearchAutoComplete_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchTermMinimumLength, model.ProductSearchTermMinimumLength_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, model.ProductsAlsoPurchasedEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsAlsoPurchasedNumber, model.ProductsAlsoPurchasedNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfProductTags, model.NumberOfProductTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagPageSize, model.ProductsByTagPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagPageSizeOptions, model.ProductsByTagPageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, model.IncludeShortDescriptionInCompareProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, model.IncludeFullDescriptionInCompareProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, model.ManufacturersBlockItemsToDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoFooter, model.DisplayTaxShippingInfoFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, model.DisplayTaxShippingInfoProductBoxes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, model.DisplayTaxShippingInfoWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductReviewsPerStore, model.ShowProductReviewsPerStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, model.ShowProductReviewsOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, model.ProductReviewsPageSizeOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ExportImportProductAttributes, model.ExportImportProductAttributes_OverrideForStore, storeScope, false);
            //now settings not overridable per store
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreDiscounts, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreFeaturedProducts, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreAcl, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreStoreLimitations, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.CacheProductPrices, 0, false);


            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("Catalog");
        }

        #region Sort options

        [HttpPost]
        public ActionResult SortOptionsList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var catalogSettings = _settingService.LoadSetting<CatalogSettings>();
            var model = new List<SortOptionModel>();
            foreach (int option in Enum.GetValues(typeof(ProductSortingEnum)))
            {
                int value;
                model.Add(new SortOptionModel()
                {
                    Id = option,
                    Name = ((ProductSortingEnum)option).GetLocalizedEnum(_localizationService, _workContext),
                    IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains(option),
                    DisplayOrder = catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(option, out value) ? value : option
                });
            }
            var gridModel = new DataSourceResult
            {
                Data = model.OrderBy(option => option.DisplayOrder),
                Total = model.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult SortOptionUpdate(SortOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);

            catalogSettings.ProductSortingEnumDisplayOrder[model.Id] = model.DisplayOrder;
            if (model.IsActive && catalogSettings.ProductSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ProductSortingEnumDisabled.Remove(model.Id);
            if (!model.IsActive && !catalogSettings.ProductSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ProductSortingEnumDisabled.Add(model.Id);


            _settingService.SaveSetting(catalogSettings);

            return new NullJsonResult();
        }

        #endregion

        public ActionResult RewardPoints()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var rewardPointsSettings = _settingService.LoadSetting<RewardPointsSettings>(storeScope);
            var model = rewardPointsSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.Enabled, storeScope);
                model.ExchangeRate_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.ExchangeRate, storeScope);
                model.MinimumRewardPointsToUse_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeScope);
                model.PointsForRegistration_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
                model.PointsForPurchases_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeScope) ||
                    _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Points, storeScope);
                model.PointsForPurchases_Awarded_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Awarded, storeScope);
                model.PointsForPurchases_Canceled_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Canceled, storeScope);
                model.DisplayHowMuchWillBeEarned_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeScope);
                model.PointsForRegistration_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
                model.PageSize_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PageSize, storeScope);
            }
            var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope); 
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            return View(model);
        }
        [HttpPost]
        public ActionResult RewardPoints(RewardPointsSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var rewardPointsSettings = _settingService.LoadSetting<RewardPointsSettings>(storeScope);
                rewardPointsSettings = model.ToEntity(rewardPointsSettings);

                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.ExchangeRate, model.ExchangeRate_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.MinimumRewardPointsToUse, model.MinimumRewardPointsToUse_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForRegistration, model.PointsForRegistration_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Amount, model.PointsForPurchases_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Points, model.PointsForPurchases_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Awarded, model.PointsForPurchases_Awarded_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Canceled, model.PointsForPurchases_Canceled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, model.DisplayHowMuchWillBeEarned_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PageSize, model.PageSize_OverrideForStore, storeScope, false);
                _settingService.SaveSetting(rewardPointsSettings, x => x.PointsAccumulatedForAllStores, 0, false);

                //now clear settings cache
                _settingService.ClearCache();

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            }
            else
            {
                //If we got this far, something failed, redisplay form
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        ErrorNotification(error.ErrorMessage);
            }
            return RedirectToAction("RewardPoints");
        }




        public ActionResult Order()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();



            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var orderSettings = _settingService.LoadSetting<OrderSettings>(storeScope);
            var model = orderSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.IsReOrderAllowed_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.IsReOrderAllowed, storeScope);
                model.MinOrderSubtotalAmount_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.MinOrderSubtotalAmount, storeScope);
                model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeScope);
                model.MinOrderTotalAmount_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.MinOrderTotalAmount, storeScope);
                model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeScope);
                model.AnonymousCheckoutAllowed_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.AnonymousCheckoutAllowed, storeScope);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeScope);
                model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeScope);
                model.OnePageCheckoutEnabled_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.OnePageCheckoutEnabled, storeScope);
                model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeScope);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeScope);
                model.DisableOrderCompletedPage_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.DisableOrderCompletedPage, storeScope);
                model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeScope);
                model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeScope);
                model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeScope);
                model.ReturnRequestsEnabled_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.ReturnRequestsEnabled, storeScope);
                model.ReturnRequestNumberMask_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.ReturnRequestNumberMask, storeScope);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeScope);
            }

            var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //gift card activation/deactivation
            model.GiftCards_Activated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Activated_OrderStatuses.Insert(0, new SelectListItem { Text = "---", Value = "0" });
            model.GiftCards_Deactivated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Deactivated_OrderStatuses.Insert(0, new SelectListItem { Text = "---", Value = "0" });
            
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
                //load settings for a chosen store scope
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var orderSettings = _settingService.LoadSetting<OrderSettings>(storeScope);
                orderSettings = model.ToEntity(orderSettings);

                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.IsReOrderAllowed, model.IsReOrderAllowed_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.MinOrderSubtotalAmount, model.MinOrderSubtotalAmount_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, model.MinOrderSubtotalAmountIncludingTax_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.MinOrderTotalAmount, model.MinOrderTotalAmount_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.AnonymousCheckoutAllowed, model.AnonymousCheckoutAllowed_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, model.TermsOfServiceOnShoppingCartPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, model.TermsOfServiceOnOrderConfirmPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.OnePageCheckoutEnabled, model.OnePageCheckoutEnabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.DisableBillingAddressCheckoutStep, model.DisableBillingAddressCheckoutStep_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.DisableOrderCompletedPage, model.DisableOrderCompletedPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.ReturnRequestsEnabled, model.ReturnRequestsEnabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, model.NumberOfDaysReturnRequestAvailable_OverrideForStore, storeScope, false);
                _settingService.SaveSetting(orderSettings, x => x.GiftCards_Activated_OrderStatusId, 0, false);
                _settingService.SaveSetting(orderSettings, x => x.GiftCards_Deactivated_OrderStatusId, 0, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.ReturnRequestsEnabled, model.ReturnRequestsEnabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(orderSettings, x => x.ReturnRequestNumberMask, model.ReturnRequestNumberMask_OverrideForStore, storeScope, false);
               
                //now clear settings cache
                _settingService.ClearCache();
                
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
                //If we got this far, something failed, redisplay form
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        ErrorNotification(error.ErrorMessage);
            }

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("Order");
        }

        #region Return request reasons

        public ActionResult ReturnRequestReasonList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the order settings page

            //select "return request" tab
            SaveSelectedTabName("tab-returnrequest");
            return RedirectToAction("Order", "Setting");
        }
        [HttpPost]
        public ActionResult ReturnRequestReasonList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var reasons = _returnRequestService.GetAllReturnRequestReasons();
            var gridModel = new DataSourceResult
            {
                Data = reasons.Select(x => x.ToModel()),
                Total = reasons.Count
            };
            return Json(gridModel);
        }
        //create
        public ActionResult ReturnRequestReasonCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new ReturnRequestReasonModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var rrr = model.ToEntity();
                _returnRequestService.InsertReturnRequestReason(rrr);
                //locales
                UpdateLocales(rrr, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Added"));
                return continueEditing ? RedirectToAction("ReturnRequestReasonEdit", new { id = rrr.Id }) : RedirectToAction("ReturnRequestReasonList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public ActionResult ReturnRequestReasonEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(id);
            if (rrr == null)
                //No reason found with the specified id
                return RedirectToAction("ReturnRequestReasonList");

            var model = rrr.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = rrr.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(model.Id);
            if (rrr == null)
                //No reason found with the specified id
                return RedirectToAction("ReturnRequestReasonList");

            if (ModelState.IsValid)
            {
                rrr = model.ToEntity(rrr);
                _returnRequestService.UpdateReturnRequestReason(rrr);
                //locales
                UpdateLocales(rrr, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ReturnRequestReasonEdit", new { id = rrr.Id });
                }
                return RedirectToAction("ReturnRequestReasonList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //delete
        [HttpPost]
        public ActionResult ReturnRequestReasonDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(id);
            _returnRequestService.DeleteReturnRequestReason(rrr);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted"));
            return RedirectToAction("ReturnRequestReasonList");
        }

        #endregion

        #region Return request actions

        public ActionResult ReturnRequestActionList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the order settings page

            //select "return request" tab
            SaveSelectedTabName("tab-returnrequest");
            return RedirectToAction("Order", "Setting");
        }
        [HttpPost]
        public ActionResult ReturnRequestActionList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var actions = _returnRequestService.GetAllReturnRequestActions();
            var gridModel = new DataSourceResult
            {
                Data = actions.Select(x => x.ToModel()),
                Total = actions.Count
            };
            return Json(gridModel);
        }
        //create
        public ActionResult ReturnRequestActionCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new ReturnRequestActionModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var rra = model.ToEntity();
                _returnRequestService.InsertReturnRequestAction(rra);
                //locales
                UpdateLocales(rra, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Added"));
                return continueEditing ? RedirectToAction("ReturnRequestActionEdit", new { id = rra.Id }) : RedirectToAction("ReturnRequestActionList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public ActionResult ReturnRequestActionEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(id);
            if (rra == null)
                //No action found with the specified id
                return RedirectToAction("ReturnRequestActionList");

            var model = rra.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = rra.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(model.Id);
            if (rra == null)
                //No action found with the specified id
                return RedirectToAction("ReturnRequestActionList");

            if (ModelState.IsValid)
            {
                rra = model.ToEntity(rra);
                _returnRequestService.UpdateReturnRequestAction(rra);
                //locales
                UpdateLocales(rra, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ReturnRequestActionEdit", new { id = rra.Id });
                }
                return RedirectToAction("ReturnRequestActionList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //delete
        [HttpPost]
        public ActionResult ReturnRequestActionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(id);
            _returnRequestService.DeleteReturnRequestAction(rra);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted"));
            return RedirectToAction("ReturnRequestActionList");
        }

        #endregion





        public ActionResult ShoppingCart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shoppingCartSettings = _settingService.LoadSetting<ShoppingCartSettings>(storeScope);
            var model = shoppingCartSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DisplayCartAfterAddingProduct_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeScope);
                model.DisplayWishlistAfterAddingProduct_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeScope);
                model.MaximumShoppingCartItems_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeScope);
                model.MaximumWishlistItems_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MaximumWishlistItems, storeScope);
                model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeScope);
                model.MoveItemsFromWishlistToCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeScope);
                model.ShowProductImagesOnShoppingCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeScope);
                model.ShowProductImagesOnWishList_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeScope);
                model.ShowDiscountBox_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowDiscountBox, storeScope);
                model.ShowGiftCardBox_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowGiftCardBox, storeScope);
                model.CrossSellsNumber_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.CrossSellsNumber, storeScope);
                model.EmailWishlistEnabled_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.EmailWishlistEnabled, storeScope);
                model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeScope);
                model.MiniShoppingCartEnabled_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeScope);
                model.ShowProductImagesInMiniShoppingCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeScope);
                model.MiniShoppingCartProductNumber_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeScope);
                model.AllowCartItemEditing_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowCartItemEditing, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ShoppingCart(ShoppingCartSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shoppingCartSettings = _settingService.LoadSetting<ShoppingCartSettings>(storeScope);
            shoppingCartSettings = model.ToEntity(shoppingCartSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, model.DisplayCartAfterAddingProduct_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, model.DisplayWishlistAfterAddingProduct_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MaximumShoppingCartItems, model.MaximumShoppingCartItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MaximumWishlistItems, model.MaximumWishlistItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, model.MoveItemsFromWishlistToCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, model.ShowProductImagesOnShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowProductImagesOnWishList, model.ShowProductImagesOnWishList_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowDiscountBox, model.ShowDiscountBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowGiftCardBox, model.ShowGiftCardBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.CrossSellsNumber, model.CrossSellsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.EmailWishlistEnabled, model.EmailWishlistEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MiniShoppingCartEnabled, model.MiniShoppingCartEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, model.ShowProductImagesInMiniShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, model.MiniShoppingCartProductNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowCartItemEditing, model.AllowCartItemEditing_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();
            

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("ShoppingCart");
        }




        public ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            var model = mediaSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AvatarPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, storeScope);
                model.ProductThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSize, storeScope);
                model.ProductDetailsPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductDetailsPictureSize, storeScope);
                model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeScope);
                model.AssociatedProductPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AssociatedProductPictureSize, storeScope);
                model.CategoryThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CategoryThumbPictureSize, storeScope);
                model.ManufacturerThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ManufacturerThumbPictureSize, storeScope);
                model.VendorThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.VendorThumbPictureSize, storeScope);
                model.CartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CartThumbPictureSize, storeScope);
                model.MiniCartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope);
                model.MaximumImageSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, storeScope);
                model.MultipleThumbDirectories_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MultipleThumbDirectories, storeScope);
                model.DefaultImageQuality_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, storeScope);
                model.ImportProductImagesUsingHash_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ImportProductImagesUsingHash, storeScope);
            }
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            mediaSettings = model.ToEntity(mediaSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSize, model.ProductThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductDetailsPictureSize, model.ProductDetailsPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AssociatedProductPictureSize, model.AssociatedProductPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ManufacturerThumbPictureSize, model.ManufacturerThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.VendorThumbPictureSize, model.VendorThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ImportProductImagesUsingHash, model.ImportProductImagesUsingHash_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();
            
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

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            //merge settings
            var model = new CustomerUserSettingsModel();
            model.CustomerSettings = customerSettings.ToModel();
            model.AddressSettings = addressSettings.ToModel();

            model.DateTimeSettings.AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            customerSettings = model.CustomerSettings.ToEntity(customerSettings);
            _settingService.SaveSetting(customerSettings);

            addressSettings = model.AddressSettings.ToEntity(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("CustomerUser");
        }






        public ActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            model.ActiveStoreScopeConfiguration = storeScope;
            //store information
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            model.StoreInformationSettings.StoreClosed = storeInformationSettings.StoreClosed;
            //themes
            model.StoreInformationSettings.DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme;
            model.StoreInformationSettings.AvailableStoreThemes = _themeProvider
                .GetThemeConfigurations()
                .Select(x => new GeneralCommonSettingsModel.StoreInformationSettingsModel.ThemeConfigurationModel
                {
                    ThemeTitle = x.ThemeTitle,
                    ThemeName = x.ThemeName,
                    PreviewImageUrl = x.PreviewImageUrl,
                    PreviewText = x.PreviewText,
                    SupportRtl = x.SupportRtl,
                    Selected = x.ThemeName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                })
                .ToList();
            model.StoreInformationSettings.AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme;
            model.StoreInformationSettings.LogoPictureId = storeInformationSettings.LogoPictureId;
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            model.StoreInformationSettings.FacebookLink = storeInformationSettings.FacebookLink;
            model.StoreInformationSettings.TwitterLink = storeInformationSettings.TwitterLink;
            model.StoreInformationSettings.YoutubeLink = storeInformationSettings.YoutubeLink;
            model.StoreInformationSettings.GooglePlusLink = storeInformationSettings.GooglePlusLink;
            //contact us
            model.StoreInformationSettings.SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm;
            model.StoreInformationSettings.UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm;
            //sitemap
            model.StoreInformationSettings.SitemapEnabled = commonSettings.SitemapEnabled;
            model.StoreInformationSettings.SitemapIncludeCategories = commonSettings.SitemapIncludeCategories;
            model.StoreInformationSettings.SitemapIncludeManufacturers = commonSettings.SitemapIncludeManufacturers;
            model.StoreInformationSettings.SitemapIncludeProducts = commonSettings.SitemapIncludeProducts;

            //override settings
            if (storeScope > 0)
            {
                model.StoreInformationSettings.StoreClosed_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeScope);
                model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);
                model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope);
                model.StoreInformationSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.LogoPictureId, storeScope);
                model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);
                model.StoreInformationSettings.FacebookLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeScope);
                model.StoreInformationSettings.TwitterLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeScope);
                model.StoreInformationSettings.YoutubeLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeScope);
                model.StoreInformationSettings.GooglePlusLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.GooglePlusLink, storeScope);
                model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
                model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
                model.StoreInformationSettings.SitemapEnabled_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapEnabled, storeScope);
                model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeCategories, storeScope);
                model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeManufacturers, storeScope);
                model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeProducts, storeScope);
            }

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList();
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.WwwRequirement = (int)seoSettings.WwwRequirement;
            model.SeoSettings.WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList();
            model.SeoSettings.EnableJsBundling = seoSettings.EnableJsBundling;
            model.SeoSettings.EnableCssBundling = seoSettings.EnableCssBundling;
            model.SeoSettings.TwitterMetaTags = seoSettings.TwitterMetaTags;
            model.SeoSettings.OpenGraphMetaTags = seoSettings.OpenGraphMetaTags;
            model.SeoSettings.CustomHeadTags = seoSettings.CustomHeadTags;
            //override settings
            if (storeScope > 0)
            {
                model.SeoSettings.PageTitleSeparator_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator, storeScope);
                model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment, storeScope);
                model.SeoSettings.DefaultTitle_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultTitle, storeScope);
                model.SeoSettings.DefaultMetaKeywords_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords, storeScope);
                model.SeoSettings.DefaultMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription, storeScope);
                model.SeoSettings.GenerateProductMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.GenerateProductMetaDescription, storeScope);
                model.SeoSettings.ConvertNonWesternChars_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars, storeScope);
                model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled, storeScope);
                model.SeoSettings.WwwRequirement_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.WwwRequirement, storeScope);
                model.SeoSettings.EnableJsBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableJsBundling, storeScope);
                model.SeoSettings.EnableCssBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableCssBundling, storeScope);
                model.SeoSettings.TwitterMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags, storeScope);
                model.SeoSettings.OpenGraphMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags, storeScope);
                model.SeoSettings.CustomHeadTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CustomHeadTags, storeScope);
            }
            
            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.ForceSslForAllPages = securitySettings.ForceSslForAllPages;
            model.SecuritySettings.EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea;
            model.SecuritySettings.EnableXsrfProtectionForPublicStore = securitySettings.EnableXsrfProtectionForPublicStore;
            model.SecuritySettings.HoneypotEnabled = securitySettings.HoneypotEnabled;
            model.SecuritySettings.CaptchaEnabled = captchaSettings.Enabled;
            model.SecuritySettings.CaptchaShowOnLoginPage = captchaSettings.ShowOnLoginPage;
            model.SecuritySettings.CaptchaShowOnRegistrationPage = captchaSettings.ShowOnRegistrationPage;
            model.SecuritySettings.CaptchaShowOnContactUsPage = captchaSettings.ShowOnContactUsPage;
            model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage = captchaSettings.ShowOnEmailWishlistToFriendPage;
            model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage = captchaSettings.ShowOnEmailProductToFriendPage;
            model.SecuritySettings.CaptchaShowOnBlogCommentPage = captchaSettings.ShowOnBlogCommentPage;
            model.SecuritySettings.CaptchaShowOnNewsCommentPage = captchaSettings.ShowOnNewsCommentPage;
            model.SecuritySettings.CaptchaShowOnProductReviewPage = captchaSettings.ShowOnProductReviewPage;
            model.SecuritySettings.CaptchaShowOnApplyVendorPage = captchaSettings.ShowOnApplyVendorPage;
            model.SecuritySettings.ReCaptchaVersion = captchaSettings.ReCaptchaVersion;
            model.SecuritySettings.AvailableReCaptchaVersions = ReCaptchaVersion.Version1.ToSelectList(false).ToList();
            model.SecuritySettings.ReCaptchaPublicKey = captchaSettings.ReCaptchaPublicKey;
            model.SecuritySettings.ReCaptchaPrivateKey = captchaSettings.ReCaptchaPrivateKey;

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            model.PdfSettings.LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled;
            model.PdfSettings.LogoPictureId = pdfSettings.LogoPictureId;
            model.PdfSettings.DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders;
            model.PdfSettings.InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1;
            model.PdfSettings.InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2;
            //override settings
            if (storeScope > 0)
            {
                model.PdfSettings.LetterPageSizeEnabled_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LetterPageSizeEnabled, storeScope);
                model.PdfSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LogoPictureId, storeScope);
                model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn1, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn2, storeScope);
            }

            //localization
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            model.LocalizationSettings.AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage;
            model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup;
            model.LocalizationSettings.LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup;

            //full-text support
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchMode = (int)commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();


            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //store information settings
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.GooglePlusLink = model.StoreInformationSettings.GooglePlusLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
            //sitemap
            commonSettings.SitemapEnabled = model.StoreInformationSettings.SitemapEnabled;
            commonSettings.SitemapIncludeCategories = model.StoreInformationSettings.SitemapIncludeCategories;
            commonSettings.SitemapIncludeManufacturers = model.StoreInformationSettings.SitemapIncludeManufacturers;
            commonSettings.SitemapIncludeProducts = model.StoreInformationSettings.SitemapIncludeProducts;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.AllowCustomerToSelectTheme, model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.GooglePlusLink, model.StoreInformationSettings.GooglePlusLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapEnabled, model.StoreInformationSettings.SitemapEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeCategories, model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeManufacturers, model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeProducts, model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();



            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.GenerateProductMetaDescription = model.SeoSettings.GenerateProductMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.EnableJsBundling = model.SeoSettings.EnableJsBundling;
            seoSettings.EnableCssBundling = model.SeoSettings.EnableCssBundling;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
            seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultTitle, model.SeoSettings.DefaultTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaKeywords, model.SeoSettings.DefaultMetaKeywords_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaDescription, model.SeoSettings.DefaultMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.GenerateProductMetaDescription, model.SeoSettings.GenerateProductMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableJsBundling, model.SeoSettings.EnableJsBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableCssBundling, model.SeoSettings.EnableCssBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();



            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtectionForAdminArea = model.SecuritySettings.EnableXsrfProtectionForAdminArea;
            securitySettings.EnableXsrfProtectionForPublicStore = model.SecuritySettings.EnableXsrfProtectionForPublicStore;
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            _settingService.SaveSetting(securitySettings);
            captchaSettings.Enabled = model.SecuritySettings.CaptchaEnabled;
            captchaSettings.ShowOnLoginPage = model.SecuritySettings.CaptchaShowOnLoginPage;
            captchaSettings.ShowOnRegistrationPage = model.SecuritySettings.CaptchaShowOnRegistrationPage;
            captchaSettings.ShowOnContactUsPage = model.SecuritySettings.CaptchaShowOnContactUsPage;
            captchaSettings.ShowOnEmailWishlistToFriendPage = model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage;
            captchaSettings.ShowOnEmailProductToFriendPage = model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage;
            captchaSettings.ShowOnBlogCommentPage = model.SecuritySettings.CaptchaShowOnBlogCommentPage;
            captchaSettings.ShowOnNewsCommentPage = model.SecuritySettings.CaptchaShowOnNewsCommentPage;
            captchaSettings.ShowOnProductReviewPage = model.SecuritySettings.CaptchaShowOnProductReviewPage;
            captchaSettings.ShowOnApplyVendorPage = model.SecuritySettings.CaptchaShowOnApplyVendorPage;
            captchaSettings.ReCaptchaVersion = model.SecuritySettings.ReCaptchaVersion;
            captchaSettings.ReCaptchaPublicKey = model.SecuritySettings.ReCaptchaPublicKey;
            captchaSettings.ReCaptchaPrivateKey = model.SecuritySettings.ReCaptchaPrivateKey;
            _settingService.SaveSetting(captchaSettings);
            if (captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification("Captcha is enabled but the appropriate keys are not entered");
            }

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
            pdfSettings.DisablePdfInvoicesForPendingOrders = model.PdfSettings.DisablePdfInvoicesForPendingOrders;
            pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
            pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LetterPageSizeEnabled, model.PdfSettings.LetterPageSizeEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LogoPictureId, model.PdfSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn1, model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn2, model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore, storeScope, false);
           
            //now clear settings cache
            _settingService.ClearCache();




            //localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                //clear cached values of routes
                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
            }
            localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
            localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
            _settingService.SaveSetting(localizationSettings);

            //full-text
            commonSettings.FullTextMode = (FulltextSearchMode)model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(commonSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public ActionResult ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update encrypted order info
                var orders = _orderService.SearchOrders();
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

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            try
            {
                if (! _fulltextService.IsFullTextSupported())
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                if (commonSettings.UseFullTextSearch)
                {
                    _fulltextService.DisableFullText();

                    commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                {
                    _fulltextService.EnableFullText();

                    commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            
            return RedirectToAction("GeneralCommon");
        }




        //all settings
        public ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)] 
        public ActionResult AllSettings(DataSourceRequest command, AllSettingsListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var query = _settingService.GetAllSettings().AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchSettingName))
                query = query.Where(s => s.Name.ToLowerInvariant().Contains(model.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(model.SearchSettingValue))
                query = query.Where(s => s.Value.ToLowerInvariant().Contains(model.SearchSettingValue.ToLowerInvariant()));

            var settings = query.ToList()
                .Select(x =>
                            {
                                string storeName;
                                if (x.StoreId == 0)
                                {
                                    storeName = _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                                }
                                else
                                {
                                    var store = _storeService.GetStoreById(x.StoreId);
                                    storeName = store != null ? store.Name : "Unknown";
                                }
                                var settingModel = new SettingModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Value = x.Value,
                                    Store = storeName,
                                    StoreId = x.StoreId
                                };
                                return settingModel;
                            })
                .AsQueryable();

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            var storeId = model.StoreId;

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeId)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            var storeId = model.StoreId;
            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }
        [HttpPost]
        public ActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }

        #endregion
    }
}
