using System.Diagnostics;
using AwesomeAssertions;
using ClosedXML.Excel;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using Nop.Services.Orders;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ExportImport;

[TestFixture]
public class ExportManagerTests : ServiceTest
{
    #region Fields

    private CustomerSettings _customerSettings;
    private CatalogSettings _catalogSettings;
    private IAddressService _addressService;
    private ICategoryService _categoryService;
    private ICountryService _countryService;
    private ICustomerService _customerService;
    private IDateRangeService _dateRangeService;
    private IExportManager _exportManager;
    private IManufacturerService _manufacturerService;
    private IMeasureService _measureService;
    private IProductTemplateService _productTemplateService;
    private IRepository<Product> _productRepository;
    private ITaxCategoryService _taxCategoryService;
    private IVendorService _vendorService;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _customerSettings = GetService<CustomerSettings>();
        _catalogSettings = GetService<CatalogSettings>();
        _addressService = GetService<IAddressService>();
        _categoryService = GetService<ICategoryService>();
        _countryService = GetService<ICountryService>();
        _customerService = GetService<ICustomerService>();
        _dateRangeService = GetService<IDateRangeService>();
        _exportManager = GetService<IExportManager>();
        _manufacturerService = GetService<IManufacturerService>();
        _measureService = GetService<IMeasureService>();
        _productTemplateService = GetService<IProductTemplateService>();
        _productRepository = GetService<IRepository<Product>>();
        _taxCategoryService = GetService<ITaxCategoryService>();
        _vendorService = GetService<IVendorService>();

        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                true);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                true);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                true);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                false);
    }

    #endregion

    #region Utilities

    protected static T PropertiesShouldEqual<T, Tp>(T actual, PropertyManager<Tp> manager, IDictionary<string, string> replacePairs, params string[] filter)
    {
        var objectProperties = typeof(T).GetProperties();
        foreach (var property in manager.GetDefaultProperties)
        {
            if (filter.Contains(property.PropertyName))
                continue;

            var objectProperty = replacePairs.TryGetValue(property.PropertyName, out var value) ? objectProperties.FirstOrDefault(p => p.Name == value)
                : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

            if (objectProperty == null)
                continue;

            var objectPropertyValue = actual == null ? null : objectProperty.GetValue(actual);
            var propertyValue = property.PropertyValue;

            if (propertyValue is XLCellValue { IsBlank: true })
                propertyValue = null;

            if (string.IsNullOrEmpty(propertyValue?.ToString() ?? string.Empty) && objectPropertyValue is null)
                continue;

            switch (objectPropertyValue)
            {
                case int:
                    propertyValue = property.IntValue;
                    break;
                case Guid:
                    propertyValue = property.GuidValue;
                    break;
                case string:
                    propertyValue = property.StringValue;
                    break;
                case DateTime time:
                    objectPropertyValue = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                    if (DateTime.TryParse(property.StringValue, out var date))
                        propertyValue = date;
                    else
                        propertyValue = null;
                    break;
                case bool:
                    propertyValue = property.BooleanValue;
                    break;
                case decimal:
                    propertyValue = property.DecimalValue;
                    break;
            }

            if (objectProperty.PropertyType.IsEnum && objectPropertyValue != null)
            {
                objectPropertyValue = (int)objectPropertyValue;
                propertyValue = property.IntValue;
            }

            propertyValue.Should().Be(objectPropertyValue, $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
        }

        return actual;
    }

    private static PropertyManager<T> GetPropertyManager<T>(XLWorkbook workbook, CatalogSettings catalogSettings)
    {
        //the columns
        var metadata = ImportManager.GetWorkbookMetadata<T>(workbook, new List<Language>());
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        return new PropertyManager<T>(defaultProperties, catalogSettings, localizedProperties);
    }

    private static XLWorkbook GetWorkbook(byte[] excelData)
    {
        var stream = new MemoryStream(excelData);
        return new XLWorkbook(stream);
    }

    protected T AreAllObjectPropertiesPresent<T>(T obj, PropertyManager<T> manager, params string[] filters)
    {
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            if (filters.Contains(propertyInfo.Name))
                continue;

            if (manager.GetDefaultProperties.Any(p => p.PropertyName == propertyInfo.Name))
                continue;

            Assert.Fail($"The property \"{typeof(T).Name}.{propertyInfo.Name}\" no present on excel file");
        }

        return obj;
    }

    public static IEnumerable<object[]> GetExportOrdersTestCases()
    {
        var orderService = GetService<IOrderService>();
        var orders = orderService.SearchOrdersAsync().Result;
        var exportManager = GetService<IExportManager>();
        var excelData = exportManager.ExportOrdersToXlsxAsync(orders).Result;
        var workbook = GetWorkbook(excelData);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        var replacePairs = new Dictionary<string, string>
        {
            { "OrderId", "Id" },
            { "OrderStatus", "OrderStatusId" },
            { "PaymentStatus", "PaymentStatusId" },
            { "ShippingStatus", "ShippingStatusId" },
            { "ShippingPickupInStore", "PickupInStore" }
        };

        var ignore = new List<string>();
        ignore.AddRange(replacePairs.Values);

        //not exported fields
        ignore.AddRange(new[]
        {
            "BillingAddressId", "ShippingAddressId", "PickupAddressId", "CustomerTaxDisplayTypeId",
            "RewardPointsHistoryEntryId", "CheckoutAttributeDescription", "CheckoutAttributesXml",
            "CustomerLanguageId", "CustomerIp", "AllowStoringCreditCardNumber", "CardType", "CardName",
            "CardNumber", "MaskedCreditCardNumber", "CardCvv2", "CardExpirationMonth", "CardExpirationYear",
            "AuthorizationTransactionId", "AuthorizationTransactionCode", "AuthorizationTransactionResult",
            "CaptureTransactionId", "CaptureTransactionResult", "SubscriptionTransactionId", "PaidDateUtc",
            "Deleted", "PickupAddress", "RedeemedRewardPointsEntryId", "DiscountUsageHistory",
            "GiftCardUsageHistory", "OrderNotes", "OrderItems", "Shipments", "OrderStatus", "PaymentStatus",
            "ShippingStatus", "CustomerTaxDisplayType", "CustomOrderNumber", "DesiredDeliveryDateUtc",
            "LastPendingOrderFollowUpNumber", "LastPendingOrderFollowUpDateUtc"
        });

        //fields tested individually
        ignore.AddRange(new[]
        {
            "Customer", "BillingAddressId", "ShippingAddressId", "EntityCacheKey"
        });

        var addressFields = new List<string>
        {
            "FirstName",
            "LastName",
            "Email",
            "Company",
            "Country",
            "StateProvince",
            "City",
            "County",
            "Address1",
            "Address2",
            "ZipPostalCode",
            "PhoneNumber",
            "FaxNumber"
        };

        var catalogSettings = GetService<CatalogSettings>();

        var index = 2;

        foreach (var order in orders)
        {
            var manager = GetPropertyManager<Order>(workbook, catalogSettings);
            manager.SetSelectList("OrderStatus", OrderStatus.Pending.ToSelectListAsync(useLocalization: false).Result);
            manager.SetSelectList("PaymentStatus", PaymentStatus.Pending.ToSelectListAsync(useLocalization: false).Result);
            manager.SetSelectList("ShippingStatus", ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false).Result);

            while (worksheet.Row(index).OutlineLevel == 1)
                index++;

            manager.ReadDefaultFromXlsx(worksheet, index++);

            yield return [order, manager, ignore, replacePairs, addressFields, $"Order #{order.Id}"];
        }
    }

    #endregion

    #region Test export to excel

    [Test]
    [TestCaseSource(nameof(GetExportOrdersTestCases))]
    public async Task CanExportOrdersXlsx(Order order, PropertyManager<Order> manager, List<string> ignore, Dictionary<string, string> replacePairs, List<string> addressFields, string testName)
    {
        Debug.WriteLine($"Test for {testName}");
        AreAllObjectPropertiesPresent(order, manager, ignore.ToArray());
        PropertiesShouldEqual(order, manager, replacePairs);

        const string billingPattern = "Billing";
        var pairs = addressFields.ToDictionary(p => billingPattern + p, p => p);

        var testBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        PropertiesShouldEqual(testBillingAddress, manager, pairs, "CreatedOnUtc", "BillingCountry");

        var country = await _countryService.GetCountryByAddressAsync(testBillingAddress);
        manager.GetDefaultProperties.First(p => p.PropertyName == "BillingCountry").StringValue.Should().Be(country.Name);

        const string shippingPattern = "Shipping";
        pairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);

        var testShippingAddress = await _addressService.GetAddressByIdAsync((order.PickupInStore ? order.PickupAddressId : order.ShippingAddressId) ?? 0);
        PropertiesShouldEqual(testShippingAddress, manager, pairs, "CreatedOnUtc", "ShippingCountry");
        country = await _countryService.GetCountryByAddressAsync(testShippingAddress);
        manager.GetDefaultProperties.First(p => p.PropertyName == "ShippingCountry").StringValue.Should().Be(country?.Name ?? string.Empty);
    }

    [Test]
    public async Task CanExportManufacturersXlsx()
    {
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync();

        var excelData = await _exportManager.ExportManufacturersToXlsxAsync(manufacturers);
        var workbook = GetWorkbook(excelData);
        var manager = GetPropertyManager<Manufacturer>(workbook, _catalogSettings);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        var manufacturer = manufacturers.First();

        var ignore = new List<string> { "Picture", "EntityCacheKey", "PictureId", "SubjectToAcl", "LimitedToStores", "Deleted", "CreatedOnUtc", "UpdatedOnUtc", "AppliedDiscounts", "DiscountManufacturerMappings" };

        AreAllObjectPropertiesPresent(manufacturer, manager, ignore.ToArray());
        PropertiesShouldEqual(manufacturer, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
    }

    [Test]
    public async Task CanExportCustomersToXlsx()
    {
        var customers = await _customerService.GetAllCustomersAsync();

        var excelData = await _exportManager.ExportCustomersToXlsxAsync(customers);
        var workbook = GetWorkbook(excelData);
        var manager = GetPropertyManager<Customer>(workbook, _catalogSettings);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        manager.SetSelectList("VatNumberStatus", await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));

        var customer = customers.First();

        var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "ShoppingCartItems",
            "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses", "AdminComment",
            "EmailToRevalidate", "PhoneSmsVerified", "HasShoppingCartItems", "RequireReLogin", "FailedLoginAttempts",
            "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName", "LastIpAddress",
            "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId",
            "CustomerCustomerRoleMappings", "CustomerAddressMappings", "EntityCacheKey", "VendorId",
            "DateOfBirth", "CountryId",
            "StateProvinceId", "VatNumberStatusId", "TimeZoneId",
            "CurrencyId", "LanguageId", "TaxDisplayTypeId", "TaxDisplayType", "TaxDisplayType", "VatNumberStatusId", "MustChangePassword",
            "LastShoppingCartUpdateDateUtc", "LastAbandonedCartFollowUpNumber", "LastAbandonedCartFollowUpDateUtc", "RegistrationFollowUpDateUtc" };

        if (!_customerSettings.FirstNameEnabled)
            ignore.Add("FirstName");

        if (!_customerSettings.LastNameEnabled)
            ignore.Add("LastName");

        if (!_customerSettings.GenderEnabled)
            ignore.Add("Gender");

        if (!_customerSettings.CompanyEnabled)
            ignore.Add("Company");

        if (!_customerSettings.StreetAddressEnabled)
            ignore.Add("StreetAddress");

        if (!_customerSettings.StreetAddress2Enabled)
            ignore.Add("StreetAddress2");

        if (!_customerSettings.ZipPostalCodeEnabled)
            ignore.Add("ZipPostalCode");

        if (!_customerSettings.CityEnabled)
            ignore.Add("City");

        if (!_customerSettings.CountyEnabled)
            ignore.Add("County");

        if (!_customerSettings.CountryEnabled)
            ignore.Add("Country");

        if (!_customerSettings.StateProvinceEnabled)
            ignore.Add("StateProvince");

        if (!_customerSettings.PhoneEnabled)
            ignore.Add("Phone");

        if (!_customerSettings.FaxEnabled)
            ignore.Add("Fax");

        AreAllObjectPropertiesPresent(customer, manager, ignore.ToArray());
        PropertiesShouldEqual(customer, manager, new Dictionary<string, string>());
    }

    [Test]
    public async Task CanExportCategoriesToXlsx()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        var excelData = await _exportManager.ExportCategoriesToXlsxAsync(categories);
        var workbook = GetWorkbook(excelData);
        var manager = GetPropertyManager<Category>(workbook, _catalogSettings);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        var category = categories.First();

        var ignore = new List<string> { "CreatedOnUtc", "EntityCacheKey", "Picture", "PictureId", "AppliedDiscounts", "UpdatedOnUtc", "SubjectToAcl", "LimitedToStores", "Deleted", "DiscountCategoryMappings", "RestrictFromVendors" };

        AreAllObjectPropertiesPresent(category, manager, ignore.ToArray());
        PropertiesShouldEqual(category, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
    }

    [Test]
    public async Task CanExportProductsToXlsx()
    {
        var replacePairs = new Dictionary<string, string>
        {
            { "ProductId", "Id" },
            { "ProductType", "ProductTypeId" },
            { "GiftCardType", "GiftCardTypeId" },
            { "Vendor", "VendorId" },
            { "ProductTemplate", "ProductTemplateId" },
            { "DeliveryDate", "DeliveryDateId" },
            { "TaxCategory", "TaxCategoryId" },
            { "ManageInventoryMethod", "ManageInventoryMethodId" },
            { "ProductAvailabilityRange", "ProductAvailabilityRangeId" },
            { "LowStockActivity", "LowStockActivityId" },
            { "BackorderMode", "BackorderModeId" },
            { "BasepriceUnit", "BasepriceUnitId" },
            { "BasepriceBaseUnit", "BasepriceBaseUnitId" },
            { "SKU", "Sku" },
            { "DownloadActivationType", "DownloadActivationTypeId" },
            { "RecurringCyclePeriod", "RecurringCyclePeriodId" },
            { "RentalPricePeriod", "RentalPricePeriodId" }
        };

        var ignore = new List<string> { "Categories", "Manufacturers", "AdminComment",
            "ProductType", "BackorderMode", "DownloadActivationType", "GiftCardType", "LowStockActivity",
            "ManageInventoryMethod", "RecurringCyclePeriod", "RentalPricePeriod", "ProductCategories",
            "ProductManufacturers", "ProductPictures", "ProductReviews", "ProductSpecificationAttributes",
            "ProductTags", "ProductAttributeMappings", "ProductAttributeCombinations", "TierPrices",
            "AppliedDiscounts", "ProductWarehouseInventory", "ApprovedRatingSum", "NotApprovedRatingSum",
            "ApprovedTotalReviews", "NotApprovedTotalReviews", "SubjectToAcl", "LimitedToStores", "Deleted",
            "DownloadExpirationDays", "AvailableStartDateTimeUtc",
            "AvailableEndDateTimeUtc", "DisplayOrder", "CreatedOnUtc", "UpdatedOnUtc", "ProductProductTagMappings",
            "DiscountProductMappings", "EntityCacheKey" };

        ignore.AddRange(replacePairs.Values);

        var product = _productRepository.Table.ToList().First();

        var excelData = await _exportManager.ExportProductsToXlsxAsync(new[] { product });
        var workbook = GetWorkbook(excelData);
        var manager = GetPropertyManager<Product>(workbook, _catalogSettings);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.SetSelectList("ProductType", await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("DownloadActivationType", await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("ManageInventoryMethod", await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("LowStockActivity", await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("RecurringCyclePeriod", await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
        manager.SetSelectList("Vendor", vendors.Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
        var templates = await _productTemplateService.GetAllProductTemplatesAsync();
        manager.SetSelectList("ProductTemplate", templates.Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
        var dates = await _dateRangeService.GetAllDeliveryDatesAsync();
        manager.SetSelectList("DeliveryDate", dates.Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
        var availabilityRanges = await _dateRangeService.GetAllProductAvailabilityRangesAsync();
        manager.SetSelectList("ProductAvailabilityRange", availabilityRanges.Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
        var categories = await _taxCategoryService.GetAllTaxCategoriesAsync();
        manager.SetSelectList("TaxCategory", categories.Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
        var measureWeights = await _measureService.GetAllMeasureWeightsAsync();
        manager.SetSelectList("BasepriceUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
        manager.SetSelectList("BasepriceBaseUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));

        manager.Remove("ProductTags");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        AreAllObjectPropertiesPresent(product, manager, ignore.ToArray());
        PropertiesShouldEqual(product, manager, replacePairs);
    }

    #endregion
}
