using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BackInStockSubscriptionController(CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ILocalizationService localizationService,
            IProductService productService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerSettings = customerSettings;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _localizationService = localizationService;
            _productService = productService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        public virtual IActionResult SubscribePopup(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var model = new BackInStockSubscribeModel
            {
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered(),
                MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService
                .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                .TotalCount
            };
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id) != null;
            }
            return PartialView(model);
        }

        [HttpPost]
        public virtual IActionResult SubscribePopupPOST(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Content(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);

                    return Json(new
                    {
                        result = "Unsubscribed"
                    });
                }

                //subscription does not exist
                //subscribe
                if (_backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    Customer = _workContext.CurrentCustomer,
                    Product = product,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _backInStockSubscriptionService.InsertSubscription(subscription);

                return Json(new
                {
                    result = "Subscribed"
                });
            }

            //subscription not possible
            return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual IActionResult CustomerSubscriptions(int? pageNumber)
        {
            if (_customerSettings.HideBackInStockSubscriptionsTab)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var pageIndex = 0;
            if (pageNumber > 0)
            {
                pageIndex = pageNumber.Value - 1;
            }
            var pageSize = 10;

            var customer = _workContext.CurrentCustomer;
            var list = _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(customer.Id,
                _storeContext.CurrentStore.Id, pageIndex, pageSize);

            var model = new CustomerBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = subscription.Product;

                if (product != null)
                {
                    var subscriptionModel = new CustomerBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name),
                        SeName = _urlRecordService.GetSeName(product),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerBackInStockSubscriptions",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { pageNumber = pageIndex }
            };

            return View(model);
        }

        [HttpPost, ActionName("CustomerSubscriptions")]
        public virtual IActionResult CustomerSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    if (int.TryParse(id, out int subscriptionId))
                    {
                        var subscription = _backInStockSubscriptionService.GetSubscriptionById(subscriptionId);
                        if (subscription != null && subscription.CustomerId == _workContext.CurrentCustomer.Id)
                        {
                            _backInStockSubscriptionService.DeleteSubscription(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerBackInStockSubscriptions");
        }

        #endregion
    }
}