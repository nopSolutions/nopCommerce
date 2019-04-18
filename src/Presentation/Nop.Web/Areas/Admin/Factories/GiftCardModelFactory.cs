using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the gift card model factory implementation
    /// </summary>
    public partial class GiftCardModelFactory : IGiftCardModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public GiftCardModelFactory(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare gift card usage history search model
        /// </summary>
        /// <param name="searchModel">Gift card usage history search model</param>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Gift card usage history search model</returns>
        protected virtual GiftCardUsageHistorySearchModel PrepareGiftCardUsageHistorySearchModel(GiftCardUsageHistorySearchModel searchModel,
            GiftCard giftCard)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            searchModel.GiftCardId = giftCard.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare gift card search model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>Gift card search model</returns>
        public virtual GiftCardSearchModel PrepareGiftCardSearchModel(GiftCardSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.All")
            });
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.ActivatedOnly")
            });
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.GiftCards.List.Activated.DeactivatedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged gift card list model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>Gift card list model</returns>
        public virtual GiftCardListModel PrepareGiftCardListModel(GiftCardSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter gift cards
            var isActivatedOnly = searchModel.ActivatedId == 0 ? null : searchModel.ActivatedId == 1 ? true : (bool?)false;

            //get gift cards
            var giftCards = _giftCardService.GetAllGiftCards(isGiftCardActivated: isActivatedOnly,
                giftCardCouponCode: searchModel.CouponCode,
                recipientName: searchModel.RecipientName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new GiftCardListModel().PrepareToGrid(searchModel, giftCards, () =>
            {
                return giftCards.Select(giftCard =>
                {
                    //fill in model values from the entity
                    var giftCardModel = giftCard.ToModel<GiftCardModel>();

                    //convert dates to the user time
                    giftCardModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    giftCardModel.RemainingAmountStr = _priceFormatter.FormatPrice(_giftCardService.GetGiftCardRemainingAmount(giftCard), true, false);
                    giftCardModel.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);

                    return giftCardModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare gift card model
        /// </summary>
        /// <param name="model">Gift card model</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Gift card model</returns>
        public virtual GiftCardModel PrepareGiftCardModel(GiftCardModel model, GiftCard giftCard, bool excludeProperties = false)
        {
            if (giftCard != null)
            {
                //fill in model values from the entity
                model = model ?? giftCard.ToModel<GiftCardModel>();

                model.PurchasedWithOrderId = giftCard.PurchasedWithOrderItem?.OrderId;
                model.RemainingAmountStr = _priceFormatter.FormatPrice(_giftCardService.GetGiftCardRemainingAmount(giftCard), true, false);
                model.AmountStr = _priceFormatter.FormatPrice(giftCard.Amount, true, false);
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
                model.PurchasedWithOrderNumber = giftCard.PurchasedWithOrderItem?.Order?.CustomOrderNumber;

                //prepare nested search model
                PrepareGiftCardUsageHistorySearchModel(model.GiftCardUsageHistorySearchModel, giftCard);
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode;

            return model;
        }

        /// <summary>
        /// Prepare paged gift usage history card list model
        /// </summary>
        /// <param name="searchModel">Gift card usage history search model</param>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Gift card usage history list model</returns>
        public virtual GiftCardUsageHistoryListModel PrepareGiftCardUsageHistoryListModel(GiftCardUsageHistorySearchModel searchModel,
            GiftCard giftCard)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            //get gift card usage history
            var usageHistory = giftCard.GiftCardUsageHistory
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new GiftCardUsageHistoryListModel().PrepareToGrid(searchModel, usageHistory, () =>
            {
                return usageHistory.Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var giftCardUsageHistoryModel = historyEntry.ToModel<GiftCardUsageHistoryModel>();

                    //convert dates to the user time
                    giftCardUsageHistoryModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    giftCardUsageHistoryModel.OrderId = historyEntry.UsedWithOrderId;
                    giftCardUsageHistoryModel.CustomOrderNumber = historyEntry.UsedWithOrder.CustomOrderNumber;
                    giftCardUsageHistoryModel.UsedValue = _priceFormatter.FormatPrice(historyEntry.UsedValue, true, false);

                    return giftCardUsageHistoryModel;
                });
            });

            return model;
        }

        #endregion
    }
}