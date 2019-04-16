using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.DataTables;
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
            searchModel.Grid = PrepareGiftCardUsageHistoryGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareGiftCardGridModel(GiftCardSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "giftcards-grid",
                UrlRead = new DataUrl("GiftCardList", "GiftCard", null),
                SearchButtonId = "search-giftcards",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.ActivatedId)),
                new FilterParameter(nameof(searchModel.CouponCode)),
                new FilterParameter(nameof(searchModel.RecipientName))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(GiftCardModel.AmountStr))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.Amount")
                },
                new ColumnProperty(nameof(GiftCardModel.RemainingAmountStr))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.RemainingAmount"),
                },
                new ColumnProperty(nameof(GiftCardModel.GiftCardCouponCode))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.GiftCardCouponCode")
                },
                new ColumnProperty(nameof(GiftCardModel.RecipientName))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.RecipientName")
                },
                new ColumnProperty(nameof(GiftCardModel.IsGiftCardActivated))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.IsGiftCardActivated"),
                    Width = "200",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(GiftCardModel.CreatedOn))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.Fields.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(GiftCardModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareGiftCardUsageHistoryGridModel(GiftCardUsageHistorySearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "usagehistory-grid",
                UrlRead = new DataUrl("UsageHistoryList", "GiftCard", new RouteValueDictionary { [nameof(GiftCardUsageHistorySearchModel.GiftCardId)] = searchModel.GiftCardId }),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(GiftCardUsageHistoryModel.CustomOrderNumber))
                {
                    Visible = false
                },
                new ColumnProperty(nameof(GiftCardUsageHistoryModel.CreatedOn))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.History.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(GiftCardUsageHistoryModel.OrderId))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.History.CustomOrderNumber"),
                    Width = "200",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderCustom("renderColumnOrderNumber")
                },
                new ColumnProperty(nameof(GiftCardUsageHistoryModel.UsedValue))
                {
                    Title = _localizationService.GetResource("Admin.GiftCards.History.UsedValue"),
                    Width = "200"
                }
            };

            return model;
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
            searchModel.Grid = PrepareGiftCardGridModel(searchModel);

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