using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Rss;
using Nop.Services.Catalog;
using Nop.Services.Events;
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
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    public partial class ProductController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
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
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
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
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
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

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("Homepage");

                return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
                _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor == null || _workContext.CurrentVendor.Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);
            //template
            var productTemplateViewPath = _productModelFactory.PrepareProductTemplateViewPath(product);

            return View(productTemplateViewPath, model);
        }

        #endregion

        #region Recently viewed products

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult RecentlyViewedProducts()
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return Content("");

            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);

            var model = new List<ProductOverviewModel>();
            model.AddRange(_productModelFactory.PrepareProductOverviewModels(products));

            return View(model);
        }

        #endregion

        #region New (recently added) products page

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult NewProducts()
        {
            if (!_catalogSettings.NewProductsEnabled)
                return Content("");

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);

            var model = new List<ProductOverviewModel>();
            model.AddRange(_productModelFactory.PrepareProductOverviewModels(products));

            return View(model);
        }

        public virtual IActionResult NewProductsRss()
        {
            var feed = new RssFeed(
                $"{_localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name)}: New products",
                "Information about products",
                new Uri(_webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_catalogSettings.NewProductsEnabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);
            foreach (var product in products)
            {
                var productUrl = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) }, _webHelper.CurrentRequestProtocol);
                var productName = _localizationService.GetLocalized(product, x => x.Name);
                var productDescription = _localizationService.GetLocalized(product, x => x.ShortDescription);
                var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{_storeContext.CurrentStore.Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
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
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Product reviews

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("Homepage");

            var model = new ProductReviewsModel();
            model = _productModelFactory.PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1).Any();
                if (!hasCompletedOrders)
                    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
            }

            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                {
                    additionalProductReview.Rating = additionalProductReview.IsRequired ? _catalogSettings.DefaultProductRatingValue : 0;
                }

            return View(model);
        }

        [HttpPost, ActionName("ProductReviews")]
        [PublicAntiForgery]
        [FormValueRequired("add-review")]
        [ValidateCaptcha]
        public virtual IActionResult ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1).Any();
                if (!hasCompletedOrders)
                    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
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
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };

                product.ProductReviews.Add(productReview);

                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };
                    productReview.ProductReviewReviewTypeMappingEntries.Add(additionalProductReview);
                }

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview",
                    string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                model = _productModelFactory.PrepareProductReviewsModel(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _productModelFactory.PrepareProductReviewsModel(model, product);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SetProductReviewHelpfulness(int productReviewId, bool washelpful)
        {
            var productReview = _productService.GetProductReviewById(productReviewId);
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            //customers aren't allowed to vote for their own reviews
            if (productReview.CustomerId == _workContext.CurrentCustomer.Id)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            //delete previous helpfulness
            var prh = productReview.ProductReviewHelpfulnessEntries
                .FirstOrDefault(x => x.CustomerId == _workContext.CurrentCustomer.Id);
            if (prh != null)
            {
                //existing one
                prh.WasHelpful = washelpful;
            }
            else
            {
                //insert new helpfulness
                prh = new ProductReviewHelpfulness
                {
                    ProductReviewId = productReview.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    WasHelpful = washelpful,
                };
                productReview.ProductReviewHelpfulnessEntries.Add(prh);
            }
            _productService.UpdateProduct(productReview.Product);

            //new totals
            productReview.HelpfulYesTotal = productReview.ProductReviewHelpfulnessEntries.Count(x => x.WasHelpful);
            productReview.HelpfulNoTotal = productReview.ProductReviewHelpfulnessEntries.Count(x => !x.WasHelpful);
            _productService.UpdateProduct(productReview.Product);

            return Json(new
            {
                Result = _localizationService.GetResource("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        public virtual IActionResult CustomerProductReviews(int? pageNumber)
        {
            if (_workContext.CurrentCustomer.IsGuest())
                return Challenge();

            if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _productModelFactory.PrepareCustomerProductReviewsModel(pageNumber);
            return View(model);
        }

        #endregion

        #region Email a friend

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductEmailAFriend(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new ProductEmailAFriendModel();
            model = _productModelFactory.PrepareProductEmailAFriendModel(model, product, false);
            return View(model);
        }

        [HttpPost, ActionName("ProductEmailAFriend")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual IActionResult ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
        {
            var product = _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            //check whether the current customer is guest and ia allowed to email a friend
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Products.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendProductEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, product,
                        model.YourEmailAddress, model.FriendEmail,
                        Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Products.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
            return View(model);
        }

        #endregion

        #region Comparing products

        [HttpPost]
        public virtual IActionResult AddProductToCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
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

            _compareProductsService.AddProductToCompareList(productId);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToCompareList",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

            return Json(new
            {
                success = true,
                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareProducts"))
                //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
                //redirect = Url.RouteUrl("CompareProducts"),
            });
        }

        public virtual IActionResult RemoveProductFromCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoute("Homepage");

            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            _compareProductsService.RemoveProductFromCompareList(productId);

            return RedirectToRoute("CompareProducts");
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult CompareProducts()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            var model = new CompareProductsModel
            {
                IncludeShortDescriptionInCompareProducts = _catalogSettings.IncludeShortDescriptionInCompareProducts,
                IncludeFullDescriptionInCompareProducts = _catalogSettings.IncludeFullDescriptionInCompareProducts,
            };

            var products = _compareProductsService.GetComparedProducts();

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            //prepare model
            _productModelFactory.PrepareProductOverviewModels(products, prepareSpecificationAttributes: true)
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