using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents translation model factory
/// </summary>
public partial interface ITranslationModelFactory
{
    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params string[] propertiesToTranslate)
        where T : ILocalizedLocaleModel;

    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params (string PropertyName, bool IsHtml)[] propertiesToTranslate)
        where T : ILocalizedLocaleModel;
}