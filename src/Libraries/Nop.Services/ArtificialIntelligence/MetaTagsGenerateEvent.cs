using Nop.Core.Events;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents a meta tags generation event
/// </summary>
public partial class MetaTagsGenerateEvent : IStopProcessingEvent
{
    #region Fields

    private readonly IArtificialIntelligenceService _artificialIntelligenceService;

    #endregion

    #region Ctor

    public MetaTagsGenerateEvent(IArtificialIntelligenceService artificialIntelligenceService)
    {
        _artificialIntelligenceService = artificialIntelligenceService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Validates title and text
    /// </summary>
    /// <param name="textRequiredLocale">Locale for raising an exception about the title field required</param>
    /// <param name="titleRequiredLocale">Locale for raising an exception about the text field required</param>
    /// <param name="title">Title for validate</param>
    /// <param name="text">Text for validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validated title and text
    /// </returns>
    public virtual async Task<(string title, string text)> ValidateTitleAndTextAsync(string textRequiredLocale, string titleRequiredLocale, string title, string text)
    {
        return await _artificialIntelligenceService.ValidateTitleAndTextAsync(LanguageName, textRequiredLocale, titleRequiredLocale, title, text);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    public bool StopProcessing { get; set; }

    /// <summary>
    /// Gets or sets the name of entity type to which generating meta tags
    /// </summary>
    public string EntityTypeName { get; set; }

    /// <summary>
    /// Gets or sets the entity identifier to which generating meta tags
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the language identifier to which generating meta tags
    /// </summary>
    public int LanguageId { get; set; }

    /// <summary>
    /// Gets or sets the language name to which generating meta tags
    /// </summary>
    public string LanguageName { get; set; }

    /// <summary>
    /// Gets or sets the current meta title of entity
    /// </summary>
    public string CurrentMetaTitle { get; set; }

    /// <summary>
    /// Gets or sets the current meta keywords of entity
    /// </summary>
    public string CurrentMetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the Current meta description of entity
    /// </summary>
    public string CurrentMetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the title of entity
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the main text of entity
    /// </summary>
    public string Text { get; set; }

    #endregion
}