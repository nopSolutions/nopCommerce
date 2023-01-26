using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Plugin.Widgets.CustomProductReviews.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using WebOptimizer;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Libwebp.Net;
using Libwebp.Net.utility;
using Libwebp.Standard;
using Microsoft.CodeAnalysis.Diagnostics;
using NReco.VideoConverter;
using Microsoft.AspNetCore.StaticFiles;
using Nito.Disposables;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Nop.Plugin.Widgets.CustomProductReviews.Controllers
{

    [AutoValidateAntiforgeryToken]
    public class CustomProductReviewsController : BasePluginController
    {

        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPictureService _pictureService;
        private readonly IVideoService _videoService;
        private readonly ICustomProductReviewMappingService _customProductReviewMappingService;
        private readonly INopFileProvider _fileProvider;

        
        


        #endregion

        #region Ctor

        public CustomProductReviewsController(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICompareProductsService compareProductsService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IProductAttributeParser productAttributeParser,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IReviewTypeService reviewTypeService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ShoppingCartSettings shoppingCartSettings,
            IPictureService pictureService,
            IVideoService videoService,
            INopFileProvider fileProvider,
            ShippingSettings shippingSettings,
            ICustomProductReviewMappingService customProductReviewMappingService

        )
        {
            _captchaSettings = captchaSettings;
            _pictureService = pictureService;
            _videoService = videoService;
            _fileProvider = fileProvider;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _compareProductsService = compareProductsService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _productAttributeParser = productAttributeParser;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _reviewTypeService = reviewTypeService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _shippingSettings = shippingSettings;
            _customProductReviewMappingService = customProductReviewMappingService;
           
        }


        #endregion

        #region Methods

        //public async Task<IActionResult> Configure()
        //{
        //    //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
        //    //    return AccessDeniedView();

        //    ////prepare model
        //    //var model = await _storePickupPointModelFactory.PrepareStorePickupPointSearchModelAsync(new StorePickupPointSearchModel());

        //    return View("~/Plugins/Pickup.PickupInStore/Views/Configure.cshtml", model);
        //}


        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //[FormValueRequired("add-review")]
        [HttpPost]
        [ValidateCaptcha]
        [RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
        [RequestSizeLimit(1048576000)]
        public virtual async Task<IActionResult> ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid, List<IFormFile> photos)
        {
           
            var product = await _productService.GetProductByIdAsync(productId);
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
                !await _productService.CanAddReviewAsync(product.Id,
                    _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0))
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            await ValidateProductReviewAvailabilityAsync(product);

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;
                var customer = await _workContext.GetCurrentCustomerAsync();

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = customer.Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = currentStore.Id,
                };
                await _productService.InsertProductReviewAsync(productReview);
                var reviewId =  productReview.Id;
               
            


                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await _reviewTypeService.InsertProductReviewReviewTypeMappingsAsync(additionalProductReview);
                }

                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    await _workflowMessageService.SendProductReviewNotificationMessageAsync(productReview,
                        _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
                    string.Format(
                        await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"),
                        product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));

                model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;

                //pictures
                List<string> tempList = new List<string>();

              
                foreach (var photo in photos)
                    {
                    FileInfo fileInfo = new FileInfo(photo.FileName);
                    
                    string fileName = "tempUpload"+DateTime.UtcNow.ToFileTime() + fileInfo.Extension;


                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        
                       await photo.CopyToAsync(stream);
                    }
                    tempList.Add(fileName);
                    }
                


                foreach (var filename in tempList)
                {
                    string filetype = "";
                    new FileExtensionContentTypeProvider().TryGetContentType(filename, out filetype);
                    string name = model.ProductSeName + "-" + DateTime.UtcNow.ToFileTime();
                    Stopwatch sw = new Stopwatch();
            
                    Task.Factory.StartNew(async () =>
                    {

                       

                        Core.Domain.Media.Picture pic = new Core.Domain.Media.Picture();
                        Video vid = new Video();
                        if (filetype.Contains("image"))
                        {
                            sw.Start();
                            //var oFileName = $"{Path.GetFileNameWithoutExtension(filename)}.webp";

                            //var configuration = new WebpConfigurationBuilder()
                            //    .Preset(Preset.PICTURE).QualityFactor(90).AlphaQ(10).Output(oFileName).Build();
                            //var encoder = new WebpEncoder(configuration);
                                //FileStream file=new FileStream(filename, FileMode.Open);
                                FileStream fileStream = new FileStream(filename, FileMode.Open);
                                var ms = new MemoryStream();
                            //await fileStream.CopyToAsync(ms);
                            await fileStream.DisposeAsync();
                            // file.CopyTo(ms);

                            //var fs =await encoder.EncodeAsync(ms, filename);
                            ImageFactory imageFactory = new ImageFactory(preserveExifData: false);
                            Image img = Image.FromFile(filename);
                            imageFactory.Load(img).Format(new WebPFormat()).Quality(90).Save(ms);
                             




                            sw.Stop();
                                Console.WriteLine("Elapsed Picture Encode={0}", sw.Elapsed);
                                System.IO.File.AppendAllText(@"ImageProcessPerformace.log", String.Format("Elapsed Picture Encode={0}", sw.Elapsed) + Environment.NewLine);

                                byte[] raw = ms.ToArray();
                                await ms.DisposeAsync();
                            imageFactory.Dispose();
                            img.Dispose();
                            try
                            {
                                System.IO.File.Delete(filename);
                            }
                            catch (Exception e)
                            {
                                System.IO.File.AppendAllText(@"customProductReview.log", e.Message + Environment.NewLine);

                            }
                                




                                pic =await _pictureService.InsertPictureAsync(raw, "image/webp", name);
                        }
                        else if (filetype.Contains("video"))
                        {

                            vid =await  _videoService.InsertVideoAsync(filename, name);
                            try
                            {
                                System.IO.File.Delete(filename);
                            }
                            catch (Exception e)
                            {
                                System.IO.File.AppendAllText(@"customProductReview.log", e.InnerException + Environment.NewLine);

                            }

                        }

                            int? lastPicId = pic.Id;
                        int? lastVidId = vid.Id;
                        if (lastPicId == 0)
                        {
                            lastPicId = null;
                        }

                        if (lastVidId == 0)
                        {
                            lastVidId = null;
                        }



                        if (!(lastPicId == null && lastVidId == null))
                        {
                           await  _customProductReviewMappingService.InsertCustomProductReviewMappingAsync(reviewId, lastPicId,
                                lastVidId);
                        }


                    });
                }
               

                if (!isApproved)
                    model.AddProductReview.Result =
                        await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result =
                        await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded");

                return Json(model.AddProductReview);
            }

            //if we got this far, something failed, redisplay form
            model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);
            return Json( model.AddProductReview);
        }

    


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

        #endregion



    }
}