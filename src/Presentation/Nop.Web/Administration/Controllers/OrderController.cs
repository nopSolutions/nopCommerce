using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;
using System.Collections.Generic;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class OrderController : BaseNopController
	{
		private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public OrderController(IOrderService orderService, IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter, ILocalizationService localizationService,
            IWorkContext workContext)
		{
			this._orderService = orderService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._workContext = workContext;
		}

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
		{
            var model = new OrderListModel();
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult OrderList(GridCommand command, OrderListModel model)
		{
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


            var orders = _orderService.SearchOrders(startDateValue, endDateValue, orderStatus,
                paymentStatus, shippingStatus, model.CustomerEmail, model.OrderGuid, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<OrderModel>
            {
                Data = orders.Select(x =>
                {
                    return new OrderModel()
                    {
                        Id = x.Id,
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        CustomerEmail = x.BillingAddress.Email,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString()
                    };
                }),
                Total = orders.TotalCount
            };
			return new JsonResult
			{
				Data = gridModel
			};
		}

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public ActionResult GoToOrderId(OrderListModel model)
        {
            var order = _orderService.GetOrderById(model.GoDirectlyToNumber);
            if (order != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            else
                return List();
        }
	}
}
