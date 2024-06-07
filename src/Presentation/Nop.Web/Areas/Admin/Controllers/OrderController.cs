using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class OrderController : BaseAdminController
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExportManager _exportManager;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderModelFactory _orderModelFactory;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IPdfService _pdfService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly OrderSettings _orderSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public OrderController(IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEncryptionService encryptionService,
        IEventPublisher eventPublisher,
        IExportManager exportManager,
        IGiftCardService giftCardService,
        IImportManager importManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderModelFactory orderModelFactory,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IPdfService pdfService,
        IPermissionService permissionService,
        IPriceCalculationService priceCalculationService,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        OrderSettings orderSettings)
    {
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _encryptionService = encryptionService;
        _eventPublisher = eventPublisher;
        _exportManager = exportManager;
        _giftCardService = giftCardService;
        _importManager = importManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderModelFactory = orderModelFactory;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _paymentService = paymentService;
        _pdfService = pdfService;
        _permissionService = permissionService;
        _priceCalculationService = priceCalculationService;
        _productAttributeFormatter = productAttributeFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _orderSettings = orderSettings;
    }

    #endregion

    #region Utilities

    protected virtual async ValueTask<bool> HasAccessToOrderAsync(Order order)
    {
        return order != null && await HasAccessToOrderAsync(order.Id);
    }

    protected virtual async Task<bool> HasAccessToOrderAsync(int orderId)
    {
        if (orderId == 0)
            return false;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null)
            //not a vendor; has access
            return true;

        var vendorId = currentVendor.Id;
        var hasVendorProducts = (await _orderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

        return hasVendorProducts;
    }

    protected virtual async ValueTask<bool> HasAccessToProductAsync(OrderItem orderItem)
    {
        if (orderItem == null || orderItem.ProductId == 0)
            return false;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null)
            //not a vendor; has access
            return true;

        var vendorId = currentVendor.Id;

        return (await _productService.GetProductByIdAsync(orderItem.ProductId))?.VendorId == vendorId;
    }

    protected virtual async ValueTask<bool> HasAccessToShipmentAsync(Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        if (await _workContext.GetCurrentVendorAsync() == null)
            //not a vendor; has access
            return true;

        return await HasAccessToOrderAsync(shipment.OrderId);
    }

    protected virtual async Task LogEditOrderAsync(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        await _customerActivityService.InsertActivityAsync("EditOrder",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
    }

    #endregion

    #region Order list

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareOrderSearchModelAsync(new OrderSearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses,
            ShippingStatusIds = shippingStatuses
        });

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderList(OrderSearchModel searchModel)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareOrderListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> ReportAggregates(OrderSearchModel searchModel)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareOrderAggregatorModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("go-to-order-by-number")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> GoToOrderId(OrderSearchModel model)
    {
        var order = await _orderService.GetOrderByCustomOrderNumberAsync(model.GoDirectlyToCustomOrderNumber);

        if (order == null)
            return await List();

        return RedirectToAction("Edit", new { id = order.Id });
    }

    #endregion

    #region Export / Import

    [HttpPost, ActionName("ExportXml")]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlAll(OrderSearchModel model)
    {
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
        }

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            var xml = await _exportManager.ExportOrdersToXmlAsync(orders);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
    {
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            orders.AddRange(await (await _orderService.GetOrdersByIdsAsync(ids))
                .WhereAwait(HasAccessToOrderAsync).ToListAsync());
        }

        try
        {
            var xml = await _exportManager.ExportOrdersToXmlAsync(orders);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost, ActionName("ExportExcel")]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelAll(OrderSearchModel model)
    {
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
        }

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            var bytes = await _exportManager.ExportOrdersToXlsxAsync(orders);
            return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
    {
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            orders.AddRange(await (await _orderService.GetOrdersByIdsAsync(ids)).WhereAwait(HasAccessToOrderAsync).ToListAsync());
        }

        try
        {
            var bytes = await _exportManager.ExportOrdersToXlsxAsync(orders);
            return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
        //a vendor cannot import orders
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportOrdersFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Order details

    #region Payments and other order workflow

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("cancelorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CancelOrder(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.CancelOrderAsync(order, true);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("captureorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CaptureOrder(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.CaptureAsync(order);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("markorderaspaid")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkOrderAsPaid(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.MarkOrderAsPaidAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("refundorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RefundOrder(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.RefundAsync(order);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("refundorderoffline")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RefundOrderOffline(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.RefundOfflineAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("voidorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VoidOrder(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.VoidAsync(order);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("voidorderoffline")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VoidOrderOffline(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.VoidOfflineAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //prepare model
        var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("partialrefundorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var amountToRefund = model.AmountToRefund;
            if (amountToRefund <= decimal.Zero)
                throw new NopException("Enter amount to refund");

            var maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
            if (amountToRefund > maxAmountToRefund)
                amountToRefund = maxAmountToRefund;

            var errors = new List<string>();
            if (online)
                errors = (await _orderProcessingService.PartiallyRefundAsync(order, amountToRefund)).ToList();
            else
                await _orderProcessingService.PartiallyRefundOfflineAsync(order, amountToRefund);

            await LogEditOrderAsync(order.Id);

            if (!errors.Any())
            {
                //success
                ViewBag.RefreshPage = true;

                //prepare model
                model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                return View(model);
            }

            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return View(model);
        }
        catch (Exception exc)
        {
            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveOrderStatus")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ChangeOrderStatus(int id, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var prevOrderStatus = order.OrderStatus;

            order.OrderStatusId = model.OrderStatusId;
            await _orderService.UpdateOrderAsync(order);

            await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(order, prevOrderStatus));

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = $"Order status has been edited. New status: {await _localizationService.GetLocalizedEnumAsync(order.OrderStatus)}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    #endregion

    #region Edit, delete

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null || order.Deleted)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        await _orderProcessingService.DeleteOrderAsync(order);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteOrder",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteOrder"), order.Id), order);

        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoice(int orderId)
    {
        //a vendor should have access only to their orders
        if (!await HasAccessToOrderAsync(orderId))
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        var order = await _orderService.GetOrderByIdAsync(orderId);

        byte[] bytes;
        await using var stream = new MemoryStream();

        await _pdfService.PrintOrderToPdfAsync(stream, order, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), store: null, vendor: currentVendor);
        bytes = stream.ToArray();

        return File(bytes, MimeTypes.ApplicationPdf, string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf");
    }

    [HttpPost, ActionName("PdfInvoice")]
    [FormValueRequired("pdf-invoice-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoiceAll(OrderSearchModel model)
    {
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
        }

        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "orders.zip");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoiceSelected(string selectedIds)
    {
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            orders.AddRange(await _orderService.GetOrdersByIdsAsync(ids));
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "orders.zip");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    //currently we use this method on the add product to order details pages
    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return new NullJsonResult();

        var errors = new List<string>();
        var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);

        //conditional attributes
        var enabledAttributeMappingIds = new List<int>();
        var disabledAttributeMappingIds = new List<int>();
        if (validateAttributeConditions)
        {
            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                if (!conditionMet.HasValue)
                    continue;

                if (conditionMet.Value)
                    enabledAttributeMappingIds.Add(attribute.Id);
                else
                    disabledAttributeMappingIds.Add(attribute.Id);
            }
        }

        return Json(new
        {
            enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
            disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            message = errors.Any() ? errors.ToArray() : null
        });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveCC")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditCreditCardInfo(int id, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        if (order.AllowStoringCreditCardNumber)
        {
            var cardType = model.CardType;
            var cardName = model.CardName;
            var cardNumber = model.CardNumber;
            var cardCvv2 = model.CardCvv2;
            var cardExpirationMonth = model.CardExpirationMonth;
            var cardExpirationYear = model.CardExpirationYear;

            order.CardType = _encryptionService.EncryptText(cardType);
            order.CardName = _encryptionService.EncryptText(cardName);
            order.CardNumber = _encryptionService.EncryptText(cardNumber);
            order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
            order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
            order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
            order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
            await _orderService.UpdateOrderAsync(order);
        }

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Credit card info has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveOrderTotals")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditOrderTotals(int id, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
        order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
        order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
        order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
        order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
        order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
        order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
        order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
        order.TaxRates = model.TaxRatesValue;
        order.OrderTax = model.TaxValue;
        order.OrderDiscount = model.OrderTotalDiscountValue;
        order.OrderTotal = model.OrderTotalValue;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Order totals have been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("save-shipping-method")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditShippingMethod(int id, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        order.ShippingMethod = model.ShippingMethod;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Shipping method has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-billing-shipping");

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditOrderItem(int id, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //get order item identifier
        var orderItemId = 0;
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnSaveOrderItem".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        if (!decimal.TryParse(form["pvUnitPriceInclTax" + orderItemId], out var unitPriceInclTax))
            unitPriceInclTax = orderItem.UnitPriceInclTax;
        if (!decimal.TryParse(form["pvUnitPriceExclTax" + orderItemId], out var unitPriceExclTax))
            unitPriceExclTax = orderItem.UnitPriceExclTax;
        if (!int.TryParse(form["pvQuantity" + orderItemId], out var quantity))
            quantity = orderItem.Quantity;
        if (!decimal.TryParse(form["pvDiscountInclTax" + orderItemId], out var discountInclTax))
            discountInclTax = orderItem.DiscountAmountInclTax;
        if (!decimal.TryParse(form["pvDiscountExclTax" + orderItemId], out var discountExclTax))
            discountExclTax = orderItem.DiscountAmountExclTax;
        if (!decimal.TryParse(form["pvPriceInclTax" + orderItemId], out var priceInclTax))
            priceInclTax = orderItem.PriceInclTax;
        if (!decimal.TryParse(form["pvPriceExclTax" + orderItemId], out var priceExclTax))
            priceExclTax = orderItem.PriceExclTax;

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

        if (quantity > 0)
        {
            var qtyDifference = orderItem.Quantity - quantity;

            if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
            {
                orderItem.UnitPriceInclTax = unitPriceInclTax;
                orderItem.UnitPriceExclTax = unitPriceExclTax;
                orderItem.Quantity = quantity;
                orderItem.DiscountAmountInclTax = discountInclTax;
                orderItem.DiscountAmountExclTax = discountExclTax;
                orderItem.PriceInclTax = priceInclTax;
                orderItem.PriceExclTax = priceExclTax;
                await _orderService.UpdateOrderItemAsync(orderItem);
            }

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, qtyDifference, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
        }
        else
        {
            //adjust inventory
            await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

            //delete item
            await _orderService.DeleteOrderItemAsync(orderItem);
        }

        //update order totals
        var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
        {
            PriceInclTax = unitPriceInclTax,
            PriceExclTax = unitPriceExclTax,
            DiscountAmountInclTax = discountInclTax,
            DiscountAmountExclTax = discountExclTax,
            SubTotalInclTax = priceInclTax,
            SubTotalExclTax = priceExclTax,
            Quantity = quantity
        };
        await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Order item has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        foreach (var warning in updateOrderParameters.Warnings)
            _notificationService.WarningNotification(warning);

        //selected card
        SaveSelectedCardName("order-products");

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteOrderItem(int id, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //get order item identifier
        var orderItemId = 0;
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnDeleteOrderItem".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        if ((await _giftCardService.GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Any())
        {
            //we cannot delete an order item with associated gift cards
            //a store owner should delete them first

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"));
        }
        else
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

            //delete item
            await _orderService.DeleteOrderItemAsync(orderItem);

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem);
            await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);
        }

        //selected card
        SaveSelectedCardName("order-products");

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ResetDownloadCount(int id, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //get order item identifier
        var orderItemId = 0;
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnResetDownloadCount".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        orderItem.DownloadCount = 0;
        await _orderService.UpdateOrderItemAsync(orderItem);
        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-products");

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ActivateDownloadItem(int id, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //get order item identifier
        var orderItemId = 0;
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnPvActivateDownload".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-products");

        return RedirectToAction("Edit", new { id = order.Id });
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UploadLicenseFilePopup(int id, int orderItemId)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //try to get an order item with the specified id
        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId)
            ?? throw new ArgumentException("No product found with the specified order item id");

        if (!product.IsDownload)
            throw new ArgumentException("Product is not downloadable");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareUploadLicenseModelAsync(new UploadLicenseModel(), order, orderItem);

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("uploadlicense")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UploadLicenseFilePopup(UploadLicenseModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        var orderItem = await _orderService.GetOrderItemByIdAsync(model.OrderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //attach license
        if (model.LicenseDownloadId > 0)
            orderItem.LicenseDownloadId = model.LicenseDownloadId;
        else
            orderItem.LicenseDownloadId = null;

        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //success
        ViewBag.RefreshPage = true;

        return View(model);
    }

    [HttpPost, ActionName("UploadLicenseFilePopup")]
    [FormValueRequired("deletelicense")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteLicenseFilePopup(UploadLicenseModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        var orderItem = await _orderService.GetOrderItemByIdAsync(model.OrderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //attach license
        orderItem.LicenseDownloadId = null;

        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //success
        ViewBag.RefreshPage = true;

        return View(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrder(int orderId)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderSearchModelAsync(new AddProductToOrderSearchModel(), order);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrder(AddProductToOrderSearchModel searchModel)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderListModelAsync(searchModel, order);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId, IFormCollection form)
    {
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //basic properties
        _ = decimal.TryParse(form["UnitPriceInclTax"], out var unitPriceInclTax);
        _ = decimal.TryParse(form["UnitPriceExclTax"], out var unitPriceExclTax);
        _ = int.TryParse(form["Quantity"], out var quantity);
        _ = decimal.TryParse(form["SubTotalInclTax"], out var priceInclTax);
        _ = decimal.TryParse(form["SubTotalExclTax"], out var priceExclTax);

        //warnings
        var warnings = new List<string>();

        //attributes
        var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, warnings);

        //rental product
        _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

        //warnings
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType.ShoppingCart, product, attributesXml));
        warnings.AddRange(await _shoppingCartService.GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));
        if (!warnings.Any())
        {
            //no errors
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //attributes
            var attributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product, attributesXml, customer, currentStore);

            //weight
            var itemWeight = await _shippingService.GetShoppingCartItemWeightAsync(product, attributesXml);

            //save item
            var orderItem = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                UnitPriceInclTax = unitPriceInclTax,
                UnitPriceExclTax = unitPriceExclTax,
                PriceInclTax = priceInclTax,
                PriceExclTax = priceExclTax,
                OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, attributesXml),
                AttributeDescription = attributeDescription,
                AttributesXml = attributesXml,
                Quantity = quantity,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = itemWeight,
                RentalStartDateUtc = rentalStartDate,
                RentalEndDateUtc = rentalEndDate
            };

            await _orderService.InsertOrderItemAsync(orderItem);

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, -orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
            {
                PriceInclTax = unitPriceInclTax,
                PriceExclTax = unitPriceExclTax,
                SubTotalInclTax = priceInclTax,
                SubTotalExclTax = priceExclTax,
                Quantity = quantity
            };
            await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A new order item has been added",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //gift cards
            if (product.IsGiftCard)
            {
                _productAttributeParser.GetGiftCardAttribute(
                    attributesXml, out var recipientName, out var recipientEmail, out var senderName, out var senderEmail, out var giftCardMessage);

                for (var i = 0; i < orderItem.Quantity; i++)
                {
                    var gc = new GiftCard
                    {
                        GiftCardType = product.GiftCardType,
                        PurchasedWithOrderItemId = orderItem.Id,
                        Amount = unitPriceExclTax,
                        IsGiftCardActivated = false,
                        GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                        RecipientName = recipientName,
                        RecipientEmail = recipientEmail,
                        SenderName = senderName,
                        SenderEmail = senderEmail,
                        Message = giftCardMessage,
                        IsRecipientNotified = false,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _giftCardService.InsertGiftCardAsync(gc);
                }
            }

            //redirect to order details page
            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);

            //selected card
            SaveSelectedCardName("order-products");
            return RedirectToAction("Edit", new { id = order.Id });
        }

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);
        model.Warnings.AddRange(warnings);

        return View(model);
    }

    #endregion

    #endregion

    #region Addresses

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(int addressId, int orderId)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(addressId)
            ?? throw new ArgumentException("No address found with the specified id", nameof(addressId));

        //prepare model
        var model = await _orderModelFactory.PrepareOrderAddressModelAsync(new OrderAddressModel(), order, address);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(OrderAddressModel model, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(model.Address.Id)
            ?? throw new ArgumentException("No address found with the specified id");

        //custom address attributes
        var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
        foreach (var error in customAttributeWarnings)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (ModelState.IsValid)
        {
            address = model.Address.ToEntity(address);
            address.CustomAttributes = customAttributes;
            await _addressService.UpdateAddressAsync(address);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Address has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

            return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
        }

        //prepare model
        model = await _orderModelFactory.PrepareOrderAddressModelAsync(model, order, address);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion

    #region Shipments

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentList()
    {
        //prepare model
        var model = await _orderModelFactory.PrepareShipmentSearchModelAsync(new ShipmentSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentListSelect(ShipmentSearchModel searchModel)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareShipmentListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentsByOrder(OrderShipmentSearchModel searchModel)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderShipmentListModelAsync(searchModel, order);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentsItemsByShipmentId(ShipmentItemSearchModel searchModel)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(searchModel.ShipmentId)
            ?? throw new ArgumentException("No shipment found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && !await HasAccessToShipmentAsync(shipment))
            return Content(string.Empty);

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor should have access only to his products
        if (currentVendor != null && !await HasAccessToOrderAsync(order))
            return Content(string.Empty);

        //prepare model
        searchModel.SetGridPageSize();
        var model = await _orderModelFactory.PrepareShipmentItemListModelAsync(searchModel, shipment);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddShipment(int id)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareShipmentModelAsync(new ShipmentModel(), null, order);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddShipment(ShipmentModel model, IFormCollection form, bool continueEditing)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);
        //a vendor should have access only to his products
        if (currentVendor != null)
        {
            orderItems = await orderItems.WhereAwait(HasAccessToProductAsync).ToListAsync();
        }

        var shipment = new Shipment
        {
            OrderId = order.Id,
            TrackingNumber = model.TrackingNumber,
            TotalWeight = null,
            AdminComment = model.AdminComment,
            CreatedOnUtc = DateTime.UtcNow
        };

        var shipmentItems = new List<ShipmentItem>();

        decimal? totalWeight = null;

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            //ensure that this product can be shipped (have at least one item to ship)
            var maxQtyToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
            if (maxQtyToAdd <= 0)
                continue;

            var qtyToAdd = 0; //parse quantity
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out qtyToAdd);
                    break;
                }

            var warehouseId = 0;
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.UseMultipleWarehouses)
            {
                //multiple warehouses supported
                //warehouse is chosen by a store owner
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(form[formKey], out warehouseId);
                        break;
                    }
            }
            else
            {
                //multiple warehouses are not supported
                warehouseId = product.WarehouseId;
            }

            //validate quantity
            if (qtyToAdd <= 0)
                continue;
            if (qtyToAdd > maxQtyToAdd)
                qtyToAdd = maxQtyToAdd;

            //ok. we have at least one item. let's create a shipment (if it does not exist)

            var orderItemTotalWeight = orderItem.ItemWeight * qtyToAdd;
            if (orderItemTotalWeight.HasValue)
            {
                if (!totalWeight.HasValue)
                    totalWeight = 0;
                totalWeight += orderItemTotalWeight.Value;
            }

            //create a shipment item
            shipmentItems.Add(new ShipmentItem
            {
                OrderItemId = orderItem.Id,
                Quantity = qtyToAdd,
                WarehouseId = warehouseId
            });
        }

        //if we have at least one item in the shipment, then save it
        if (shipmentItems.Any())
        {
            shipment.TotalWeight = totalWeight;
            await _shipmentService.InsertShipmentAsync(shipment);

            foreach (var shipmentItem in shipmentItems)
            {
                shipmentItem.ShipmentId = shipment.Id;
                await _shipmentService.InsertShipmentItemAsync(shipmentItem);
            }

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been added",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

            if (!string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                await _eventPublisher.PublishAsync(new ShipmentTrackingNumberSetEvent(shipment));

            var canShip = !order.PickupInStore && model.CanShip;
            if (canShip)
                await _orderProcessingService.ShipAsync(shipment, true);

            var canMarkAsReadyForPickup = order.PickupInStore && model.CanMarkAsReadyForPickup;
            if (canMarkAsReadyForPickup)
                await _orderProcessingService.ReadyForPickupAsync(shipment, true);

            if ((canShip || canMarkAsReadyForPickup) && model.CanDeliver)
                await _orderProcessingService.DeliverAsync(shipment, true);

            await LogEditOrderAsync(order.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Added"));
            return continueEditing
                ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
                : RedirectToAction("Edit", new { id = model.OrderId });
        }

        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.NoProductsSelected"));

        return RedirectToAction("AddShipment", model);
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentDetails(int id)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareShipmentModelAsync(null, shipment, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteShipment(int id)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
        {
            var orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
            if (orderItem == null)
                continue;

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            await _productService.ReverseBookedInventoryAsync(product, shipmentItem,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
        }

        var orderId = shipment.OrderId;
        await _shipmentService.DeleteShipmentAsync(shipment);

        var order = await _orderService.GetOrderByIdAsync(orderId);
        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "A shipment has been deleted",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Deleted"));
        return RedirectToAction("Edit", new { id = orderId });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("settrackingnumber")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetTrackingNumber(ShipmentModel model)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        if (shipment.TrackingNumber == model.TrackingNumber)
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });

        shipment.TrackingNumber = model.TrackingNumber;
        await _shipmentService.UpdateShipmentAsync(shipment);

        await _eventPublisher.PublishAsync(new ShipmentTrackingNumberSetEvent(shipment));

        return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setadmincomment")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetShipmentAdminComment(ShipmentModel model)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        shipment.AdminComment = model.AdminComment;
        await _shipmentService.UpdateShipmentAsync(shipment);

        return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasshipped")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsShipped(int id)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.ShipAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            //error
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("saveshippeddate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditShippedDate(ShipmentModel model)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.ShippedDateUtc.HasValue)
            {
                throw new Exception("Enter shipped date");
            }

            shipment.ShippedDateUtc = model.ShippedDateUtc;
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            //error
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasreadyforpickup")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsReadyForPickup(int id)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.ReadyForPickupAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            //error
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("savereadyforpickupdate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditReadyForPickupDate(ShipmentModel model)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.ReadyForPickupDateUtc.HasValue)
                throw new Exception("Enter ready for pickup date");

            shipment.ReadyForPickupDateUtc = model.ReadyForPickupDateUtc;
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasdelivered")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsDelivered(int id)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.DeliverAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            //error
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("savedeliverydate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditDeliveryDate(ShipmentModel model)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.DeliveryDateUtc.HasValue)
            {
                throw new Exception("Enter delivery date");
            }

            shipment.DeliveryDateUtc = model.DeliveryDateUtc;
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
            //error
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlip(int shipmentId)
    {
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        byte[] bytes;
        await using (var stream = new MemoryStream())
        {
            await _pdfService.PrintPackagingSlipToPdfAsync(stream, shipment, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
            bytes = stream.ToArray();
        }

        return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
    }

    [HttpPost, ActionName("ShipmentList")]
    [FormValueRequired("exportpackagingslips-all")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlipAll(ShipmentSearchModel model)
    {
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var vendorId = 0;
        if (currentVendor != null)
            vendorId = currentVendor.Id;

        //load shipments
        var shipments = await _shipmentService.GetAllShipmentsAsync(vendorId: vendorId,
            warehouseId: model.WarehouseId,
            shippingCountryId: model.CountryId,
            shippingStateId: model.StateProvinceId,
            shippingCounty: model.County,
            shippingCity: model.City,
            trackingNumber: model.TrackingNumber,
            loadNotShipped: model.LoadNotShipped,
            loadNotReadyForPickup: model.LoadNotReadyForPickup,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue);

        //ensure that we at least one shipment selected
        if (!shipments.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.NoShipmentsSelected"));
            return RedirectToAction("ShipmentList");
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "packagingslips.zip");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentList");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlipSelected(string selectedIds)
    {
        var shipments = new List<Shipment>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            shipments.AddRange(await _shipmentService.GetShipmentsByIdsAsync(ids));
        }
        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "packagingslips.zip");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("ShipmentList");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsShippedSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.ShipAsync(shipment, true);
            }
            catch
            {
                //ignore any exception
            }
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsReadyForPickupSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.ReadyForPickupAsync(shipment, true);
            }
            catch
            {
                //ignore any exception
            }
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsDeliveredSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.DeliverAsync(shipment, true);
            }
            catch
            {
                //ignore any exception
            }
        }

        return Json(new { Result = true });
    }

    #endregion

    #region Order notes

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderNotesSelect(OrderNoteSearchModel searchModel)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderNoteListModelAsync(searchModel, order);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
    {
        if (string.IsNullOrEmpty(message))
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Orders.OrderNotes.Fields.Note.Validation"));

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return ErrorJson("Order cannot be loaded");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return ErrorJson("No access for vendors");

        var orderNote = new OrderNote
        {
            OrderId = order.Id,
            DisplayToCustomer = displayToCustomer,
            Note = message,
            DownloadId = downloadId,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _orderService.InsertOrderNoteAsync(orderNote);

        //new order notification
        if (displayToCustomer)
        {
            //email
            await _workflowMessageService.SendNewOrderNoteAddedCustomerNotificationAsync(orderNote, (await _workContext.GetWorkingLanguageAsync()).Id);
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OrderNoteDelete(int id, int orderId)
    {
        //try to get an order with the specified id
        _ = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an order note with the specified id
        var orderNote = await _orderService.GetOrderNoteByIdAsync(id)
            ?? throw new ArgumentException("No order note found with the specified id");

        await _orderService.DeleteOrderNoteAsync(orderNote);

        return new NullJsonResult();
    }

    #endregion

    #region Reports

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> BestsellersBriefReportByQuantityList(BestsellerBriefSearchModel searchModel)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> BestsellersBriefReportByAmountList(BestsellerBriefSearchModel searchModel)
    {
        //prepare model
        var model = await _orderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderAverageReportList(OrderAverageReportSearchModel searchModel)
    {
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderAverageReportListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderIncompleteReportList(OrderIncompleteReportSearchModel searchModel)
    {
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderIncompleteReportListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> LoadOrderStatistics(string period)
    {
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        var result = new List<object>();

        var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
        var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();

        var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

        switch (period)
        {
            case "year":
                //year statistics
                var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                for (var i = 0; i <= 12; i++)
                {
                    result.Add(new
                    {
                        date = searchYearDateUser.Date.ToString("Y", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchYearDateUser = searchYearDateUser.AddMonths(1);
                }

                break;
            case "month":
                //month statistics
                var monthAgoDt = nowDt.AddDays(-30);
                var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                for (var i = 0; i <= 30; i++)
                {
                    result.Add(new
                    {
                        date = searchMonthDateUser.Date.ToString("M", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchMonthDateUser = searchMonthDateUser.AddDays(1);
                }

                break;
            case "week":
            default:
                //week statistics
                var weekAgoDt = nowDt.AddDays(-7);
                var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                for (var i = 0; i <= 7; i++)
                {
                    result.Add(new
                    {
                        date = searchWeekDateUser.Date.ToString("d dddd", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchWeekDateUser = searchWeekDateUser.AddDays(1);
                }

                break;
        }

        return Json(result);
    }

    #endregion
}
