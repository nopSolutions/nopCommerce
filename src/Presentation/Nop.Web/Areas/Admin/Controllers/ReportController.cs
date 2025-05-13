using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ReportController : BaseAdminController
{
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly ICustomerReportService _customerReportService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExportManager _exportManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderReportService _orderReportService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IReportModelFactory _reportModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ReportController(
        ICountryService countryService,
        ICustomerReportService customerReportService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IExportManager exportManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderReportService orderReportService,
        IPermissionService permissionService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IReportModelFactory reportModelFactory,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _countryService = countryService; 
        _customerReportService = customerReportService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _exportManager = exportManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderReportService = orderReportService;
        _permissionService = permissionService;
        _priceFormatter = priceFormatter;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _reportModelFactory = reportModelFactory;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    #region Sales summary

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummary(List<int> orderStatuses = null, List<int> paymentStatuses = null)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummarySearchModelAsync(new SalesSummarySearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses
        });

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummaryList(SalesSummarySearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummaryListModelAsync(searchModel);

        return Json(model);
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> ExportSalesSummaryXmlAll(SalesSummarySearchModel searchModel)
    {
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();

        var currentVendor = await _workContext.GetCurrentVendorAsync();

        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var salesSummary = await _orderReportService.SalesSummaryReportAsync(
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            billingCountryId: searchModel.BillingCountryId,
            groupBy: (GroupByOptions)searchModel.SearchGroupId,
            categoryId: searchModel.CategoryId,
            productId: searchModel.ProductId,
            manufacturerId: searchModel.ManufacturerId,
            vendorId: currentVendor?.Id ?? searchModel.VendorId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        try
        {
            var xml = await _exportManager.ExportSalesSummaryToXmlAsync(salesSummary);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "SalesSummary.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("SalesSummary");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> ExportSalesSummaryExcelAll(SalesSummarySearchModel searchModel)
    {
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();

        var currentVendor = await _workContext.GetCurrentVendorAsync();

        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var salesSummary = await _orderReportService.SalesSummaryReportAsync(
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            billingCountryId: searchModel.BillingCountryId,
            groupBy: (GroupByOptions)searchModel.SearchGroupId,
            categoryId: searchModel.CategoryId,
            productId: searchModel.ProductId,
            manufacturerId: searchModel.ManufacturerId,
            vendorId: currentVendor?.Id ?? searchModel.VendorId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        try
        {
            var bytes = await _exportManager.ExportSalesSummaryToXlsxAsync(salesSummary);
            return File(bytes, MimeTypes.TextXlsx, "SalesSummary.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("SalesSummary");
        }
    }


    #endregion

    #region Low stock

    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStock()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductListModelAsync(searchModel);

        return Json(model);
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ExportLowStockXmlAll(LowStockProductSearchModel searchModel)
    {
        var publishedOnly = searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1 ? true : (bool?)false;
        var vendor = await _workContext.GetCurrentVendorAsync();
        var vendorId = vendor?.Id ?? 0;

        var products = await _productService.GetLowStockProductsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);
        var combinations = await _productService.GetLowStockProductCombinationsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);

        var lowStockProducts = new List<LowStockProductReportLine>();
        lowStockProducts.AddRange(await products.SelectAwait(async product => new LowStockProductReportLine
        {
            Id = product.Id,
            Name = product.Name,

            ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),
            StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
            Published = product.Published
        }).ToListAsync());

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        lowStockProducts.AddRange(await combinations.SelectAwait(async combination =>
        {
            var product = await _productService.GetProductByIdAsync(combination.ProductId);

            return new LowStockProductReportLine
            {
                Id = combination.ProductId,
                Name = product.Name,

                Attributes = await _productAttributeFormatter
                    .FormatAttributesAsync(product, combination.AttributesXml, currentCustomer, currentStore, "<br />", true, true, true, false),
                ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),

                StockQuantity = combination.StockQuantity,
                Published = product.Published
            };
        }).ToListAsync());

        try
        {
            var xml = await _exportManager.ExportLowStockToXmlAsync(lowStockProducts);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "LowStock.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("LowStock");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ExportLowStockExcelAll(LowStockProductSearchModel searchModel)
    {
        var publishedOnly = searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1 ? true : (bool?)false;
        var vendor = await _workContext.GetCurrentVendorAsync();
        var vendorId = vendor?.Id ?? 0;

        var products = await _productService.GetLowStockProductsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);
        var combinations = await _productService.GetLowStockProductCombinationsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);

        var lowStockProducts = new List<LowStockProductReportLine>();
        lowStockProducts.AddRange(await products.SelectAwait(async product => new LowStockProductReportLine
        {
            Id = product.Id,
            Name = product.Name,

            ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),
            StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
            Published = product.Published
        }).ToListAsync());

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        lowStockProducts.AddRange(await combinations.SelectAwait(async combination =>
        {
            var product = await _productService.GetProductByIdAsync(combination.ProductId);

            return new LowStockProductReportLine
            {
                Id = combination.ProductId,
                Name = product.Name,

                Attributes = await _productAttributeFormatter
                    .FormatAttributesAsync(product, combination.AttributesXml, currentCustomer, currentStore, "<br />", true, true, true, false),
                ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),

                StockQuantity = combination.StockQuantity,
                Published = product.Published
            };
        }).ToListAsync());

        try
        {
            var bytes = await _exportManager.ExportLowStockToXlsxAsync(lowStockProducts);
            return File(bytes, MimeTypes.TextXlsx, "LowStock.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("LowStock");
        }
    }

    #endregion

    #region Bestsellers

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> Bestsellers()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
    {
        //prepare model
        var totalAmount = await _reportModelFactory.GetBestsellerTotalAmountAsync(searchModel);

        return Json(new { aggregatortotal = totalAmount });
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> ExportBestSellersXmlAll(BestsellerSearchModel searchModel)
    {
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            billingCountryId: searchModel.BillingCountryId,
            orderBy: OrderByEnum.OrderByTotalAmount,
            vendorId: searchModel.VendorId,
            categoryId: searchModel.CategoryId,
            manufacturerId: searchModel.ManufacturerId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
        
        var models = await bestsellers.SelectAwait(async item =>
            {
                item.TotalAmountStr= await _priceFormatter.FormatPriceAsync(item.TotalAmount, true, false);
                return item;
            }
        ).ToListAsync();

        try
        {
            var xml = await _exportManager.ExportBestSellersToXmlAsync(models);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "BestSellers.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestSellers");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> ExportBestSellersExcelAll(BestsellerSearchModel searchModel)
    {
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            billingCountryId: searchModel.BillingCountryId,
            orderBy: OrderByEnum.OrderByTotalAmount,
            vendorId: searchModel.VendorId,
            categoryId: searchModel.CategoryId,
            manufacturerId: searchModel.ManufacturerId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
        
        var models = await bestsellers.SelectAwait(async item =>
            {
                item.TotalAmountStr= await _priceFormatter.FormatPriceAsync(item.TotalAmount, true, false);
                return item;
            }
        ).ToListAsync();

        try
        {
            var bytes = await _exportManager.ExportBestSellersToXlsxAsync(models);
            return File(bytes, MimeTypes.TextXlsx, "BestSellers.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestSellers");
        }
    }

    #endregion

    #region Never Sold

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSold()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldListModelAsync(searchModel);

        return Json(model);
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> ExportNeverSoldXmlAll(NeverSoldReportSearchModel searchModel)
    {
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var items = await _orderReportService.ProductsNeverSoldAsync(showHidden: true,
            vendorId: searchModel.SearchVendorId,
            storeId: searchModel.SearchStoreId,
            categoryId: searchModel.SearchCategoryId,
            manufacturerId: searchModel.SearchManufacturerId,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        try
        {
            var xml = await _exportManager.ExportNeverSoldToXmlAsync(items);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "NeverPurchased.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("NeverSold");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> ExportNeverSoldExcelAll(NeverSoldReportSearchModel searchModel)
    {
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var items = await _orderReportService.ProductsNeverSoldAsync(showHidden: true,
            vendorId: searchModel.SearchVendorId,
            storeId: searchModel.SearchStoreId,
            categoryId: searchModel.SearchCategoryId,
            manufacturerId: searchModel.SearchManufacturerId,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        try
        {
            var bytes = await _exportManager.ExportNeverSoldToXlsxAsync(items);
            return File(bytes, MimeTypes.TextXlsx, "NeverPurchased.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("NeverSold");
        }
    }

    #endregion

    #region Country sales

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySales()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesListModelAsync(searchModel);

        return Json(model);
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> ExportCountrySalesXmlAll(CountryReportSearchModel searchModel)
    {
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var items = await _orderReportService.GetCountryReportAsync(os: orderStatus,
            ps: paymentStatus,
            storeId: searchModel.SearchStoreId,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue);

        var models = await items.SelectAwait(async item =>
            {
                item.SumOrdersStr = await _priceFormatter.FormatPriceAsync(item.SumOrders, true, false);
                item.CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId ?? 0))?.Name;

                return item;
            }
        ).ToListAsync();

        try
        {
            var xml = await _exportManager.ExportCountrySalesToXmlAsync(models);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "CountrySales.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("CountrySales");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> ExportCountrySalesExcelAll(CountryReportSearchModel searchModel)
    {
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var items = await _orderReportService.GetCountryReportAsync(os: orderStatus,
            ps: paymentStatus,
            storeId: searchModel.SearchStoreId,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue);
            
        var models = await items.SelectAwait(async item =>
            {
                item.SumOrdersStr = await _priceFormatter.FormatPriceAsync(item.SumOrders, true, false);
                item.CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId ?? 0))?.Name;

                return item;
            }
        ).ToListAsync();

        try
        {
            var bytes = await _exportManager.ExportCountrySalesToXlsxAsync(models);
            return File(bytes, MimeTypes.TextXlsx, "CountrySales.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("CountrySales");
        }
    }

    #endregion

    #region Customer reports

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> RegisteredCustomers()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> BestCustomersByOrderTotal()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> BestCustomersByNumberOfOrders()
    {
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
    {
        //prepare model
        var model = await _reportModelFactory.PrepareRegisteredCustomersReportListModelAsync(searchModel);

        return Json(model);
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportRegisteredCustomersXmlAll(RegisteredCustomersReportSearchModel searchModel)
    {
        var reportItems = new List<RegisteredCustomersReportLine>
        {
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.7days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(7)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.14days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(14)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.month"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(30)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.year"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(365)
            }
        };

        try
        {
            var xml = await _exportManager.ExportRegisteredCustomersToXmlAsync(reportItems);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "RegisteredCustomers.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("RegisteredCustomers");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportRegisteredCustomersExcelAll(CountryReportSearchModel searchModel)
    {
        var reportItems = new List<RegisteredCustomersReportLine>
        {
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.7days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(7)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.14days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(14)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.month"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(30)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.year"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(365)
            }
        };

        try
        {
            var bytes = await _exportManager.ExportRegisteredCustomersToXlsxAsync(reportItems);
            return File(bytes, MimeTypes.TextXlsx, "RegisteredCustomers.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("RegisteredCustomers");
        }
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportBestCustomersByOrderTotalXmlAll(CustomerReportsSearchModel search)
    {
        var searchModel = search.BestCustomersByOrderTotal;
        
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;

        var reportItems = await _customerReportService.GetBestCustomersReportAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            ss: shippingStatus,
            orderBy: OrderByEnum.OrderByTotalAmount,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var models = await reportItems.SelectAwait(async item =>
                {
                    var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = (await _customerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    }
                    item.OrderTotalStr = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false);
                    return item;
                }
            ).ToListAsync();

        try
        {
            var xml = await _exportManager.ExportBestCustomersByOrderTotalToXmlAsync(models);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "BestCustomersByOrderTotal.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestCustomersByOrderTotal");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportBestCustomersByOrderTotalExcelAll(CustomerReportsSearchModel search)
    {
        var searchModel = search.BestCustomersByOrderTotal;
        
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;

        var reportItems = await _customerReportService.GetBestCustomersReportAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            ss: shippingStatus,
            orderBy: OrderByEnum.OrderByTotalAmount,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            
        var models = await reportItems.SelectAwait(async item =>
                {
                    var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = (await _customerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    }
                    
                    item.OrderTotalStr = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false);
                    return item;
                }
            ).ToListAsync();

        try
        {
            var bytes = await _exportManager.ExportBestCustomersByOrderTotalToXlsxAsync(models);
            return File(bytes, MimeTypes.TextXlsx, "BestCustomersByOrderTotal.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestCustomersByOrderTotal");
        }
    }


    [HttpPost]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportBestCustomersByNumberOfOrdersXmlAll(CustomerReportsSearchModel search)
    {
        var searchModel = search.BestCustomersByNumberOfOrders;
        
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;
        
        var reportItems = await _customerReportService.GetBestCustomersReportAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            ss: shippingStatus,
            orderBy: OrderByEnum.OrderByQuantity,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            
        var models = await reportItems.SelectAwait(async item =>
                {
                    var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = (await _customerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    }
                    
                    item.OrderTotalStr = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false);
                    return item;
                }
            ).ToListAsync();

        try
        {
            var xml = await _exportManager.ExportBestCustomersByNumberOfOrdersToXmlAsync(models);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "BestCustomersByNumberOfOrders.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestCustomersByNumberOfOrders");
        }
    }


    [HttpPost]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ExportBestCustomersByNumberOfOrdersExcelAll(CustomerReportsSearchModel search)
    {
        var searchModel = search.BestCustomersByNumberOfOrders;
        
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
        var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;
        
        var reportItems = await _customerReportService.GetBestCustomersReportAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            ss: shippingStatus,
            orderBy: OrderByEnum.OrderByQuantity,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            
        var models = await reportItems.SelectAwait(async item =>
                {
                    //fill in additional values (not existing in the entity)
                    var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = (await _customerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    }
                    
                    item.OrderTotalStr = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false);
                    return item;
                }
            ).ToListAsync();

        try
        {

            var bytes = await _exportManager.ExportBestCustomersByNumberOfOrdersToXlsxAsync(models);
            return File(bytes, MimeTypes.TextXlsx, "BestCustomersByNumberOfOrders.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("BestCustomersByNumberOfOrders");
        }
    }

    #endregion

    #endregion
}