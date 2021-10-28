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

        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderService OrderService { get; }
        protected IPermissionService PermissionService { get; }
        protected IRecurringPaymentModelFactory RecurringPaymentModelFactory { get; }

        #endregion Fields

        #region Ctor

        public RecurringPaymentController(ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPermissionService permissionService,
            IRecurringPaymentModelFactory recurringPaymentModelFactory)
        {
            LocalizationService = localizationService;
            NotificationService = notificationService;
            OrderProcessingService = orderProcessingService;
            OrderService = orderService;
            PermissionService = permissionService;
            RecurringPaymentModelFactory = recurringPaymentModelFactory;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //prepare model
            var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentSearchModelAsync(new RecurringPaymentSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(RecurringPaymentSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(RecurringPaymentModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(model.Id);
            if (payment == null || payment.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                payment = model.ToEntity(payment);
                await OrderService.UpdateRecurringPaymentAsync(payment);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.RecurringPayments.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = payment.Id });
            }

            //prepare model
            model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(model, payment, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            await OrderService.DeleteRecurringPaymentAsync(payment);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.RecurringPayments.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> HistoryList(RecurringPaymentHistorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return await AccessDeniedDataTablesJson();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(searchModel.RecurringPaymentId)
                ?? throw new ArgumentException("No recurring payment found with the specified id");

            //prepare model
            var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentHistoryListModelAsync(searchModel, payment);

            return Json(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public virtual async Task<IActionResult> ProcessNextPayment(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = (await OrderProcessingService.ProcessNextRecurringPaymentAsync(payment)).ToList();
                if (errors.Any())
                    errors.ForEach(error => NotificationService.ErrorNotification(error));
                else
                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.RecurringPayments.NextPaymentProcessed"));

                //prepare model
                var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);

                //prepare model
                var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public virtual async Task<IActionResult> CancelRecurringPayment(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            //try to get a recurring payment with the specified id
            var payment = await OrderService.GetRecurringPaymentByIdAsync(id);
            if (payment == null)
                return RedirectToAction("List");

            try
            {
                var errors = await OrderProcessingService.CancelRecurringPaymentAsync(payment);
                if (errors.Any())
                {
                    foreach (var error in errors)
                        NotificationService.ErrorNotification(error);
                }
                else
                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.RecurringPayments.Cancelled"));

                //prepare model
                var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);

                //prepare model
                var model = await RecurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

                //selected card
                SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

                return View(model);
            }
        }

        #endregion
    }
}