using System;
using System.Linq;
using System.Threading.Tasks;
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

        protected CurrencySettings CurrencySettings { get; }
        protected ICurrencyService CurrencyService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGiftCardService GiftCardService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IOrderService OrderService { get; }
        protected IPriceFormatter PriceFormatter { get; }

        #endregion

        #region Ctor

        public GiftCardModelFactory(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPriceFormatter priceFormatter)
        {
            CurrencySettings = currencySettings;
            CurrencyService = currencyService;
            DateTimeHelper = dateTimeHelper;
            GiftCardService = giftCardService;
            LocalizationService = localizationService;
            OrderService = orderService;
            PriceFormatter = priceFormatter;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card search model
        /// </returns>
        public virtual async Task<GiftCardSearchModel> PrepareGiftCardSearchModelAsync(GiftCardSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "0",
                Text = await LocalizationService.GetResourceAsync("Admin.GiftCards.List.Activated.All")
            });
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "1",
                Text = await LocalizationService.GetResourceAsync("Admin.GiftCards.List.Activated.ActivatedOnly")
            });
            searchModel.ActivatedList.Add(new SelectListItem
            {
                Value = "2",
                Text = await LocalizationService.GetResourceAsync("Admin.GiftCards.List.Activated.DeactivatedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged gift card list model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card list model
        /// </returns>
        public virtual async Task<GiftCardListModel> PrepareGiftCardListModelAsync(GiftCardSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter gift cards
            var isActivatedOnly = searchModel.ActivatedId == 0 ? null : searchModel.ActivatedId == 1 ? true : (bool?)false;

            //get gift cards
            var giftCards = await GiftCardService.GetAllGiftCardsAsync(isGiftCardActivated: isActivatedOnly,
                giftCardCouponCode: searchModel.CouponCode,
                recipientName: searchModel.RecipientName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new GiftCardListModel().PrepareToGridAsync(searchModel, giftCards, () =>
            {
                return giftCards.SelectAwait(async giftCard =>
                {
                    //fill in model values from the entity
                    var giftCardModel = giftCard.ToModel<GiftCardModel>();

                    //convert dates to the user time
                    giftCardModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var giftAmount = await GiftCardService.GetGiftCardRemainingAmountAsync(giftCard);
                    giftCardModel.RemainingAmountStr = await PriceFormatter.FormatPriceAsync(giftAmount, true, false);
                    giftCardModel.AmountStr = await PriceFormatter.FormatPriceAsync(giftCard.Amount, true, false);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card model
        /// </returns>
        public virtual async Task<GiftCardModel> PrepareGiftCardModelAsync(GiftCardModel model, GiftCard giftCard, bool excludeProperties = false)
        {
            if (giftCard != null)
            {
                //fill in model values from the entity
                model ??= giftCard.ToModel<GiftCardModel>();

                var order = await OrderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

                model.PurchasedWithOrderId = order?.Id;
                model.RemainingAmountStr = await PriceFormatter.FormatPriceAsync(await GiftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
                model.AmountStr = await PriceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
                model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
                model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

                //prepare nested search model
                PrepareGiftCardUsageHistorySearchModel(model.GiftCardUsageHistorySearchModel, giftCard);
            }

            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;

            return model;
        }

        /// <summary>
        /// Prepare paged gift usage history card list model
        /// </summary>
        /// <param name="searchModel">Gift card usage history search model</param>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card usage history list model
        /// </returns>
        public virtual async Task<GiftCardUsageHistoryListModel> PrepareGiftCardUsageHistoryListModelAsync(GiftCardUsageHistorySearchModel searchModel,
            GiftCard giftCard)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            //get gift card usage history
            var usageHistory = (await GiftCardService.GetGiftCardUsageHistoryAsync(giftCard))
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new GiftCardUsageHistoryListModel().PrepareToGridAsync(searchModel, usageHistory, () =>
            {
                return usageHistory.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity
                    var giftCardUsageHistoryModel = historyEntry.ToModel<GiftCardUsageHistoryModel>();

                    //convert dates to the user time
                    giftCardUsageHistoryModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    giftCardUsageHistoryModel.OrderId = historyEntry.UsedWithOrderId;
                    giftCardUsageHistoryModel.CustomOrderNumber = (await OrderService.GetOrderByIdAsync(historyEntry.UsedWithOrderId))?.CustomOrderNumber;
                    giftCardUsageHistoryModel.UsedValue = await PriceFormatter.FormatPriceAsync(historyEntry.UsedValue, true, false);

                    return giftCardUsageHistoryModel;
                });
            });

            return model;
        }

        #endregion
    }
}