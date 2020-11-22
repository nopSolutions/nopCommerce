using System;
using System.Linq;
using System.Threading.Tasks;
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentSearchModelAsync(new RecurringPaymentSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(RecurringPaymentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(RecurringPaymentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(model.Id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                payment = model.ToEntity(payment);
                await _orderService.UpdateRecurringPaymentAsync(payment);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = payment.Id });
            }

            //prepare model
            model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(model, payment, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            await _orderService.DeleteRecurringPaymentAsync(payment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> HistoryList(RecurringPaymentHistorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return await AccessDeniedDataTablesJson();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(searchModel.RecurringPaymentId)
                ?? throw new ArgumentException("No recurring payment found with the specified id");

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentHistoryListModelAsync(searchModel, payment);

            return Json(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public virtual async Task<IActionResult> ProcessNextPayment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = (await _orderProcessingService.ProcessNextRecurringPaymentAsync(payment)).ToList();
                if (errors.Any())
                    errors.ForEach(error => _notificationService.ErrorNotification(error));
                else
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.NextPaymentProcessed"));

                //prepare model
                var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);

                //prepare model
                var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public virtual async Task<IActionResult> CancelRecurringPayment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = await _orderProcessingService.CancelRecurringPaymentAsync(payment);
                if (errors.Any())
                {
                    foreach (var error in errors)
                        _notificationService.ErrorNotification(error);
                }
                else
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Cancelled"));

                //prepare model
                var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);

                //prepare model
                var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        #endregion
    }
}