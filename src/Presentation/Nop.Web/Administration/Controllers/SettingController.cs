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
            IGenericAttributeService genericAttributeService)
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
        }

		#endregion 
        
        #region Methods
        
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
            
            if (model.VendorsBlockItemsToDisplay_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeScope);

            if (model.ShowVendorOnProductDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeScope);

            if (model.AllowCustomersToContactVendors_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(vendorSettings, x => x.AllowCustomersToContactVendors, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(vendorSettings, x => x.AllowCustomersToContactVendors, storeScope);

            if (model.AllowCustomersToApplyForVendorAccount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeScope);

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

            if (model.ActiveDiscussionsPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(forumSettings, x => x.ActiveDiscussionsPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(forumSettings, x => x.ActiveDiscussionsPageSize, storeScope);
            
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

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(storeScope);
            var model = shippingSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllowPickUpInStore_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.AllowPickUpInStore, storeScope);
                model.PickUpInStoreFee_OverrideForStore = _settingService.SettingExists(shippingSettings, x => x.PickUpInStoreFee, storeScope);
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
            if (states.Count > 0)
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
            if (model.AllowPickUpInStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.AllowPickUpInStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.AllowPickUpInStore, storeScope);

            if (model.PickUpInStoreFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.PickUpInStoreFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.PickUpInStoreFee, storeScope);

            if (model.UseWarehouseLocation_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.UseWarehouseLocation, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.UseWarehouseLocation, storeScope);

            if (model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeScope);

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

            if (model.DisplayShipmentEventsToStoreOwner_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeScope);

            if (model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeScope);

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
            if (states.Count > 0)
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
            if (model.PricesIncludeTax_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.PricesIncludeTax, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.PricesIncludeTax, storeScope);
            
            if (model.AllowCustomersToSelectTaxDisplayType_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeScope);
            
            if (model.TaxDisplayType_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.TaxDisplayType, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.TaxDisplayType, storeScope);
            
            if (model.DisplayTaxSuffix_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.DisplayTaxSuffix, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.DisplayTaxSuffix, storeScope);
            
            if (model.DisplayTaxRates_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.DisplayTaxRates, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.DisplayTaxRates, storeScope);
            
            if (model.HideZeroTax_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.HideZeroTax, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.HideZeroTax, storeScope);
            
            if (model.HideTaxInOrderSummary_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.HideTaxInOrderSummary, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.HideTaxInOrderSummary, storeScope);

            if (model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeScope);
            
            if (model.TaxBasedOn_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.TaxBasedOn, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.TaxBasedOn, storeScope);



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




            if (model.ShippingIsTaxable_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.ShippingIsTaxable, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.ShippingIsTaxable, storeScope);
            
            if (model.ShippingPriceIncludesTax_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.ShippingPriceIncludesTax, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.ShippingPriceIncludesTax, storeScope);
            
            if (model.ShippingTaxClassId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.ShippingTaxClassId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.ShippingTaxClassId, storeScope);
            
            if (model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeScope);
            
            if (model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeScope);
            
            if (model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeScope);
            
            if (model.EuVatEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatEnabled, storeScope);
            
            if (model.EuVatShopCountryId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatShopCountryId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatShopCountryId, storeScope);
            
            if (model.EuVatAllowVatExemption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatAllowVatExemption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatAllowVatExemption, storeScope);

            if (model.EuVatUseWebService_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatUseWebService, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatUseWebService, storeScope);

            if (model.EuVatAssumeValid_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatAssumeValid, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatAssumeValid, storeScope);

            if (model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeScope);

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
                model.IgnoreDiscounts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreDiscounts, storeScope);
                model.IgnoreFeaturedProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreFeaturedProducts, storeScope);
                model.IgnoreAcl_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreAcl, storeScope);
                model.IgnoreStoreLimitations_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreStoreLimitations, storeScope);
                model.CacheProductPrices_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CacheProductPrices, storeScope);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeScope);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeScope);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeScope);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeScope);
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
            if (model.AllowViewUnpublishedProductPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeScope);
            
            if (model.ShowProductSku_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowProductSku, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowProductSku, storeScope);
            
            if (model.ShowManufacturerPartNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowManufacturerPartNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowManufacturerPartNumber, storeScope);
            
            if (model.ShowGtin_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowGtin, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowGtin, storeScope);

            if (model.ShowFreeShippingNotification_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowFreeShippingNotification, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowFreeShippingNotification, storeScope);
            
            if (model.AllowProductSorting_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.AllowProductSorting, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.AllowProductSorting, storeScope);
            
            if (model.AllowProductViewModeChanging_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.AllowProductViewModeChanging, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.AllowProductViewModeChanging, storeScope);
            
            if (model.ShowProductsFromSubcategories_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowProductsFromSubcategories, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowProductsFromSubcategories, storeScope);
            
            if (model.ShowCategoryProductNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowCategoryProductNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowCategoryProductNumber, storeScope);
            
            if (model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeScope);
            
            if (model.CategoryBreadcrumbEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeScope);
            
            if (model.ShowShareButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowShareButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowShareButton, storeScope);

            if (model.PageShareCode_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.PageShareCode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.PageShareCode, storeScope);

            if (model.ProductReviewsMustBeApproved_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductReviewsMustBeApproved, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductReviewsMustBeApproved, storeScope);
            
            if (model.AllowAnonymousUsersToReviewProduct_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeScope);
            
            if (model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeScope);
            
            if (model.EmailAFriendEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.EmailAFriendEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.EmailAFriendEnabled, storeScope);
            
            if (model.AllowAnonymousUsersToEmailAFriend_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeScope);
            
            if (model.RecentlyViewedProductsNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.RecentlyViewedProductsNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.RecentlyViewedProductsNumber, storeScope);
            
            if (model.RecentlyViewedProductsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeScope);
            
            if (model.NewProductsNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.NewProductsNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.NewProductsNumber, storeScope);
            
            if (model.NewProductsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.NewProductsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.NewProductsEnabled, storeScope);
            
            if (model.CompareProductsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.CompareProductsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.CompareProductsEnabled, storeScope);
            
            if (model.ShowBestsellersOnHomepage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowBestsellersOnHomepage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowBestsellersOnHomepage, storeScope);
            
            if (model.NumberOfBestsellersOnHomepage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeScope);
            
            if (model.SearchPageProductsPerPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.SearchPageProductsPerPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.SearchPageProductsPerPage, storeScope);

            if (model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeScope);

            if (model.SearchPagePageSizeOptions_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.SearchPagePageSizeOptions, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.SearchPagePageSizeOptions, storeScope);
            
            if (model.ProductSearchAutoCompleteEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeScope);
            
            if (model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeScope);
            
            if (model.ShowProductImagesInSearchAutoComplete_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeScope);

            if (model.ProductSearchTermMinimumLength_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductSearchTermMinimumLength, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductSearchTermMinimumLength, storeScope);
            
            if (model.ProductsAlsoPurchasedEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeScope);
            
            if (model.ProductsAlsoPurchasedNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeScope);
            
            if (model.NumberOfProductTags_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.NumberOfProductTags, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.NumberOfProductTags, storeScope);
            
            if (model.ProductsByTagPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductsByTagPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductsByTagPageSize, storeScope);
            
            if (model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeScope);
            
            if (model.ProductsByTagPageSizeOptions_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeScope);
            
            if (model.IncludeShortDescriptionInCompareProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeScope);
            
            if (model.IncludeFullDescriptionInCompareProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeScope);
            
            if (model.IgnoreDiscounts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IgnoreDiscounts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IgnoreDiscounts, storeScope);
            
            if (model.IgnoreFeaturedProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IgnoreFeaturedProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IgnoreFeaturedProducts, storeScope);

            if (model.IgnoreAcl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IgnoreAcl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IgnoreAcl, storeScope);

            if (model.IgnoreStoreLimitations_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.IgnoreStoreLimitations, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.IgnoreStoreLimitations, storeScope);

            if (model.CacheProductPrices_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.CacheProductPrices, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.CacheProductPrices, storeScope);

            if (model.ManufacturersBlockItemsToDisplay_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeScope);

            if (model.DisplayTaxShippingInfoFooter_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope);

            if (model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeScope);

            if (model.DisplayTaxShippingInfoProductBoxes_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeScope);

            if (model.DisplayTaxShippingInfoShoppingCart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope);

            if (model.DisplayTaxShippingInfoWishlist_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope);

            if (model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("Catalog");
        }



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
                model.DisplayHowMuchWillBeEarned_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeScope); model.PointsForRegistration_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
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
                if (model.Enabled_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.Enabled, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.Enabled, storeScope);
                
                if (model.ExchangeRate_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.ExchangeRate, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.ExchangeRate, storeScope);
                
                if (model.MinimumRewardPointsToUse_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeScope);
                
                if (model.PointsForRegistration_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.PointsForRegistration, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
                
                if (model.PointsForPurchases_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeScope);

                if (model.PointsForPurchases_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.PointsForPurchases_Points, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.PointsForPurchases_Points, storeScope);
                
                if (model.PointsForPurchases_Awarded_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.PointsForPurchases_Awarded, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.PointsForPurchases_Awarded, storeScope);
                
                if (model.PointsForPurchases_Canceled_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.PointsForPurchases_Canceled, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.PointsForPurchases_Canceled, storeScope);
                
                if (model.DisplayHowMuchWillBeEarned_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeScope);

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
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = _settingService.SettingExists(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeScope);
            }

            var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //gift card activation/deactivation
            model.GiftCards_Activated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Activated_OrderStatuses.Insert(0, new SelectListItem { Text = "---", Value = "0" });
            model.GiftCards_Deactivated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.GiftCards_Deactivated_OrderStatuses.Insert(0, new SelectListItem { Text = "---", Value = "0" });


            //parse return request actions
            for (int i = 0; i < orderSettings.ReturnRequestActions.Count; i++)
            {
                model.ReturnRequestActionsParsed += orderSettings.ReturnRequestActions[i];
                if (i != orderSettings.ReturnRequestActions.Count - 1)
                    model.ReturnRequestActionsParsed += ",";
            }
            //parse return request reasons
            for (int i = 0; i < orderSettings.ReturnRequestReasons.Count; i++)
            {
                model.ReturnRequestReasonsParsed += orderSettings.ReturnRequestReasons[i];
                if (i != orderSettings.ReturnRequestReasons.Count - 1)
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
                //load settings for a chosen store scope
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var orderSettings = _settingService.LoadSetting<OrderSettings>(storeScope);
                orderSettings = model.ToEntity(orderSettings);

                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                if (model.IsReOrderAllowed_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.IsReOrderAllowed, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.IsReOrderAllowed, storeScope);
                
                if (model.MinOrderSubtotalAmount_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.MinOrderSubtotalAmount, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.MinOrderSubtotalAmount, storeScope);

                if (model.MinOrderSubtotalAmountIncludingTax_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeScope);

                if (model.MinOrderTotalAmount_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.MinOrderTotalAmount, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.MinOrderTotalAmount, storeScope);
                
                if (model.AnonymousCheckoutAllowed_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.AnonymousCheckoutAllowed, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.AnonymousCheckoutAllowed, storeScope);

                if (model.TermsOfServiceOnShoppingCartPage_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeScope);

                if (model.TermsOfServiceOnOrderConfirmPage_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeScope);
                
                if (model.OnePageCheckoutEnabled_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.OnePageCheckoutEnabled, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.OnePageCheckoutEnabled, storeScope);

                if (model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeScope);

                if (model.DisableBillingAddressCheckoutStep_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeScope);

                if (model.DisableOrderCompletedPage_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.DisableOrderCompletedPage, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.DisableOrderCompletedPage, storeScope);

                if (model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeScope);

                if (model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeScope);

                if (model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeScope);
                
                if (model.ReturnRequestsEnabled_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.ReturnRequestsEnabled, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.ReturnRequestsEnabled, storeScope);

                if (model.NumberOfDaysReturnRequestAvailable_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeScope);

                _settingService.SaveSetting(orderSettings, x => x.GiftCards_Activated_OrderStatusId, 0, false);
                _settingService.SaveSetting(orderSettings, x => x.GiftCards_Deactivated_OrderStatusId, 0, false);

                if (model.ReturnRequestsEnabled_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(orderSettings, x => x.ReturnRequestsEnabled, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(orderSettings, x => x.ReturnRequestsEnabled, storeScope);

                //parse return request actions
                orderSettings.ReturnRequestActions.Clear();
                if (model.ReturnRequestActionsParsed != null)
                {
                    foreach (var returnAction in model.ReturnRequestActionsParsed.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        orderSettings.ReturnRequestActions.Add(returnAction);
                }
                //note that we do not store this setting for a store (cannot be overridden)
                //pass 0 as "storeScope" parameter
                _settingService.SaveSetting(orderSettings, x => x.ReturnRequestActions, 0, false);
                //parse return request reasons
                orderSettings.ReturnRequestReasons.Clear();
                if (model.ReturnRequestReasonsParsed != null)
                {
                    foreach (var returnReason in model.ReturnRequestReasonsParsed.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        orderSettings.ReturnRequestReasons.Add(returnReason);
                }
                //note that we do not store this setting for a store (cannot be overridden)
                //pass 0 as "storeScope" parameter
                _settingService.SaveSetting(orderSettings, x => x.ReturnRequestReasons, 0, false);


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
            SaveSelectedTabIndex();

            return RedirectToAction("Order");
        }




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
            if (model.DisplayCartAfterAddingProduct_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeScope);
            
            if (model.DisplayWishlistAfterAddingProduct_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeScope);
            
            if (model.MaximumShoppingCartItems_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeScope);
            
            if (model.MaximumWishlistItems_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.MaximumWishlistItems, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.MaximumWishlistItems, storeScope);
            
            if (model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeScope);
            
            if (model.MoveItemsFromWishlistToCart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeScope);
            
            if (model.ShowProductImagesOnShoppingCart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeScope);
            
            if (model.ShowProductImagesOnWishList_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeScope);
            
            if (model.ShowDiscountBox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.ShowDiscountBox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.ShowDiscountBox, storeScope);
            
            if (model.ShowGiftCardBox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.ShowGiftCardBox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.ShowGiftCardBox, storeScope);
            
            if (model.CrossSellsNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.CrossSellsNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.CrossSellsNumber, storeScope);
            
            if (model.EmailWishlistEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.EmailWishlistEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.EmailWishlistEnabled, storeScope);
            
            if (model.AllowAnonymousUsersToEmailWishlist_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeScope);
            
            if (model.MiniShoppingCartEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeScope);
            
            if (model.ShowProductImagesInMiniShoppingCart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeScope);

            if (model.MiniShoppingCartProductNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeScope);

            if (model.AllowCartItemEditing_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(shoppingCartSettings, x => x.AllowCartItemEditing, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(shoppingCartSettings, x => x.AllowCartItemEditing, storeScope);

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
            if (model.AvatarPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.AvatarPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.AvatarPictureSize, storeScope);
            
            if (model.ProductThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.ProductThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.ProductThumbPictureSize, storeScope);
            
            if (model.ProductDetailsPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.ProductDetailsPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.ProductDetailsPictureSize, storeScope);
            
            if (model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeScope);

            if (model.AssociatedProductPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.AssociatedProductPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.AssociatedProductPictureSize, storeScope);
            
            if (model.CategoryThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.CategoryThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.CategoryThumbPictureSize, storeScope);
            
            if (model.ManufacturerThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.ManufacturerThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.ManufacturerThumbPictureSize, storeScope);

            if (model.VendorThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.VendorThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.VendorThumbPictureSize, storeScope);
            
            if (model.CartThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.CartThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.CartThumbPictureSize, storeScope);
            
            if (model.MiniCartThumbPictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope);

            if (model.MaximumImageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.MaximumImageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.MaximumImageSize, storeScope);

            if (model.MultipleThumbDirectories_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(mediaSettings, x => x.MultipleThumbDirectories, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(mediaSettings, x => x.MultipleThumbDirectories, storeScope);

            if (model.DefaultImageQuality_OverrideForStore || storeScope == 0)
              _settingService.SaveSetting(mediaSettings, x => x.DefaultImageQuality, storeScope, false);
            else if (storeScope > 0)
              _settingService.DeleteSetting(mediaSettings, x => x.DefaultImageQuality, storeScope);

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
            SaveSelectedTabIndex();

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
            model.StoreInformationSettings.StoreClosedAllowForAdmins = storeInformationSettings.StoreClosedAllowForAdmins;
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
            //override settings
            if (storeScope > 0)
            {
                model.StoreInformationSettings.StoreClosed_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeScope);
                model.StoreInformationSettings.StoreClosedAllowForAdmins_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosedAllowForAdmins, storeScope);
                model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);
                model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope);
                model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);
                model.StoreInformationSettings.FacebookLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeScope);
                model.StoreInformationSettings.TwitterLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeScope);
                model.StoreInformationSettings.YoutubeLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeScope);
                model.StoreInformationSettings.GooglePlusLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.GooglePlusLink, storeScope);
                model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
                model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
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
            storeInformationSettings.StoreClosedAllowForAdmins = model.StoreInformationSettings.StoreClosedAllowForAdmins;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
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

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            if (model.StoreInformationSettings.StoreClosed_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.StoreClosed, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.StoreClosed, storeScope);

            if (model.StoreInformationSettings.StoreClosedAllowForAdmins_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.StoreClosedAllowForAdmins, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.StoreClosedAllowForAdmins, storeScope);

            if (model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.DefaultStoreTheme, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);

            if (model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope);
            
            if (model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);

            if (model.StoreInformationSettings.FacebookLink_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.FacebookLink, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.FacebookLink, storeScope);

            if (model.StoreInformationSettings.TwitterLink_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.TwitterLink, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.TwitterLink, storeScope);

            if (model.StoreInformationSettings.YoutubeLink_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.YoutubeLink, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.YoutubeLink, storeScope);

            if (model.StoreInformationSettings.GooglePlusLink_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(storeInformationSettings, x => x.GooglePlusLink, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(storeInformationSettings, x => x.GooglePlusLink, storeScope);

            if (model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
            
            if (model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
            
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

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.SeoSettings.PageTitleSeparator_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.PageTitleSeparator, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.PageTitleSeparator, storeScope);
            
            if (model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.PageTitleSeoAdjustment, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.PageTitleSeoAdjustment, storeScope);
            
            if (model.SeoSettings.DefaultTitle_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.DefaultTitle, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.DefaultTitle, storeScope);
            
            if (model.SeoSettings.DefaultMetaKeywords_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.DefaultMetaKeywords, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.DefaultMetaKeywords, storeScope);
            
            if (model.SeoSettings.DefaultMetaDescription_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.DefaultMetaDescription, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.DefaultMetaDescription, storeScope);

            if (model.SeoSettings.GenerateProductMetaDescription_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.GenerateProductMetaDescription, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.GenerateProductMetaDescription, storeScope);
            
            if (model.SeoSettings.ConvertNonWesternChars_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.ConvertNonWesternChars, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.ConvertNonWesternChars, storeScope);
            
            if (model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.CanonicalUrlsEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.CanonicalUrlsEnabled, storeScope);

            if (model.SeoSettings.WwwRequirement_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.WwwRequirement, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.WwwRequirement, storeScope);
            
            if (model.SeoSettings.EnableJsBundling_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.EnableJsBundling, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.EnableJsBundling, storeScope);

            if (model.SeoSettings.EnableCssBundling_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.EnableCssBundling, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.EnableCssBundling, storeScope);

            if (model.SeoSettings.TwitterMetaTags_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.TwitterMetaTags, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.TwitterMetaTags, storeScope);

            if (model.SeoSettings.OpenGraphMetaTags_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(seoSettings, x => x.OpenGraphMetaTags, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(seoSettings, x => x.OpenGraphMetaTags, storeScope);

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
            
            if (model.PdfSettings.LetterPageSizeEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(pdfSettings, x => x.LetterPageSizeEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(pdfSettings, x => x.LetterPageSizeEnabled, storeScope);

            if (model.PdfSettings.LogoPictureId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(pdfSettings, x => x.LogoPictureId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(pdfSettings, x => x.LogoPictureId, storeScope);

            if (model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeScope);

            if (model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(pdfSettings, x => x.InvoiceFooterTextColumn1, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(pdfSettings, x => x.InvoiceFooterTextColumn1, storeScope);

            if (model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(pdfSettings, x => x.InvoiceFooterTextColumn2, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(pdfSettings, x => x.InvoiceFooterTextColumn2, storeScope);

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

            //selected tab
            SaveSelectedTabIndex();

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

            //selected tab
            SaveSelectedTabIndex();

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

            //selected tab
            SaveSelectedTabIndex();

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
        public ActionResult AllSettings(DataSourceRequest command,
            Nop.Web.Framework.Kendoui.Filter filter = null, IEnumerable<Sort> sort = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService
                .GetAllSettings()
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
                .AsQueryable()
                .Filter(filter)
                .Sort(sort);

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
