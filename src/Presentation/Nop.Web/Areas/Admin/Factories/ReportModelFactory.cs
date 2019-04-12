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
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

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
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _customerReportService = customerReportService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderReportService = orderReportService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareLowStockProductGridModel(LowStockProductSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "products-grid",
                UrlRead = new DataUrl("LowStockList", "Report", null),
                SearchButtonId = "search-products",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.SearchPublishedId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(LowStockProductModel.Attributes))
                {
                    Visible = false
                },
                new ColumnProperty(nameof(LowStockProductModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Products.Fields.Name"),
                    Width = "300",
                    Render = new RenderCustom("renderColumnName")
                },
                new ColumnProperty(nameof(LowStockProductModel.ManageInventoryMethod))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Products.Fields.ManageInventoryMethod"),
                    Width = "150"
                },
                new ColumnProperty(nameof(LowStockProductModel.StockQuantity))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Products.Fields.StockQuantity"),
                    Width = "100"
                },
                new ColumnProperty(nameof(LowStockProductModel.Published))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Products.Fields.Published"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(LowStockProductModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareBestsellerGridModel(BestsellerSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "salesreport-grid",
                UrlRead = new DataUrl("BestsellersList", "Report", null),
                SearchButtonId = "search-salesreport",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.StartDate)),
                new FilterParameter(nameof(searchModel.EndDate)),
                new FilterParameter(nameof(searchModel.StoreId)),
                new FilterParameter(nameof(searchModel.OrderStatusId)),
                new FilterParameter(nameof(searchModel.PaymentStatusId)),
                new FilterParameter(nameof(searchModel.CategoryId)),
                new FilterParameter(nameof(searchModel.ManufacturerId)),
                new FilterParameter(nameof(searchModel.BillingCountryId)),
                new FilterParameter(nameof(searchModel.VendorId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(BestsellerModel.ProductName))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Bestsellers.Fields.Name")
                },
                new ColumnProperty(nameof(BestsellerModel.TotalQuantity))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")
                },
                new ColumnProperty(nameof(BestsellerModel.TotalAmount))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")
                },
                new ColumnProperty(nameof(BestsellerModel.ProductId))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareNeverSoldReportGridModel(NeverSoldReportSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "neversoldreport-grid",
                UrlRead = new DataUrl("NeverSoldList", "Report", null),
                SearchButtonId = "search-neversoldreport",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.StartDate)),
                new FilterParameter(nameof(searchModel.EndDate)),
                new FilterParameter(nameof(searchModel.SearchCategoryId)),
                new FilterParameter(nameof(searchModel.SearchManufacturerId)),
                new FilterParameter(nameof(searchModel.SearchStoreId)),
                new FilterParameter(nameof(searchModel.SearchVendorId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(NeverSoldReportModel.ProductName))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.NeverSold.Fields.Name")
                },
                new ColumnProperty(nameof(NeverSoldReportModel.ProductId))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareCountryReportGridModel(CountryReportSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "countryreport-grid",
                UrlRead = new DataUrl("CountrySalesList", "Report", null),
                SearchButtonId = "search-countryreport",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.StartDate)),
                new FilterParameter(nameof(searchModel.EndDate)),
                new FilterParameter(nameof(searchModel.OrderStatusId)),
                new FilterParameter(nameof(searchModel.PaymentStatusId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(CountryReportModel.CountryName))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Country.Fields.CountryName")
                },
                new ColumnProperty(nameof(CountryReportModel.TotalOrders))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Country.Fields.TotalOrders")
                },
                new ColumnProperty(nameof(CountryReportModel.SumOrders))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Sales.Country.Fields.SumOrders")
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareRegisteredCustomersReportGridModel(RegisteredCustomersReportSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "registered-customers-grid",
                UrlRead = new DataUrl("ReportRegisteredCustomersList", "Report", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = null;

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(RegisteredCustomersReportModel.Period))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Period")
                },
                new ColumnProperty(nameof(RegisteredCustomersReportModel.Customers))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.RegisteredCustomers.Fields.Customers"),
                    Width = "150"
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareBestCustomersReportByOrderTotalGridModel(BestCustomersReportSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "customers-ordertotal-grid",
                UrlRead = new DataUrl("ReportBestCustomersByOrderTotalList", "Report", null),
                SearchButtonId = "search-best-customers-ordertotal",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.OrderBy), typeof(int), 1),
                new FilterParameter(nameof(searchModel.StartDate)),
                new FilterParameter(nameof(searchModel.EndDate)),
                new FilterParameter(nameof(searchModel.OrderStatusId)),
                new FilterParameter(nameof(searchModel.PaymentStatusId)),
                new FilterParameter(nameof(searchModel.ShippingStatusId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(BestCustomersReportModel.CustomerName))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.Customer")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.OrderTotal))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.OrderTotal")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.OrderCount))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.OrderCount")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.CustomerId))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Customer/Edit/"))
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareBestCustomersReportByNumberOfOrdersGridModel(BestCustomersReportSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "best-customers-numberoforders-grid",
                UrlRead = new DataUrl("ReportBestCustomersByNumberOfOrdersList", "Report", null),
                SearchButtonId = "search-best-customers-numberoforders-grid",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.OrderBy), typeof(int), 2),
                new FilterParameter(nameof(searchModel.StartDate)),
                new FilterParameter(nameof(searchModel.EndDate)),
                new FilterParameter(nameof(searchModel.OrderStatusId)),
                new FilterParameter(nameof(searchModel.PaymentStatusId)),
                new FilterParameter(nameof(searchModel.ShippingStatusId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(BestCustomersReportModel.CustomerName))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.Customer")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.OrderTotal))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.OrderTotal")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.OrderCount))
                {
                    Title = _localizationService.GetResource("Admin.Reports.Customers.BestBy.Fields.OrderCount")
                },
                new ColumnProperty(nameof(BestCustomersReportModel.CustomerId))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Customer/Edit/"))
                }
            };

            return model;
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
            searchModel.Grid = PrepareLowStockProductGridModel(searchModel);

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

            var pagesList = lowStockProductModels.ToPagedList(searchModel);

            //prepare list model
            var model = new LowStockProductListModel().PrepareToGrid(searchModel, pagesList, () =>
            {
                return pagesList;
            });

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
            searchModel.Grid = PrepareBestsellerGridModel(searchModel);

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
            var model = new BestsellerListModel().PrepareToGrid(searchModel, bestsellers, () =>
            {
                return bestsellers.Select(bestseller =>
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
                });
            });

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
            searchModel.Grid = PrepareNeverSoldReportGridModel(searchModel);

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
            var model = new NeverSoldReportListModel().PrepareToGrid(searchModel, items, () =>
            {
                //fill in model values from the entity
                return items.Select(item => new NeverSoldReportModel
                {
                    ProductId = item.Id,
                    ProductName = item.Name
                });
            });

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
            searchModel.Grid = PrepareCountryReportGridModel(searchModel);

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
                endTimeUtc: endDateValue).ToPagedList(searchModel);

            //prepare list model
            var model = new CountryReportListModel().PrepareToGrid(searchModel, items, () =>
            {
                return items.Select(item =>
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
                });
            });

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
            searchModel.Grid = PrepareBestCustomersReportByOrderTotalGridModel(searchModel);
            searchModel.Grid = PrepareBestCustomersReportByNumberOfOrdersGridModel(searchModel);

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
            searchModel.Grid = PrepareRegisteredCustomersReportGridModel(searchModel);

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
            var model = new BestCustomersReportListModel().PrepareToGrid(searchModel, reportItems, () =>
            {
                return reportItems.Select(item =>
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
               });
            });

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

            var pagedList = reportItems.ToPagedList(searchModel);

            //prepare list model
            var model = new RegisteredCustomersReportListModel().PrepareToGrid(searchModel, pagedList, () =>
            {
                return pagedList;
            });

            return model;
        }

        #endregion

        #endregion
    }
}