using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Artificial intelligence service interface
/// </summary>
public partial interface IArtificialIntelligenceService
{
    /// <summary>
    /// Validates title and text
    /// </summary>
    /// <param name="languageName">Target language name</param>
    /// <param name="textRequiredLocale">Locale for raising an exception about the title field required</param>
    /// <param name="titleRequiredLocale">Locale for raising an exception about the text field required</param>
    /// <param name="title">Title for validate</param>
    /// <param name="text">Text for validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validated title and text
    /// </returns>
    Task<(string title, string text)> ValidateTitleAndTextAsync(string languageName, string textRequiredLocale, string titleRequiredLocale, string title, string text);

    /// <summary>
    /// Create product description by artificial intelligence
    /// </summary>
    /// <param name="productName">Product name</param>
    /// <param name="keywords">Features and keywords</param>
    /// <param name="toneOfVoice">Tone of voice</param>
    /// <param name="instruction">Special instruction</param>
    /// <param name="customToneOfVoice">Custom tone of voice (applicable only for ToneOfVoiceType.Custom)</param>
    /// <param name="languageId">The language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated product description
    /// </returns>
    Task<string> CreateProductDescriptionAsync(string productName, string keywords, ToneOfVoiceType toneOfVoice, string instruction, string customToneOfVoice = null, int languageId = 0);

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entity">The entity to which need to generate meta tags</param>
    /// <param name="languageId">The language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsForLocalizedEntityAsync<TEntity>(TEntity entity, int languageId)
        where TEntity : BaseEntity, IMetaTagsSupported, ILocalizedEntity;

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entity">The entity to which need to generate meta tags</param>
    /// <param name="languageId">The language identifier; leave 0 to use <see cref="LocalizationSettings.DefaultAdminLanguageId"/></param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<TEntity>(TEntity entity, int languageId = 0)
        where TEntity : BaseEntity, IMetaTagsSupported;

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entityTypeName">The name of entity type to which need to generate meta tags</param>
    /// <param name="entityId">The entity identifier to which need to generate meta tags</param>
    /// <param name="languageId">The language identifier; leave 0 to use <see cref="LocalizationSettings.DefaultAdminLanguageId"/></param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync(string entityTypeName, int entityId, int languageId);
}