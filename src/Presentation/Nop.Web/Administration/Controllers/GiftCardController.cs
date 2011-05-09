using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class GiftCardController : BaseNopController
    {
        #region Fields

        private readonly IGiftCardService _giftCardService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion Fields

        #region Constructors

        public GiftCardController(IGiftCardService giftCardService, 
            IPriceFormatter priceFormatter, ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper)
        {
            this._giftCardService = giftCardService;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var model = new GiftCardListModel();
            model.ActivatedList.Add(new SelectListItem()
                {
                    Value = "0",
                    Selected = true,
                    Text = "All"
                });
            model.ActivatedList.Add(new SelectListItem()
            {
                Value = "1",
                Text = "Activated"
            });
            model.ActivatedList.Add(new SelectListItem()
            {
                Value = "2",
                Text = "Deactivated"
            });
            return View(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GiftCardList(GridCommand command, GiftCardListModel model)
        {
            bool? isGiftCardActivated = null;
            if (model.ActivatedId == 1)
                isGiftCardActivated = true;
            else if (model.ActivatedId == 2)
                isGiftCardActivated = false;
            var queuedEmails = _giftCardService.GetAllGiftCards(null, null, isGiftCardActivated, model.CouponCode);
            var gridModel = new GridModel<GiftCardModel>
            {
                Data = queuedEmails.PagedForCommand(command).Select(x =>
                {
                    var m = x.ToModel();
                    m.RemainingAmountStr = _priceFormatter.FormatPrice(x.GetGiftCardRemainingAmount(), true, false);
                    m.AmountStr = _priceFormatter.FormatPrice(x.Amount, true, false);
                    m.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString();
                    return m;
                }),
                Total = queuedEmails.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            var model = new GiftCardModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(GiftCardModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                _giftCardService.InsertGiftCard(giftCard);
                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var giftCard = _giftCardService.GetGiftCardById(id);
            if (giftCard == null) 
                throw new ArgumentException("No gift card found with the specified id", "id");

            var model = giftCard.ToModel();
            model.PurchasedWithOrderId = giftCard.PurchasedWithOrderProductVariant != null ? (int?)giftCard.PurchasedWithOrderProductVariant.Id : null;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(giftCard.GetGiftCardRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc).ToString();

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(GiftCardModel model, bool continueEditing)
        {
            var giftCard = _giftCardService.GetGiftCardById(model.Id);

            model.PurchasedWithOrderId = giftCard.PurchasedWithOrderProductVariant != null ? (int?)giftCard.PurchasedWithOrderProductVariant.Id : null;
            model.RemainingAmountStr = _priceFormatter.FormatPrice(giftCard.GetGiftCardRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc).ToString();

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                _giftCardService.UpdateGiftCard(giftCard);
                return continueEditing ? RedirectToAction("Edit", giftCard.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpPost]
        public ActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public ActionResult NotifyRecipient(GiftCardModel model)
        {
            var giftCard = _giftCardService.GetGiftCardById(model.Id);

            int queuedEmailId = 0;
            //TODO queue email notification 
            // queuedEmailId = this.MessageService.SendGiftCardNotification(gc, customerLang.LanguageId);
            if (queuedEmailId > 0)
            {
                giftCard.IsRecipientNotified = true;
                _giftCardService.UpdateGiftCard(giftCard);
            }

            return RedirectToAction("Edit", giftCard.Id);
        }
        

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var giftCard = _giftCardService.GetGiftCardById(id);
            _giftCardService.DeleteGiftCard(giftCard);
            return RedirectToAction("List");
        }
        #endregion
    }
}
