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
        #endregion

        #region Ctor

        public CustomProductReviewsViewComponent(CustomProductReviewsSettings customProductReviewsSettings,IProductService productService, IProductModelFactory productModelFactory,IWidgetModelFactory widgetModelFactory)
        {
            //_accessiBeService = accessiBeService;
            _customProductReviewsSettings = customProductReviewsSettings;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _widgetModelFactory = widgetModelFactory;
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
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new ProductReviewsModel();
            var productDetailModel = new ProductDetailsModel();

         

            if (additionalData.GetType()== model.GetType())
            {
                model = (ProductReviewsModel)additionalData;
            }
            else
            {
                productDetailModel = (ProductDetailsModel)additionalData;
                int productId=productDetailModel.Id;
                var product = await _productService.GetProductByIdAsync(productId);
                model = await _productModelFactory.PrepareProductReviewsModelAsync(new ProductReviewsModel(), product);

            }
            var result = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, model);

            //return View(result);

            return View("~/Plugins/Widgets.CustomProductReviews/Views/ProductReviewComponent.cshtml", model);

        }

        #endregion
    }
}
