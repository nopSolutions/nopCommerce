using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the newsletter subscription model factory implementation
/// </summary>
public partial class NewsLetterSubscriptionModelFactory : INewsLetterSubscriptionModelFactory
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        IDateTimeHelper dateTimeHelper,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IStoreService storeService)
    {
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
        _dateTimeHelper = dateTimeHelper;
        _languageService = languageService;
        _localizationService = localizationService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare newsletter subscription search model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription search model
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionSearchModel> PrepareNewsLetterSubscriptionSearchModelAsync(NewsLetterSubscriptionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available customer roles
        await _baseAdminModelFactory.PrepareCustomerRolesAsync(searchModel.AvailableCustomerRoles);

        //prepare available newsletter subscription types
        await _baseAdminModelFactory.PrepareSubscriptionTypesAsync(searchModel.AvailableSubscriptionTypes);

        //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
        searchModel.ActiveList.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.All")
        });
        searchModel.ActiveList.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.ActiveOnly")
        });
        searchModel.ActiveList.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.NotActiveOnly")
        });

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged newsletter subscription list model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription list model
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionListModel> PrepareNewsLetterSubscriptionListModelAsync(NewsLetterSubscriptionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter newsletter subscriptions
        var isActivatedOnly = searchModel.ActiveId == 0 ? null : searchModel.ActiveId == 1 ? true : (bool?)false;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get newsletter subscriptions
        var newsletterSubscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(email: searchModel.SearchEmail,
            customerRoleId: searchModel.CustomerRoleId,
            subscriptionTypeId: searchModel.SubscriptionTypeId,
            storeId: searchModel.StoreId,
            isActive: isActivatedOnly,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var types = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(searchModel.StoreId);

        //prepare list model
        var model = await new NewsLetterSubscriptionListModel().PrepareToGridAsync(searchModel, newsletterSubscriptions, () =>
        {
            return newsletterSubscriptions.SelectAwait(async subscription =>
            {
                //fill in model values from the entity
                var subscriptionModel = subscription.ToModel<NewsLetterSubscriptionModel>();

                //convert dates to the user time
                subscriptionModel.CreatedOn = (await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc)).ToString();

                //fill in additional values (not existing in the entity)
                subscriptionModel.StoreName = (await _storeService.GetStoreByIdAsync(subscription.StoreId))?.Name ?? "Deleted";
                subscriptionModel.LanguageName = (await _languageService.GetLanguageByIdAsync(subscription.LanguageId))?.Name ?? string.Empty;

                subscriptionModel.SubscriptionTypeName = types.FirstOrDefault(type => type.Id == subscription.TypeId)?.Name ?? string.Empty;

                return subscriptionModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare newsletter subscription model
    /// </summary>
    /// <param name="model">Newsletter subscription model</param>
    /// <param name="subscription">Subscription</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription model
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionModel> PrepareNewsLetterSubscriptionModelAsync(NewsLetterSubscriptionModel model,
        NewsLetterSubscription subscription, bool excludeProperties = false)
    {
        if (subscription != null)
        {
            //fill in model values from the entity
            model ??= subscription.ToModel<NewsLetterSubscriptionModel>();

            //convert dates to the user time
            model.CreatedOn = (await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc)).ToString();

            model.SubscriptionTypeName = (await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(subscription.TypeId))?.Name;

            //prepare localized models
            if (!excludeProperties)
            {
                //prepare model newsletter subscriptions
                model.SelectedNewsLetterSubscriptionTypeId = subscription.TypeId;
                model.SelectedNewsLetterSubscriptionStoreId = subscription.StoreId;
                model.SelectedNewsLetterSubscriptionLanguageId = subscription.LanguageId;
            }
        }

        //prepare model subscription types for newsletter subscription
        await _baseAdminModelFactory.PrepareSubscriptionTypesAsync(model.AvailableNewsLetterSubscriptionTypes, false);

        //prepare model stores for newsletter subscription
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableNewsLetterSubscriptionStores, false);

        //prepare model languages for newsletter subscription
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableNewsLetterSubscriptionLanguages, false);

        return model;
    }

    #endregion
}