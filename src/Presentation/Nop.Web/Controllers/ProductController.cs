using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Rss;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
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

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ProductController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
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

        #endregion

        #region Ctor

        public ProductController(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICompareProductsService compareProductsService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
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
            ShoppingCartSettings shoppingCartSettings)
        {
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _compareProductsService = compareProductsService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
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
        }

        #endregion

        #region Product details page

        public virtual async Task<IActionResult> ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !await _aclService.Authorize(product) ||
                //Store mapping
                !await _storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = await _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("Homepage");

                return RedirectToRoutePermanent("Product", new { SeName = await _urlRecordService.GetSeName(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), storeId: (await _storeContext.GetCurrentStore()).Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = await _urlRecordService.GetSeName(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = await _urlRecordService.GetSeName(product) });
                }
            }

            //save as recently viewed
            await _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //display "edit" (manage) link
            if (await _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
                await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                if (await _workContext.GetCurrentVendor() == null || (await _workContext.GetCurrentVendor()).Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            await _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(await _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = await _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);
            //template
            var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPath(product);

            return View(productTemplateViewPath, model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> EstimateShipping([FromQuery] ProductDetailsModel.ProductEstimateShippingModel model, IFormCollection form)
        {
            if (model == null)
                model = new ProductDetailsModel.ProductEstimateShippingModel();

            var errors = new List<string>();
            if (string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(await _localizationService.GetResource("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await _localizationService.GetResource("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new { 
                    success = false,
                    errors = errors
                });
            
            var product = await _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted)
            {
                errors.Add(await _localizationService.GetResource("Shipping.EstimateShippingPopUp.Product.IsNotFound"));
                return Json(new
                {
                    success = false,
                    errors = errors
                });
            }

            var wrappedProduct = new ShoppingCartItem()
            {
                StoreId = (await _storeContext.GetCurrentStore()).Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                ProductId = product.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var addToCartWarnings = new List<string>();
            //customer entered price
            wrappedProduct.CustomerEnteredPrice = await _productAttributeParser.ParseCustomerEnteredPrice(product, form);

            //entered quantity
            wrappedProduct.Quantity = _productAttributeParser.ParseEnteredQuantity(product, form);

            //product and gift card attributes
            wrappedProduct.AttributesXml = await _productAttributeParser.ParseProductAttributes(product, form, addToCartWarnings);

            //rental attributes
            _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);
            wrappedProduct.RentalStartDateUtc = rentalStartDate;
            wrappedProduct.RentalEndDateUtc = rentalEndDate;

            var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModel(new [] { wrappedProduct }, model.CountryId, model.StateProvinceId, model.ZipPostalCode, false);

            return Json(new
            {
                success = true,
                result = result
            });
        }

        #endregion

        #region Recently viewed products

        public virtual async Task<IActionResult> RecentlyViewedProducts()
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return Content("");

            var products = await _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);

            var model = new List<ProductOverviewModel>();
            model.AddRange(await _productModelFactory.PrepareProductOverviewModels(products));

            return View(model);
        }

        #endregion

        #region New (recently added) products page

        public virtual async Task<IActionResult> NewProducts()
        {
            if (!_catalogSettings.NewProductsEnabled)
                return Content("");

            var products = await _productService.SearchProducts(0,
                storeId: (await _storeContext.GetCurrentStore()).Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);

            var model = new List<ProductOverviewModel>();
            model.AddRange(await _productModelFactory.PrepareProductOverviewModels(products));

            return View(model);
        }

        public virtual async Task<IActionResult> NewProductsRss()
        {
            var feed = new RssFeed(
                $"{await _localizationService.GetLocalized(await _storeContext.GetCurrentStore(), x => x.Name)}: New products",
                "Information about products",
                new Uri(await _webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_catalogSettings.NewProductsEnabled)
                return new RssActionResult(feed, await _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();

            var products = await _productService.SearchProducts(0,
                storeId: (await _storeContext.GetCurrentStore()).Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);
            foreach (var product in products)
            {
                var productUrl = Url.RouteUrl("Product", new { SeName = await _urlRecordService.GetSeName(product) }, _webHelper.CurrentRequestProtocol);
                var productName = await _localizationService.GetLocalized(product, x => x.Name);
                var productDescription = await _localizationService.GetLocalized(product, x => x.ShortDescription);
                var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{(await _storeContext.GetCurrentStore()).Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
                items.Add(item);
                //uncomment below if you want to add RSS enclosure for pictures
                //var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                //if (picture != null)
                //{
                //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductDetailsPictureSize);
                //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl), new XAttribute("length", picture.PictureBinary.Length)));
                //}

            }
            feed.Items = items;
            return new RssActionResult(feed, await _webHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Product reviews

        public virtual async Task<IActionResult> ProductReviews(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("Homepage");

            var model = new ProductReviewsModel();
            model = await _productModelFactory.PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", await _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = (await _orderService.SearchOrders(customerId: (await _workContext.GetCurrentCustomer()).Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1)).Any();
                if (!hasCompletedOrders)
                    ModelState.AddModelError(string.Empty, await _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
            }

            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

            model.AddProductReview.CanAddNewReview = await _productService.CanAddReview(product.Id, (await _storeContext.GetCurrentStore()).Id);

            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                {
                    additionalProductReview.Rating = additionalProductReview.IsRequired ? _catalogSettings.DefaultProductRatingValue : 0;
                }

            return View(model);
        }

        [HttpPost, ActionName("ProductReviews")]        
        [FormValueRequired("add-review")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid)
        {
            var product = await _productService.GetProductById(productId);
            
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
                !await _productService.CanAddReview(product.Id, (await _storeContext.GetCurrentStore()).Id))
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = (await _orderService.SearchOrders(customerId: (await _workContext.GetCurrentCustomer()).Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1)).Any();
                if (!hasCompletedOrders)
                    ModelState.AddModelError(string.Empty, await _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
            }

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = (await _storeContext.GetCurrentStore()).Id,
                };

                await _productService.InsertProductReview(productReview);

                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await _reviewTypeService.InsertProductReviewReviewTypeMappings(additionalProductReview);
                }

                //update product totals
                await _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    await _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _customerActivityService.InsertActivity("PublicStore.AddProductReview",
                    string.Format(await _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    await _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                model = await _productModelFactory.PrepareProductReviewsModel(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = await _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = await _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _productModelFactory.PrepareProductReviewsModel(model, product);
            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> SetProductReviewHelpfulness(int productReviewId, bool washelpful)
        {
            var productReview = await _productService.GetProductReviewById(productReviewId);
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResource("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            //customers aren't allowed to vote for their own reviews
            if (productReview.CustomerId == (await _workContext.GetCurrentCustomer()).Id)
            {
                return Json(new
                {
                    Result = await _localizationService.GetResource("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            await _productService.SetProductReviewHelpfulness(productReview, washelpful);

            //new totals
            await _productService.UpdateProductReviewHelpfulnessTotals(productReview);

            return Json(new
            {
                Result = await _localizationService.GetResource("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        public virtual async Task<IActionResult> CustomerProductReviews(int? pageNumber)
        {
            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()))
                return Challenge();

            if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = await _productModelFactory.PrepareCustomerProductReviewsModel(pageNumber);
            
            return View(model);
        }

        #endregion

        #region Email a friend

        public virtual async Task<IActionResult> ProductEmailAFriend(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new ProductEmailAFriendModel();
            model = await _productModelFactory.PrepareProductEmailAFriendModel(model, product, false);
            return View(model);
        }

        [HttpPost, ActionName("ProductEmailAFriend")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
        {
            var product = await _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            //check whether the current customer is guest and ia allowed to email a friend
            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Products.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                await _workflowMessageService.SendProductEmailAFriendMessage(await _workContext.GetCurrentCustomer(),
                        (await _workContext.GetWorkingLanguage()).Id, product,
                        model.YourEmailAddress, model.FriendEmail,
                        Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = await _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResource("Products.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
            return View(model);
        }

        #endregion

        #region Comparing products

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> AddProductToCompareList(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            if (!_catalogSettings.CompareProductsEnabled)
                return Json(new
                {
                    success = false,
                    message = "Product comparison is disabled"
                });

            await _compareProductsService.AddProductToCompareList(productId);

            //activity log
            await _customerActivityService.InsertActivity("PublicStore.AddToCompareList",
                string.Format(await _localizationService.GetResource("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

            return Json(new
            {
                success = true,
                message = string.Format(await _localizationService.GetResource("Products.ProductHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareProducts"))
                //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
                //redirect = Url.RouteUrl("CompareProducts"),
            });
        }

        public virtual async Task<IActionResult> RemoveProductFromCompareList(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoute("Homepage");

            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            await _compareProductsService.RemoveProductFromCompareList(productId);

            return RedirectToRoute("CompareProducts");
        }

        public virtual async Task<IActionResult> CompareProducts()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            var model = new CompareProductsModel
            {
                IncludeShortDescriptionInCompareProducts = _catalogSettings.IncludeShortDescriptionInCompareProducts,
                IncludeFullDescriptionInCompareProducts = _catalogSettings.IncludeFullDescriptionInCompareProducts,
            };

            var products = await _compareProductsService.GetComparedProducts();

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p).Result && _storeMappingService.Authorize(p).Result).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            //prepare model
            (await _productModelFactory.PrepareProductOverviewModels(products, prepareSpecificationAttributes: true))
                .ToList()
                .ForEach(model.Products.Add);

            return View(model);
        }

        public virtual IActionResult ClearCompareList()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            _compareProductsService.ClearCompareProducts();

            return RedirectToRoute("CompareProducts");
        }

        #endregion
    }
}