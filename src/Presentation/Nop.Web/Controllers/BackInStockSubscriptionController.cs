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

        protected IBackInStockSubscriptionService BackInStockSubscriptionService { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IProductService ProductService { get; }
        protected IStoreContext StoreContext { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }

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
            CatalogSettings = catalogSettings;
            CustomerSettings = customerSettings;
            BackInStockSubscriptionService = backInStockSubscriptionService;
            CustomerService = customerService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            ProductService = productService;
            StoreContext = storeContext;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SubscribePopup(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var model = new BackInStockSubscribeModel
            {
                ProductId = product.Id,
                ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                IsCurrentCustomerRegistered = await CustomerService.IsRegisteredAsync(customer),
                MaximumBackInStockSubscriptions = CatalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = (await BackInStockSubscriptionService
                .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                .TotalCount
            };
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await ProductService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = await BackInStockSubscriptionService
                    .FindSubscriptionAsync(customer.Id, product.Id, store.Id) != null;
            }

            return PartialView(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscribePopupPOST(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Content(await LocalizationService.GetResourceAsync("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await ProductService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                var store = await StoreContext.GetCurrentStoreAsync();
                var subscription = await BackInStockSubscriptionService
                    .FindSubscriptionAsync(customer.Id, product.Id, store.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    await BackInStockSubscriptionService.DeleteSubscriptionAsync(subscription);

                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Unsubscribed"));
                    return new OkResult();
                }

                //subscription does not exist
                //subscribe
                if ((await BackInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                    .TotalCount >= CatalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(await LocalizationService.GetResourceAsync("BackInStockSubscriptions.MaxSubscriptions"), CatalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    StoreId = store.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await BackInStockSubscriptionService.InsertSubscriptionAsync(subscription);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Subscribed"));
                return new OkResult();
            }

            //subscription not possible
            return Content(await LocalizationService.GetResourceAsync("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual async Task<IActionResult> CustomerSubscriptions(int? pageNumber)
        {
            if (CustomerSettings.HideBackInStockSubscriptionsTab)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var pageIndex = 0;
            if (pageNumber > 0)
            {
                pageIndex = pageNumber.Value - 1;
            }
            var pageSize = 10;

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var list = await BackInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
                store.Id, pageIndex, pageSize);

            var model = new CustomerBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = await ProductService.GetProductByIdAsync(subscription.ProductId);

                if (product != null)
                {
                    var subscriptionModel = new CustomerBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                        SeName = await UrlRecordService.GetSeNameAsync(product),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel(LocalizationService)
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
                        var subscription = await BackInStockSubscriptionService.GetSubscriptionByIdAsync(subscriptionId);
                        var customer = await WorkContext.GetCurrentCustomerAsync();
                        if (subscription != null && subscription.CustomerId == customer.Id)
                        {
                            await BackInStockSubscriptionService.DeleteSubscriptionAsync(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerBackInStockSubscriptions");
        }

        #endregion
    }
}