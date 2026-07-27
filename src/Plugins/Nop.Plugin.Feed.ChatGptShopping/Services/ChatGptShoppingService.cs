using System.Globalization;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Feed.ChatGptShopping.Domain;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Feed.ChatGptShopping.Services;

/// <summary>
/// Represents the plugin service 
/// </summary>
public class ChatGptShoppingService
{
    #region Fields

    private readonly CurrencySettings _currencySettings;
    private readonly IAclService _aclService;
    private readonly ICategoryService _categoryService;
    private readonly ICurrencyService _currencyService;
    private readonly ICustomerService _customerService;
    private readonly IHtmlFormatter _htmlFormatter;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;
    private readonly IManufacturerService _manufacturerService;
    private readonly INopFileProvider _nopFileProvider;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IProductService _productService;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Manufacturer> _manufacturerRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly ISettingService _settingService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreService _storeService;
    private readonly ITaxService _taxService;
    private readonly IVideoService _videoService;

    #endregion

    #region Ctor

    public ChatGptShoppingService(CurrencySettings currencySettings,
        IAclService aclService,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        ILogger logger,
        IManufacturerService manufacturerService,
        INopFileProvider nopFileProvider,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IPriceCalculationService priceCalculationService,
        IProductService productService,
        IRepository<Category> categoryRepository,
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<Product> productRepository,
        ISettingService settingService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        ITaxService taxService,
        IVideoService videoService)
    {
        _currencySettings = currencySettings;
        _aclService = aclService;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _nopFileProvider = nopFileProvider;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _productService = productService;
        _categoryRepository = categoryRepository;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
        _settingService = settingService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _taxService = taxService;
        _videoService = videoService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Generate a feed for the passed store
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of notifications
    /// </returns>
    private async Task<IList<(NotifyType Type, string Message)>> GenerateFeedAsync(Store store)
    {
        ArgumentNullException.ThrowIfNull(store);

        var messages = new List<(NotifyType, string)>();
        try
        {
            var settings = await _settingService.LoadSettingAsync<ChatGptShoppingSettings>(store.Id);

            var customer = await _customerService.GetCustomerByIdAsync(settings.CustomerId)
                ?? await _customerService.GetOrCreateSearchEngineUserAsync();

            var currency = await _currencyService.GetCurrencyByIdAsync(settings.CurrencyId);
            if (currency == null || !currency.Published)
                currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            var pathToFile = string.Format(ChatGptShoppingDefaults.PathToFeedFile, store.Id);
            var localFilePath = _nopFileProvider.GetAbsolutePath(pathToFile);
            await using var fileStream = _nopFileProvider.GetOrCreateFile(localFilePath);
            await using var gzip = new GZipStream(fileStream, CompressionLevel.Optimal);
            await using var writer = new StreamWriter(gzip, new UTF8Encoding(false));

            //performance optimization, load all categories in one SQL request
            var allCategories = await GetAllCategoriesAsync(customer, store.Id);

            //performance optimization, load all manufacturers in one SQL request
            var allManufacturers = await GetAllManufacturersAsync(customer, store.Id);

            var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var pageIndex = 0;
            while (true)
            {
                var products = await SearchProductsAsync(customer, store.Id, pageIndex, ChatGptShoppingDefaults.PageSize);
                if (!products.Any())
                    break;

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = await _categoryService.GetProductCategoryIdsAsync(products.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = await _manufacturerService.GetProductManufacturerIdsAsync(products.Select(p => p.Id).ToArray());

                foreach (var product in products)
                {
                    try
                    {
                        var productCategories = !allProductsCategoryIds.TryGetValue(product.Id, out var productCategoriesValue)
                            ? Array.Empty<int>()
                            : productCategoriesValue;
                        var category = allCategories.FirstOrDefault(c => productCategories.Contains(c.Id));

                        var productManufacturers = !allProductsManufacturerIds.TryGetValue(product.Id, out var productManufacturersValue)
                            ? Array.Empty<int>()
                            : productManufacturersValue;
                        var manufacturer = allManufacturers.FirstOrDefault(m => productManufacturers.Contains(m.Id));

                        //prepare a product to sync
                        var dto = await BuildProductAsync(product, category, manufacturer, store, customer, currency, settings);
                        if (dto == null)
                            continue;

                        //and add it to the feed
                        var json = JsonConvert.SerializeObject(dto, jsonSerializerSettings);
                        await writer.WriteLineAsync(json);
                    }
                    catch (Exception ex)
                    {
                        await _logger.WarningAsync($"{ChatGptShoppingDefaults.SystemName} - Unable to export product {product.Id}.", ex);
                        messages.Add((NotifyType.Error, $"{ChatGptShoppingDefaults.SystemName} - Unable to export product {product.Id}: {ex.Message}"));
                    }
                }

                pageIndex++;
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"{ChatGptShoppingDefaults.SystemName} Error generating feed for the store {store.Name}.", ex);
            messages.Add((NotifyType.Error, $"{ChatGptShoppingDefaults.SystemName} Error generating feed for the store {store.Name}: {ex.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Prepare product details
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="category">Category</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="store">Store</param>
    /// <param name="customer">Customer</param>
    /// <param name="currency">Currency</param>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product details
    /// </returns>
    private async Task<ChatGptShoppingProductDto> BuildProductAsync(Product product,
        Category category, Manufacturer manufacturer, Store store, Customer customer, Currency currency, ChatGptShoppingSettings settings)
    {
        var languageId = settings.LanguageId;
        var productPictureSize = settings.ProductPictureSize;
        var targetCountries = settings.TargetCountries;
        var storeCountry = settings.StoreCountry;

        var productDto = new ChatGptShoppingProductDto();

        #region OpenAI Flags

        productDto.IsEligibleSearch = true;
        productDto.IsEligibleCheckout = false;
        productDto.IsAdsEligible = true;

        #endregion

        #region Basic Product Data

        productDto.ItemId = product.Sku;
        productDto.Gtin = product.Gtin;
        productDto.Mpn = product.ManufacturerPartNumber;

        //title should be not longer than 150 characters
        var title = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);
        productDto.Title = CommonHelper.EnsureMaximumLength(title, 150);

        //description should be not longer than 5000 characters
        var description = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId);
        if (string.IsNullOrEmpty(description))
            description = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId);
        description = _htmlFormatter.StripTags(_htmlFormatter.ConvertHtmlToPlainText(description, decode: true));
        description = CommonHelper.EnsureMaximumLength(description, 5000);
        productDto.Description = description;

        productDto.Url = $"{store.Url.TrimEnd('/')}{await _nopUrlHelper.RouteGenericUrlAsync(product, languageId: languageId)}";

        #endregion

        #region Item Information

        productDto.Brand = manufacturer is not null
            ? await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name, languageId)
            : null;
        productDto.ProductCategory = category is not null
            ? await _categoryService.GetFormattedBreadCrumbAsync(category, separator: ">", languageId: languageId)
            : null;
        productDto.Condition = "new"; // Assuming all products are new. Adjust as necessary.

        #endregion

        #region Media

        const int maximumPictures = 10;
        var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id, maximumPictures);
        var additionalImageUrls = new List<string>();

        for (var i = 0; i < pictures.Count; i++)
        {
            var picture = pictures[i];
            var imageUrl = await _pictureService.GetPictureUrlAsync(picture.Id, productPictureSize, storeLocation: store.Url);

            //default or additional image
            if (i == 0)
                productDto.ImageUrl = imageUrl;
            else
                additionalImageUrls.Add(imageUrl);
        }

        //no picture? submit a default one
        if (!pictures.Any())
            productDto.ImageUrl = await _pictureService.GetDefaultPictureUrlAsync(productPictureSize, storeLocation: store.Url);

        productDto.AdditionalImageUrls = additionalImageUrls.ToArray();
        productDto.VideoUrl = (await _videoService.GetVideosByProductIdAsync(product.Id)).FirstOrDefault()?.VideoUrl;

        #endregion

        #region Price & Promotions

        var taxDisplayType = await _customerService.GetCustomerTaxDisplayTypeAsync(customer);
        var (productPrice, _, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false);
        var (price, _) = await _taxService.GetProductPriceAsync(product, productPrice, taxDisplayType == TaxDisplayType.IncludingTax, customer);
        price = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price, currency);
        price = await _priceCalculationService.RoundPriceAsync(price, currency);
        productDto.Price = $"{price.ToString(new CultureInfo("en-US", false).NumberFormat)} {currency.CurrencyCode}";

        #endregion

        #region Availability & Inventory

        var availability = ChatGptShoppingDefaults.ProductAvailabilityInStock; //in stock by default

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock
            && product.BackorderMode == BackorderMode.NoBackorders
            && await _productService.GetTotalStockQuantityAsync(product) <= 0)
        {
            availability = ChatGptShoppingDefaults.ProductAvailabilityOutOfStock;
        }

        productDto.Availability = availability;

        if (product.AvailableForPreOrder
            && (!product.PreOrderAvailabilityStartDateTimeUtc.HasValue || product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow))
        {
            productDto.Availability = ChatGptShoppingDefaults.ProductAvailabilityPreOrder;
            productDto.AvailabilityDate = product.PreOrderAvailabilityStartDateTimeUtc;
        }

        #endregion

        #region Merchant Info

        productDto.SellerName = store.Name;
        productDto.SellerUrl = store.Url;

        #endregion

        #region Reviews and Q&A

        productDto.ReviewCount = product.ApprovedTotalReviews;
        productDto.StarRating = product.ApprovedRatingSum > 0
            ? ((double)product.ApprovedRatingSum / product.ApprovedTotalReviews).ToString("F1")
            : "0.0";

        #endregion

        #region Geo Tagging

        productDto.TargetCountries = targetCountries.Split(',', StringSplitOptions.RemoveEmptyEntries);
        productDto.StoreCountry = storeCountry;

        #endregion

        return productDto;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the categories
    /// </returns>
    private async Task<IList<Category>> GetAllCategoriesAsync(Customer customer, int storeId)
    {
        return await _categoryRepository.GetAllAsync(async query =>
        {
            query = query.Where(c => c.Published && !c.Deleted);
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            query = await _aclService.ApplyAcl(query, customer);

            return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
        });
    }

    /// <summary>
    /// Gets all manufacturers
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturers
    /// </returns>
    private async Task<IList<Manufacturer>> GetAllManufacturersAsync(Customer customer, int storeId)
    {
        return await _manufacturerRepository.GetAllAsync(async query =>
        {
            query = query.Where(m => m.Published && !m.Deleted);
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            query = await _aclService.ApplyAcl(query, customer);

            return query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);
        });
    }

    /// <summary>
    /// Search products
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    private async Task<IPagedList<Product>> SearchProductsAsync(Customer customer, int storeId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return await _productRepository.GetAllPagedAsync(async query =>
        {
            query = query.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            query = await _aclService.ApplyAcl(query, customer);

            return query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id);
        }, pageIndex, pageSize);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate a feed
    /// </summary>
    /// <param name="storeId">Identifier of a store to generate the feed; pass 0 to use all stores; pass null to indicate it's the auto synchronization</param>   
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of notifications
    /// </returns>
    public async Task<IList<(NotifyType Type, string Message)>> GenerateChatGptFeedAsync(int? storeId = null)
    {
        var messages = new List<(NotifyType, string)>();

        var stores = storeId > 0
            ? [await _storeService.GetStoreByIdAsync(storeId.Value)]
            : (await _storeService.GetAllStoresAsync()).ToList();

        foreach (var store in stores)
        {
            var generateFeedMessages = await GenerateFeedAsync(store);
            messages.AddRange(generateFeedMessages);
        }

        return messages;
    }

    #endregion
}
