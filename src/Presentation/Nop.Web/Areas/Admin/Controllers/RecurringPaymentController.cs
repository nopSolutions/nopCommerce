using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class RecurringPaymentController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IRecurringPaymentModelFactory _recurringPaymentModelFactory;

        #endregion Fields

        #region Ctor

        public RecurringPaymentController(ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPermissionService permissionService,
            IRecurringPaymentModelFactory recurringPaymentModelFactory)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _permissionService = permissionService;
            _recurringPaymentModelFactory = recurringPaymentModelFactory;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //prepare model
            var model = _recurringPaymentModelFactory.PrepareRecurringPaymentSearchModel(new RecurringPaymentSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(RecurringPaymentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _recurringPaymentModelFactory.PrepareRecurringPaymentListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(null, payment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(RecurringPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(model.Id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                payment = model.ToEntity(payment);
                _orderService.UpdateRecurringPayment(payment);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = payment.Id });
            }

            //prepare model
            model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(model, payment, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                return RedirectToAction("List");

            _orderService.DeleteRecurringPayment(payment);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult HistoryList(RecurringPaymentHistorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedDataTablesJson();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(searchModel.RecurringPaymentId)
                ?? throw new ArgumentException("No recurring payment found with the specified id");

            //prepare model
            var model = _recurringPaymentModelFactory.PrepareRecurringPaymentHistoryListModel(searchModel, payment);

            return Json(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public virtual IActionResult ProcessNextPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = _orderProcessingService.ProcessNextRecurringPayment(payment).ToList();
                if (errors.Any())
                    errors.ForEach(error => _notificationService.ErrorNotification(error));
                else
                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.NextPaymentProcessed"));

                //prepare model
                var model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(null, payment);

                //selected panel
                SaveSelectedPanelName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);

                //prepare model
                var model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(null, payment);

                //selected panel
                SaveSelectedPanelName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public virtual IActionResult CancelRecurringPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = _orderProcessingService.CancelRecurringPayment(payment);
                if (errors.Any())
                {
                    foreach (var error in errors)
                        _notificationService.ErrorNotification(error);
                }
                else
                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Cancelled"));

                //prepare model
                var model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(null, payment);

                //selected panel
                SaveSelectedPanelName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);

                //prepare model
                var model = _recurringPaymentModelFactory.PrepareRecurringPaymentModel(null, payment);

                //selected panel
                SaveSelectedPanelName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        #endregion
    }
}