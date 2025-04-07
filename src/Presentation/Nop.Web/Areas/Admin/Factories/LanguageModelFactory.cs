using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the language model factory implementation
/// </summary>
public partial class LanguageModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        INopFileProvider fileProvider,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory) : ILanguageModelFactory
{
    #region Utilities

    /// <summary>
    /// Prepare locale resource search model
    /// </summary>
    /// <param name="searchModel">Locale resource search model</param>
    /// <param name="language">Language</param>
    /// <returns>Locale resource search model</returns>
    protected virtual LocaleResourceSearchModel PrepareLocaleResourceSearchModel(LocaleResourceSearchModel searchModel, Language language)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(language);

        searchModel.LanguageId = language.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare language search model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language search model
    /// </returns>
    public virtual Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged language list model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language list model
    /// </returns>
    public virtual async Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get languages
        var languages = (await languageService.GetAllLanguagesAsync(showHidden: true)).ToPagedList(searchModel);

        //prepare list model
        var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
        {
            return languages.Select(language => language.ToModel<LanguageModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare language model
    /// </summary>
    /// <param name="model">Language model</param>
    /// <param name="language">Language</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language model
    /// </returns>
    public virtual async Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false)
    {
        if (language != null)
        {
            //fill in model values from the entity
            model ??= language.ToModel<LanguageModel>();

            //prepare nested search model
            PrepareLocaleResourceSearchModel(model.LocaleResourceSearchModel, language);
        }

        //set default values for the new model
        if (language == null)
        {
            model.DisplayOrder = (await languageService.GetAllLanguagesAsync()).Max(l => l.DisplayOrder) + 1;
            model.Published = true;
        }
        var flagNames = fileProvider
            .EnumerateFiles(fileProvider.GetAbsolutePath(@"images\flags"), "*.png")
            .Select(fileProvider.GetFileName)
            .ToList();

        model.AvailableFlagImages = flagNames.ConvertAll(flagName => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Text = flagName,
            Value = flagName
        });

        //prepare available currencies
        await baseAdminModelFactory.PrepareCurrenciesAsync(model.AvailableCurrencies,
            defaultItemText: await localizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

        //prepare available stores
        await storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, language, excludeProperties);

        return model;
    }

    /// <summary>
    /// Prepare paged locale resource list model
    /// </summary>
    /// <param name="searchModel">Locale resource search model</param>
    /// <param name="language">Language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale resource list model
    /// </returns>
    public virtual async Task<LocaleResourceListModel> PrepareLocaleResourceListModelAsync(LocaleResourceSearchModel searchModel, Language language)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(language);

        //get locale resources
        var localeResources = (await localizationService.GetAllResourceValuesAsync(language.Id, loadPublicLocales: null))
            .OrderBy(localeResource => localeResource.Key).AsQueryable();

        //filter locale resources
        if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
            localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
        if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
            localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

        var pagedLocaleResources = await localeResources.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

        //prepare list model
        var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
        {
            //fill in model values from the entity
            return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
            {
                LanguageId = language.Id,
                Id = localeResource.Value.Key,
                ResourceName = localeResource.Key,
                ResourceValue = localeResource.Value.Value
            });
        });

        return model;
    }

    #endregion
}