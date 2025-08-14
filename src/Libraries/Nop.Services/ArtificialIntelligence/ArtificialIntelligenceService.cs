using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Localization;
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
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ArtificialIntelligenceService(ArtificialIntelligenceHttpClient httpClient,
        ArtificialIntelligenceSettings artificialIntelligenceSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        LocalizationSettings localizationSettings)
    {
        _httpClient = httpClient;
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
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
            return await _httpClient.SendQueryAsync(query);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);

            return string.Format(await _localizationService.GetResourceAsync("ArtificialIntelligence.CreateProductFailed"), e.Message);
        }
    }

    #endregion
}
