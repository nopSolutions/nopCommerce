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
using Nop.Services.Configuration;
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
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Api.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly IOrderReportService _orderReportService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductTagService _productTagService;
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
        private readonly IWebHelper _webHelper;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ISettingService _settingService;
        private readonly IProductAttributeService _productAttributeService;

        #endregion

        #region Ctor

        public CatalogApiController(
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            IWorkflowMessageService workflowMessageService,
            IOrderService orderService,
            IOrderReportService orderReportService,
            IPriceFormatter priceFormatter,
            ISpecificationAttributeService specificationAttributeService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            ICatalogModelFactory catalogModelService,
            IProductModelFactory productModelFactory,
            ITopicModelFactory topicModelFactory,
            ICategoryService categoryService,
            IProductService productService,
            IProductTagService productTagService,
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
            IEventPublisher eventPublisher,
            IVendorService vendorService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IStaticCacheManager staticCacheManager,
            ISettingService settingService,
            IProductAttributeService productAttributeService)
        {
            _shoppingCartSettings = shoppingCartSettings;
            _localizationSettings = localizationSettings;
            _workflowMessageService = workflowMessageService;
            _orderReportService = orderReportService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _specificationAttributeService = specificationAttributeService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _categoryService = categoryService;
            _catalogModelFactory = catalogModelService;
            _topicModelFactory = topicModelFactory;
            _productService = productService;
            _productTagService = productTagService;
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
            _vendorService = vendorService;
            _eventPublisher = eventPublisher;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _staticCacheManager = staticCacheManager;
            _settingService = settingService;
            _productAttributeService = productAttributeService;
        }

        #endregion

        #region Utility

        [NonAction]
        protected virtual async Task<ProductSpecificationApiModel> PrepareProductSpecificationAttributeModelAsync(Product product)
        {
            var result = new ProductSpecificationApiModel();
            if (product == null)
            {
                var allProductSpecifications = new ProductSpecificationApiModel();
                var specificationCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.AllProductSpecificationsModelKey, product, await _storeContext.GetCurrentStoreAsync());

                allProductSpecifications = await _staticCacheManager.GetAsync(specificationCacheKey, async () =>
                {
                    var productAllSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync();
                    foreach (var psa in productAllSpecificationAttributes)
                    {
                        var singleOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);
                        var checkModel = result.ProductSpecificationAttribute.FirstOrDefault(model => model.Id == singleOption.SpecificationAttributeId || model.Name == singleOption.Name);
                        if (checkModel == null)
                        {
                            var model1 = new ProductSpecificationAttributeApiModel();
                            var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(singleOption.SpecificationAttributeId);
                            model1.Id = attribute.Id;
                            model1.Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name);
                            var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(attribute.Id);
                            foreach (var option in options)
                            {
                                model1.Values.Add(new ProductSpecificationAttributeValueApiModel
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
                                });
                            }
                            result.ProductSpecificationAttribute.Add(model1);
                        }
                    }
                    var allVendors = await _vendorService.GetAllVendorsAsync();
                    result.ProductVendors = allVendors.Select(x => new VendorBriefInfoModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SeName = _urlRecordService.GetSeNameAsync(x).GetAwaiter().GetResult()
                    }).ToList();
                    return result;
                });

                return allProductSpecifications;
            }

            var productSpecifications = new ProductSpecificationApiModel();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductSpecificationsModelKey, product, await _storeContext.GetCurrentStoreAsync());

            productSpecifications = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(
                product.Id, showOnProductPage: true);

                foreach (var psa in productSpecificationAttributes)
                {
                    var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);
                    var checkModel = result.ProductSpecificationAttribute.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                    var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    var attributeName = await _localizationService.GetLocalizedAsync(attribute, x => x.Name);
                    if (checkModel == null)
                    {
                        var model1 = new ProductSpecificationAttributeApiModel();
                        model1.Id = attribute.Id;
                        model1.Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name);
                        model1.Values.Add(new ProductSpecificationAttributeValueApiModel
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
                        });
                        result.ProductSpecificationAttribute.Add(model1);
                    }
                    else if (result.ProductSpecificationAttribute.Select(x => x.Name == attributeName).Any())
                    {
                        var addAttributeModel = result.ProductSpecificationAttribute.Where(x => x.Name == attributeName).FirstOrDefault();
                        addAttributeModel.Values.Add(new ProductSpecificationAttributeValueApiModel
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
                        });
                    }
                }
                var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                if (vendor != null)
                {
                    result.ProductVendors.Add(new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.Name,
                        SeName = _urlRecordService.GetSeNameAsync(vendor).GetAwaiter().GetResult()
                    });
                }
                return result;
            });
            return productSpecifications;
        }

        [NonAction]
        protected virtual async Task<IEnumerable<ProductOverviewApiModel>> PrepareApiProductOverviewModels(IEnumerable<Product> products, Dictionary<int, List<KeyValuePair<ProductAttributeValue, ProductAttributeMapping>>> attributeValuesDictionary)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewApiModel>();

            var allCategories =
                (await _catalogModelFactory.PrepareCategorySimpleModelsAsync()).ToImmutableDictionary(sc => sc.Id);

            foreach (var product in products)
            {
                List<KeyValuePair<ProductAttributeValue,ProductAttributeMapping>> attributeValues = null;
                attributeValuesDictionary.TryGetValue(product.Id, out attributeValues);
                var specifications = await PrepareProductSpecificationAttributeModelAsync(product);
                if (specifications != null && product.Published)
                {
                    var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                    string categoryNameJoined;
                    {
                        var categoryNames = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id))
                            .Select(pc =>
                                allCategories.TryGetValue(pc.CategoryId, out var category) ? category.Name : string.Empty);
                        categoryNameJoined = string.Join(',', categoryNames.Where(c => !string.IsNullOrEmpty(c)));
                    }

                    var popularityByVendor = (await _staticCacheManager.GetAsync(
                        _staticCacheManager.PrepareKeyForDefaultCache(
                            NopModelCacheDefaults.HomepageBestsellersIdsKey, product.VendorId),
                        async () => (await _orderReportService.BestSellersReportAsync(
                            showHidden: true,
                            vendorId: product.VendorId)).ToImmutableDictionary(k => k.ProductId)));

                    var productReviewOverviewModel =
                        await _productModelFactory.PrepareProductReviewOverviewModelAsync(product);
                    var productPictureModel =
                        await _productModelFactory.PrepareProductOverviewPictureModelAsync(product);

                    var model = new ProductOverviewApiModel
                    {
                        Id = product.Id,
                        Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                        ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                        FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                        SeName = await _urlRecordService.GetSeNameAsync(product),
                        CategoryName = categoryNameJoined,
                        Price = await _priceFormatter.FormatPriceAsync(product.Price),
                        PriceValue = product.Price,
                        RatingSum = productReviewOverviewModel.RatingSum,
                        TotalReviews = productReviewOverviewModel.TotalReviews,
                        PopularityCount = popularityByVendor.TryGetValue(product.Id, out var productPopularity) ?
                            productPopularity.TotalQuantity : 0,
                        ImageUrl = productPictureModel.ImageUrl,
                        RibbonEnable = product.RibbonEnable,
                        RibbonText = product.RibbonText,
                        VendorLogoPictureUrl = await _pictureService.GetPictureUrlAsync(vendor?.PictureId ?? 0, showDefaultPicture: true),
                        ProductSpecificationModel = specifications,
                        ProductAttributeValues = attributeValues
                    };

                    models.Add(model);

                }
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
            return Ok(new { message = await _localizationService.GetResourceAsync("Category.Not.Found") });
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

        [HttpGet("product-specification-attributes-and-productvendors")]
        public async Task<IActionResult> AllProductSpecificationAndProductVendors()
        {
            //model
            var model = await PrepareProductSpecificationAttributeModelAsync(null);
            return Ok(model);
        }

        [HttpGet("product-attributes")]
        public async Task<IActionResult> AllProductAttributes()
        {
            //model
            var model = await PrepareProductSpecificationAttributeModelAsync(null);
            return Ok(model);
        }

        [HttpGet("product-search")]
        public async Task<IActionResult> SearchProducts(SearchProductByFilters searchModel)
        {
            var categoryIds = (await _categoryService.GetAllCategoriesAsync()).Select(c => c.Id).Where(id => id != 0).ToList();
            var productAttributes = await _productAttributeService.GetAllGroupedProductAttributeValuesAsync();
            var products = await _productService.SearchProductsAsync(keywords: searchModel.Keyword, showHidden: true, categoryIds: categoryIds);
            if (!products.Any())
                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Product.Not.Found") });

            //model
            var model = await PrepareApiProductOverviewModels(products, productAttributes);
            return Ok(model);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id, int updatecartitemid = 0)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.Deleted)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Product.Not.Found") });

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
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Product.Not.Found") });

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
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Category.Not.Found") });

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
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Product.Not.Found") });

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
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Product.Not.Found") });

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
                     string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

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
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

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
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(storeId);

            string[] dates = null;
            var scheduleDateSetting = await _settingService.SettingExistsAsync(orderSettings, x => x.ScheduleDate, storeId);
            if (scheduleDateSetting)
            {
                dates = (await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.StoreScheduleDate, await _storeContext.GetCurrentStoreAsync()),
                async () =>
                {
                    var scheduleDate = await _settingService.GetSettingAsync("ordersettings.scheduledate", (await _storeContext.GetCurrentStoreAsync()).Id, true);
                    return !string.IsNullOrWhiteSpace(scheduleDate.Value) ? scheduleDate.Value.Split(',') : null;
                }));

                return Ok(new { success = true, dates = dates });
            }

            return Ok(new { success = true, dates, message = await _localizationService.GetResourceAsync("Setting.Not.Found") });
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
