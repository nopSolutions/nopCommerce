using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Widgets.CustomProductReviews.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Services.Media;
using Picture = Nop.Core.Domain.Media.Picture;
using Nop.Web.Models.Media;
using Nop.Services.Localization;
using Nop.Core.Domain.Media;
using DocumentFormat.OpenXml.Bibliography;

namespace Nop.Plugin.Widgets.CustomProductReviews.Components
{
    [ViewComponent(Name = "ProductReviewPictures")]
    public class ProductReviewPictures : NopViewComponent
    {

        #region Fields

        //private readonly CustomProductReviewsService _customProductReviewsServiceService;
        private readonly CustomProductReviewsSettings _customProductReviewsSettings;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPictureService _pictureService;
        private readonly ICustomProductReviewMappingService _customProductReviewMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;



        #endregion

        #region Ctor

        public ProductReviewPictures(CustomProductReviewsSettings customProductReviewsSettings, IProductService productService, IProductModelFactory productModelFactory,IPictureService pictureService, ICustomProductReviewMappingService customProductReviewMappingService, ILocalizationService localizationService, MediaSettings mediaSettings)
        {
            //_accessiBeService = accessiBeService;
            _customProductReviewsSettings = customProductReviewsSettings;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _pictureService = pictureService;
            _customProductReviewMappingService = customProductReviewMappingService;
            _localizationService=localizationService;
            _mediaSettings = mediaSettings;
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

            //Todo:photo view sorunu çöz
            var model = new ProductReviewModel();
            if (additionalData.GetType() == model.GetType())
            {
                model = (ProductReviewModel)additionalData;
            }
            List<Picture> reviewPicList = new List<Picture>();
            List<PictureModel> pictureModelList = new List<PictureModel>();
            var reviewMappings= await _customProductReviewMappingService.GetCustomProductReviewMappingByProductReviewIdAsync(model.Id);
           if (reviewMappings == null)
           {
               return View("~/Plugins/Widgets.CustomProductReviews/Views/_ProductReviewPictures.cshtml", pictureModelList);

           }
           else
           {
               //default picture size
               var defaultPictureSize = _mediaSettings.ProductDetailsPictureSize;
                foreach (var mapping in reviewMappings)
               {
                   var picId = mapping.PictureId;
                   if (picId != null)
                   {
                       var pic = await _pictureService.GetPictureByIdAsync(picId.Value);
                       reviewPicList.Add(pic);
                       


                  
                      
                   }
                }

                string fullSizeImageUrl, imageUrl;

                for (var i = 0; i < reviewPicList.Count; i++)
                {
                    var picture = reviewPicList[i];

                    (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, true);
                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (thumbImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage);

                    var pictureModel = new PictureModel
                    {
                        ImageUrl = imageUrl,
                        ThumbImageUrl = thumbImageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                    picture.TitleAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                    picture.AltAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                    pictureModels.Add(pictureModel);
                }
            }
            
            
            
           


            return View("~/Plugins/Widgets.CustomProductReviews/Views/_ProductReviewPictures.cshtml", reviewPicList);

        }

        #endregion
    }
}
