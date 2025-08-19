using System.Text.RegularExpressions;
using Markdig;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represent Artificial intelligence service
/// </summary>
public partial class ArtificialIntelligenceService : IArtificialIntelligenceService
{
    #region Fields

    protected readonly ArtificialIntelligenceHttpClient _httpClient;
    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IProductService _productService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ArtificialIntelligenceService(ArtificialIntelligenceHttpClient httpClient,
        ArtificialIntelligenceSettings artificialIntelligenceSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        IProductService productService,
        LocalizationSettings localizationSettings)
    {
        _httpClient = httpClient;
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
        _productService = productService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get tone of voice instruction for query
    /// </summary>
    /// <param name="toneOfVoice">Tone of voice type</param>
    /// <param name="customToneOfVoice">Custom tone of voice instruction (applicable only for ToneOfVoiceType.Custom)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tone of voice instruction
    /// </returns>
    protected virtual async Task<string> GetTonOfVoiceInstructionAsync(ToneOfVoiceType toneOfVoice, string customToneOfVoice)
    {
        if (toneOfVoice == ToneOfVoiceType.Custom && string.IsNullOrEmpty(customToneOfVoice))
            toneOfVoice = ToneOfVoiceType.Expert;

        return toneOfVoice switch
        {
            ToneOfVoiceType.Expert or ToneOfVoiceType.Supportive => await _localizationService.GetResourceAsync(
                $"ArtificialIntelligence.ToneOfVoice.{toneOfVoice}"),
            ToneOfVoiceType.Custom => customToneOfVoice,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Fill localized meta data of entity
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="metaTags">Meta tags supported item to store meta data</param>
    /// <param name="entity">Entity to get localized meta data</param>
    /// <param name="languageId">The language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task FillLocalizedMetaDataAsync<T>(IMetaTagsSupported metaTags, T entity, int languageId) where T : BaseEntity, ILocalizedEntity, IMetaTagsSupported
    {
        metaTags.MetaTitle = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaTitle, languageId, false);
        metaTags.MetaKeywords = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaKeywords, languageId, false);
        metaTags.MetaDescription = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaDescription, languageId, false);
    }

    #endregion

    #region Methods

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
    public virtual async Task<string> CreateProductDescriptionAsync(string productName, string keywords, ToneOfVoiceType toneOfVoice, string instruction, string customToneOfVoice = null, int languageId = 0)
    {
        var toneOfVoiceInstruction = await GetTonOfVoiceInstructionAsync(toneOfVoice, customToneOfVoice);

        if (languageId == 0)
            languageId = _localizationSettings.DefaultAdminLanguageId;

        var lang = await _languageService.GetLanguageByIdAsync(languageId);

        var query = string.Format(string.IsNullOrEmpty(_artificialIntelligenceSettings.ProductDescriptionQuery) ? ArtificialIntelligenceDefaults.ProductDescriptionQuery : _artificialIntelligenceSettings.ProductDescriptionQuery, productName, keywords, toneOfVoiceInstruction, instruction, lang.Name);

        try
        {
            var result = await _httpClient.SendQueryAsync(query);
            return Markdown.ToHtml(result);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);

            throw new NopException(string.Format(await _localizationService.GetResourceAsync("ArtificialIntelligence.CreateProductFailed"), e.Message));
        }
    }

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entity">The entity to which need to generate meta tags</param>
    /// <param name="languageId">The language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    public virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<T>(T entity, int languageId) where T: BaseEntity, IMetaTagsSupported
    {
        var title = string.Empty;
        var text = string.Empty;
        var metaTitle = string.Empty;
        var metaKeywords = string.Empty;
        var metaDescription = string.Empty;

        if (entity is Product product)
        {
            if (languageId == 0)
            {
                title = product.Name;
                text = product.FullDescription;
            }
            else
            {
                await FillLocalizedMetaDataAsync(entity, product, languageId);
                title = await _localizationService.GetLocalizedAsync(product, p => p.Name, languageId);
                text = await _localizationService.GetLocalizedAsync(product, p => p.FullDescription, languageId);
            }

            text = Regex.Replace(text, "<.*?>", string.Empty);

            if (string.IsNullOrEmpty(text))
                throw new NopException(await _localizationService.GetResourceAsync("Admin.ArtificialIntelligence.ProductDescriptionRequired"));
        }

        var currentLanguageId = languageId == 0 ? _localizationSettings.DefaultAdminLanguageId : languageId;
        var lang = await _languageService.GetLanguageByIdAsync(currentLanguageId);

        try
        {
            if (_artificialIntelligenceSettings.AllowMetaTitleGeneration && string.IsNullOrEmpty(entity.MetaTitle))
            {
                var metaTitleQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaTitleQuery)
                    ? ArtificialIntelligenceDefaults.MetaTitleQuery
                    : _artificialIntelligenceSettings.MetaTitleQuery;
                var metaTitleQuery = string.Format(metaTitleQueryFormat, title, text, lang.Name);
                var result = await _httpClient.SendQueryAsync(metaTitleQuery);
                metaTitle = result.Trim('"');
            }

            if (_artificialIntelligenceSettings.AllowMetaKeywordsGeneration && string.IsNullOrEmpty(entity.MetaKeywords))
            {
                var metaKeywordsQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaKeywordsQuery)
                    ? ArtificialIntelligenceDefaults.MetaKeywordsQuery
                    : _artificialIntelligenceSettings.MetaKeywordsQuery;
                var metaKeywordsQuery = string.Format(metaKeywordsQueryFormat, title, text, lang.Name);
                metaKeywords = await _httpClient.SendQueryAsync(metaKeywordsQuery);
            }

            if (_artificialIntelligenceSettings.AllowMetaDescriptionGeneration && string.IsNullOrEmpty(entity.MetaDescription))
            {
                var metaDescriptionQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaDescriptionQuery)
                    ? ArtificialIntelligenceDefaults.MetaDescriptionQuery
                    : _artificialIntelligenceSettings.MetaDescriptionQuery;
                var metaDescriptionQuery = string.Format(metaDescriptionQueryFormat, title, text, lang.Name);
                var result = await _httpClient.SendQueryAsync(metaDescriptionQuery);
                metaDescription = result.Trim('"');
            }
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);

            throw new NopException(e.Message);
        }

        return (metaTitle, metaKeywords, metaDescription);
    }

    #endregion
}
