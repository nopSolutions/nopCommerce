using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class RecurringPaymentController : BaseNopController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;

        #endregion Fields

        #region Constructors

        public RecurringPaymentController(IOrderService orderService,
            IOrderProcessingService orderProcessingService, ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper, IPaymentService paymentService)
        {
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
        }

        #endregion Constructors

        #region Utilities

        [NonAction]
        private RecurringPaymentModel PrepareRecurringPaymentModel(RecurringPayment recurringPayment, bool includeHistory)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var model = new RecurringPaymentModel()
                    {
                        Id = recurringPayment.Id,
                        CycleLength = recurringPayment.CycleLength,
                        CyclePeriodId = recurringPayment.CyclePeriodId,
                        CyclePeriodStr = recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext),
                        TotalCycles = recurringPayment.TotalCycles,
                        StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                        IsActive = recurringPayment.IsActive,
                        NextPaymentDate = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                        CyclesRemaining = recurringPayment.CyclesRemaining,
                        InitialOrderId = recurringPayment.InitialOrder.Id,
                        PaymentType = _paymentService.GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName).GetLocalizedEnum(_localizationService, _workContext),
                        CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    };
            if (includeHistory)
                foreach (var rph in recurringPayment.RecurringPaymentHistory.OrderBy(x => x.CreatedOnUtc))
                    model.History.Add(PrepareRecurringPaymentHistoryModel(rph));
            return model;
        }

        [NonAction]
        private RecurringPaymentModel.RecurringPaymentHistoryModel PrepareRecurringPaymentHistoryModel(RecurringPaymentHistory history)
        {
            if (history == null)
                throw new ArgumentNullException("history");

            var order = _orderService.GetOrderById(history.OrderId);

            var model = new RecurringPaymentModel.RecurringPaymentHistoryModel()
            {
                Id = history.Id,
                OrderId = history.OrderId,
                RecurringPaymentId = history.RecurringPaymentId,
                OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                CreatedOn = _dateTimeHelper.ConvertToUserTime(history.CreatedOnUtc, DateTimeKind.Utc).ToString()
            };
            return model;
        }

        #endregion

        #region Recurring payment

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var gridModel = new GridModel<RecurringPaymentModel>();
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var payments = _orderService.SearchRecurringPayments(0, 0, null, true);
            var gridModel = new GridModel<RecurringPaymentModel>
            {
                Data = payments.PagedForCommand(command).Select(x =>
                {
                    return PrepareRecurringPaymentModel(x, false);
                }),
                Total = payments.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //edit
        public ActionResult Edit(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null || payment.Deleted)
                throw new ArgumentException("No recurring payment found with the specified id", "id");
            var model = PrepareRecurringPaymentModel(payment, true);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(RecurringPaymentModel model, bool continueEditing)
        {
            var payment = _orderService.GetRecurringPaymentById(model.Id);
            if (payment == null || payment.Deleted)
                throw new ArgumentException("No recurring payment found with the specified id");

            payment.CycleLength = model.CycleLength;
            payment.CyclePeriodId = model.CyclePeriodId;
            payment.TotalCycles = model.TotalCycles;
            payment.IsActive = model.IsActive;
            _orderService.UpdateRecurringPayment(payment);

            return continueEditing ? RedirectToAction("Edit", payment.Id) : RedirectToAction("List");
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            _orderService.DeleteRecurringPayment(payment);
            return RedirectToAction("List");
        }

        #endregion

        #region History

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult HistoryList(int recurringPaymentId, GridCommand command)
        {
            var payment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            var historyModel = payment.RecurringPaymentHistory.OrderBy(x => x.CreatedOnUtc)
                .Select(x =>
                {
                    var m = PrepareRecurringPaymentHistoryModel(x);
                    return m;
                })
                .ToList();
            var model = new GridModel<RecurringPaymentModel.RecurringPaymentHistoryModel>
            {
                Data = historyModel,
                Total = historyModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public ActionResult ProcessNextPayment(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            ViewData["selectedTab"] = "history";

            try
            {
                _orderProcessingService.ProcessNextRecurringPayment(payment);
                var model = PrepareRecurringPaymentModel(payment, true);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareRecurringPaymentModel(payment, true);
                model.ProcessPaymentErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public ActionResult CancelRecurringPayment(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            ViewData["selectedTab"] = "history";

            try
            {
                var errors = _orderProcessingService.CancelRecurringPayment(payment);
                var model = PrepareRecurringPaymentModel(payment, true);
                model.ProcessPaymentErrors = errors.ToList();
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareRecurringPaymentModel(payment, true);
                model.ProcessPaymentErrors.Add(exc.Message);
                return View(model);
            }
        }

        #endregion
    }
}
