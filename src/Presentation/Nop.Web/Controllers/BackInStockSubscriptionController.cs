using System;
using System.Web.Mvc;
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

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        
        #endregion

		#region Constructors

        public BackInStockSubscriptionController(IProductService productService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._catalogSettings = catalogSettings;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        public ActionResult SubscribePopup(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var model = new BackInStockSubscribeModel();
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered();
            model.MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions;
            model.CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService
                .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                .TotalCount;
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id) != null;
            }
            return View(model);
        }
        [HttpPost, ActionName("SubscribePopup")]
        public ActionResult SubscribePopupPOST(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Content(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);
                    return Content("Unsubscribed");
                }

                //subscription does not exist
                //subscribe
                if (_backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Content(string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions));
                }
                subscription = new BackInStockSubscription
                {
                    Customer = _workContext.CurrentCustomer,
                    Product = product,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _backInStockSubscriptionService.InsertSubscription(subscription);
                return Content("Subscribed");
            }

            //subscription not possible
            return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
        }


        // My account / Back in stock subscriptions
        public ActionResult CustomerSubscriptions(int? page)
        {
            if (_customerSettings.HideBackInStockSubscriptionsTab)
            {
                return RedirectToRoute("CustomerInfo");
            }

            int pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page.Value - 1;
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
                        ProductName = product.GetLocalized(x => x.Name),
                        SeName = product.GetSeName(),
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
                RouteActionName = "CustomerBackInStockSubscriptionsPaged",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { page = pageIndex }
            };

            return View(model);
        }
        [HttpPost, ActionName("CustomerSubscriptions")]
        public ActionResult CustomerSubscriptionsPOST(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    int subscriptionId;
                    if (Int32.TryParse(id, out subscriptionId))
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
