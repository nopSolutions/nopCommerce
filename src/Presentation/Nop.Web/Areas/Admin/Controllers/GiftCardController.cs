using System;
using System.Linq;
using System.Threading.Tasks;
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

        protected CurrencySettings CurrencySettings { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGiftCardModelFactory GiftCardModelFactory { get; }
        protected IGiftCardService GiftCardService { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IOrderService OrderService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }

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
            CurrencySettings = currencySettings;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            DateTimeHelper = dateTimeHelper;
            GiftCardModelFactory = giftCardModelFactory;
            GiftCardService = giftCardService;
            LanguageService = languageService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            OrderService = orderService;
            PermissionService = permissionService;
            PriceFormatter = priceFormatter;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await GiftCardModelFactory.PrepareGiftCardSearchModelAsync(new GiftCardSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GiftCardList(GiftCardSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await GiftCardModelFactory.PrepareGiftCardListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await GiftCardModelFactory.PrepareGiftCardModelAsync(new GiftCardModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(GiftCardModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity<GiftCard>();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                await GiftCardService.InsertGiftCardAsync(giftCard);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewGiftCard",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.GiftCards.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await GiftCardModelFactory.PrepareGiftCardModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await GiftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            //prepare model
            var model = await GiftCardModelFactory.PrepareGiftCardModelAsync(null, giftCard);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(GiftCardModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await GiftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            var order = await OrderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

            model.PurchasedWithOrderId = order?.Id;
            model.RemainingAmountStr = await PriceFormatter.FormatPriceAsync(await GiftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
            model.AmountStr = await PriceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
            model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                await GiftCardService.UpdateGiftCardAsync(giftCard);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditGiftCard",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.GiftCards.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }

            //prepare model
            model = await GiftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = GiftCardService.GenerateGiftCardCode() });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual async Task<IActionResult> NotifyRecipient(GiftCardModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await GiftCardService.GetGiftCardByIdAsync(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new NopException("Recipient email is not valid");

                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new NopException("Sender email is not valid");

                var languageId = 0;
                var order = await OrderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);
                
                if (order != null)
                {
                    var customerLang = await LanguageService.GetLanguageByIdAsync(order.CustomerLanguageId);
                    if (customerLang == null)
                        customerLang = (await LanguageService.GetAllLanguagesAsync()).FirstOrDefault();
                    if (customerLang != null)
                        languageId = customerLang.Id;
                }
                else
                {
                    languageId = LocalizationSettings.DefaultAdminLanguageId;
                }

                var queuedEmailIds = await WorkflowMessageService.SendGiftCardNotificationAsync(giftCard, languageId);
                if (queuedEmailIds.Any())
                {
                    giftCard.IsRecipientNotified = true;
                    await GiftCardService.UpdateGiftCardAsync(giftCard);
                    model.IsRecipientNotified = true;
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.GiftCards.RecipientNotified"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await GiftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await GiftCardService.GetGiftCardByIdAsync(id);
            if (giftCard == null)
                return RedirectToAction("List");

            await GiftCardService.DeleteGiftCardAsync(giftCard);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteGiftCard",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.GiftCards.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageGiftCards))
                return await AccessDeniedDataTablesJson();

            //try to get a gift card with the specified id
            var giftCard = await GiftCardService.GetGiftCardByIdAsync(searchModel.GiftCardId)
                ?? throw new ArgumentException("No gift card found with the specified id");

            //prepare model
            var model = await GiftCardModelFactory.PrepareGiftCardUsageHistoryListModelAsync(searchModel, giftCard);

            return Json(model);
        }

        #endregion
    }
}