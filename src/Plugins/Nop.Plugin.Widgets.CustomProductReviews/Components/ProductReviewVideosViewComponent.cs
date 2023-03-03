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
using Nop.Plugin.Widgets.CustomProductReviews.Domains;

namespace Nop.Plugin.Widgets.CustomProductReviews.Components
{
    [ViewComponent(Name = "ProductReviewVideos")]
    public class ProductReviewVideos : NopViewComponent
    {

        #region Fields

        //private readonly CustomProductReviewsService _customProductReviewsServiceService;
        private readonly CustomProductReviewsSettings _customProductReviewsSettings;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IVideoService _videoService;
        private readonly ICustomProductReviewMappingService _customProductReviewMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;



        #endregion

        #region Ctor

        public ProductReviewVideos(CustomProductReviewsSettings customProductReviewsSettings, IProductService productService, IProductModelFactory productModelFactory,IVideoService videoService, ICustomProductReviewMappingService customProductReviewMappingService, ILocalizationService localizationService, MediaSettings mediaSettings)
        {
            //_accessiBeService = accessiBeService;
            _customProductReviewsSettings = customProductReviewsSettings;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _videoService = videoService;
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

            
            var model = new ProductReviewModel();
            if (additionalData.GetType() == model.GetType())
            {
                model = (ProductReviewModel)additionalData;
            }
            List<Video> reviewVidList = new List<Video>();
            List<VideoModel> videoModelList = new List<VideoModel>();
            var reviewMappings= await _customProductReviewMappingService.GetCustomProductReviewMappingByProductReviewIdAsync(model.Id);
           if (reviewMappings == null)
           {
               return View("~/Plugins/Widgets.CustomProductReviews/Views/_ProductReviewVideos.cshtml", videoModelList);

           }
           else
           {
                //default picture size
                //var defaultPictureSize = _mediaSettings.ProductDetailsPictureSize;
                var defaultPictureSize = 0;
                foreach (var mapping in reviewMappings)
               {
                    if (mapping.VideoId != null)
                   {
                       var vidId = mapping.VideoId.Value;

                        var vid = await _videoService.GetVideoByIdAsync(vidId);
                       reviewVidList.Add(vid);

                   }
                }

                string fullSizeImageUrl, imageUrl;

                for (var i = 0; i < reviewVidList.Count; i++)
                {
                    var video = reviewVidList[i];

                    (imageUrl, video) = await _videoService.GetVideoUrlAsync(video, defaultPictureSize, false);
                    (fullSizeImageUrl, video) = await _videoService.GetVideoUrlAsync(video, defaultPictureSize, false);

                    var videoModel = new VideoModel()
                    {
                        ImageUrl = imageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), model.Title),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), model.Title),
                    };
                    //"title" attribute
                    videoModel.Title = !string.IsNullOrEmpty(video.TitleAttribute) ?
                    video.TitleAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), model.Title);
                    //"alt" attribute
                    videoModel.AlternateText = !string.IsNullOrEmpty(video.AltAttribute) ?
                    video.AltAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), model.Title);

                    videoModelList.Add(videoModel);
                }
            }
            
            
            
           


            return View("~/Plugins/Widgets.CustomProductReviews/Views/_ProductReviewVideos.cshtml", videoModelList);

        }

        #endregion
    }
}
