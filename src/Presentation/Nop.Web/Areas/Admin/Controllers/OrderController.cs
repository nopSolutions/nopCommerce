using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class OrderController : BaseAdminController
    {
        #region Fields

        protected IAddressAttributeParser AddressAttributeParser { get; }
        protected IAddressService AddressService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IDownloadService DownloadService { get; }
        protected IEncryptionService EncryptionService { get; }
        protected IExportManager ExportManager { get; }
        protected IGiftCardService GiftCardService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IOrderModelFactory OrderModelFactory { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderService OrderService { get; }
        protected IPaymentService PaymentService { get; }
        protected IPdfService PdfService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPriceCalculationService PriceCalculationService { get; }
        protected IProductAttributeFormatter ProductAttributeFormatter { get; }
        protected IProductAttributeParser ProductAttributeParser { get; }
        protected IProductAttributeService ProductAttributeService { get; }
        protected IProductService ProductService { get; }
        protected IShipmentService ShipmentService { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected OrderSettings OrderSettings { get; }

        #endregion

        #region Ctor

        public OrderController(IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IEncryptionService encryptionService,
            IExportManager exportManager,
            IGiftCardService giftCardService,
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
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            OrderSettings orderSettings)
        {
            AddressAttributeParser = addressAttributeParser;
            AddressService = addressService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            DownloadService = downloadService;
            EncryptionService = encryptionService;
            ExportManager = exportManager;
            GiftCardService = giftCardService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            OrderModelFactory = orderModelFactory;
            OrderProcessingService = orderProcessingService;
            OrderService = orderService;
            PaymentService = paymentService;
            PdfService = pdfService;
            PermissionService = permissionService;
            PriceCalculationService = priceCalculationService;
            ProductAttributeFormatter = productAttributeFormatter;
            ProductAttributeParser = productAttributeParser;
            ProductAttributeService = productAttributeService;
            ProductService = productService;
            ShipmentService = shipmentService;
            ShippingService = shippingService;
            ShoppingCartService = shoppingCartService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            OrderSettings = orderSettings;
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

            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;
            var hasVendorProducts = (await OrderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

            return hasVendorProducts;
        }

        protected virtual async ValueTask<bool> HasAccessToProductAsync(OrderItem orderItem)
        {
            if (orderItem == null || orderItem.ProductId == 0)
                return false;

            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;

            return (await ProductService.GetProductByIdAsync(orderItem.ProductId))?.VendorId == vendorId;
        }

        protected virtual async ValueTask<bool> HasAccessToShipmentAsync(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (await WorkContext.GetCurrentVendorAsync() == null)
                //not a vendor; has access
                return true;

            return await HasAccessToOrderAsync(shipment.OrderId);
        }

        protected virtual async Task LogEditOrderAsync(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);

            await CustomerActivityService.InsertActivityAsync("EditOrder",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
        }

        #endregion

        #region Order list

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await OrderModelFactory.PrepareOrderSearchModelAsync(new OrderSearchModel
            {
                OrderStatusIds = orderStatuses ?? new List<int>(),
                PaymentStatusIds = paymentStatuses ?? new List<int>(),
                ShippingStatusIds = shippingStatuses ?? new List<int>()
            });

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderList(OrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await OrderModelFactory.PrepareOrderListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportAggregates(OrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await OrderModelFactory.PrepareOrderAggregatorModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public virtual async Task<IActionResult> GoToOrderId(OrderSearchModel model)
        {
            var order = await OrderService.GetOrderByCustomOrderNumberAsync(model.GoDirectlyToCustomOrderNumber);

            if (order == null)
                return await List();

            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("ExportXml")]
        [FormValueRequired("exportxml-all")]
        public virtual async Task<IActionResult> ExportXmlAll(OrderSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
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
            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = await OrderService.SearchOrdersAsync(storeId: model.StoreId,
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
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                var xml = await ExportManager.ExportOrdersToXmlAsync(orders);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(await (await OrderService.GetOrdersByIdsAsync(ids))
                    .WhereAwait(HasAccessToOrderAsync).ToListAsync());
            }

            try
            {
                var xml = await ExportManager.ExportOrdersToXmlAsync(orders);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll(OrderSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
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
            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = await OrderService.SearchOrdersAsync(storeId: model.StoreId,
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
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                var bytes = await ExportManager.ExportOrdersToXlsxAsync(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(await (await OrderService.GetOrdersByIdsAsync(ids)).WhereAwait(HasAccessToOrderAsync).ToListAsync());
            }

            try
            {
                var bytes = await ExportManager.ExportOrdersToXlsxAsync(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Order details

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public virtual async Task<IActionResult> CancelOrder(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                await OrderProcessingService.CancelOrderAsync(order, true);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public virtual async Task<IActionResult> CaptureOrder(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = await OrderProcessingService.CaptureAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                foreach (var error in errors)
                    NotificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public virtual async Task<IActionResult> MarkOrderAsPaid(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                await OrderProcessingService.MarkOrderAsPaidAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public virtual async Task<IActionResult> RefundOrder(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = await OrderProcessingService.RefundAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                foreach (var error in errors)
                    NotificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public virtual async Task<IActionResult> RefundOrderOffline(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                await OrderProcessingService.RefundOfflineAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public virtual async Task<IActionResult> VoidOrder(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = await OrderProcessingService.VoidAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                foreach (var error in errors)
                    NotificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public virtual async Task<IActionResult> VoidOrderOffline(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                await OrderProcessingService.VoidOfflineAsync(order);
                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            //prepare model
            var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

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
                    errors = (await OrderProcessingService.PartiallyRefundAsync(order, amountToRefund)).ToList();
                else
                    await OrderProcessingService.PartiallyRefundOfflineAsync(order, amountToRefund);

                await LogEditOrderAsync(order.Id);

                if (!errors.Any())
                {
                    //success
                    ViewBag.RefreshPage = true;

                    //prepare model
                    model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

                    return View(model);
                }

                //prepare model
                model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

                foreach (var error in errors)
                    NotificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderStatus")]
        public virtual async Task<IActionResult> ChangeOrderStatus(int id, OrderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                order.OrderStatusId = model.OrderStatusId;
                await OrderService.UpdateOrderAsync(order);

                //add a note
                await OrderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Order status has been edited. New status: {await LocalizationService.GetLocalizedEnumAsync(order.OrderStatus)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                //prepare model
                model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

                await NotificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null || order.Deleted)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            //prepare model
            var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            await OrderProcessingService.DeleteOrderAsync(order);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteOrder",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteOrder"), order.Id), order);

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> PdfInvoice(int orderId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                vendorId = currentVendor.Id;
            }

            var order = await OrderService.GetOrderByIdAsync(orderId);
            var orders = new List<Order>
            {
                order
            };

            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await PdfService.PrintOrdersToPdfAsync(stream, orders, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id, vendorId);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        [HttpPost, ActionName("PdfInvoice")]
        [FormValueRequired("pdf-invoice-all")]
        public virtual async Task<IActionResult> PdfInvoiceAll(OrderSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
            }

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

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
            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = await OrderService.SearchOrdersAsync(storeId: model.StoreId,
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
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await PdfService.PrintOrdersToPdfAsync(stream, orders, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id, model.VendorId);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfInvoiceSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(await OrderService.GetOrdersByIdsAsync(ids));
            }

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
                vendorId = currentVendor.Id;
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await PdfService.PrintOrdersToPdfAsync(stream, orders, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id, vendorId);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        //currently we use this method on the add product to order details pages
        [HttpPost]
        public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = await ProductAttributeParser.ParseProductAttributesAsync(product, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = await ProductAttributeParser.IsConditionMetAsync(attribute, attributeXml);
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
        public virtual async Task<IActionResult> EditCreditCardInfo(int id, OrderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            if (order.AllowStoringCreditCardNumber)
            {
                var cardType = model.CardType;
                var cardName = model.CardName;
                var cardNumber = model.CardNumber;
                var cardCvv2 = model.CardCvv2;
                var cardExpirationMonth = model.CardExpirationMonth;
                var cardExpirationYear = model.CardExpirationYear;

                order.CardType = EncryptionService.EncryptText(cardType);
                order.CardName = EncryptionService.EncryptText(cardName);
                order.CardNumber = EncryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = EncryptionService.EncryptText(PaymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = EncryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = EncryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = EncryptionService.EncryptText(cardExpirationYear);
                await OrderService.UpdateOrderAsync(order);
            }

            //add a note
            await OrderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //prepare model
            model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public virtual async Task<IActionResult> EditOrderTotals(int id, OrderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

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
            await OrderService.UpdateOrderAsync(order);

            //add a note
            await OrderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //prepare model
            model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public virtual async Task<IActionResult> EditShippingMethod(int id, OrderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            order.ShippingMethod = model.ShippingMethod;
            await OrderService.UpdateOrderAsync(order);

            //add a note
            await OrderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //prepare model
            model = await OrderModelFactory.PrepareOrderModelAsync(model, order);

            //selected card
            SaveSelectedCardName("order-billing-shipping", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
        public virtual async Task<IActionResult> EditOrderItem(int id, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnSaveOrderItem".Length..]);

            var orderItem = await OrderService.GetOrderItemByIdAsync(orderItemId)
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

            var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

            if (quantity > 0)
            {
                var qtyDifference = orderItem.Quantity - quantity;

                if (!OrderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                {
                    orderItem.UnitPriceInclTax = unitPriceInclTax;
                    orderItem.UnitPriceExclTax = unitPriceExclTax;
                    orderItem.Quantity = quantity;
                    orderItem.DiscountAmountInclTax = discountInclTax;
                    orderItem.DiscountAmountExclTax = discountExclTax;
                    orderItem.PriceInclTax = priceInclTax;
                    orderItem.PriceExclTax = priceExclTax;
                    await OrderService.UpdateOrderItemAsync(orderItem);
                }

                //adjust inventory
                await ProductService.AdjustInventoryAsync(product, qtyDifference, orderItem.AttributesXml,
                    string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
            }
            else
            {
                //adjust inventory
                await ProductService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                await OrderService.DeleteOrderItemAsync(orderItem);
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
            await OrderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            //add a note
            await OrderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

            foreach (var warning in updateOrderParameters.Warnings)
                NotificationService.WarningNotification(warning);

            //selected card
            SaveSelectedCardName("order-products", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
        public virtual async Task<IActionResult> DeleteOrderItem(int id, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnDeleteOrderItem".Length..]);

            var orderItem = await OrderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            if ((await GiftCardService.GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Any())
            {
                //we cannot delete an order item with associated gift cards
                //a store owner should delete them first

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"));

                //selected card
                SaveSelectedCardName("order-products", persistForTheNextRequest: false);

                return View(model);
            }
            else
            {
                var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

                //adjust inventory
                await ProductService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                await OrderService.DeleteOrderItemAsync(orderItem);

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem);
                await OrderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

                //add a note
                await OrderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Order item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                //prepare model
                var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

                foreach (var warning in updateOrderParameters.Warnings)
                    NotificationService.WarningNotification(warning);

                //selected card
                SaveSelectedCardName("order-products", persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        public virtual async Task<IActionResult> ResetDownloadCount(int id, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnResetDownloadCount".Length..]);

            var orderItem = await OrderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            orderItem.DownloadCount = 0;
            await OrderService.UpdateOrderItemAsync(orderItem);
            await LogEditOrderAsync(order.Id);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

            //selected card
            SaveSelectedCardName("order-products", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        public virtual async Task<IActionResult> ActivateDownloadItem(int id, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnPvActivateDownload".Length..]);

            var orderItem = await OrderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
            await OrderService.UpdateOrderItemAsync(orderItem);

            await LogEditOrderAsync(order.Id);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderModelAsync(null, order);

            //selected card
            SaveSelectedCardName("order-products", persistForTheNextRequest: false);
            return View(model);
        }

        public virtual async Task<IActionResult> UploadLicenseFilePopup(int id, int orderItemId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //try to get an order item with the specified id
            var orderItem = await OrderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            var product = await ProductService.GetProductByIdAsync(orderItem.ProductId)
                ?? throw new ArgumentException("No product found with the specified order item id");

            if (!product.IsDownload)
                throw new ArgumentException("Product is not downloadable");

            //ensure a vendor has access only to his products 
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            //prepare model
            var model = await OrderModelFactory.PrepareUploadLicenseModelAsync(new UploadLicenseModel(), order, orderItem);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public virtual async Task<IActionResult> UploadLicenseFilePopup(UploadLicenseModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            var orderItem = await OrderService.GetOrderItemByIdAsync(model.OrderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                orderItem.LicenseDownloadId = model.LicenseDownloadId;
            else
                orderItem.LicenseDownloadId = null;

            await OrderService.UpdateOrderItemAsync(orderItem);

            await LogEditOrderAsync(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public virtual async Task<IActionResult> DeleteLicenseFilePopup(UploadLicenseModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            var orderItem = await OrderService.GetOrderItemByIdAsync(model.OrderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            //attach license
            orderItem.LicenseDownloadId = null;

            await OrderService.UpdateOrderItemAsync(orderItem);

            await LogEditOrderAsync(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        public virtual async Task<IActionResult> AddProductToOrder(int orderId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //prepare model
            var model = await OrderModelFactory.PrepareAddProductToOrderSearchModelAsync(new AddProductToOrderSearchModel(), order);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddProductToOrder(AddProductToOrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            //prepare model
            var model = await OrderModelFactory.PrepareAddProductToOrderListModelAsync(searchModel, order);

            return Json(model);
        }

        public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //prepare model
            var model = await OrderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(order.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //basic properties
            decimal.TryParse(form["UnitPriceInclTax"], out var unitPriceInclTax);
            decimal.TryParse(form["UnitPriceExclTax"], out var unitPriceExclTax);
            int.TryParse(form["Quantity"], out var quantity);
            decimal.TryParse(form["SubTotalInclTax"], out var priceInclTax);
            decimal.TryParse(form["SubTotalExclTax"], out var priceExclTax);

            //warnings
            var warnings = new List<string>();

            //attributes
            var attributesXml = await ProductAttributeParser.ParseProductAttributesAsync(product, form, warnings);

            //rental product
            ProductAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

            //warnings
            warnings.AddRange(await ShoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));
            warnings.AddRange(await ShoppingCartService.GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType.ShoppingCart, product, attributesXml));
            warnings.AddRange(await ShoppingCartService.GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));
            if (!warnings.Any())
            {
                //no errors

                //attributes
                var attributeDescription = await ProductAttributeFormatter.FormatAttributesAsync(product, attributesXml, customer);

                //weight
                var itemWeight = await ShippingService.GetShoppingCartItemWeightAsync(product, attributesXml);

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
                    OriginalProductCost = await PriceCalculationService.GetProductCostAsync(product, attributesXml),
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

                await OrderService.InsertOrderItemAsync(orderItem);

                //adjust inventory
                await ProductService.AdjustInventoryAsync(product, -orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
                {
                    PriceInclTax = unitPriceInclTax,
                    PriceExclTax = unitPriceExclTax,
                    SubTotalInclTax = priceInclTax,
                    SubTotalExclTax = priceExclTax,
                    Quantity = quantity
                };
                await OrderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

                //add a note
                await OrderService.InsertOrderNoteAsync(new OrderNote
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
                    ProductAttributeParser.GetGiftCardAttribute(
                        attributesXml, out var recipientName, out var recipientEmail, out var senderName, out var senderEmail, out var giftCardMessage);

                    for (var i = 0; i < orderItem.Quantity; i++)
                    {
                        var gc = new GiftCard
                        {
                            GiftCardType = product.GiftCardType,
                            PurchasedWithOrderItemId = orderItem.Id,
                            Amount = unitPriceExclTax,
                            IsGiftCardActivated = false,
                            GiftCardCouponCode = GiftCardService.GenerateGiftCardCode(),
                            RecipientName = recipientName,
                            RecipientEmail = recipientEmail,
                            SenderName = senderName,
                            SenderEmail = senderEmail,
                            Message = giftCardMessage,
                            IsRecipientNotified = false,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        await GiftCardService.InsertGiftCardAsync(gc);
                    }
                }

                //redirect to order details page
                foreach (var warning in updateOrderParameters.Warnings)
                    NotificationService.WarningNotification(warning);

                //selected card
                SaveSelectedCardName("order-products");
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }

            //prepare model
            var model = await OrderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);
            model.Warnings.AddRange(warnings);

            return View(model);
        }

        #endregion

        #endregion

        #region Addresses

        public virtual async Task<IActionResult> AddressEdit(int addressId, int orderId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an address with the specified id
            var address = await AddressService.GetAddressByIdAsync(addressId)
                ?? throw new ArgumentException("No address found with the specified id", nameof(addressId));

            //prepare model
            var model = await OrderModelFactory.PrepareOrderAddressModelAsync(new OrderAddressModel(), order, address);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(OrderAddressModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });

            //try to get an address with the specified id
            var address = await AddressService.GetAddressByIdAsync(model.Address.Id)
                ?? throw new ArgumentException("No address found with the specified id");

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await AddressService.UpdateAddressAsync(address);

                //add a note
                await OrderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
            }

            //prepare model
            model = await OrderModelFactory.PrepareOrderAddressModelAsync(model, order, address);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Shipments

        public virtual async Task<IActionResult> ShipmentList()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await OrderModelFactory.PrepareShipmentSearchModelAsync(new ShipmentSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ShipmentListSelect(ShipmentSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await OrderModelFactory.PrepareShipmentListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ShipmentsByOrder(OrderShipmentSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return Content(string.Empty);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderShipmentListModelAsync(searchModel, order);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ShipmentsItemsByShipmentId(ShipmentItemSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(searchModel.ShipmentId)
                ?? throw new ArgumentException("No shipment found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && !await HasAccessToShipmentAsync(shipment))
                return Content(string.Empty);

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(shipment.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (currentVendor != null && !await HasAccessToOrderAsync(order))
                return Content(string.Empty);

            //prepare model
            searchModel.SetGridPageSize();
            var model = await OrderModelFactory.PrepareShipmentItemListModelAsync(searchModel, shipment);

            return Json(model);
        }

        public virtual async Task<IActionResult> AddShipment(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            //prepare model
            var model = await OrderModelFactory.PrepareShipmentModelAsync(new ShipmentModel(), null, order);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> AddShipment(ShipmentModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            var orderItems = await OrderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);
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
            var form = model.Form;

            foreach (var orderItem in orderItems)
            {
                var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

                //ensure that this product can be shipped (have at least one item to ship)
                var maxQtyToAdd = await OrderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                if (maxQtyToAdd <= 0)
                    continue;

                var qtyToAdd = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out qtyToAdd);
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
                            int.TryParse(form[formKey], out warehouseId);
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
                await ShipmentService.InsertShipmentAsync(shipment);

                foreach (var shipmentItem in shipmentItems)
                {
                    shipmentItem.ShipmentId = shipment.Id;
                    await ShipmentService.InsertShipmentItemAsync(shipmentItem);
                }

                //add a note
                await OrderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                if (model.CanShip)
                    await OrderProcessingService.ShipAsync(shipment, true);

                if (model.CanShip && model.CanDeliver)
                    await OrderProcessingService.DeliverAsync(shipment, true);

                await LogEditOrderAsync(order.Id);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Orders.Shipments.Added"));
                return continueEditing
                        ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
                        : RedirectToAction("Edit", new { id = model.OrderId });
            }

            NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.Shipments.NoProductsSelected"));

            return RedirectToAction("AddShipment", model);
        }

        public virtual async Task<IActionResult> ShipmentDetails(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            //prepare model
            var model = await OrderModelFactory.PrepareShipmentModelAsync(null, shipment, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteShipment(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            foreach (var shipmentItem in await ShipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
            {
                var orderItem = await OrderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

                await ProductService.ReverseBookedInventoryAsync(product, shipmentItem,
                    string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
            }

            var orderId = shipment.OrderId;
            await ShipmentService.DeleteShipmentAsync(shipment);

            var order = await OrderService.GetOrderByIdAsync(orderId);
            //add a note
            await OrderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Orders.Shipments.Deleted"));
            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public virtual async Task<IActionResult> SetTrackingNumber(ShipmentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            shipment.TrackingNumber = model.TrackingNumber;
            await ShipmentService.UpdateShipmentAsync(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setadmincomment")]
        public virtual async Task<IActionResult> SetShipmentAdminComment(ShipmentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            shipment.AdminComment = model.AdminComment;
            await ShipmentService.UpdateShipmentAsync(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasshipped")]
        public virtual async Task<IActionResult> SetAsShipped(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                await OrderProcessingService.ShipAsync(shipment, true);
                await LogEditOrderAsync(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("saveshippeddate")]
        public virtual async Task<IActionResult> EditShippedDate(ShipmentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ShippedDateUtc.HasValue)
                {
                    throw new Exception("Enter shipped date");
                }

                shipment.ShippedDateUtc = model.ShippedDateUtc;
                await ShipmentService.UpdateShipmentAsync(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasdelivered")]
        public virtual async Task<IActionResult> SetAsDelivered(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                await OrderProcessingService.DeliverAsync(shipment, true);
                await LogEditOrderAsync(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savedeliverydate")]
        public virtual async Task<IActionResult> EditDeliveryDate(ShipmentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.DeliveryDateUtc.HasValue)
                {
                    throw new Exception("Enter delivery date");
                }

                shipment.DeliveryDateUtc = model.DeliveryDateUtc;
                await ShipmentService.UpdateShipmentAsync(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        public virtual async Task<IActionResult> PdfPackagingSlip(int shipmentId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await ShipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            var shipments = new List<Shipment>
            {
                shipment
            };

            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await PdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
        }

        [HttpPost, ActionName("ShipmentList")]
        [FormValueRequired("exportpackagingslips-all")]
        public virtual async Task<IActionResult> PdfPackagingSlipAll(ShipmentSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
                vendorId = currentVendor.Id;

            //load shipments
            var shipments = await ShipmentService.GetAllShipmentsAsync(vendorId: vendorId,
                warehouseId: model.WarehouseId,
                shippingCountryId: model.CountryId,
                shippingStateId: model.StateProvinceId,
                shippingCounty: model.County,
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue);

            //ensure that we at least one shipment selected
            if (!shipments.Any())
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Orders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await PdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentList");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfPackagingSlipSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                shipments.AddRange(await ShipmentService.GetShipmentsByIdsAsync(ids));
            }
            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null)
            {
                shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await PdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, OrderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await WorkContext.GetWorkingLanguageAsync()).Id);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentList");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetAsShippedSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var shipments = await ShipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null)
            {
                shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
            }

            foreach (var shipment in shipments)
            {
                try
                {
                    await OrderProcessingService.ShipAsync(shipment, true);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetAsDeliveredSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var shipments = await ShipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());

            //a vendor should have access only to his products
            if (await WorkContext.GetCurrentVendorAsync() != null)
            {
                shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
            }

            foreach (var shipment in shipments)
            {
                try
                {
                    await OrderProcessingService.DeliverAsync(shipment, true);
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
        public virtual async Task<IActionResult> OrderNotesSelect(OrderNoteSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderNoteListModelAsync(searchModel, order);

            return Json(model);
        }

        public virtual async Task<IActionResult> OrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(message))
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Orders.OrderNotes.Fields.Note.Validation"));

            //try to get an order with the specified id
            var order = await OrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return ErrorJson("Order cannot be loaded");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return ErrorJson("No access for vendors");

            var orderNote = new OrderNote
            {
                OrderId = order.Id,
                DisplayToCustomer = displayToCustomer,
                Note = message,
                DownloadId = downloadId,
                CreatedOnUtc = DateTime.UtcNow
            };

            await OrderService.InsertOrderNoteAsync(orderNote);

            //new order notification
            if (displayToCustomer)
            {
                //email
                await WorkflowMessageService.SendNewOrderNoteAddedCustomerNotificationAsync(orderNote, (await WorkContext.GetWorkingLanguageAsync()).Id);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderNoteDelete(int id, int orderId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            _ = await OrderService.GetOrderByIdAsync(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order note with the specified id
            var orderNote = await OrderService.GetOrderNoteByIdAsync(id)
                ?? throw new ArgumentException("No order note found with the specified id");

            await OrderService.DeleteOrderNoteAsync(orderNote);

            return new NullJsonResult();
        }

        #endregion

        #region Reports

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersBriefReportByQuantityList(BestsellerBriefSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await OrderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersBriefReportByAmountList(BestsellerBriefSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await OrderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderAverageReportList(OrderAverageReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //a vendor doesn't have access to this report
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderAverageReportListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderIncompleteReportList(OrderIncompleteReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //a vendor doesn't have access to this report
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            //prepare model
            var model = await OrderModelFactory.PrepareOrderIncompleteReportListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> LoadOrderStatistics(string period)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return Content(string.Empty);

            //a vendor doesn't have access to this report
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = await DateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var timeZone = await DateTimeHelper.GetCurrentTimeZoneAsync();

            var culture = new CultureInfo((await WorkContext.GetWorkingLanguageAsync()).LanguageCulture);

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
                            value = (await OrderService.SearchOrdersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
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
                            value = (await OrderService.SearchOrdersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
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
                            value = (await OrderService.SearchOrdersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
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
}
