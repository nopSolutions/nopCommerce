using Nop.Core.Domain.Localization;
using Nop.Web.Areas.Admin.Models.Localization;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the language model factory
/// </summary>
public partial interface ILanguageModelFactory
{
    /// <summary>
    /// Prepare language search model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language search model
    /// </returns>
    Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel);

    /// <summary>
    /// Prepare paged language list model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language list model
    /// </returns>
    Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel);

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
    Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged locale resource list model
    /// </summary>
    /// <param name="searchModel">Locale resource search model</param>
    /// <param name="language">Language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale resource list model
    /// </returns>
    Task<LocaleResourceListModel> PrepareLocaleResourceListModelAsync(LocaleResourceSearchModel searchModel, Language language);
}