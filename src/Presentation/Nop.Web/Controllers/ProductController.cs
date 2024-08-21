using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
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
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ProductController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICompareProductsService _compareProductsService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;
    protected readonly ShippingSettings _shippingSettings;

    #endregion

    #region Ctor

    public ProductController(CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        IAclService aclService,
        ICompareProductsService compareProductsService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        INotificationService notificationService,
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
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        ShoppingCartSettings shoppingCartSettings,
        ShippingSettings shippingSettings)
    {
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _compareProductsService = compareProductsService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _notificationService = notificationService;
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
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _shoppingCartSettings = shoppingCartSettings;
        _shippingSettings = shippingSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task ValidateProductReviewAvailabilityAsync(Product product)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

        if (!_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            return;

        var hasCompletedOrders = product.ProductType == ProductType.SimpleProduct
            ? await HasCompletedOrdersAsync(product)
            : await (await _productService.GetAssociatedProductsAsync(product.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

        if (!hasCompletedOrders)
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
    }

    protected virtual async ValueTask<bool> HasCompletedOrdersAsync(Product product)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        return (await _orderService.SearchOrdersAsync(customerId: customer.Id,
                productId: product.Id,
                osIds: [(int)OrderStatus.Complete],
            pageSize: 1)).Any();
    }

    #endregion

    #region Product details page

    public virtual async Task<IActionResult> ProductDetails(int productId, int updatecartitemid = 0)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted)
            return InvokeHttp404();

        var notAvailable =
            //published?
            (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(product) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(product) ||
            //availability dates
            !_productService.ProductIsAvailable(product);
        //Check whether the current user has a "Manage products" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        //visible individually?
        if (!product.VisibleIndividually)
        {
            //is this one an associated products?
            var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
            if (parentGroupedProduct == null)
                return RedirectToRoute("Homepage");

            var seName = await _urlRecordService.GetSeNameAsync(parentGroupedProduct);
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName });
            return LocalRedirectPermanent(productUrl);
        }

        //update existing shopping cart or wishlist  item?
        ShoppingCartItem updatecartitem = null;
        if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        {
            var seName = await _urlRecordService.GetSeNameAsync(product);
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName });
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: store.Id);
            updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

            //not found?
            if (updatecartitem == null)
                return LocalRedirect(productUrl);

            //is it this product?
            if (product.Id != updatecartitem.ProductId)
                return LocalRedirect(productUrl);
        }

        //save as recently viewed
        await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) &&
            await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW))
        {
            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null || currentVendor.Id == product.VendorId)
            {
                DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.ADMIN }));
            }
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

        //model
        var model = await _productModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);
        //template
        var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);

        return View(productTemplateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> EstimateShipping([FromQuery] ProductDetailsModel.ProductEstimateShippingModel model, IFormCollection form)
    {
        if (model == null)
            model = new ProductDetailsModel.ProductEstimateShippingModel();

        var errors = new List<string>();

        if (!_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.ZipPostalCode))
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

        if (_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.City))
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.City.Required"));

        if (model.CountryId == null || model.CountryId == 0)
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

        if (errors.Count > 0)
            return Json(new
            {
                Success = false,
                Errors = errors
            });

        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product == null || product.Deleted)
        {
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Product.IsNotFound"));
            return Json(new
            {
                Success = false,
                Errors = errors
            });
        }

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        var wrappedProduct = new ShoppingCartItem()
        {
            StoreId = store.Id,
            ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
            CustomerId = customer.Id,
            ProductId = product.Id,
            CreatedOnUtc = DateTime.UtcNow
        };

        var addToCartWarnings = new List<string>();
        //customer entered price
        wrappedProduct.CustomerEnteredPrice = await _productAttributeParser.ParseCustomerEnteredPriceAsync(product, form);

        //entered quantity
        wrappedProduct.Quantity = _productAttributeParser.ParseEnteredQuantity(product, form);

        //product and gift card attributes
        wrappedProduct.AttributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);

        //rental attributes
        _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);
        wrappedProduct.RentalStartDateUtc = rentalStartDate;
        wrappedProduct.RentalEndDateUtc = rentalEndDate;

        var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new[] { wrappedProduct }, model, false);

        return Json(result);
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetProductCombinations(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return NotFound();

        var model = await _productModelFactory.PrepareProductCombinationModelsAsync(product);
        return Ok(model);
    }

    #endregion

    #region Recently viewed products

    public virtual async Task<IActionResult> RecentlyViewedProducts()
    {
        if (!_catalogSettings.RecentlyViewedProductsEnabled)
            return Content("");

        var products = await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(_catalogSettings.RecentlyViewedProductsNumber);

        var model = new List<ProductOverviewModel>();
        model.AddRange(await _productModelFactory.PrepareProductOverviewModelsAsync(products));

        return View(model);
    }

    #endregion

    #region Product reviews

    public virtual async Task<IActionResult> ProductReviews(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
            return RedirectToRoute("Homepage");

        var model = new ProductReviewsModel();
        model = await _productModelFactory.PrepareProductReviewsModelAsync(product);

        await ValidateProductReviewAvailabilityAsync(product);

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
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
            !await _productService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0))
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
                await _workflowMessageService.SendProductReviewStoreOwnerNotificationMessageAsync(productReview, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

            //raise event
            if (productReview.IsApproved)
                await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));

            model = await _productModelFactory.PrepareProductReviewsModelAsync(product);
            model.AddProductReview.Title = null;
            model.AddProductReview.ReviewText = null;

            if (!isApproved)
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving"));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded"));

            var seName = await _urlRecordService.GetSeNameAsync(product);
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName });
            return LocalRedirect(productUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "ProductDetails";

        //model
        var productModel = await _productModelFactory.PrepareProductDetailsModelAsync(product);
        //template
        var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);

        return View(productTemplateViewPath, productModel);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SetProductReviewHelpfulness(int productReviewId, bool washelpful)
    {
        var productReview = await _productService.GetProductReviewByIdAsync(productReviewId) ?? throw new ArgumentException("No product review found with the specified id");

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
        {
            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.OnlyRegistered"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        //customers aren't allowed to vote for their own reviews
        if (productReview.CustomerId == customer.Id)
        {
            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.YourOwnReview"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        await _productService.SetProductReviewHelpfulnessAsync(productReview, washelpful);

        //new totals
        await _productService.UpdateProductReviewHelpfulnessTotalsAsync(productReview);

        return Json(new
        {
            Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.SuccessfullyVoted"),
            TotalYes = productReview.HelpfulYesTotal,
            TotalNo = productReview.HelpfulNoTotal
        });
    }

    public virtual async Task<IActionResult> CustomerProductReviews(int? pageNumber)
    {
        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
        {
            return RedirectToRoute("CustomerInfo");
        }

        var model = await _productModelFactory.PrepareCustomerProductReviewsModelAsync(pageNumber);

        return View(model);
    }

    #endregion

    #region Email a friend

    public virtual async Task<IActionResult> ProductEmailAFriend(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
            return RedirectToRoute("Homepage");

        var model = new ProductEmailAFriendModel();
        model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, false);
        return View(model);
    }

    [HttpPost, ActionName("ProductEmailAFriend")]
    [FormValueRequired("send-email")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
    {
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
            return RedirectToRoute("Homepage");

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        //check whether the current customer is guest and ia allowed to email a friend
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Products.EmailAFriend.OnlyRegisteredUsers"));
        }

        if (ModelState.IsValid)
        {
            //email
            await _workflowMessageService.SendProductEmailAFriendMessageAsync(customer,
                (await _workContext.GetWorkingLanguageAsync()).Id, product,
                model.YourEmailAddress, model.FriendEmail,
                _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false));

            model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
            model.SuccessfullySent = true;
            model.Result = await _localizationService.GetResourceAsync("Products.EmailAFriend.SuccessfullySent");

            return View(model);
        }

        //If we got this far, something failed, redisplay form
        model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
        return View(model);
    }

    #endregion

    #region Comparing products

    [HttpPost]
    public virtual async Task<IActionResult> AddProductToCompareList(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
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

        await _compareProductsService.AddProductToCompareListAsync(productId);

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.AddToCompareList",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

        return Json(new
        {
            success = true,
            message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareProducts"))
            //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
            //redirect = Url.RouteUrl("CompareProducts"),
        });
    }

    public virtual async Task<IActionResult> RemoveProductFromCompareList(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return RedirectToRoute("Homepage");

        if (!_catalogSettings.CompareProductsEnabled)
            return RedirectToRoute("Homepage");

        await _compareProductsService.RemoveProductFromCompareListAsync(productId);

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

        var products = await (await _compareProductsService.GetComparedProductsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();

        //prepare model
        var poModels = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, prepareSpecificationAttributes: true))
            .ToList();
        foreach (var poModel in poModels)
        {
            model.Products.Add(poModel);
        }

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