using Nop.Core.Domain.Localization;
using Nop.Web.Areas.Admin.Models.Localization;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the language model factory
    /// </summary>
    public partial interface ILanguageModelFactory
    {
        /// <summary>
        /// Prepare language search model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language search model</returns>
        LanguageSearchModel PrepareLanguageSearchModel(LanguageSearchModel searchModel);

        /// <summary>
        /// Prepare paged language list model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language list model</returns>
        LanguageListModel PrepareLanguageListModel(LanguageSearchModel searchModel);

        /// <summary>
        /// Prepare language model
        /// </summary>
        /// <param name="model">Language model</param>
        /// <param name="language">Language</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Language model</returns>
        LanguageModel PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged locale resource list model
        /// </summary>
        /// <param name="searchModel">Locale resource search model</param>
        /// <param name="language">Language</param>
        /// <returns>Locale resource list model</returns>
        LocaleResourceListModel PrepareLocaleResourceListModel(LocaleResourceSearchModel searchModel, Language language);
    }
}