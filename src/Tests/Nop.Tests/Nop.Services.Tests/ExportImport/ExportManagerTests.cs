using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
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

namespace Nop.Tests.Nop.Services.Tests.ExportImport
{
    [TestFixture]
    public class ExportManagerTests : ServiceTest
    {
        #region Fields

        private CatalogSettings _catalogSettings;
        private IAddressService _addressService;
        private ICategoryService _categoryService;
        private ICountryService _countryService;
        private ICustomerService _customerService;
        private IDateRangeService _dateRangeService;
        private IExportManager _exportManager;
        private IManufacturerService _manufacturerService;
        private IMeasureService _measureService;
        private IOrderService _orderService;
        private IProductTemplateService _productTemplateService;
        private IRepository<Product> _productRepository;
        private ITaxCategoryService _taxCategoryService;
        private IVendorService _vendorService;

        #endregion

        #region Setup

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _catalogSettings = GetService<CatalogSettings>();
            _addressService = GetService<IAddressService>();
            _categoryService = GetService<ICategoryService>();
            _countryService = GetService<ICountryService>();
            _customerService = GetService<ICustomerService>();
            _dateRangeService = GetService<IDateRangeService>();
            _exportManager = GetService<IExportManager>();
            _manufacturerService = GetService<IManufacturerService>();
            _measureService = GetService<IMeasureService>();
            _orderService = GetService<IOrderService>();
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
            foreach (var property in manager.GetProperties)
            {
                if (filter.Contains(property.PropertyName))
                    continue;

                var objectProperty = replacePairs.ContainsKey(property.PropertyName)
                    ? objectProperties.FirstOrDefault(p => p.Name == replacePairs[property.PropertyName])
                    : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

                if (objectProperty == null)
                    continue;

                var objectPropertyValue = objectProperty.GetValue(actual);

                if (objectProperty.PropertyType == typeof(Guid))
                    objectPropertyValue = objectPropertyValue.ToString();

                if (objectProperty.PropertyType == typeof(string))
                    objectPropertyValue = (property.PropertyValue?.ToString() == string.Empty && objectPropertyValue == null) ? string.Empty : objectPropertyValue;

                if (objectProperty.PropertyType.IsEnum)
                    objectPropertyValue = (int)objectPropertyValue;

                //https://github.com/ClosedXML/ClosedXML/blob/develop/ClosedXML/Extensions/ObjectExtensions.cs#L61
                if (objectProperty.PropertyType == typeof(DateTime))
                    objectPropertyValue = DateTime.FromOADate(double.Parse(((DateTime)objectPropertyValue).ToOADate().ToString("G15", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));

                if (objectProperty.PropertyType == typeof(DateTime?))
                    objectPropertyValue = objectPropertyValue != null ? DateTime.FromOADate(double.Parse(((DateTime?)objectPropertyValue)?.ToOADate().ToString("G15", CultureInfo.InvariantCulture))) : null;

                //https://github.com/ClosedXML/ClosedXML/issues/544
                property.PropertyValue.Should().Be(objectPropertyValue ?? "", $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
            }

            return actual;
        }

        protected PropertyManager<T> GetPropertyManager<T>(IXLWorksheet worksheet)
        {
            //the columns
            var properties = ImportManager.GetPropertiesByExcelCells<T>(worksheet);

            return new PropertyManager<T>(properties, _catalogSettings);
        }

        protected IXLWorksheet GetWorksheets(byte[] excelData)
        {
            var stream = new MemoryStream(excelData);
            var workbook = new XLWorkbook(stream);

            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new NopException("No worksheet found");

            return worksheet;
        }

        protected T AreAllObjectPropertiesPresent<T>(T obj, PropertyManager<T> manager, params string[] filters)
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (filters.Contains(propertyInfo.Name))
                    continue;

                if (manager.GetProperties.Any(p => p.PropertyName == propertyInfo.Name))
                    continue;

                Assert.Fail("The property \"{0}.{1}\" no present on excel file", typeof(T).Name, propertyInfo.Name);
            }

            return obj;
        }

        #endregion

        #region Test export to excel

        [Test]
        public async Task CanExportOrdersXlsx()
        {
            var orders = await _orderService.SearchOrdersAsync();

            var excelData = await _exportManager.ExportOrdersToXlsxAsync(orders);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Order>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);

            var replacePairs = new Dictionary<string, string>
                {
                    { "OrderId", "Id" },
                    { "OrderStatus", "OrderStatusId" },
                    { "PaymentStatus", "PaymentStatusId" },
                    { "ShippingStatus", "ShippingStatusId" },
                    { "ShippingPickupInStore", "PickupInStore" }
                };

            var order = orders.First();

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
                "Deleted", "PickupAddress", "RedeemedRewardPointsEntryId", "DiscountUsageHistory", "GiftCardUsageHistory",
                "OrderNotes", "OrderItems", "Shipments", "OrderStatus", "PaymentStatus", "ShippingStatus",
                "CustomerTaxDisplayType", "CustomOrderNumber"
            });

            //fields tested individually
            ignore.AddRange(new[]
            {
               "Customer", "BillingAddressId", "ShippingAddressId", "EntityCacheKey"
            });

            manager.SetSelectList("OrderStatus", await OrderStatus.Pending.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("PaymentStatus", await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ShippingStatus", await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false));
            
            AreAllObjectPropertiesPresent(order, manager, ignore.ToArray());
            PropertiesShouldEqual(order, manager, replacePairs);

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

            const string billingPattern = "Billing";
            replacePairs = addressFields.ToDictionary(p => billingPattern + p, p => p);

            var testBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            PropertiesShouldEqual(testBillingAddress, manager, replacePairs, "CreatedOnUtc", "BillingCountry");

            var country = await _countryService.GetCountryByAddressAsync(testBillingAddress);
            manager.GetProperties.First(p => p.PropertyName == "BillingCountry").PropertyValue.Should().Be(country.Name);

            const string shippingPattern = "Shipping";
            replacePairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);
            var testShippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
            PropertiesShouldEqual(testShippingAddress, manager, replacePairs, "CreatedOnUtc", "ShippingCountry");
            country = await _countryService.GetCountryByAddressAsync(testShippingAddress);
            manager.GetProperties.First(p => p.PropertyName == "ShippingCountry").PropertyValue.Should().Be(country.Name);
        }

        [Test]
        public async Task CanExportManufacturersXlsx()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync();

            var excelData = await _exportManager.ExportManufacturersToXlsxAsync(manufacturers);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Manufacturer>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);

            var manufacturer = manufacturers.First();

            var ignore = new List<string> { "Picture", "EntityCacheKey", "PictureId", "SubjectToAcl", "LimitedToStores", "Deleted", "CreatedOnUtc", "UpdatedOnUtc", "AppliedDiscounts", "DiscountManufacturerMappings" };

            AreAllObjectPropertiesPresent(manufacturer, manager, ignore.ToArray());
            PropertiesShouldEqual(manufacturer, manager, new Dictionary<string, string>());

            manager.GetProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
        }

        [Test]
        public async Task CanExportCustomersToXlsx()
        {
            var customers = await _customerService.GetAllCustomersAsync();

            var excelData = await _exportManager.ExportCustomersToXlsxAsync(customers);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Customer>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);
            var customer = customers.First();

            var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "CustomerRoles", "ShoppingCartItems",
                "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses", "AdminComment",
                "EmailToRevalidate", "HasShoppingCartItems", "RequireReLogin", "FailedLoginAttempts",
                "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName", "LastIpAddress",
                "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId",
                "CustomerCustomerRoleMappings", "CustomerAddressMappings", "EntityCacheKey", "VendorId" };

            AreAllObjectPropertiesPresent(customer, manager, ignore.ToArray());
            PropertiesShouldEqual(customer, manager, new Dictionary<string, string>());
        }

        [Test]
        public async Task CanExportCategoriesToXlsx()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            var excelData = await _exportManager.ExportCategoriesToXlsxAsync(categories);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Category>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);
            var category = categories.First();

            var ignore = new List<string> { "CreatedOnUtc", "EntityCacheKey", "Picture", "PictureId", "AppliedDiscounts", "UpdatedOnUtc", "SubjectToAcl", "LimitedToStores", "Deleted", "DiscountCategoryMappings" };

            AreAllObjectPropertiesPresent(category, manager, ignore.ToArray());
            PropertiesShouldEqual(category, manager, new Dictionary<string, string>());

            manager.GetProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
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
                "DownloadExpirationDays", "HasTierPrices", "HasDiscountsApplied", "AvailableStartDateTimeUtc",
                "AvailableEndDateTimeUtc", "DisplayOrder", "CreatedOnUtc", "UpdatedOnUtc", "ProductProductTagMappings",
                "DiscountProductMappings", "EntityCacheKey" };

            ignore.AddRange(replacePairs.Values);

            var product = _productRepository.Table.ToList().First();

            var excelData = await _exportManager.ExportProductsToXlsxAsync(new[] {product});
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Product>(worksheet);

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

            manager.ReadFromXlsx(worksheet, 2);

            AreAllObjectPropertiesPresent(product, manager, ignore.ToArray());
            PropertiesShouldEqual(product, manager, replacePairs);
        }

        #endregion
    }
}
