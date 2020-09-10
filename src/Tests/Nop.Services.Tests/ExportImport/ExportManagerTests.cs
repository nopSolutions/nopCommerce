using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Data;
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
using Nop.Tests;
using NUnit.Framework;
using OfficeOpenXml;

namespace Nop.Services.Tests.ExportImport
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

        [SetUp]
        public void SetUp()
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

            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                    true);
            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                    true);
            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                    true);
        }

        [TearDown]
        public void TearDown()
        {
            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                    false);
            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                    false);
            GetService<IGenericAttributeService>()
                .SaveAttribute(_customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail), "product-advanced-mode",
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

                if (objectProperty.PropertyType == typeof(DateTime)) 
                    objectPropertyValue = ((DateTime)objectPropertyValue).ToOADate();

                if (objectProperty.PropertyType == typeof(DateTime?)) 
                    objectPropertyValue = ((DateTime?)objectPropertyValue)?.ToOADate();

                property.PropertyValue.Should().Be(objectPropertyValue, $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
            }

            return actual;
        }

        protected PropertyManager<T> GetPropertyManager<T>(ExcelWorksheet worksheet)
        {
            //the columns
            var properties = ImportManager.GetPropertiesByExcelCells<T>(worksheet);

            return new PropertyManager<T>(properties, _catalogSettings);
        }

        protected ExcelWorksheet GetWorksheets(byte[] excelData)
        {
            var stream = new MemoryStream(excelData);
            var xlPackage = new ExcelPackage(stream);

            // get the first worksheet in the workbook
            var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
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
        public void CanExportOrdersXlsx()
        {
            var orders = _orderService.SearchOrders();

            var excelData = _exportManager.ExportOrdersToXlsx(orders);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Order>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);

            var replacePairs = new Dictionary<string, string>
                {
                    { "OrderId", "Id" },
                    { "OrderStatusId", "OrderStatus" },
                    { "PaymentStatusId", "PaymentStatus" },
                    { "ShippingStatusId", "ShippingStatus" },
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
                "OrderNotes", "OrderItems", "Shipments", "OrderStatus", "PaymentStatus", "ShippingStatus ",
                "CustomerTaxDisplayType", "CustomOrderNumber"
            });

            //fields tested individually
            ignore.AddRange(new[]
            {
               "Customer", "BillingAddressId", "ShippingAddressId", "EntityCacheKey"
            });

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

            var testBillingAddress = _addressService.GetAddressById(order.BillingAddressId);

            PropertiesShouldEqual(testBillingAddress, manager, replacePairs, "CreatedOnUtc", "BillingCountry"); 
            
            manager.GetProperties.First(p => p.PropertyName == "BillingCountry").PropertyValue.Should().Be(_countryService.GetCountryByAddress(testBillingAddress).Name);

            const string shippingPattern = "Shipping";
            replacePairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);
            var testShippingAddress = _addressService.GetAddressById(order.ShippingAddressId ?? 0);
            PropertiesShouldEqual(testShippingAddress, manager, replacePairs, "CreatedOnUtc", "ShippingCountry");
            manager.GetProperties.First(p => p.PropertyName == "ShippingCountry").PropertyValue.Should().Be(_countryService.GetCountryByAddress(testShippingAddress).Name);
        }

        [Test]
        public void CanExportManufacturersXlsx()
        {
            var manufacturers = _manufacturerService.GetAllManufacturers();

            var excelData = _exportManager.ExportManufacturersToXlsx(manufacturers);
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
        public void CanExportCustomersToXlsx()
        {
            var customers = _customerService.GetAllCustomers();

            var excelData = _exportManager.ExportCustomersToXlsx(customers);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Customer>(worksheet);

            manager.ReadFromXlsx(worksheet, 2);
            var customer = customers.First();

            var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "CustomerRoles", "ShoppingCartItems",
                "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses", "AdminComment",
                "EmailToRevalidate", "HasShoppingCartItems", "RequireReLogin", "FailedLoginAttempts",
                "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName", "LastIpAddress",
                "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId",
                "CustomerCustomerRoleMappings", "CustomerAddressMappings", "EntityCacheKey" };

            AreAllObjectPropertiesPresent(customer, manager, ignore.ToArray());
            PropertiesShouldEqual(customer, manager, new Dictionary<string, string>());
        }

        [Test]
        public void CanExportCategoriesToXlsx()
        {
            var categories = _categoryService.GetAllCategories();

            var excelData = _exportManager.ExportCategoriesToXlsx(categories);
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
        public void CanExportProductsToXlsx()
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

            var products = _productRepository.Table.ToList();

            var excelData = _exportManager.ExportProductsToXlsx(products);
            var worksheet = GetWorksheets(excelData);
            var manager = GetPropertyManager<Product>(worksheet);

            manager.SetSelectList("ProductType", ProductType.SimpleProduct.ToSelectList(useLocalization: false));
            manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
            manager.SetSelectList("DownloadActivationType", DownloadActivationType.Manually.ToSelectList(useLocalization: false));
            manager.SetSelectList("ManageInventoryMethod", ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false));
            manager.SetSelectList("LowStockActivity", LowStockActivity.Nothing.ToSelectList(useLocalization: false));
            manager.SetSelectList("BackorderMode", BackorderMode.NoBackorders.ToSelectList(useLocalization: false));
            manager.SetSelectList("RecurringCyclePeriod", RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false));
            manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

            manager.SetSelectList("Vendor", _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
            manager.SetSelectList("ProductTemplate", _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
            manager.SetSelectList("DeliveryDate", _dateRangeService.GetAllDeliveryDates().Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
            manager.SetSelectList("ProductAvailabilityRange", _dateRangeService.GetAllProductAvailabilityRanges().Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
            manager.SetSelectList("TaxCategory", _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceBaseUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));

            manager.Remove("ProductTags");

            manager.ReadFromXlsx(worksheet, 2);
            var product = products.First();

            AreAllObjectPropertiesPresent(product, manager, ignore.ToArray());
            PropertiesShouldEqual(product, manager, replacePairs);
        }

        #endregion
    }
}
