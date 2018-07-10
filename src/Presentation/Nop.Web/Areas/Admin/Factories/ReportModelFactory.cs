using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the report model factory implementation
    /// </summary>
    public partial class ReportModelFactory : IReportModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerReportService _customerReportService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderReportService _orderReportService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ReportModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerReportService customerReportService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderReportService orderReportService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IWorkContext workContext)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._countryService = countryService;
            this._customerReportService = customerReportService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._orderReportService = orderReportService;
            this._priceFormatter = priceFormatter;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productService = productService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        #region LowStock

        /// <summary>
        /// Prepare low stock product search model
        /// </summary>
        /// <param name="searchModel">Low stock product search model</param>
        /// <returns>Low stock product search model</returns>
        public virtual LowStockProductSearchModel PrepareLowStockProductSearchModel(LowStockProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Reports.LowStock.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Reports.LowStock.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Reports.LowStock.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged low stock product list model
        /// </summary>
        /// <param name="searchModel">Low stock product search model</param>
        /// <returns>Low stock product list model</returns>
        public virtual LowStockProductListModel PrepareLowStockProductListModel(LowStockProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var publishedOnly = searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1 ? true : (bool?)false;
            var vendorId = _workContext.CurrentVendor?.Id ?? 0;

            //get low stock product and product combinations
            var products = _productService.GetLowStockProducts(vendorId: vendorId, loadPublishedOnly: publishedOnly);
            var combinations = _productService.GetLowStockProductCombinations(vendorId: vendorId, loadPublishedOnly: publishedOnly);

            //prepare low stock product models
            var lowStockProductModels = new List<LowStockProductModel>();
            lowStockProductModels.AddRange(products.Select(product => new LowStockProductModel
            {
                Id = product.Id,
                Name = product.Name,
                ManageInventoryMethod = _localizationService.GetLocalizedEnum(product.ManageInventoryMethod),
                StockQuantity = _productService.GetTotalStockQuantity(product),
                Published = product.Published
            }));

            lowStockProductModels.AddRange(combinations.Select(combination => new LowStockProductModel
            {
                Id = combination.Product.Id,
                Name = combination.Product.Name,
                Attributes = _productAttributeFormatter
                    .FormatAttributes(combination.Product, combination.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false),
                ManageInventoryMethod = _localizationService.GetLocalizedEnum(combination.Product.ManageInventoryMethod),
                StockQuantity = combination.StockQuantity,
                Published = combination.Product.Published
            }));

            //prepare list model
            var model = new LowStockProductListModel
            {
                Data = lowStockProductModels.PaginationByRequestModel(searchModel),
                Total = lowStockProductModels.Count
            };

            return model;
        }

        #endregion

        #region Bestsellers

        /// <summary>
        /// Prepare bestseller search model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>Bestseller search model</returns>
        public virtual BestsellerSearchModel PrepareBestsellerSearchModel(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available order statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);

            //prepare available payment statuses
            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available billing countries
            searchModel.AvailableCountries = _countryService.GetAllCountriesForBilling(showHidden: true)
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged bestseller list model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>Bestseller list model</returns>
        public virtual BestsellerListModel PrepareBestsellerListModel(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter bestsellers
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            if (_workContext.CurrentVendor != null)
                searchModel.VendorId = _workContext.CurrentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get bestsellers
            var bestsellers = _orderReportService.BestSellersReport(showHidden: true,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: searchModel.BillingCountryId,
                orderBy: 2,
                vendorId: searchModel.VendorId,
                categoryId: searchModel.CategoryId,
                manufacturerId: searchModel.ManufacturerId,
                storeId: searchModel.StoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new BestsellerListModel
            {
                Data = bestsellers.Select(bestseller =>
                {
                    //fill in model values from the entity
                    var bestsellerModel = new BestsellerModel
                    {
                        ProductId = bestseller.ProductId,
                        TotalQuantity = bestseller.TotalQuantity
                    };

                    //fill in additional values (not existing in the entity)
                    bestsellerModel.ProductName = _productService.GetProductById(bestseller.ProductId)?.Name;
                    bestsellerModel.TotalAmount = _priceFormatter.FormatPrice(bestseller.TotalAmount, true, false);

                    return bestsellerModel;
                }),
                Total = bestsellers.TotalCount
            };

            return model;
        }

        #endregion

        #region NeverSold

        /// <summary>
        /// Prepare never sold report search model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>Never sold report search model</returns>
        public virtual NeverSoldReportSearchModel PrepareNeverSoldSearchModel(NeverSoldReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged never sold report list model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>Never sold report list model</returns>
        public virtual NeverSoldReportListModel PrepareNeverSoldListModel(NeverSoldReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter neverSoldReports
            if (_workContext.CurrentVendor != null)
                searchModel.SearchVendorId = _workContext.CurrentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get report items
            var items = _orderReportService.ProductsNeverSold(showHidden: true,
                vendorId: searchModel.SearchVendorId,
                storeId: searchModel.SearchStoreId,
                categoryId: searchModel.SearchCategoryId,
                manufacturerId: searchModel.SearchManufacturerId,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new NeverSoldReportListModel
            {
                //fill in model values from the entity
                Data = items.Select(item => new NeverSoldReportModel
                {
                    ProductId = item.Id,
                    ProductName = item.Name
                }),
                Total = items.TotalCount
            };

            return model;
        }

        #endregion

        #region Country sales

        /// <summary>
        /// Prepare country report search model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>Country report search model</returns>
        public virtual CountryReportSearchModel PrepareCountrySalesSearchModel(CountryReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);

            //prepare available payment statuses
            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged country report list model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>Country report list model</returns>
        public virtual CountryReportListModel PrepareCountrySalesListModel(CountryReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter countryReports
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get items
            var items = _orderReportService.GetCountryReport(os: orderStatus,
                ps: paymentStatus,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue);

            //prepare list model
            var model = new CountryReportListModel
            {
                Data = items.PaginationByRequestModel(searchModel).Select(item =>
                {
                    //fill in model values from the entity
                    var countryReportModel = new CountryReportModel
                    {
                        TotalOrders = item.TotalOrders
                    };

                    //fill in additional values (not existing in the entity)
                    countryReportModel.SumOrders = _priceFormatter.FormatPrice(item.SumOrders, true, false);
                    countryReportModel.CountryName = _countryService.GetCountryById(item.CountryId ?? 0)?.Name;

                    return countryReportModel;
                }),
                Total = items.Count
            };

            return model;
        }

        #endregion

        #region Customer reports

        /// <summary>
        /// Prepare customer reports search model
        /// </summary>
        /// <param name="searchModel">Customer reports search model</param>
        /// <returns>Customer reports search model</returns>
        public virtual CustomerReportsSearchModel PrepareCustomerReportsSearchModel(CustomerReportsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            PrepareBestCustomersReportSearchModel(searchModel.BestCustomersByOrderTotal);
            PrepareBestCustomersReportSearchModel(searchModel.BestCustomersByNumberOfOrders);
            PrepareRegisteredCustomersReportSearchModel(searchModel.RegisteredCustomers);

            return searchModel;
        }

        /// <summary>
        /// Prepare best customers report search model
        /// </summary>
        /// <param name="searchModel">Best customers report search model</param>
        /// <returns>Best customers report search model</returns>
        protected virtual BestCustomersReportSearchModel PrepareBestCustomersReportSearchModel(BestCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order, payment and shipping statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);
            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);
            _baseAdminModelFactory.PrepareShippingStatuses(searchModel.AvailableShippingStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare registered customers report search model
        /// </summary>
        /// <param name="searchModel">Registered customers report search model</param>
        /// <returns>Registered customers report search model</returns>
        protected virtual RegisteredCustomersReportSearchModel PrepareRegisteredCustomersReportSearchModel(RegisteredCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged best customers report list model
        /// </summary>
        /// <param name="searchModel">Best customers report search model</param>
        /// <returns>Best customers report list model</returns>
        public virtual BestCustomersReportListModel PrepareBestCustomersReportListModel(BestCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;

            //get report items
            var reportItems = _customerReportService.GetBestCustomersReport(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                orderBy: searchModel.OrderBy,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new BestCustomersReportListModel
            {
                Data = reportItems.Select(item =>
                {
                    //fill in model values from the entity
                    var bestCustomersReportModel = new BestCustomersReportModel
                    {
                        CustomerId = item.CustomerId,
                        OrderTotal = _priceFormatter.FormatPrice(item.OrderTotal, true, false),
                        OrderCount = item.OrderCount
                    };

                    //fill in additional values (not existing in the entity)
                    var customer = _customerService.GetCustomerById(item.CustomerId);
                    if (customer != null)
                    {
                        bestCustomersReportModel.CustomerName = customer.IsRegistered() ? customer.Email :
                            _localizationService.GetResource("Admin.Customers.Guest");
                    }

                    return bestCustomersReportModel;
                }),
                Total = reportItems.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged registered customers report list model
        /// </summary>
        /// <param name="searchModel">Registered customers report search model</param>
        /// <returns>Registered customers report list model</returns>
        public virtual RegisteredCustomersReportListModel PrepareRegisteredCustomersReportListModel(RegisteredCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get report items
            var reportItems = new List<RegisteredCustomersReportModel>
            {
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.7days"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(7)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.14days"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(14)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.month"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(30)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.year"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(365)
                }
            };

            //prepare list model
            var model = new RegisteredCustomersReportListModel
            {
                Data = reportItems.PaginationByRequestModel(searchModel),
                Total = reportItems.Count
            };

            return model;
        }

        #endregion

        #endregion
    }
}