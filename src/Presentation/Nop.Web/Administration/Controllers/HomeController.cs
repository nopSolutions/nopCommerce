using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Nop.Admin.Infrastructure.Cache;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext, 
            CommonSettings commonSettings, 
            ISettingService settingService,
            IPermissionService permissionService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IReturnRequestService returnRequestService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._storeContext = storeContext;
            this._commonSettings = commonSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._returnRequestService = returnRequestService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var model = new DashboardModel();
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult NopCommerceNews()
        {
            try
            {
                string feedUrl = string.Format("http://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}",
                    NopVersion.CurrentVersion, 
                    Request.Url.IsLoopback, 
                    _commonSettings.HideAdvertisementsOnAdminArea,
                    _storeContext.CurrentStore.Url)
                    .ToLowerInvariant();

                var rssData = _cacheManager.Get(ModelCacheEventConsumer.OFFICIAL_NEWS_MODEL_KEY, () =>
                {
                    //specify timeout (5 secs)
                    var request = WebRequest.Create(feedUrl);
                    request.Timeout = 5000;
                    using (var response = request.GetResponse())
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        return SyndicationFeed.Load(reader);
                    }
                });
 
                return PartialView(rssData);
            }
            catch (Exception)
            {
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult NopCommerceNewsHideAdv()
        {
            _commonSettings.HideAdvertisementsOnAdminArea = !_commonSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_commonSettings);
            return Content("Setting changed");
        }

        [ChildActionOnly]
        public ActionResult DayStatistics()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageOrders) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Content("");

            var model = new DayStatisticsModel();

            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            model.NumberOfOrders = _orderService.SearchOrders(vendorId: vendorId, pageIndex: 0, pageSize: 1).TotalCount;
            model.NumberOfCustomers = _customerService.GetAllCustomers(
                customerRoleIds: new [] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id }, 
                pageIndex: 0, pageSize: 1).TotalCount;
            model.NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(0, 0, 0, ReturnRequestStatus.Pending, 0, 1).TotalCount;

            IList<Product> products;
            IList<ProductAttributeCombination> combinations;
            _productService.GetLowStockProducts(vendorId, out products, out combinations);
            model.NumberOfLowStockProducts = products.Count;

            return PartialView(model);
        }

        #endregion
    }
}
