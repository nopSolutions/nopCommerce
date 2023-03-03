using System;
using System.Linq;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Widgets.CustomCustomProductReviews.Models;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.CustomProductReviews.Controllers;
using Nop.Core.Domain.Orders;
using Nop.Core;
using System.Collections.Generic;
using LinqToDB.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widgets.CustomProductReviews.Components
{
    [ViewComponent(Name = "CustomProductReviews")]
    public class CustomProductReviewsViewComponent : NopViewComponent
    {

        #region Fields

        //private readonly CustomProductReviewsService _customProductReviewsServiceService;
        private readonly CustomProductReviewsSettings _customProductReviewsSettings;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IWidgetModelFactory _widgetModelFactory;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPluginService _pluginService;
        #endregion

        #region Ctor

        public CustomProductReviewsViewComponent(CustomProductReviewsSettings customProductReviewsSettings,IProductService productService, IProductModelFactory productModelFactory,IWidgetModelFactory widgetModelFactory, IWorkContext workContext, IOrderService orderService, ICustomerService customerService, CatalogSettings catalogSettings,ILocalizationService localizationService,IPluginService pluginService)
        {
        _customProductReviewsSettings = customProductReviewsSettings;                                                                                                                                                
            _productService = productService;                                                                                                                                                                             
            _productModelFactory = productModelFactory;
            _widgetModelFactory = widgetModelFactory;
            _workContext = workContext;
            _orderService = orderService;
            _customerService = customerService;
            _catalogSettings = catalogSettings;
            _localizationService = localizationService;
            _pluginService= pluginService;;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>

        protected virtual async Task ValidateProductReviewAvailabilityAsync(Product product)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError(string.Empty,
                    await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (!_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
                return;

            var hasCompletedOrders = product.ProductType == ProductType.SimpleProduct
                ? await HasCompletedOrdersAsync(product)
                : await (await _productService.GetAssociatedProductsAsync(product.Id)).AnyAwaitAsync(
                    HasCompletedOrdersAsync);

            if (!hasCompletedOrders)
                ModelState.AddModelError(string.Empty,
                    await _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
        }

        protected virtual async ValueTask<bool> HasCompletedOrdersAsync(Product product)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            return (await _orderService.SearchOrdersAsync(customerId: customer.Id,
                productId: product.Id,
                osIds: new List<int> { (int)OrderStatus.Complete },
                pageSize: 1)).Any();
        }
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (_customProductReviewsSettings.data.IsNullOrEmpty())
            {


                await _pluginService.UninstallPluginsAsync();
            await _pluginService.DeletePluginsAsync();
            return View("~/Plugins/Widgets.CustomProductReviews/Views/Denied.html");




            }
            else
            {
                var model = new ProductReviewsModel();
                var productDetailModel = new ProductDetailsModel();



                if (additionalData.GetType() == model.GetType())
                {
                    model = (ProductReviewsModel)additionalData;
                }
                else
                {
                    productDetailModel = (ProductDetailsModel)additionalData;
                    int productId = productDetailModel.Id;
                    var product = await _productService.GetProductByIdAsync(productId);


                    model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);

                    await ValidateProductReviewAvailabilityAsync(product);

                    //default value
                    model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

                    //default value for all additional review types
                    if (model.ReviewTypeList.Count > 0)
                        foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                        {
                            additionalProductReview.Rating = additionalProductReview.IsRequired
                                ? _catalogSettings.DefaultProductRatingValue
                                : 0;
                        }


                }


                return View("~/Plugins/Widgets.CustomProductReviews/Views/ProductReviewComponent.cshtml", model);
            }
        }

        #endregion
    }
}
