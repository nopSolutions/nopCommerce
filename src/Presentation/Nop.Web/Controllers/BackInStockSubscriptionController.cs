using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BackInStockSubscriptionController(CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IProductService productService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerSettings = customerSettings;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _customerService = customerService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productService = productService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SubscribePopup(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var model = new BackInStockSubscribeModel
            {
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                IsCurrentCustomerRegistered = await _customerService.IsRegisteredAsync(customer),
                MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = (await _backInStockSubscriptionService
                .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                .TotalCount
            };
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = await _backInStockSubscriptionService
                    .FindSubscriptionAsync(customer.Id, product.Id, store.Id) != null;
            }

            return PartialView(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscribePopupPOST(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                var store = await _storeContext.GetCurrentStoreAsync();
                var subscription = await _backInStockSubscriptionService
                    .FindSubscriptionAsync(customer.Id, product.Id, store.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Unsubscribed"));
                    return new OkResult();
                }

                //subscription does not exist
                //subscribe
                if ((await _backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(await _localizationService.GetResourceAsync("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    StoreId = store.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _backInStockSubscriptionService.InsertSubscriptionAsync(subscription);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Subscribed"));
                return new OkResult();
            }

            //subscription not possible
            return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual async Task<IActionResult> CustomerSubscriptions(int? pageNumber)
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

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var list = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
                store.Id, pageIndex, pageSize);

            var model = new CustomerBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = await _productService.GetProductByIdAsync(subscription.ProductId);

                if (product != null)
                {
                    var subscriptionModel = new CustomerBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(product),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerBackInStockSubscriptions",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { PageNumber = pageIndex }
            };

            return View(model);
        }

        [HttpPost, ActionName("CustomerSubscriptions")]
        public virtual async Task<IActionResult> CustomerSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    if (int.TryParse(id, out var subscriptionId))
                    {
                        var subscription = await _backInStockSubscriptionService.GetSubscriptionByIdAsync(subscriptionId);
                        var customer = await _workContext.GetCurrentCustomerAsync();
                        if (subscription != null && subscription.CustomerId == customer.Id)
                        {
                            await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerBackInStockSubscriptions");
        }

        #endregion
    }
}