using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.ExportImport;
using Nop.Services.Media;
using Nop.Services.Stores;


namespace Nop.Plugin.Theme.KungFu.Seeding.JkooSword;

public class JkooStoreSyncService : IJkooStoreSyncService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStoreContext _storeContext;
    private readonly IProductService _productService;
    private readonly IPictureService _httpPictureService;
    private readonly ICategoryService _categoryService;
    private readonly IImportManager _importManager;


    public JkooStoreSyncService(IHttpClientFactory httpClientFactory, IStoreContext storeContext,
        IProductService productService, IPictureService httpPictureService, ICategoryService categoryService, IImportManager importManager)
    {
        _httpClientFactory = httpClientFactory;
        _storeContext = storeContext;
        _productService = productService;
        _httpPictureService = httpPictureService;
        _categoryService = categoryService;
        _importManager = importManager;
    }

    public async Task<SiteMapModel[]> LoadSiteMapAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, JkooSwordSettings.SiteMapUrl);
        using var response = await httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode) return [];
        var content = await response.Content.ReadAsStringAsync();
        var xDocument = XDocument.Parse(content);
        var siteMapModels = xDocument.ToSiteMap();
        return siteMapModels;
    }

    public Task LoadRobotsTxtAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<string[]> LoadUrls()
    {
        var map = await LoadSiteMapAsync();
        var urls = map.Select(m => m.Url).ToArray();
        return urls;
    }

    public async Task SyncProductsAsync()
    {
        var loadUrls = await LoadUrls();


        foreach (var link in loadUrls)
        {
            var isParent = link.IsParentCategory();
            var isSub = link.isSubCategory();
            var chineseCategory = JkooSwordSettings.ChineseSwordsLinkRegex.IsMatch(link);
            var chineseDaoCategory = JkooSwordSettings.ChineseDaoBroadSwordLinkRegex.IsMatch(link);
            var chineseJianCategory = JkooSwordSettings.ChineseJianSwordLinkRegex.IsMatch(link);
            var japaneseCategory = JkooSwordSettings.JapaneseSwordsLinkRegex.IsMatch(link);
            var katanaCategory = JkooSwordSettings.KatanaShinkenLinkRegex.IsMatch(link);
            var wakizashiCategory = JkooSwordSettings.WakizashiShortSwordLinkRegex.IsMatch(link);
            var tantoCategory = JkooSwordSettings.TantoDaggerLinkRegex.IsMatch(link);
            var ninjatoCategory = JkooSwordSettings.NinjatoLinkRegex.IsMatch(link);
            var iaitoCategory = JkooSwordSettings.IaitoTrainingSwordLinkRegex.IsMatch(link);
            var tachiCategory = JkooSwordSettings.TachiGreatSwordsLinkRegex.IsMatch(link);
            var daishoCategory = JkooSwordSettings.DaishoLinkRegex.IsMatch(link);

            if (isParent && isSub) continue;

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, link);
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) continue;
            var content = await response.Content.ReadAsStringAsync();
            var isProductPage = content.IsProductPage();
            if (isProductPage)
            {
                var Images = content.ImageLinks();
                var Name = content.ProductName();
                var Sku = content.ProductSku();
                var Description = content.ProductDescription();
                var Price = content.ProductPrice();
                var Currency = content.Currency();

                var pictureIds = new List<int>();

                foreach (var imageLink in Images)
                {
                    var imageRequest = new HttpRequestMessage(HttpMethod.Get, imageLink);
                    var imageResponse = await client.SendAsync(imageRequest);
                    if (!imageResponse.IsSuccessStatusCode) continue;
                    var fileName = imageLink.Split('/').Last();
                    var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                    var formFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, fileName, fileName)
                    {
                        Headers = new HeaderDictionary(), ContentType = "image/jpeg"
                    };
                    var picture = await _httpPictureService.InsertPictureAsync(formFile, fileName);
                    pictureIds.Add(picture.Id);
                }

                var product = new Nop.Core.Domain.Catalog.Product
                {
                    ProductTypeId = (int)ProductType.SimpleProduct,
                    ParentGroupedProductId = 0,
                    Name = Name,
                    ShortDescription = Name,
                    FullDescription = Description,
                    AdminComment = null,
                    ProductTemplateId = 0,
                    VendorId = 0,
                    ShowOnHomepage = false,
                    MetaKeywords = null,
                    MetaDescription = null,
                    MetaTitle = null,
                    AllowCustomerReviews = false,
                    ApprovedRatingSum = 0,
                    NotApprovedRatingSum = 0,
                    ApprovedTotalReviews = 0,
                    NotApprovedTotalReviews = 0,
                    SubjectToAcl = false,
                    LimitedToStores = false,
                    Sku = Sku,
                    ManufacturerPartNumber = null,
                    Gtin = null,
                    IsGiftCard = false,
                    GiftCardTypeId = 0,
                    OverriddenGiftCardAmount = null,
                    RequireOtherProducts = false,
                    RequiredProductIds = null,
                    AutomaticallyAddRequiredProducts = false,
                    IsDownload = false,
                    DownloadId = 0,
                    UnlimitedDownloads = false,
                    MaxNumberOfDownloads = 0,
                    DownloadExpirationDays = null,
                    DownloadActivationTypeId = 0,
                    HasSampleDownload = false,
                    SampleDownloadId = 0,
                    HasUserAgreement = false,
                    UserAgreementText = null,
                    IsRecurring = false,
                    RecurringCycleLength = 0,
                    RecurringCyclePeriodId = 0,
                    RecurringTotalCycles = 0,
                    IsRental = false,
                    RentalPriceLength = 0,
                    RentalPricePeriodId = 0,
                    IsShipEnabled = false,
                    IsFreeShipping = false,
                    ShipSeparately = false,
                    AdditionalShippingCharge = 0,
                    DeliveryDateId = 0,
                    IsTaxExempt = false,
                    TaxCategoryId = 0,
                    ManageInventoryMethodId = 0,
                    ProductAvailabilityRangeId = 0,
                    UseMultipleWarehouses = false,
                    WarehouseId = 0,
                    StockQuantity = 0,
                    DisplayStockAvailability = false,
                    DisplayStockQuantity = false,
                    MinStockQuantity = 0,
                    LowStockActivityId = 0,
                    NotifyAdminForQuantityBelow = 0,
                    BackorderModeId = 0,
                    AllowBackInStockSubscriptions = false,
                    OrderMinimumQuantity = 0,
                    OrderMaximumQuantity = 0,
                    AllowedQuantities = null,
                    AllowAddingOnlyExistingAttributeCombinations = false,
                    DisplayAttributeCombinationImagesOnly = false,
                    NotReturnable = false,
                    DisableBuyButton = false,
                    DisableWishlistButton = false,
                    AvailableForPreOrder = false,
                    PreOrderAvailabilityStartDateTimeUtc = null,
                    CallForPrice = false,
                    Price = 0,
                    OldPrice = 0,
                    ProductCost = Price,
                    CustomerEntersPrice = false,
                    MinimumCustomerEnteredPrice = 0,
                    MaximumCustomerEnteredPrice = 0,
                    BasepriceEnabled = false,
                    BasepriceAmount = 0,
                    BasepriceUnitId = 0,
                    BasepriceBaseAmount = 0,
                    BasepriceBaseUnitId = 0,
                    MarkAsNew = false,
                    MarkAsNewStartDateTimeUtc = null,
                    MarkAsNewEndDateTimeUtc = null,
                    Weight = 1.1m,
                    Length = 0.71m,
                    Width = 6,
                    Height = 2,
                    AvailableStartDateTimeUtc = null,
                    AvailableEndDateTimeUtc = null,
                    DisplayOrder = 0,
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = default,
                    UpdatedOnUtc = default,
                    AgeVerification = false,
                    MinimumAgeToPurchase = 0,
                    ProductType = ProductType.SimpleProduct,
                    BackorderMode = BackorderMode.NoBackorders,
                    DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                    GiftCardType = GiftCardType.Virtual,
                    LowStockActivity = LowStockActivity.Unpublish,
                    ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                    RecurringCyclePeriod = RecurringProductCyclePeriod.Months,
                    RentalPricePeriod = RentalPricePeriod.Days,
                    VisibleIndividually = true,
                };

                await _productService.InsertProductAsync(product);
                var createdProduct = await _productService.GetProductBySkuAsync(Sku);
                if (createdProduct == null) continue;

                // Product Picture Mappings
                foreach (var pictureId in pictureIds)
                {
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        ProductId = createdProduct.Id, PictureId = pictureId, DisplayOrder = 0
                    });
                }

                var forgedSteelCategory = await _categoryService.GetAllCategoriesAsync("Lonquan Forged");

                // Categories
                if (!forgedSteelCategory?.Any() ?? false)
                {
                    // Create Lonquan Forged Steel Swords category if it doesn't exist
                    var newCategory = new Core.Domain.Catalog.Category
                    {
                        Id = 0,
                        Name = "Lonquan Forged Steel Swords",
                        Description = "Lonquan Forged Steel Swords Category",
                        MetaKeywords = null,
                        MetaDescription = null,
                        MetaTitle = null,
                        ParentCategoryId = 0,
                        PictureId = 0,
                        PageSize = 0,
                        AllowCustomersToSelectPageSize = false,
                        PageSizeOptions = null,
                        ShowOnHomepage = false,
                        SubjectToAcl = false,
                        LimitedToStores = false,
                        Published = true,
                        Deleted = false,
                        DisplayOrder = 0,
                        CreatedOnUtc = default,
                        UpdatedOnUtc = default,
                        PriceRangeFiltering = false,
                        PriceFrom = 0,
                        PriceTo = 0,
                        ManuallyPriceRange = false,
                        RestrictFromVendors = false,

                    };
                    
                    await _categoryService.InsertCategoryAsync(newCategory);
                }
            }
        }
    }
}