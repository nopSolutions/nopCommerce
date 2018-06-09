using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the newsletter subscription model factory implementation
    /// </summary>
    public partial class NewsletterSubscriptionModelFactory : INewsletterSubscriptionModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public NewsletterSubscriptionModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStoreService storeService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare newsletter subscription search model
        /// </summary>
        /// <param name="searchModel">Newsletter subscription search model</param>
        /// <returns>Newsletter subscription search model</returns>
        public virtual NewsletterSubscriptionSearchModel PrepareNewsletterSubscriptionSearchModel(NewsletterSubscriptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available customer roles
            _baseAdminModelFactory.PrepareCustomerRoles(searchModel.AvailableCustomerRoles);

            //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
            searchModel.ActiveList.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.All")
            });
            searchModel.ActiveList.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.ActiveOnly")
            });
            searchModel.ActiveList.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.NotActiveOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged newsletter subscription list model
        /// </summary>
        /// <param name="searchModel">Newsletter subscription search model</param>
        /// <returns>Newsletter subscription list model</returns>
        public virtual NewsletterSubscriptionListModel PrepareNewsletterSubscriptionListModel(NewsletterSubscriptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter newsletter subscriptions
            var isActivatedOnly = searchModel.ActiveId == 0 ? null : searchModel.ActiveId == 1 ? true : (bool?)false;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get newsletter subscriptions
            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(email: searchModel.SearchEmail,
                customerRoleId: searchModel.CustomerRoleId,
                storeId: searchModel.StoreId,
                isActive: isActivatedOnly,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new NewsletterSubscriptionListModel
            {
                Data = newsletterSubscriptions.Select(subscription =>
                {
                    //fill in model values from the entity
                    var subscriptionModel = subscription.ToModel<NewsletterSubscriptionModel>();

                    //convert dates to the user time
                    subscriptionModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    subscriptionModel.StoreName = _storeService.GetStoreById(subscription.StoreId)?.Name ?? "Deleted";

                    return subscriptionModel;
                }),
                Total = newsletterSubscriptions.TotalCount
            };

            return model;
        }

        #endregion
    }
}