using System;
using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Mvc.Rss;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory implementation
    /// </summary>
    public partial class HomeModelFactory : IHomeModelFactory
    {
        #region Constants

        /// <summary>
        /// nopCommerce news URL
        /// </summary>
        /// <remarks>
        /// {0} : nopCommerce version
        /// {1} : whether the store based is on the localhost
        /// {2} : whether advertisements are hidden
        /// {3} : store URL
        /// </remarks>
        private static string NopCommerceNewsUrl => "https://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}";

        #endregion

        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ISearchTermService _searchTermService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HomeModelFactory(AdminAreaSettings adminAreaSettings,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            IOrderService orderService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            ISearchTermService searchTermService,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            this._adminAreaSettings = adminAreaSettings;
            this._customerService = customerService;
            this._httpContextAccessor = httpContextAccessor;
            this._orderService = orderService;
            this._productService = productService;
            this._returnRequestService = returnRequestService;
            this._searchTermService = searchTermService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>Dashboard model</returns>
        public virtual DashboardModel PrepareDashboardModel(DashboardModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare nested search model
            PreparePopularSearchTermSearchModel(model.PopularSearchTermSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare popular search term search model
        /// </summary>
        /// <param name="model">Popular search term search model</param>
        /// <returns>Popular search term search model</returns>
        public virtual PopularSearchTermSearchModel PreparePopularSearchTermSearchModel(PopularSearchTermSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model;
        }

        /// <summary>
        /// Prepare paged popular search term list model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term list model</returns>
        public virtual PopularSearchTermListModel PreparePopularSearchTermListModel(PopularSearchTermSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get popular search terms
            var searchTermRecordLines = _searchTermService.GetStats(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new PopularSearchTermListModel
            {
                //fill in model values from the entity
                Data = searchTermRecordLines.Select(searchTerm => new PopularSearchTermModel
                {
                    Keyword = searchTerm.Keyword,
                    Count = searchTerm.Count
                }),
                Total = searchTermRecordLines.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare nopCommerce news model
        /// </summary>
        /// <returns>nopCommerce news model</returns>
        public virtual NopCommerceNewsModel PrepareNopCommerceNewsModel()
        {
            var model = new NopCommerceNewsModel
            {
                HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
            };

            var rssData = _cacheManager.Get(ModelCacheEventConsumer.OFFICIAL_NEWS_MODEL_KEY, () =>
            {
                //compose nopCommerce news RSS feed URL
                var nopCommerceNewsUrl = string.Format(NopCommerceNewsUrl,
                    NopVersion.CurrentVersion,
                    _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
                    _adminAreaSettings.HideAdvertisementsOnAdminArea,
                    _webHelper.GetStoreLocation()).ToLowerInvariant();

                //create request
                var request = WebRequest.Create(nopCommerceNewsUrl);

                //specify request timeout
                request.Timeout = 5000;

                //try to get nopCommerce news RSS feed
                using (var response = request.GetResponse())
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                        return RssFeed.Load(reader);
            });

            for (var i = 0; i < rssData.Items.Count; i++)
            {
                var item = rssData.Items.ElementAt(i);
                var newsItem = new NopCommerceNewsDetailsModel
                {
                    Title = item.TitleText,
                    Summary = item.ContentText,
                    Url = item.Url.OriginalString,
                    PublishDate = item.PublishDate
                };
                model.Items.Add(newsItem);

                //has new items?
                if (i == 0)
                {
                    var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                    if (_adminAreaSettings.LastNewsTitleAdminArea != newsItem.Title)
                    {
                        _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                        _settingService.SaveSetting(_adminAreaSettings);

                        if (!firstRequest)
                        {
                            //new item
                            model.HasNewItems = true;
                        }
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare common statistics model
        /// </summary>
        /// <returns>Common statistics model</returns>
        public virtual CommonStatisticsModel PrepareCommonStatisticsModel()
        {
            var model = new CommonStatisticsModel
            {
                NumberOfOrders = _orderService.SearchOrders(pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount
            };

            var customerRoleIds = new[] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id };
            model.NumberOfCustomers = _customerService.GetAllCustomers(customerRoleIds: customerRoleIds,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount;

            var returnRequestStatus = ReturnRequestStatus.Pending;
            model.NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(rs: returnRequestStatus,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount;

            model.NumberOfLowStockProducts =
                _productService.GetLowStockProducts(getOnlyTotalCount: true).TotalCount +
                _productService.GetLowStockProductCombinations(getOnlyTotalCount: true).TotalCount;

            return model;
        }

        #endregion
    }
}