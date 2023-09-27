using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
    public partial class GiftCardController : BaseAdminController
    {
        #region Fields

        protected readonly CurrencySettings _currencySettings;
        protected readonly ICurrencyService _currencyService;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IGiftCardModelFactory _giftCardModelFactory;
        protected readonly IGiftCardService _giftCardService;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IOrderService _orderService;
        protected readonly IPermissionService _permissionService;
        protected readonly IPriceFormatter _priceFormatter;
        protected readonly IWorkflowMessageService _workflowMessageService;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public GiftCardController(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardModelFactory giftCardModelFactory,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardModelFactory = giftCardModelFactory;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardSearchModelAsync(new GiftCardSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GiftCardList(GiftCardSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(new GiftCardModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity<GiftCard>();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                await _giftCardService.InsertGiftCardAsync(giftCard);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewGiftCard",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(null, giftCard);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

            model.PurchasedWithOrderId = order?.Id;
            model.RemainingAmountStr = await _priceFormatter.FormatPriceAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
            model.AmountStr = await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                await _giftCardService.UpdateGiftCardAsync(giftCard);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditGiftCard",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual async Task<IActionResult> NotifyRecipient(GiftCardModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new NopException("Recipient email is not valid");

                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new NopException("Sender email is not valid");

                var languageId = 0;
                var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

                if (order != null)
                {
                    var customerLang = await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);
                    if (customerLang == null)
                        customerLang = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
                    if (customerLang != null)
                        languageId = customerLang.Id;
                }
                else
                {
                    languageId = _localizationSettings.DefaultAdminLanguageId;
                }

                var queuedEmailIds = await _workflowMessageService.SendGiftCardNotificationAsync(giftCard, languageId);
                if (queuedEmailIds.Any())
                {
                    giftCard.IsRecipientNotified = true;
                    await _giftCardService.UpdateGiftCardAsync(giftCard);
                    model.IsRecipientNotified = true;
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.RecipientNotified"));

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            await _giftCardService.DeleteGiftCardAsync(giftCard);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteGiftCard",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardByIdAsync(searchModel.GiftCardId)
                ?? throw new ArgumentException("No gift card found with the specified id");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardUsageHistoryListModelAsync(searchModel, giftCard);

            return Json(model);
        }

        #endregion
    }
}