using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class GiftCardController : BaseAdminController
    {
        #region Fields

        private readonly IGiftCardService _giftCardService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public GiftCardController(IGiftCardService giftCardService,
            IPriceFormatter priceFormatter, IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper, LocalizationSettings localizationSettings,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            ILocalizationService localizationService, ILanguageService languageService,
            ICustomerActivityService customerActivityService, IPermissionService permissionService)
        {
            this._giftCardService = giftCardService;
            this._priceFormatter = priceFormatter;
            this._workflowMessageService = workflowMessageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationSettings = localizationSettings;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var model = new GiftCardListModel();
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.All")
            });
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.ActivatedOnly")
            });
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.DeactivatedOnly")
            });
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GiftCardList(DataSourceRequest command, GiftCardListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedKendoGridJson();

            bool? isGiftCardActivated = null;
            if (model.ActivatedId == 1)
                isGiftCardActivated = true;
            else if (model.ActivatedId == 2)
                isGiftCardActivated = false;
            var giftCards = _giftCardService.GetAllGiftCards(isGiftCardActivated: isGiftCardActivated,
                giftCardCouponCode: model.CouponCode,
                recipientName: model.RecipientName,
                pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = giftCards.Select(x =>
                {
                    var m = x.ToModel();
                    m.RemainingAmountStr = _priceFormatter.FormatPrice(x.GetGiftCardRemainingAmount(), true, false);
                    m.AmountStr = _priceFormatter.FormatPrice(x.Amount, true, false);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return m;
                }),
                Total = giftCards.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var model = new GiftCardModel
            {
                PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode
            };

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(GiftCardModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                _giftCardService.InsertGiftCard(giftCard);

                //activity log
                _customerActivityService.InsertActivity("AddNewGiftCard", _localizationService.GetResource("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode);

                SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var giftCard = _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                //No gift card found with the specified id
                return RedirectToAction("List");

            var model = giftCard.ToModel();
            model.PurchasedWithOrderId = giftCard.PurchasedWithOrderItem != null ? (int?)giftCard.PurchasedWithOrderItem.OrderId : null;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(giftCard.GetGiftCardRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.PurchasedWithOrderNumber = giftCard.PurchasedWithOrderItem != null ? giftCard.PurchasedWithOrderItem.Order.CustomOrderNumber : null;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(GiftCardModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var giftCard = _giftCardService.GetGiftCardById(model.Id);

            model.PurchasedWithOrderId = giftCard.PurchasedWithOrderItem != null ? (int?)giftCard.PurchasedWithOrderItem.OrderId : null;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(giftCard.GetGiftCardRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.PurchasedWithOrderNumber = giftCard.PurchasedWithOrderItem != null ? giftCard.PurchasedWithOrderItem.Order.CustomOrderNumber : null;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                _giftCardService.UpdateGiftCard(giftCard);

                //activity log
                _customerActivityService.InsertActivity("EditGiftCard", _localizationService.GetResource("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode);

                SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = giftCard.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpPost]
        public virtual IActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual IActionResult NotifyRecipient(GiftCardModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var giftCard = _giftCardService.GetGiftCardById(model.Id);

            model = giftCard.ToModel();
            model.PurchasedWithOrderId = giftCard.PurchasedWithOrderItem != null ? (int?)giftCard.PurchasedWithOrderItem.OrderId : null;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(giftCard.GetGiftCardRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.PurchasedWithOrderNumber = giftCard.PurchasedWithOrderItem != null ? giftCard.PurchasedWithOrderItem.Order.CustomOrderNumber : null;

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new NopException("Recipient email is not valid");
                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new NopException("Sender email is not valid");

                var languageId = 0;
                var order = giftCard.PurchasedWithOrderItem != null ? giftCard.PurchasedWithOrderItem.Order : null;
                if (order != null)
                {
                    var customerLang = _languageService.GetLanguageById(order.CustomerLanguageId);
                    if (customerLang == null)
                        customerLang = _languageService.GetAllLanguages().FirstOrDefault();
                    if (customerLang != null)
                        languageId = customerLang.Id;
                }
                else
                {
                    languageId = _localizationSettings.DefaultAdminLanguageId;
                }
                var queuedEmailId = _workflowMessageService.SendGiftCardNotification(giftCard, languageId);
                if (queuedEmailId > 0)
                {
                    giftCard.IsRecipientNotified = true;
                    _giftCardService.UpdateGiftCard(giftCard);
                    model.IsRecipientNotified = true;
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc, false);
            }

            return View(model);
        }
        
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            var giftCard = _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                //No gift card found with the specified id
                return RedirectToAction("List");

            _giftCardService.DeleteGiftCard(giftCard);

            //activity log
            _customerActivityService.InsertActivity("DeleteGiftCard", _localizationService.GetResource("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode);

            SuccessNotification(_localizationService.GetResource("Admin.GiftCards.Deleted"));
            return RedirectToAction("List");
        }
        
        //Gif card usage history
        [HttpPost]
        public virtual IActionResult UsageHistoryList(int giftCardId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedKendoGridJson();

            var giftCard = _giftCardService.GetGiftCardById(giftCardId);
            if (giftCard == null)
                throw new ArgumentException("No gift card found with the specified id");

            var usageHistoryModel = giftCard.GiftCardUsageHistory.OrderByDescending(gcuh => gcuh.CreatedOnUtc)
                .Select(x => new GiftCardModel.GiftCardUsageHistoryModel
                {
                    Id = x.Id,
                    OrderId = x.UsedWithOrderId,
                    UsedValue = _priceFormatter.FormatPrice(x.UsedValue, true, false),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                    CustomOrderNumber = x.UsedWithOrder.CustomOrderNumber
                })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = usageHistoryModel.PagedForCommand(command),
                Total = usageHistoryModel.Count
            };

            return Json(gridModel);
        }

        #endregion
    }
}