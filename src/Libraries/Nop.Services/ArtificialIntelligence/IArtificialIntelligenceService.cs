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
}