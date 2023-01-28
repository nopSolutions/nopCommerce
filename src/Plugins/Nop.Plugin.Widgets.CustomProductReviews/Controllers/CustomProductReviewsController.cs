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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

using System.Runtime.InteropServices;
using Nop.Web.Models.Catalog;

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


        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        private static extern int FindMimeFromData(IntPtr pBc,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)]
            byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved
        );

        /**
         * This function will detect mime type from provided byte array
         * and if it fails, it will return default mime type
         */
        private static string GetMimeFromBytes(byte[] dataBytes, string defaultMimeType)
        {
            if (dataBytes == null)
                throw new ArgumentNullException(nameof(dataBytes));

            var mimeType = string.Empty;
            IntPtr suggestPtr = IntPtr.Zero, filePtr = IntPtr.Zero;

            try
            {
                var ret = FindMimeFromData(IntPtr.Zero, null, dataBytes, dataBytes.Length, null, 0, out var outPtr, 0);
                if (ret == 0 && outPtr != IntPtr.Zero)
                {
                    mimeType = Marshal.PtrToStringUni(outPtr);
                    Marshal.FreeCoTaskMem(outPtr);
                }

                if (!mimeType.Contains("image")|| !mimeType.Contains("video"))
                {
                    mimeType = defaultMimeType;
                }
            }
            catch
            {
                mimeType = defaultMimeType;
            }

            return mimeType;
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

                #region Product Review Media Upload Section

                

              
                //pictures
                List<UploadDataBinary> dataList = new List<UploadDataBinary>();

              
                foreach (var photo in photos)
                {
                    var uploadData = new UploadDataBinary();
                        FileInfo fileInfo = new FileInfo(photo.FileName);
                        uploadData.Extentions= photo.ContentType;
                        //string fileName = "tempUpload"+DateTime.UtcNow.ToFileTime() + fileInfo.Extension;


                    using (var ms = new MemoryStream())
                    {
                        await photo.CopyToAsync(ms);
                        uploadData.BinaryData = ms.ToArray();
                        dataList.Add(uploadData);
                    }
                   
                    }
                


                foreach (var data in dataList)
                {
                    InsertReviewMedia(model, data, reviewId);
                }

                #endregion
                if (!isApproved)
                    model.AddProductReview.Result =
                        await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving") + Environment.NewLine +
                        " Your uploaded media(photo or video ) will continue to be processed in the background." + Environment.NewLine +
                        " After processing, the media will be automatically added to your review.";

                else
                    model.AddProductReview.Result =
                        await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded")+ Environment.NewLine +
                        " Your uploaded media(photo or video ) will continue to be processed in the background." + Environment.NewLine+
                        " After processing, the media will be automatically added to your review.";

                return Json(model);
            }

            //if we got this far, something failed, redisplay form
            model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);
            return Json( model);
        }

        private void InsertReviewMedia(ProductReviewsModel model, UploadDataBinary data, int reviewId)
        {
            string filetype = GetMimeFromBytes(data.BinaryData, data.Extentions);

            string name = model.ProductSeName + "-" + DateTime.UtcNow.ToFileTime();
            Stopwatch sw = new Stopwatch();

            Task.Factory.StartNew(async () =>
            {
                var pic = new Picture();


                Video vid = new Video();
                if (filetype.Contains("image"))
                {
                    try
                    {
                        sw.Start();

                        var ms = new MemoryStream();
                        ImageFactory imageFactory = new ImageFactory(preserveExifData: false);
                        imageFactory.Load(data.BinaryData).Format(new WebPFormat()).Quality(90).Save(ms);
                        sw.Stop();
                        Console.WriteLine("Elapsed Picture Encode={0}", sw.Elapsed);
                        System.IO.File.AppendAllText(@"ImageProcessPerformace.log", string.Format("Elapsed Picture Encode={0}", sw.Elapsed) + Environment.NewLine);

                        byte[] raw = ms.ToArray();
                        await ms.DisposeAsync();
                        imageFactory.Dispose();


                        pic = await _pictureService.InsertPictureAsync(raw, "image/webp", name);
                    }
                    catch (Exception e)
                    {
                        System.IO.File.AppendAllText(@"customProductReview.log", e.Message + Environment.NewLine);
                    }
                }
                else if (filetype.Contains("video"))
                {
                    try
                    {
                        vid = await _videoService.InsertVideoAsync(data.BinaryData, name, filetype);
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
                    await _customProductReviewMappingService.InsertCustomProductReviewMappingAsync(reviewId, lastPicId,
                        lastVidId);
                }
            });
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