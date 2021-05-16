using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/catalog")]
    [AuthorizeAttribute]
    public class CatalogApiController : BaseApiController
    {
        #region Fields

        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ICurrencyService _currencyService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ITopicModelFactory _topicModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISearchTermService _searchTermService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public CatalogApiController(ICurrencyService currencyService,
            ShoppingCartSettings shoppingCartSettings,
        LocalizationSettings localizationSettings,
            IWorkflowMessageService workflowMessageService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            MediaSettings mediaSettings,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager cacheManager,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IStateProvinceService stateProvinceService,
            ICountryService countryService,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            ICatalogModelFactory catalogModelService,
            IProductModelFactory productModelFactory,
            ITopicModelFactory topicModelFactory,
            ICategoryService categoryService,
            IProductService productService,
            ILocalizationService localizationService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            VendorSettings vendorSettings,
            IHttpContextAccessor httpContextAccessor,
            ISearchTermService searchTermService,
            IEventPublisher eventPublisher,
            IVendorService vendorService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            IOrderProcessingService orderProcessingService)
        {
            _shoppingCartSettings = shoppingCartSettings;
            _localizationSettings = localizationSettings;
            _workflowMessageService = workflowMessageService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _mediaSettings = mediaSettings;
            _specificationAttributeService = specificationAttributeService;
            _cacheManager = cacheManager;
            _currencyService = currencyService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _categoryService = categoryService;
            _catalogModelFactory = catalogModelService;
            _topicModelFactory = topicModelFactory;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _localizationService = localizationService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _catalogSettings = catalogSettings;
            _storeContext = storeContext;
            _productModelFactory = productModelFactory;
            _workContext = workContext;
            _vendorSettings = vendorSettings;
            _httpContextAccessor = httpContextAccessor;
            _searchTermService = searchTermService;
            _vendorService = vendorService;
            _eventPublisher = eventPublisher;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _paymentService = paymentService;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        #region Utility

        [NonAction]
        protected virtual async Task<IList<ProductSpecificationAttributeModel>> PrepareProductSpecificationAttributeModelAsync(Product product, SpecificationAttributeGroup group)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(
                    product.Id, specificationAttributeGroupId: group?.Id, showOnProductPage: true);

            var result = new List<ProductSpecificationAttributeModel>();

            foreach (var psa in productSpecificationAttributes)
            {
                var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);

                var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                if (model == null)
                {
                    var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    model = new ProductSpecificationAttributeModel
                    {
                        Id = attribute.Id,
                        Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name)
                    };
                    result.Add(model);
                }

                var value = new ProductSpecificationAttributeValueModel
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    ColorSquaresRgb = option.ColorSquaresRgb,
                    ValueRaw = psa.AttributeType switch
                    {
                        SpecificationAttributeType.Option => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(option, x => x.Name)),
                        SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                        SpecificationAttributeType.CustomHtmlText => await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue),
                        SpecificationAttributeType.Hyperlink => $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>",
                        _ => null
                    }
                };

                model.Values.Add(value);
            }

            return result;
        }

        [NonAction]
        public virtual async Task<ProductSpecificationModel> PrepareProductSpecificationModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductSpecificationModel();

            // Add non-grouped attributes first
            model.Groups.Add(new ProductSpecificationAttributeGroupModel
            {
                Attributes = await PrepareProductSpecificationAttributeModelAsync(product, null)
            });

            // Add grouped attributes
            var groups = await _specificationAttributeService.GetProductSpecificationAttributeGroupsAsync(product.Id);
            foreach (var group in groups)
            {
                model.Groups.Add(new ProductSpecificationAttributeGroupModel
                {
                    Id = group.Id,
                    Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                    Attributes = await PrepareProductSpecificationAttributeModelAsync(product, group)
                });
            }

            return model;
        }

        [NonAction]
        protected virtual async Task<IEnumerable<ProductOverviewApiModel>> PrepareApiProductOverviewModels(IEnumerable<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewApiModel>();
            foreach (var product in products)
            {
                var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                var categoryName = "";
                var categories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
                if (categories.Any())
                {
                    foreach (var category in categories)
                    {
                        var cat = await _categoryService.GetCategoryByIdAsync(category.CategoryId);
                        categoryName += cat.Name + ",";
                    }
                }
                var productReviews = await _productService.GetAllProductReviewsAsync(productId: product.Id, approved: true, storeId: _storeContext.GetCurrentStore().Id);
                var picsById = await _pictureService.GetPicturesByProductIdAsync(product.Id);
                var model = new ProductOverviewApiModel
                {
                    Id = product.Id,
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                    CategoryName = categoryName.TrimEnd(','),
                    Average = product.Average,
                    Minimum = product.Minimum,
                    Expensive = product.Expensive,
                    Price = await _priceFormatter.FormatPriceAsync(product.Price),
                    RatingSum = productReviews.Sum(pr => pr.Rating),
                    TotalReviews = productReviews.Count,
                    ImageUrl = await _pictureService.GetPictureUrlAsync(picsById.Any() ? picsById.FirstOrDefault().Id : 0, showDefaultPicture: true),
                    RibbonEnable = product.RibbonEnable,
                    RibbonText = product.RibbonText,
                    VendorLogoPictureUrl = await _pictureService.GetPictureUrlAsync(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
                };

                models.Add(model);
                model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
            }

            return models;
        }

        [NonAction]
        protected virtual async Task<CustomerProductReviewsModel> PrepareCustomerProductReviewsModel(int? page, int customerId)
        {
            var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
            var pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var list = await _productService.GetAllProductReviewsAsync(customerId: customerId,
                approved: null,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var productReviews = new List<CustomerProductReviewModel>();

            foreach (var review in list)
            {
                var product = await _productService.GetProductByIdAsync(review.ProductId);
                var dateTime = await _dateTimeHelper.ConvertToUserTimeAsync(product.CreatedOnUtc, DateTimeKind.Utc);
                var productReviewModel = new CustomerProductReviewModel
                {
                    Title = review.Title,
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, p => p.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = dateTime.ToString("g")
                };

                if (_catalogSettings.ProductReviewsMustBeApproved)
                {
                    productReviewModel.ApprovalStatus = review.IsApproved
                        ? await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Approved")
                        : await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Pending");
                }
                productReviews.Add(productReviewModel);
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { pageNumber = pageIndex }
            };

            var model = new CustomerProductReviewsModel
            {
                ProductReviews = productReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        #endregion

        #region Category

        [HttpGet("category-list")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _catalogModelFactory.PrepareCategorySimpleModelsAsync();
            if (categories.Count > 0)
            {
                var result = from cat in categories
                             select new
                             {
                                 id = cat.Id,
                                 name = cat.Name,
                                 numberOfProducts = cat.NumberOfProducts,
                                 numberOfSubCategories = cat.SubCategories.Count,
                                 seName = cat.SeName,
                                 pictureUrl = cat.PictureUrl
                             };
                return Ok(result);
            }
            return Ok(new { message = "No Category Found" });
        }

        #endregion

        #region Product

        [HttpGet("product-search-term-autocomplete")]
        public virtual async Task<IActionResult> SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var products = await _productService.SearchProductsAsync(
                storeId: _storeContext.GetCurrentStore().Id,
                keywords: term,
                languageId: _workContext.GetWorkingLanguageAsync().Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var result = (from p in products
                          select new
                          {
                              label = p.Name,
                          })
                .ToList();
            return Json(result);
        }

        [HttpGet("product-search")]
        public async Task<IActionResult> SearchProducts(SearchProductByFilters searchModel)
        {
            var products = await _productService.SearchProductsAsync(keywords: searchModel.Keyword);
            if (!products.Any())
                return Ok(new { success = true, message = "No Product Found" });

            //model
            var model = await PrepareApiProductOverviewModels(products);
            return Ok(model);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id, int updatecartitemid = 0)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.Deleted)
                return Ok(new { success = false, message = "No product found" });

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
            if (notAvailable && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return Ok(new { success = false, message = "No product found" });

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: _storeContext.GetCurrentStore().Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
            }

            //save as recently viewed
            await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = await _productModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);
            return Ok(model);
        }

        [HttpGet("product-list/category/{categoryId}")]
        public async Task<IActionResult> GetProductList(int categoryId, CatalogProductsCommand command, string brandFilter = "")
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null || category.Deleted)
                return Ok(new { success = false, message = "No category found." });

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);
            return Ok(model);
        }

        [HttpGet("addtocart/productId/{productId}/quantity/{quantity}")]
        public virtual async Task<IActionResult> AddProductToCart(int productId = 0, int quantity = 0)
        {
            if (quantity <= 0)
                return Ok(new { success = false, message = "Quantity should be > 0" });

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Ok(new { success = false, message = "invalid customer" });

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return Ok(new { success = false, message = "No product found" });

            var cartType = (ShoppingCartType)1;
            if (product.OrderMinimumQuantity > quantity)
                return NotFound("Quantity should be > 0");

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), cartType, _storeContext.GetCurrentStore().Id);
            var shoppingCartItem = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = await _shoppingCartService
                .GetShoppingCartItemWarningsAsync(await _workContext.GetCurrentCustomerAsync(), cartType,
                product, _storeContext.GetCurrentStore().Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                if (addToCartWarnings.Contains("The maximum number of distinct products allowed in the cart is 10."))
                    return Ok("The maximum number of distinct products allowed in the cart is 10.");

                //cannot be added to the cart
                //let's display standard warnings
                return Ok(string.Join(" , ", addToCartWarnings.ToArray().ToString()));
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: await _workContext.GetCurrentCustomerAsync(),
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.GetCurrentStore().Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Ok(string.Join(" , ", addToCartWarnings.ToArray().ToString()));
            }

            return Ok(await _localizationService.GetResourceAsync("Product.Added.Successfully.To.Cart"));
        }

        #endregion

        #region Product Review

        [HttpGet("get-product-reviews/product/{productId}")]
        public virtual async Task<IActionResult> ProductReviews(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var curCus = await _workContext.GetCurrentCustomerAsync();
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return Ok(new { success = false, message = "Product Not Found" });

            var model = new ProductReviewsModel();
            model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);
            //only registered users can leave reviews
            if (await _customerService.IsGuestAsync(curCus) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews") });

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = await _orderService.SearchOrdersAsync(customerId: curCus.Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1);
                if (!hasCompletedOrders.Any())
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing") });
            }
            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return Ok(new { success = true, data = model });
        }

        [HttpPost("add-product-reviews")]
        public virtual async Task<IActionResult> ProductReviewsAdd([FromBody] AddProductReviewApiModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.Id);
            var curCus = await _workContext.GetCurrentCustomerAsync();
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return Ok(new { success = false, message = "Product Not Found" });

            if (await _customerService.IsGuestAsync(curCus) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews") });

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = await _orderService.SearchOrdersAsync(customerId: curCus.Id,
                    productId: model.Id,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1);
                if (!hasCompletedOrders.Any())
                    return Ok(new { success = false, message = _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing") });
            }

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = curCus.Id,
                    Title = model.Title,
                    ReviewText = model.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.GetCurrentStore().Id,
                };

                await _productService.InsertProductReviewAsync(productReview);
                await _productService.UpdateProductAsync(product);

                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    await _workflowMessageService.SendProductReviewNotificationMessageAsync(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
               await _customerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
                    string.Format(await  _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));

                if (!isApproved)
                    return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving") });
                else
                    return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded") });
            }

            return Ok(new { success = false });
        }

        [HttpGet("get-product-reviews")]
        public virtual async Task<IActionResult> CustomerProductReviews()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
                return Ok(new { success = false, message = "Customer Not Found" });

            var model = PrepareCustomerProductReviewsModel(0, customer.Id);
            return Ok(new { success = true, data = model });
        }

        #endregion

        #region Topic

        [HttpGet("get-topic-by-systemname")]
        public virtual async Task<IActionResult> GetTopicBySytemName(string systemName)
        {
            var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
            return Ok(model);
        }


        #endregion

        #region Setting

        [HttpGet("get-schedule-dates")]
        public async Task<IActionResult> GetScheduleDates()
        {
            string[] dates = null;
            var orderscheduleDate1 = await _localizationService.GetLocaleStringResourceByNameAsync("orderschedule.Date ", 1, false);
            if (orderscheduleDate1 != null)
                dates = orderscheduleDate1.ResourceValue.Split(',');

            return Ok(new { success = true, dates });
        }

        #endregion

        #region properties

        public partial class SearchProductByFilters
        {
            //Custom Fields
            public string Keyword { get; set; }
        }
        public partial class ProductReviewsApiModel : BaseEntity
        {
            public ProductReviewsApiModel()
            {
                Items = new List<ProductReviewsApiModel>();
            }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string Title { get; set; }
            public string ReviewText { get; set; }
            public int Rating { get; set; }
            public IList<ProductReviewsApiModel> Items { get; set; }
        }
        public partial class AddProductReviewApiModel : BaseEntity
        {
            public string Title { get; set; }
            public string ReviewText { get; set; }
            public int Rating { get; set; }
            public bool DisplayCaptcha { get; set; }
            public bool CanCurrentCustomerLeaveReview { get; set; }
            public bool SuccessfullyAdded { get; set; }
            public string Result { get; set; }
        }

        #endregion
    }
}
