using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Markdig;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
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
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ArtificialIntelligenceService(ArtificialIntelligenceHttpClient httpClient,
        ArtificialIntelligenceSettings artificialIntelligenceSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        IWorkContext workContext,
        LocalizationSettings localizationSettings)
    {
        _httpClient = httpClient;
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
        _workContext = workContext;
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
    /// Gets the title and text for AI request for corresponding entity
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">Entity to get title and text</param>
    /// <param name="languageId">Target language identifier</param>
    /// <param name="languageName">Target language name</param>
    /// <param name="titleSelector">Title field selector</param>
    /// <param name="textSelector">Text field selector</param>
    /// <param name="textRequiredLocale">Locale for raising an exception about the title field required</param>
    /// <param name="titleRequiredLocale">Locale for raising an exception about the text field required</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the title and text for AI request
    /// </returns>
    protected virtual async Task<(string title, string text)> GetTitleAndTextAsync<TEntity>(TEntity entity, int languageId, string languageName,
        Expression<Func<TEntity, string>> titleSelector, Expression<Func<TEntity, string>> textSelector,
        string textRequiredLocale, string titleRequiredLocale)
        where TEntity : BaseEntity, ILocalizedEntity, IMetaTagsSupported
    {
        var getTitle = titleSelector.Compile();
        var getText = textSelector.Compile();

        if (languageId == 0)
            return await GetTitleAndTextAsync(entity, languageName, getTitle, getText, textRequiredLocale, titleRequiredLocale);

        var title = await _localizationService.GetLocalizedAsync(entity, titleSelector, languageId, false);
        var text = await _localizationService.GetLocalizedAsync(entity, textSelector, languageId, false);

        return await ValidateTitleAndTextAsync(languageName, textRequiredLocale, titleRequiredLocale, title, text);
    }

    /// <summary>
    /// Gets the title and text for AI request for corresponding entity
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">Entity to get title and text</param>
    /// <param name="languageName">Target language name</param>
    /// <param name="titleSelector">Title field selector</param>
    /// <param name="textSelector">Text field selector</param>
    /// <param name="textRequiredLocale">Locale for raising an exception about the title field required</param>
    /// <param name="titleRequiredLocale">Locale for raising an exception about the text field required</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the title and text for AI request
    /// </returns>
    protected virtual async Task<(string title, string text)> GetTitleAndTextAsync<TEntity>(TEntity entity, string languageName,
        Func<TEntity, string> titleSelector, Func<TEntity, string> textSelector,
        string textRequiredLocale, string titleRequiredLocale)
        where TEntity : BaseEntity, IMetaTagsSupported
    {
        var title = titleSelector(entity);
        var text = textSelector(entity);

        return await ValidateTitleAndTextAsync(languageName, textRequiredLocale, titleRequiredLocale, title, text);
    }

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
    protected virtual async Task<(string title, string text)> ValidateTitleAndTextAsync(string languageName,
        string textRequiredLocale, string titleRequiredLocale, string title, string text)
    {
        if (string.IsNullOrEmpty(title))
            throw new NopException(string.Format(await _localizationService.GetResourceAsync(titleRequiredLocale), languageName));

        if (!string.IsNullOrEmpty(text))
            text = Regex.Replace(text, "<.*?>", string.Empty);

        if (string.IsNullOrEmpty(text))
            throw new NopException(string.Format(await _localizationService.GetResourceAsync(textRequiredLocale), languageName));

        return (title, text);
    }

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entity">The entity to which need to generate meta tags</param>
    /// <param name="currentMetaTitle">Current entity meta title</param>
    /// <param name="currentMetaKeywords">Current entity meta keywords</param>
    /// <param name="currentMetaDescription">Current entity meta description</param>
    /// <param name="languageId">Target language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    protected virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<TEntity>(TEntity entity,
        string currentMetaTitle, string currentMetaKeywords, string currentMetaDescription, int languageId)
        where TEntity : BaseEntity, IMetaTagsSupported
    {
        var title = string.Empty;
        var text = string.Empty;
        var metaTitle = string.Empty;
        var metaKeywords = string.Empty;
        var metaDescription = string.Empty;

        var currentLanguage = await _languageService.GetLanguageByIdAsync(languageId != 0 ? languageId : _localizationSettings.DefaultAdminLanguageId);

        (title, text) = entity switch
        {
            Product product => await GetTitleAndTextAsync(product, languageId, currentLanguage.Name, p => p.Name, p => p.FullDescription,
                "Admin.ArtificialIntelligence.ProductDescriptionRequired", "Admin.ArtificialIntelligence.ProductNameRequired"),
            Category category => await GetTitleAndTextAsync(category, languageId, currentLanguage.Name, c => c.Name, c => c.Description,
                "Admin.ArtificialIntelligence.CategoryDescriptionRequired", "Admin.ArtificialIntelligence.CategoryNameRequired"),
            BlogPost blogPost => await GetTitleAndTextAsync(blogPost, currentLanguage.Name, bp => bp.Title, bp => bp.Body,
                "Admin.ArtificialIntelligence.BlogPostBodyRequired", "Admin.ArtificialIntelligence.BlogPostTitleRequired"),
            Manufacturer manufacturer => await GetTitleAndTextAsync(manufacturer, languageId, currentLanguage.Name, m => m.Name, m => m.Description,
                "Admin.ArtificialIntelligence.ManufacturerDescriptionRequired", "Admin.ArtificialIntelligence.ManufacturerNameRequired"),
            NewsItem newsItem => await GetTitleAndTextAsync(newsItem, currentLanguage.Name, n => n.Title, n => n.Full,
                "Admin.ArtificialIntelligence.NewsItemFullRequired", "Admin.ArtificialIntelligence.NewsItemTitleRequired"),
            Topic topic => await GetTitleAndTextAsync(topic, languageId, currentLanguage.Name, t => t.Title, t => t.Body,
                "Admin.ArtificialIntelligence.TopicBodyRequired", "Admin.ArtificialIntelligence.TopicTitleRequired"),
            Vendor vendor => await GetTitleAndTextAsync(vendor, languageId, currentLanguage.Name, v => v.Name, v => v.Description,
                "Admin.ArtificialIntelligence.VendorDescriptionRequired", "Admin.ArtificialIntelligence.VendorNameRequired"),
            _ => (title, text)
        };

        try
        {
            if (_artificialIntelligenceSettings.AllowMetaTitleGeneration && string.IsNullOrEmpty(currentMetaTitle))
            {
                var metaTitleQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaTitleQuery)
                    ? ArtificialIntelligenceDefaults.MetaTitleQuery
                    : _artificialIntelligenceSettings.MetaTitleQuery;
                var metaTitleQuery = string.Format(metaTitleQueryFormat, title, text, currentLanguage.Name);
                var result = await _httpClient.SendQueryAsync(metaTitleQuery);
                metaTitle = result.Trim('"');
            }
            else
            {
                metaTitle = currentMetaTitle;
            }

            if (_artificialIntelligenceSettings.AllowMetaKeywordsGeneration && string.IsNullOrEmpty(currentMetaKeywords))
            {
                var metaKeywordsQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaKeywordsQuery)
                    ? ArtificialIntelligenceDefaults.MetaKeywordsQuery
                    : _artificialIntelligenceSettings.MetaKeywordsQuery;
                var metaKeywordsQuery = string.Format(metaKeywordsQueryFormat, title, text, currentLanguage.Name);
                metaKeywords = await _httpClient.SendQueryAsync(metaKeywordsQuery);
            }
            else
            {
                metaKeywords = currentMetaKeywords;
            }

            if (_artificialIntelligenceSettings.AllowMetaDescriptionGeneration && string.IsNullOrEmpty(currentMetaDescription))
            {
                var metaDescriptionQueryFormat = string.IsNullOrEmpty(_artificialIntelligenceSettings.MetaDescriptionQuery)
                    ? ArtificialIntelligenceDefaults.MetaDescriptionQuery
                    : _artificialIntelligenceSettings.MetaDescriptionQuery;
                var metaDescriptionQuery = string.Format(metaDescriptionQueryFormat, title, text, currentLanguage.Name);
                var result = await _httpClient.SendQueryAsync(metaDescriptionQuery);
                metaDescription = result.Trim('"');
            }
            else
            {
                metaDescription = currentMetaDescription;
            }

        }
        catch (Exception e)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _logger.ErrorAsync(e.Message, e, customer);

            throw new NopException(e.Message);
        }

        return (metaTitle, metaKeywords, metaDescription);
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
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _logger.ErrorAsync(e.Message, e, customer);

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
    public virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsForLocalizedEntityAsync<TEntity>(TEntity entity, int languageId)
        where TEntity : BaseEntity, IMetaTagsSupported, ILocalizedEntity
    {
        var currentMetaTitle = languageId == 0
            ? entity.MetaTitle
            : await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaTitle, languageId, false);
        var currentMetaKeywords = languageId == 0
            ? entity.MetaKeywords
            : await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaKeywords, languageId, false);
        var currentMetaDescription = languageId == 0
            ? entity.MetaDescription
            : await _localizationService.GetLocalizedAsync(entity, mt => mt.MetaDescription, languageId, false);

        return await CreateMetaTagsAsync(entity, currentMetaTitle, currentMetaKeywords, currentMetaDescription, languageId);
    }

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    /// <param name="entity">The entity to which need to generate meta tags</param>
    /// <param name="languageId">The language identifier; leave 0 to use <see cref="LocalizationSettings.DefaultAdminLanguageId"/></param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated meta tags
    /// </returns>
    public virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<TEntity>(TEntity entity, int languageId = 0)
        where TEntity : BaseEntity, IMetaTagsSupported
    {
        var metaTitle = entity.MetaTitle;
        var metaKeywords = entity.MetaKeywords;
        var metaDescription = entity.MetaDescription;

        return await CreateMetaTagsAsync(entity, metaTitle, metaKeywords, metaDescription, languageId);
    }

    #endregion
}