using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the newsletter subscription type model factory implementation
/// </summary>
public partial class NewsLetterSubscriptionTypeModelFactory : INewsLetterSubscriptionTypeModelFactory
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionTypeModelFactory(ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
    {
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare newsletter subscription type search model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type search model
    /// </returns>
    public virtual Task<NewsLetterSubscriptionTypeSearchModel> PrepareNewsLetterSubscriptionTypeSearchModelAsync(NewsLetterSubscriptionTypeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged newsletter subscription type list model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type list model
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionTypeListModel> PrepareNewsLetterSubscriptionTypeListModelAsync(NewsLetterSubscriptionTypeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get newsletter subscription type
        var subscriptionTypes = (await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = new NewsLetterSubscriptionTypeListModel().PrepareToGrid(searchModel, subscriptionTypes, () =>
        {
            //fill in model values from the entity
            return subscriptionTypes.Select(subscriptionType => subscriptionType.ToModel<NewsLetterSubscriptionTypeModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare newsletter subscription type model
    /// </summary>
    /// <param name="model">Newsletter subscription type model</param>
    /// <param name="subscriptionType">Subscription type</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type model
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionTypeModel> PrepareNewsLetterSubscriptionTypeModelAsync(NewsLetterSubscriptionTypeModel model,
        NewsLetterSubscriptionType subscriptionType, bool excludeProperties = false)
    {
        Func<NewsLetterSubscriptionTypeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (subscriptionType != null)
        {
            //fill in model values from the entity
            model ??= subscriptionType.ToModel<NewsLetterSubscriptionTypeModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(subscriptionType, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, subscriptionType, excludeProperties);

        return model;
    }

    #endregion
}
