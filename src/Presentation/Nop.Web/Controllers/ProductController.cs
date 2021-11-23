using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Rss;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Html;
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

        protected CaptchaSettings CaptchaSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected IAclService AclService { get; }
        protected ICompareProductsService CompareProductsService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INopHtmlHelper NopHtmlHelper { get; }
        protected IOrderService OrderService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductAttributeParser ProductAttributeParser { get; }
        protected IProductModelFactory ProductModelFactory { get; }
        protected IProductService ProductService { get; }
        protected IRecentlyViewedProductsService RecentlyViewedProductsService { get; }
        protected IReviewTypeService ReviewTypeService { get; }
        protected IShoppingCartModelFactory ShoppingCartModelFactory { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected ShoppingCartSettings ShoppingCartSettings { get; }
        protected ShippingSettings ShippingSettings { get; }

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
            INopHtmlHelper nopHtmlHelper,
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
            ShippingSettings shippingSettings)
        {
            CaptchaSettings = captchaSettings;
            CatalogSettings = catalogSettings;
            AclService = aclService;
            CompareProductsService = compareProductsService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            NopHtmlHelper = nopHtmlHelper;
            OrderService = orderService;
            PermissionService = permissionService;
            ProductAttributeParser = productAttributeParser;
            ProductModelFactory = productModelFactory;
            ProductService = productService;
            ReviewTypeService = reviewTypeService;
            RecentlyViewedProductsService = recentlyViewedProductsService;
            ShoppingCartModelFactory = shoppingCartModelFactory;
            ShoppingCartService = shoppingCartService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            ShoppingCartSettings = shoppingCartSettings;
            ShippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task ValidateProductReviewAvailabilityAsync(Product product)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !CatalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (!CatalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
                return;

            var hasCompletedOrders = product.ProductType == ProductType.SimpleProduct
                ? await HasCompletedOrdersAsync(product)
                : await (await ProductService.GetAssociatedProductsAsync(product.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

            if (!hasCompletedOrders)
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
        }

        protected virtual async ValueTask<bool> HasCompletedOrdersAsync(Product product)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            return (await OrderService.SearchOrdersAsync(customerId: customer.Id,
                productId: product.Id,
                osIds: new List<int> { (int)OrderStatus.Complete },
                pageSize: 1)).Any();
        }

        #endregion

        #region Product details page

        public virtual async Task<IActionResult> ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !CatalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !await AclService.AuthorizeAsync(product) ||
                //Store mapping
                !await StoreMappingService.AuthorizeAsync(product) ||
                //availability dates
                !ProductService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = await ProductService.GetProductByIdAsync(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("Homepage");

                return RedirectToRoutePermanent("Product", new { SeName = await UrlRecordService.GetSeNameAsync(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (ShoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(await WorkContext.GetCurrentCustomerAsync(), storeId: store.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) });
                }
            }

            //save as recently viewed
            await RecentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

            //display "edit" (manage) link
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) &&
                await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                var currentVendor = await WorkContext.GetCurrentVendorAsync();
                if (currentVendor == null || currentVendor.Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            await CustomerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = await ProductModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);
            //template
            var productTemplateViewPath = await ProductModelFactory.PrepareProductTemplateViewPathAsync(product);

            return View(productTemplateViewPath, model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> EstimateShipping([FromQuery] ProductDetailsModel.ProductEstimateShippingModel model)
        {
            if (model == null)
                model = new ProductDetailsModel.ProductEstimateShippingModel();

            var errors = new List<string>();
            
            if (!ShippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (ShippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.City))
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.City.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new
                {
                    Success = false,
                    Errors = errors
                });

            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product == null || product.Deleted)
            {
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Product.IsNotFound"));
                return Json(new
                {
                    Success = false,
                    Errors = errors
                });
            }
            
            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();

            var wrappedProduct = new ShoppingCartItem()
            {
                StoreId = store.Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                CustomerId = customer.Id,
                ProductId = product.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var form = model.Form;

            var addToCartWarnings = new List<string>();
            //customer entered price
            wrappedProduct.CustomerEnteredPrice = await ProductAttributeParser.ParseCustomerEnteredPriceAsync(product, form);

            //entered quantity
            wrappedProduct.Quantity = ProductAttributeParser.ParseEnteredQuantity(product, form);

            //product and gift card attributes
            wrappedProduct.AttributesXml = await ProductAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);

            //rental attributes
            ProductAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);
            wrappedProduct.RentalStartDateUtc = rentalStartDate;
            wrappedProduct.RentalEndDateUtc = rentalEndDate;

            var result = await ShoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new[] { wrappedProduct }, model, false);

            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetProductCombinations(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound();

            var model = await ProductModelFactory.PrepareProductCombinationModelsAsync(product);
            return Ok(model);
        }

        #endregion

        #region Recently viewed products

        public virtual async Task<IActionResult> RecentlyViewedProducts()
        {
            if (!CatalogSettings.RecentlyViewedProductsEnabled)
                return Content("");

            var products = await RecentlyViewedProductsService.GetRecentlyViewedProductsAsync(CatalogSettings.RecentlyViewedProductsNumber);

            var model = new List<ProductOverviewModel>();
            model.AddRange(await ProductModelFactory.PrepareProductOverviewModelsAsync(products));

            return View(model);
        }

        #endregion

        #region New (recently added) products page

        public virtual async Task<IActionResult> NewProducts()
        {
            if (!CatalogSettings.NewProductsEnabled)
                return Content("");

            var store = await StoreContext.GetCurrentStoreAsync();
            var storeId = store.Id;
            var products = await ProductService.GetProductsMarkedAsNewAsync(storeId);
            var model = (await ProductModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            return View(model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> NewProductsRss()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                $"{await LocalizationService.GetLocalizedAsync(store, x => x.Name)}: New products",
                "Information about products",
                new Uri(WebHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!CatalogSettings.NewProductsEnabled)
                return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();

            var storeId = store.Id;
            var products = await ProductService.GetProductsMarkedAsNewAsync(storeId);

            foreach (var product in products)
            {
                var productUrl = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) }, WebHelper.GetCurrentRequestProtocol());
                var productName = await LocalizationService.GetLocalizedAsync(product, x => x.Name);
                var productDescription = await LocalizationService.GetLocalizedAsync(product, x => x.ShortDescription);
                var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{store.Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
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
            return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Product reviews

        public virtual async Task<IActionResult> ProductReviews(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("Homepage");

            var model = new ProductReviewsModel();
            model = await ProductModelFactory.PrepareProductReviewsModelAsync(model, product);

            await ValidateProductReviewAvailabilityAsync(product);

            //default value
            model.AddProductReview.Rating = CatalogSettings.DefaultProductRatingValue;
            
            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                {
                    additionalProductReview.Rating = additionalProductReview.IsRequired ? CatalogSettings.DefaultProductRatingValue : 0;
                }

            return View(model);
        }

        [HttpPost, ActionName("ProductReviews")]
        [FormValueRequired("add-review")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            var currentStore = await StoreContext.GetCurrentStoreAsync();

            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
                !await ProductService.CanAddReviewAsync(product.Id, CatalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0))
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnProductReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            await ValidateProductReviewAvailabilityAsync(product);

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = CatalogSettings.DefaultProductRatingValue;
                var isApproved = !CatalogSettings.ProductReviewsMustBeApproved;
                var customer = await WorkContext.GetCurrentCustomerAsync();

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

                await ProductService.InsertProductReviewAsync(productReview);

                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await ReviewTypeService.InsertProductReviewReviewTypeMappingsAsync(additionalProductReview);
                }

                //update product totals
                await ProductService.UpdateProductReviewTotalsAsync(product);

                //notify store owner
                if (CatalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    await WorkflowMessageService.SendProductReviewNotificationMessageAsync(productReview, LocalizationSettings.DefaultAdminLanguageId);

                //activity log
                await CustomerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    await EventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));

                model = await ProductModelFactory.PrepareProductReviewsModelAsync(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = await LocalizationService.GetResourceAsync("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = await LocalizationService.GetResourceAsync("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //if we got this far, something failed, redisplay form
            model = await ProductModelFactory.PrepareProductReviewsModelAsync(model, product);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetProductReviewHelpfulness(int productReviewId, bool washelpful)
        {
            var productReview = await ProductService.GetProductReviewByIdAsync(productReviewId);
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !CatalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                return Json(new
                {
                    Result = await LocalizationService.GetResourceAsync("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            //customers aren't allowed to vote for their own reviews
            if (productReview.CustomerId == customer.Id)
            {
                return Json(new
                {
                    Result = await LocalizationService.GetResourceAsync("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            await ProductService.SetProductReviewHelpfulnessAsync(productReview, washelpful);

            //new totals
            await ProductService.UpdateProductReviewHelpfulnessTotalsAsync(productReview);

            return Json(new
            {
                Result = await LocalizationService.GetResourceAsync("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        public virtual async Task<IActionResult> CustomerProductReviews(int? pageNumber)
        {
            if (await CustomerService.IsGuestAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (!CatalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = await ProductModelFactory.PrepareCustomerProductReviewsModelAsync(pageNumber);

            return View(model);
        }

        #endregion

        #region Email a friend

        public virtual async Task<IActionResult> ProductEmailAFriend(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted || !product.Published || !CatalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new ProductEmailAFriendModel();
            model = await ProductModelFactory.PrepareProductEmailAFriendModelAsync(model, product, false);
            return View(model);
        }

        [HttpPost, ActionName("ProductEmailAFriend")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
        {
            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product == null || product.Deleted || !product.Published || !CatalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //check whether the current customer is guest and ia allowed to email a friend
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !CatalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Products.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                await WorkflowMessageService.SendProductEmailAFriendMessageAsync(customer,
                        (await WorkContext.GetWorkingLanguageAsync()).Id, product,
                        model.YourEmailAddress, model.FriendEmail,
                        NopHtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = await ProductModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
                model.SuccessfullySent = true;
                model.Result = await LocalizationService.GetResourceAsync("Products.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await ProductModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
            return View(model);
        }

        #endregion

        #region Comparing products

        [HttpPost]
        public virtual async Task<IActionResult> AddProductToCompareList(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted || !product.Published)
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            if (!CatalogSettings.CompareProductsEnabled)
                return Json(new
                {
                    success = false,
                    message = "Product comparison is disabled"
                });

            await CompareProductsService.AddProductToCompareListAsync(productId);

            //activity log
            await CustomerActivityService.InsertActivityAsync("PublicStore.AddToCompareList",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

            return Json(new
            {
                success = true,
                message = string.Format(await LocalizationService.GetResourceAsync("Products.ProductHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareProducts"))
                //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
                //redirect = Url.RouteUrl("CompareProducts"),
            });
        }

        public virtual async Task<IActionResult> RemoveProductFromCompareList(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return RedirectToRoute("Homepage");

            if (!CatalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            await CompareProductsService.RemoveProductFromCompareListAsync(productId);

            return RedirectToRoute("CompareProducts");
        }

        public virtual async Task<IActionResult> CompareProducts()
        {
            if (!CatalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            var model = new CompareProductsModel
            {
                IncludeShortDescriptionInCompareProducts = CatalogSettings.IncludeShortDescriptionInCompareProducts,
                IncludeFullDescriptionInCompareProducts = CatalogSettings.IncludeFullDescriptionInCompareProducts,
            };

            var products = await (await CompareProductsService.GetComparedProductsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await AclService.AuthorizeAsync(p) && await StoreMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => ProductService.ProductIsAvailable(p)).ToListAsync();

            //prepare model
            (await ProductModelFactory.PrepareProductOverviewModelsAsync(products, prepareSpecificationAttributes: true))
                .ToList()
                .ForEach(model.Products.Add);

            return View(model);
        }

        public virtual IActionResult ClearCompareList()
        {
            if (!CatalogSettings.CompareProductsEnabled)
                return RedirectToRoute("Homepage");

            CompareProductsService.ClearCompareProducts();

            return RedirectToRoute("CompareProducts");
        }

        #endregion
    }
}