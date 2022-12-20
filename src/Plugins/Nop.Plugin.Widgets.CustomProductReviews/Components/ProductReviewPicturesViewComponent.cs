using System;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.CustomProductReviews.Components
{
    [ViewComponent(Name = "ProductReviewPictures")]
    public class ProductReviewPictures : NopViewComponent
    {

        #region Fields

        //private readonly CustomProductReviewsService _customProductReviewsServiceService;
        private readonly CustomProductReviewsSettings _customProductReviewsSettings;

        #endregion

        #region Ctor

        public ProductReviewPictures(CustomProductReviewsSettings customProductReviewsSettings)
        {
            //_accessiBeService = accessiBeService;
            _customProductReviewsSettings = customProductReviewsSettings;
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
            //var model = new ProductReviewsModel();
            //var productDetailModel = new ProductDetailsModel();
            //if (additionalData.GetType() == model.GetType())
            //{
            //    model = (ProductReviewsModel)additionalData;
            //}
            //else
            //{
            //    productDetailModel = (ProductDetailsModel)additionalData;
            //    int productId = productDetailModel.Id;
            //    var product = await _productService.GetProductByIdAsync(productId);
            //    model = await _productModelFactory.PrepareProductReviewsModelAsync(new ProductReviewsModel(), product);
            //}

            //return View("~/Plugins/Widgets.CustomProductReviews/Views/ProductReviewComponent.cshtml", model);


            return View("~/Plugins/Widgets.CustomProductReviews/Views/_ProductReviewPictures.cshtml",additionalData);

        }

        #endregion
    }
}
