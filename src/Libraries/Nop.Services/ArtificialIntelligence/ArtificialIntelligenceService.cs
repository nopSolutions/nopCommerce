using System.Linq.Expressions;
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

        var currentLanguageId = languageId == 0 ? _localizationSettings.DefaultAdminLanguageId : languageId;
        var lang = await _languageService.GetLanguageByIdAsync(currentLanguageId);

        (title, text) = entity switch
        {
            Product product => await fillTitleAndText(product, p => p.Name, p => p.FullDescription,
                "Admin.ArtificialIntelligence.ProductDescriptionRequired", "Admin.ArtificialIntelligence.ProductNameRequired"),
            Category category => await fillTitleAndText(category, c => c.Name, c => c.Description,
                "Admin.ArtificialIntelligence.CategoryDescriptionRequired", "Admin.ArtificialIntelligence.CategoryNameRequired"),
            _ => (title, text)
        };

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

        async Task<(string title, string text)> fillTitleAndText<TEntity>(TEntity metaTagsSupported, Expression<Func<TEntity, string>> titleSelector, Expression<Func<TEntity, string>> textSelector, string textRequiredLocale, string titleRequiredLocale) where TEntity : BaseEntity, ILocalizedEntity, IMetaTagsSupported
        {
            string resTitle;
            string resText;

            var getTitle = titleSelector.Compile();
            var getText = titleSelector.Compile();

            if (languageId == 0)
            {
                resTitle = getTitle(metaTagsSupported);
                resText = getText(metaTagsSupported);
            }
            else
            {
                entity.MetaTitle = await _localizationService.GetLocalizedAsync(metaTagsSupported, mt => mt.MetaTitle, languageId, false);
                entity.MetaKeywords = await _localizationService.GetLocalizedAsync(metaTagsSupported, mt => mt.MetaKeywords, languageId, false);
                entity.MetaDescription = await _localizationService.GetLocalizedAsync(metaTagsSupported, mt => mt.MetaDescription, languageId, false);

                resTitle = await _localizationService.GetLocalizedAsync(metaTagsSupported, titleSelector, languageId, false);
                resText = await _localizationService.GetLocalizedAsync(metaTagsSupported, textSelector, languageId, false);
            }

            if (string.IsNullOrEmpty(resTitle))
                throw new NopException(string.Format(await _localizationService.GetResourceAsync(titleRequiredLocale), lang.Name));

            if (!string.IsNullOrEmpty(resText))
                resText = Regex.Replace(resText, "<.*?>", string.Empty);

            if (string.IsNullOrEmpty(resText))
                throw new NopException(string.Format(await _localizationService.GetResourceAsync(textRequiredLocale), lang.Name));

            return (resTitle, resText);
        }
    }

    #endregion
}
