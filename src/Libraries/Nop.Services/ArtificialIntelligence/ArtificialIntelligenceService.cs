using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Html;
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
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly IProductService _productService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ArtificialIntelligenceService(ArtificialIntelligenceHttpClient httpClient,
        ArtificialIntelligenceSettings artificialIntelligenceSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        IHtmlFormatter htmlFormatter,
        IProductService productService,
        LocalizationSettings localizationSettings)
    {
        _httpClient = httpClient;
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
        _htmlFormatter = htmlFormatter;
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
        metaTags.MetaTitle = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaTitle, languageId);
        metaTags.MetaKeywords = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaKeywords, languageId);
        metaTags.MetaDescription = await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaDescription, languageId);
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
            return await _httpClient.SendQueryAsync(query);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);

            return string.Format(await _localizationService.GetResourceAsync("ArtificialIntelligence.CreateProductFailed"), e.Message);
        }
    }

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entityType">The entity tape name</param>
    /// <param name="entityId">The entity identifier</param>
    /// <param name="languageId">The language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    public async Task<IMetaTagsSupported> CreateMetaTagsAsync(string entityType, int entityId, int languageId)
    {
        var metaTitleQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaTitleQuery)
            ? ArtificialIntelligenceDefaults.MetaTitleQuery
            : _artificialIntelligenceSettings.MetaTitleQuery;

        var metaKeywordsQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaKeywordsQuery)
            ? ArtificialIntelligenceDefaults.MetaKeywordsQuery
            : _artificialIntelligenceSettings.MetaKeywordsQuery;

        var metaDescriptionQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaDescriptionQuery)
            ? ArtificialIntelligenceDefaults.MetaDescriptionQuery
            : _artificialIntelligenceSettings.MetaDescriptionQuery;

        var entity = new BaseMetaTagsSupportedEntity();
        var title = string.Empty;
        var text = string.Empty;

        switch (entityType)
        {
            case nameof(Product):
                var product = await _productService.GetProductByIdAsync(entityId);

                if (languageId == 0)
                {
                    entity.CopyFrom(product);
                    title = product.Name;
                    text = product.FullDescription;
                }
                else
                {
                    await FillLocalizedMetaDataAsync(entity, product, languageId);
                    title = await _localizationService.GetLocalizedAsync(product, p => p.Name, languageId);
                    text = await _localizationService.GetLocalizedAsync(product, p => p.FullDescription, languageId);
                }

                break;
        }

        text = Regex.Replace(text, "<.*?>", string.Empty);

        var currentLanguageId = languageId == 0 ? _localizationSettings.DefaultAdminLanguageId : languageId;
        var lang = await _languageService.GetLanguageByIdAsync(currentLanguageId);

        var metaTitleQuery = string.Format(metaTitleQueryFormat, title, text, lang.Name);
        var metaKeywordsQuery = string.Format(metaKeywordsQueryFormat, title, text, lang.Name);
        var metaDescriptionQuery = string.Format(metaDescriptionQueryFormat, title, text, lang.Name);

        try
        {
            if (string.IsNullOrEmpty(entity.MetaTitle))
            {
                var result = await _httpClient.SendQueryAsync(metaTitleQuery, false);
                entity.MetaTitle = result.Trim('"');
            }

            if (string.IsNullOrEmpty(entity.MetaKeywords))
                entity.MetaKeywords = await _httpClient.SendQueryAsync(metaKeywordsQuery, false);

            if (string.IsNullOrEmpty(entity.MetaDescription))
            {
                var result = await _httpClient.SendQueryAsync(metaDescriptionQuery, false);
                entity.MetaDescription = result.Trim('"');
            }
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);

            throw new NopException(e.Message);
        }

        return entity;
    }

    #endregion
}
