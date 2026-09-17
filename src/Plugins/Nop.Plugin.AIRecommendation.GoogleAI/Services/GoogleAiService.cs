using System.Text;
using Google.Cloud.Retail.V2;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc.Routing;
using ILogger = Nop.Services.Logging.ILogger;
using Product = Nop.Core.Domain.Catalog.Product;

namespace Nop.Plugin.AIRecommendation.GoogleAI.Services;

/// <summary>
/// Represents the Google AI service
/// </summary>
public class GoogleAiService
{
    #region Fields

    //for search request, the maximum page size is 120.
    //see https://cloud.google.com/retail/docs/reference/rpc/google.cloud.retail.v2#google.cloud.retail.v2.SearchRequest
    private const int SEARCH_REQUEST_COUNT = 120;
    private const string CATEGORY_SEPARATOR = " > ";
    private const int PAGE_SIZE = 500;

    private string _primaryStoreCurrencyCode;

    private readonly CurrencySettings _currencySettings;
    private readonly GoogleAiSettings _googleAiSettings;
    private readonly ICategoryService _categoryService;
    private readonly ICurrencyService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILanguageService _languageService;
    private readonly ILogger _logger;
    private readonly IManufacturerService _manufacturerService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IProductTagService _productTagService;
    private readonly ISpecificationAttributeService _specificationAttributeService;
    private readonly IWebHelper _webHelper;
    private readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public GoogleAiService(CurrencySettings currencySettings,
        GoogleAiSettings googleAiSettings,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILogger logger,
        IManufacturerService manufacturerService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        ISpecificationAttributeService specificationAttributeService,
        IWebHelper webHelper,
        LocalizationSettings localizationSettings)
    {
        _currencySettings = currencySettings;
        _googleAiSettings = googleAiSettings;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _specificationAttributeService = specificationAttributeService;
        _webHelper = webHelper;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the primary store currency code
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the primary store currency code.
    /// </returns>
    private async Task<string> GetPrimaryStoreCurrencyCodeAsync()
    {
        if (!string.IsNullOrEmpty(_primaryStoreCurrencyCode))
            return _primaryStoreCurrencyCode;

        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

        _primaryStoreCurrencyCode = currency.CurrencyCode;

        return _primaryStoreCurrencyCode;
    }

    /// <summary>
    /// Gets the product categories for the specified product and formats them as breadcrumbs
    /// </summary>
    /// <param name="product">The product for which to get categories</param>
    /// <param name="allCategories">The list of all categories</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product categories formatted as breadcrumbs
    /// </returns>
    private async Task<IEnumerable<string>> GetProductCategoriesAsync(Product product, IList<Category> allCategories)
    {
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
        var ids = productCategories.Select(pc => pc.CategoryId).ToList();
        var categories = allCategories.Where(c => ids.Contains(c.Id)).ToList();

        //strongly recommended using the full category path for better search / recommendation quality.
        //to represent full path of category, use '>' sign to separate different hierarchies.
        //for example, if a shoes product belongs to both ["Shoes & Accessories" -> "Shoes"] and ["Sports & Fitness" -> "Athletic Clothing" -> "Shoes"],
        //it could be represented as: "Shoes & Accessories > Shoes", "Sports & Fitness > Athletic Clothing > Shoes"
        return await categories.SelectAwait(async category =>
            await _categoryService.GetFormattedBreadCrumbAsync(category, allCategories, CATEGORY_SEPARATOR)).ToListAsync();
    }

    /// <summary>
    /// Gets the product tags for the specified product
    /// </summary>
    /// <param name="product">The product for which to get tags</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags
    /// </returns>
    private async Task<IEnumerable<string>> GetProductTagsAsync(Product product)
    {
        var tags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        return tags.Select(tag => tag.Name);
    }

    /// <summary>
    /// Retrieves a list of manufacturer names associated with a given product
    /// </summary>
    /// <param name="product">The product for which to retrieve the vendor names</param>
    /// <param name="allManufacturers">A list of all available manufacturers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of strings representing the vendor names
    /// </returns>
    private async Task<IEnumerable<string>> GetProductManufacturersAsync(Product product, IList<Manufacturer> allManufacturers)
    {
        var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
        var ids = productManufacturers.Select(pc => pc.ManufacturerId).ToList();
        var manufacturers = allManufacturers.Where(m => ids.Contains(m.Id)).ToList();

        return manufacturers.Select(m => m.Name);
    }

    /// <summary>
    /// Retrieves an image for a given picture.
    /// </summary>
    /// <param name="picture">The picture for which to retrieve the image.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains an Image object representing the retrieved image.
    /// </returns>
    private async Task<Image> GetProductImageAsync(Core.Domain.Media.Picture picture)
    {
        var storeLocation = _webHelper.GetStoreLocation();
        var (url, _) = await _pictureService.GetPictureUrlAsync(picture, storeLocation: storeLocation);
        
        return new Image { Uri = url };
    }

    /// <summary>
    /// Converts a NopProduct to a GoogleProduct
    /// </summary>
    /// <param name="product">The product to convert</param>
    /// <param name="allCategories">A list of all available categories</param>
    /// <param name="allManufacturers">A list of all available manufacturers</param>
    /// <param name="currentLanguage">The current language. If null, the default admin language will be used</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a GoogleProduct object
    /// </returns>
    private async Task<Google.Cloud.Retail.V2.Product> ConvertProductToGoogleProductAsync(Product product, IList<Category> allCategories, IList<Manufacturer> allManufacturers, Language currentLanguage = null)
    {
        currentLanguage ??= await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);

        var id = string.IsNullOrEmpty(product.Sku) ? product.Id.ToString() : product.Sku;
        var parentId = string.Empty;

        if (product.ParentGroupedProductId > 0)
        {
            var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

            if (parentProduct != null)
                parentId = string.IsNullOrEmpty(parentProduct.Sku) ? parentProduct.Id.ToString() : parentProduct.Sku;
            else
                parentId = product.ParentGroupedProductId.ToString();
        }

        var dto = new Google.Cloud.Retail.V2.Product
        {
            Gtin = product.Gtin ?? string.Empty,
            Name = product.Name,
            Id = id,
            Type = product.ParentGroupedProductId > 0 ? Google.Cloud.Retail.V2.Product.Types.Type.Variant : Google.Cloud.Retail.V2.Product.Types.Type.Primary,
            PrimaryProductId = parentId,
            Title = product.ShortDescription ?? string.Empty,
            Description = product.FullDescription ?? string.Empty,
            LanguageCode = currentLanguage.LanguageCulture,
            PriceInfo = new PriceInfo
            {
                Price = (float)product.Price,
                CurrencyCode = await GetPrimaryStoreCurrencyCodeAsync()
            },
            Uri = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
            Rating = new Rating
            {
                AverageRating = product.ApprovedTotalReviews == 0 ? 0f : (float)product.ApprovedRatingSum / product.ApprovedTotalReviews,
                RatingCount = product.ApprovedTotalReviews
            },
            Availability = await getAvailability(),
        };

        dto.Categories.AddRange(await GetProductCategoriesAsync(product, allCategories));
        dto.Brands.AddRange(await GetProductManufacturersAsync(product, allManufacturers));
        dto.Attributes.MergeFrom(await GetSpecificationAttributesAsync(product.Id));
        dto.Tags.AddRange(await GetProductTagsAsync(product));

        var picture = (await _pictureService
            .GetPicturesByProductIdAsync(product.Id, 1)).DefaultIfEmpty(null).FirstOrDefault();

        dto.Images.Add(await GetProductImageAsync(picture));

        return dto;

        async Task<Google.Cloud.Retail.V2.Product.Types.Availability> getAvailability()
        {
            var status = Google.Cloud.Retail.V2.Product.Types.Availability.Backorder;

            if (!product.Published || product.Deleted)
                return status;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    var stockQuantity = await _productService.GetTotalStockQuantityAsync(product);

                    if (stockQuantity > 0 || product.BackorderMode == BackorderMode.AllowQtyBelow0)
                        status = Google.Cloud.Retail.V2.Product.Types.Availability.InStock;
                    else
                        status = Google.Cloud.Retail.V2.Product.Types.Availability.OutOfStock;

                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var combinations =
                        await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
                    return combinations.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                        ? Google.Cloud.Retail.V2.Product.Types.Availability.InStock
                        : Google.Cloud.Retail.V2.Product.Types.Availability.OutOfStock;

                case ManageInventoryMethod.DontManageStock:
                    status = Google.Cloud.Retail.V2.Product.Types.Availability.InStock;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return status;
        }
    }

    /// <summary>
    /// Converts a collection of NopProducts to a list of GoogleProduct based on their attribute combinations
    /// </summary>
    /// <param name="products">The products to convert</param>
    /// <param name="allCategories">A list of all available categories</param>
    /// <param name="allManufacturers">A list of all available manufacturers</param>
    /// <param name="currentLanguage">The current language. If null, the default admin language will be used</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of GoogleProduct objects
    /// </returns>
    private async Task<List<Google.Cloud.Retail.V2.Product>> GetAllGoogleProductByCombinationsAsync(IEnumerable<Product> products, IList<Category> allCategories, IList<Manufacturer> allManufacturers, Language currentLanguage = null)
    {
        var result = new List<Google.Cloud.Retail.V2.Product>();
        currentLanguage ??= await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);

        foreach (var product in products)
        {
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);

            if (!combinations.Any())
                continue;

            foreach (var combination in combinations)
            {
                var dto = await GetGoogleProductByCombinationAsync(allCategories, allManufacturers, currentLanguage, combination, product);

                result.Add(dto);
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a single NopProduct and its attribute combination to a GoogleProduct
    /// </summary>
    /// <param name="allCategories">A list of all available categories</param>
    /// <param name="allManufacturers">A list of all available manufacturers</param>
    /// <param name="currentLanguage">The current language. If null, the default admin language will be used</param>
    /// <param name="combination">The attribute combination of the product</param>
    /// <param name="product">The product to convert</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a GoogleProduct object
    /// </returns>
    private async Task<Google.Cloud.Retail.V2.Product> GetGoogleProductByCombinationAsync(IList<Category> allCategories, IList<Manufacturer> allManufacturers, Language currentLanguage,
        ProductAttributeCombination combination, Product product)
    {
        var productId = string.IsNullOrEmpty(product.Sku) ? product.Id.ToString() : product.Sku;
        Google.Cloud.Retail.V2.Product.Types.Availability status;

        if (combination.StockQuantity > 0 || combination.AllowOutOfStockOrders)
            status = Google.Cloud.Retail.V2.Product.Types.Availability.InStock;
        else
            status = Google.Cloud.Retail.V2.Product.Types.Availability.OutOfStock;

        var dto = new Google.Cloud.Retail.V2.Product
        {
            Gtin = combination.Gtin,
            Name = product.Name,
            Id = combination.Sku,
            Type = Google.Cloud.Retail.V2.Product.Types.Type.Variant,
            PrimaryProductId = productId,
            Title = product.ShortDescription,
            Description = product.FullDescription,
            LanguageCode = currentLanguage.LanguageCulture,
            PriceInfo = new PriceInfo
            {
                Price = (float?)combination.OverriddenPrice ?? (float)product.Price,
                CurrencyCode = await GetPrimaryStoreCurrencyCodeAsync()
            },
            Uri = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
            Rating = new Rating
            {
                AverageRating = product.ApprovedTotalReviews == 0 ? 0f : (float)product.ApprovedRatingSum / product.ApprovedTotalReviews,
                RatingCount = product.ApprovedTotalReviews
            },
            Availability = status,
        };

        dto.Categories.AddRange(await GetProductCategoriesAsync(product, allCategories));
        dto.Brands.AddRange(await GetProductManufacturersAsync(product, allManufacturers));
        dto.Attributes.MergeFrom(await GetSpecificationAttributesAsync(product.Id));
        dto.Tags.AddRange(await GetProductTagsAsync(product));

        var pictureId =
            (await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id))
            .FirstOrDefault()?.PictureId;

        if (pictureId != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync(pictureId.Value);
            dto.Images.Add(await GetProductImageAsync(picture));
        }

        return dto;
    }

    /// <summary>
    /// Get import products inline request.
    /// </summary>
    /// <param name="productsToImport">The list of products to import.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import products request
    /// </returns>
    private async Task<ImportProductsRequest> GetImportProductsRequestAsync(IEnumerable<Google.Cloud.Retail.V2.Product> productsToImport)
    {
        // To check error handling paste the invalid catalog name here:
        // catalogId = "invalid_catalog_name";
        var defaultBranch = new BranchName(_googleAiSettings.ProjectId, _googleAiSettings.LocationId, _googleAiSettings.CatalogId, _googleAiSettings.BranchId);

        var importRequest = new ImportProductsRequest
        {
            ParentAsBranchName = defaultBranch,
            InputConfig = new ProductInputConfig
            {
                ProductInlineSource = new ProductInlineSource
                {
                    Products = { productsToImport }
                }
            }
        };

        await _logger.InformationAsync("GoogleAI. Sync products with Google Cloud Retail");
        await LogRequestAsync(importRequest);

        return importRequest;
    }

    /// <summary>
    /// Get create product request.
    /// </summary>
    /// <param name="productToCreate">The actual product object to create</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the create product request
    /// </returns>
    private async Task<CreateProductRequest> GetCreateProductRequestAsync(Product productToCreate)
    {
        var defaultBranch = new BranchName(_googleAiSettings.ProjectId, _googleAiSettings.LocationId, _googleAiSettings.CatalogId, _googleAiSettings.BranchId);

        var allCategories = await _categoryService.GetAllCategoriesAsync();
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
        var currentLanguage = await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);
        var product = await ConvertProductToGoogleProductAsync(productToCreate, allCategories, allManufacturers, currentLanguage);

        var createProductRequest = new CreateProductRequest
        {
            Product = product,
            ProductId = product.Id,
            ParentAsBranchName = defaultBranch
        };

        await LogRequestAsync(createProductRequest);

        return createProductRequest;
    }

    /// <summary>
    /// Get create product request.
    /// </summary>
    /// <param name="productAttributeCombination">The product attribute combination</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the create product request
    /// </returns>
    private async Task<CreateProductRequest> GetCreateProductRequestAsync(ProductAttributeCombination productAttributeCombination)
    {
        var defaultBranch = new BranchName(_googleAiSettings.ProjectId, _googleAiSettings.LocationId, _googleAiSettings.CatalogId, _googleAiSettings.BranchId);

        var allCategories = await _categoryService.GetAllCategoriesAsync();
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
        var currentLanguage = await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);
        var product = await GetGoogleProductByCombinationAsync(allCategories, allManufacturers, currentLanguage, productAttributeCombination, await _productService.GetProductByIdAsync(productAttributeCombination.ProductId));

        var createProductRequest = new CreateProductRequest
        {
            Product = product,
            ProductId = product.Id,
            ParentAsBranchName = defaultBranch
        };

        await LogRequestAsync(createProductRequest);

        return createProductRequest;
    }

    /// <summary>
    /// Get delete product request.
    /// </summary>
    /// <param name="product">The product to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delete product request
    /// </returns>
    private async Task<DeleteProductRequest> GetDeleteProductRequestAsync(Product product)
    {
        var productName = new ProductName(_googleAiSettings.ProjectId, _googleAiSettings.LocationId, _googleAiSettings.CatalogId, _googleAiSettings.BranchId, string.IsNullOrEmpty(product.Sku) ? product.Id.ToString() : product.Sku).ToString();
        var deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        await LogRequestAsync(deleteProductRequest);

        return deleteProductRequest;
    }

    /// <summary>
    /// Get the update product request.
    /// </summary>
    /// <param name="productAttributeCombination">The product to update object.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the update product request
    /// </returns>
    private async Task<UpdateProductRequest> GetUpdateProductRequestAsync(ProductAttributeCombination productAttributeCombination)
    {
        var allCategories = await _categoryService.GetAllCategoriesAsync();
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
        var currentLanguage = await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);
        var product = await GetGoogleProductByCombinationAsync(allCategories, allManufacturers, currentLanguage, productAttributeCombination, await _productService.GetProductByIdAsync(productAttributeCombination.ProductId));

        var updateProductRequest = new UpdateProductRequest
        {
            Product = product,
            AllowMissing = true
        };

        await LogRequestAsync(updateProductRequest);

        return updateProductRequest;
    }

    /// <summary>
    /// Get the update product request.
    /// </summary>
    /// <param name="productToUpdate">The product to update object</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the update product request
    /// </returns>
    private async Task<UpdateProductRequest> GetUpdateProductRequestAsync(Product productToUpdate)
    {
        var allCategories = await _categoryService.GetAllCategoriesAsync();
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
        var currentLanguage = await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);
        var product = await ConvertProductToGoogleProductAsync(productToUpdate, allCategories, allManufacturers, currentLanguage);

        var updateProductRequest = new UpdateProductRequest
        {
            Product = product,
            AllowMissing = true
        };

        await LogRequestAsync(updateProductRequest);

        return updateProductRequest;
    }

    /// <summary>
    /// Retrieves a dictionary of custom attributes for a product based on its specification attributes
    /// </summary>
    /// <param name="productId">The ID of the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a dictionary of custom attributes
    /// </returns>
    private async Task<IDictionary<string, CustomAttribute>> GetSpecificationAttributesAsync(int productId)
    {
        var result = new Dictionary<string, CustomAttribute>();
        var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(productId);
        var options = await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(
            productSpecificationAttributes.Where(psa => psa.AttributeType == SpecificationAttributeType.Option).Select(psa => psa.SpecificationAttributeOptionId).ToArray());

        foreach (var spec in options.GroupBy(o => o.SpecificationAttributeId))
        {
            var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(spec.Key);

            var value = new CustomAttribute();
            value.Text.AddRange(spec.Select(o => o.Name));

            result.Add(attribute.Name, value);
        }

        return result;
    }

    /// <summary>
    /// Builds a filter string based on provided parameters.
    /// </summary>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="filteredSpecOptions">Filtered specification attribute options</param>
    /// <returns></returns>
    private async Task<StringBuilder> CreateFilterAsync(IList<int> categoryIds, IList<int> manufacturerIds, int productTagId, IList<SpecificationAttributeOption> filteredSpecOptions)
    {
        //see https://docs.cloud.google.com/retail/docs/filter-and-order to learn more about the filter syntax
        var filter = new StringBuilder();

        if (categoryIds?.Any() ?? false)
        {
            var categories = await _categoryService.GetCategoriesByIdsAsync(categoryIds.ToArray());
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            filter.Append($"category:({string.Join(" OR ", categories.SelectAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c, allCategories, CATEGORY_SEPARATOR)))})");
        }

        if (manufacturerIds?.Any() ?? false)
        {
            if (filter.Length > 0)
                filter.Append(" AND ");

            var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(manufacturerIds.ToArray());
            filter.Append($"brands:({string.Join(" OR ", manufacturers.Select(m => m.Name))})");
        }

        if (productTagId != 0)
        {
            if (filter.Length > 0)
                filter.Append(" AND ");

            var tag = await _productTagService.GetProductTagByIdAsync(productTagId);

            filter.Append($"productTag:({tag.Name})");
        }

        if (filteredSpecOptions?.Any() ?? false)
        {
            if (filter.Length > 0)
                filter.Append(" AND ");

            var attributes =
                await _specificationAttributeService.GetSpecificationAttributeByIdsAsync(filteredSpecOptions
                    .Select(o => o.Id).ToArray());

            var attributeDictionary = attributes.DistinctBy(a => a.Id).ToDictionary(a => a.Id, a => a);

            var attributeFilters = filteredSpecOptions.Select(o =>
                $"attributes.{attributeDictionary[o.SpecificationAttributeId].Name}:{o.Name}");

            filter.Append($"({string.Join(" AND ", attributeFilters)})");
        }

        return filter;
    }

    /// <summary>
    /// Logs the request to the logger if logging is enabled in the settings
    /// </summary>
    /// <param name="request">The request to log</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task LogRequestAsync(object request)
    {
        if (!_googleAiSettings.LogRequests)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("GoogleAI. Request:");
        sb.AppendLine(request.ToString());
        await _logger.InformationAsync(sb.ToString());
    }

    /// <summary>
    /// Get search request
    /// </summary>
    /// <param name="keywords">Keywords</param>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="filteredSpecOptions">Filtered specification options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search request
    /// </returns>
    private async Task<SearchRequest> GetSearchRequestAsync(string keywords,
        IList<int> categoryIds,
        IList<int> manufacturerIds,
        int productTagId,
        IList<SpecificationAttributeOption> filteredSpecOptions)
    {
        var defaultSearchPlacement =
            $"projects/{_googleAiSettings.ProjectId}/locations/{_googleAiSettings.LocationId}/catalogs/{_googleAiSettings.CatalogId}/placements/default_search";

        //try to get client_id from cookie (if available)
        _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(GoogleAiDefaults.ClientIdCookiesName, out var clientId);

        var searchRequest = new SearchRequest
        {
            Placement = defaultSearchPlacement,
            Query = keywords,
            VisitorId = clientId,
            PageSize = SEARCH_REQUEST_COUNT
        };

        var filter = await CreateFilterAsync(categoryIds, manufacturerIds, productTagId, filteredSpecOptions);

        if (filter.Length > 0)
            searchRequest.Filter = filter.ToString();

        await LogRequestAsync(searchRequest);

        return searchRequest;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Call the Retail API to import products.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<(long successCount, long failureCount)> SyncProductsAsync()
    {
        var page = 0;

        var allCategories = await _categoryService.GetAllCategoriesAsync();
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
        var currentLanguage = await _languageService.GetLanguageByIdAsync(_localizationSettings.DefaultAdminLanguageId);

        var successCount = 0L;
        var failureCount = 0L;

        while (true)
        {
            var nopProducts = await _productService.SearchProductsAsync(page, PAGE_SIZE);
            var products = await nopProducts.SelectAwait(async p => await ConvertProductToGoogleProductAsync(p, allCategories, allManufacturers, currentLanguage)).ToListAsync();
            products.AddRange(await GetAllGoogleProductByCombinationsAsync(nopProducts, allCategories, allManufacturers, currentLanguage));

            if (!products.Any())
                break;

            var importRequest = await GetImportProductsRequestAsync(products);
            var client = await ProductServiceClient.CreateAsync();
            var importResponse = await client.ImportProductsAsync(importRequest);
            var importResult = await importResponse.PollUntilCompletedAsync();

            if (importResult.IsFaulted && importResult.Exception != null)
                throw importResult.Exception;

            successCount += importResult.Metadata.SuccessCount;
            failureCount += importResult.Metadata.FailureCount;

            if (importResult.IsFaulted)
                break;
        }

        var sb = new StringBuilder();
        sb.AppendLine("GoogleAI. Import products operation is done");
        sb.AppendLine("Number of successfully imported products: " + successCount);
        sb.AppendLine("Number of failures during the importing: " + failureCount);
        await _logger.InformationAsync(sb.ToString());

        return (successCount, failureCount);
    }

    /// <summary>
    /// Create product in the Google AI platform
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task CreateProductAsync(Product product)
    {
        var createProductRequest = await GetCreateProductRequestAsync(product);

        var client = await ProductServiceClient.CreateAsync();
        _ = await client.CreateProductAsync(createProductRequest);
    }

    /// <summary>
    /// Create product in the Google AI platform
    /// </summary>
    /// <param name="productAttributeCombination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task CreateProductAsync(ProductAttributeCombination productAttributeCombination)
    {
        var createProductRequest = await GetCreateProductRequestAsync(productAttributeCombination);

        var client = await ProductServiceClient.CreateAsync();
        _ = await client.CreateProductAsync(createProductRequest);
    }

    /// <summary>
    /// Delete product in the Google AI platform
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteProductAsync(Product product)
    {
        var deleteProductRequest = await GetDeleteProductRequestAsync(product);
        var client = await ProductServiceClient.CreateAsync();

        await client.DeleteProductAsync(deleteProductRequest);
    }

    /// <summary>
    /// Delete product in the Google AI platform
    /// </summary>
    /// <param name="productAttributeCombination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteProductAsync(ProductAttributeCombination productAttributeCombination)
    {
        var deleteProductRequest = await GetDeleteProductRequestAsync(await _productService.GetProductByIdAsync(productAttributeCombination.ProductId));
        var client = await ProductServiceClient.CreateAsync();

        await client.DeleteProductAsync(deleteProductRequest);
    }

    /// <summary>
    /// Update product in the Google AI platform
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateProductAsync(Product product)
    {
        var updateProductRequest = await GetUpdateProductRequestAsync(product);
        var client = await ProductServiceClient.CreateAsync();
        _ = await client.UpdateProductAsync(updateProductRequest);
    }

    /// <summary>
    /// Update product in the Google AI platform
    /// </summary>
    /// <param name="productAttributeCombination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateProductAsync(ProductAttributeCombination productAttributeCombination)
    {
        var updateProductRequest = await GetUpdateProductRequestAsync(productAttributeCombination);
        var client = await ProductServiceClient.CreateAsync();
        _ = await client.UpdateProductAsync(updateProductRequest);
    }

    /// <summary>
    /// Get products identifiers by the specified arguments
    /// </summary>
    /// <param name="keywords">Keywords</param>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="filteredSpecOptions">Filtered specification options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains product identifiers
    /// </returns>
    public async Task<List<int>> SearchProductsAsync(string keywords,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        int productTagId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null)
    {
        var client = await SearchServiceClient.CreateAsync();
        var searchRequest = await GetSearchRequestAsync(keywords, categoryIds, manufacturerIds, productTagId, filteredSpecOptions);

        try
        {
            var firstPage = client.Search(searchRequest).AsRawResponses().FirstOrDefault();

            var result = new List<int>();
            var skus = new List<string>();

            if (!(firstPage?.Any() ?? false) || firstPage.TotalSize == 0)
                return [];

            foreach (var item in firstPage)
            {
                if (int.TryParse(item.Id, out var productId))
                    result.Add(productId);
                else
                    skus.Add(item.Id);
            }

            if (!skus.Any())
                return result;

            var products = await _productService.GetProductsBySkuAsync(skus.ToArray());
            result.AddRange(products.Select(p => p.Id));

            return result;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Google AI Commerce Search failed. Error:", ex);
            throw;
        }
    }

    #endregion
}