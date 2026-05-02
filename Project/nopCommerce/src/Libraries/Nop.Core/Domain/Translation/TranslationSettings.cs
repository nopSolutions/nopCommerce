using Nop.Core.Configuration;

namespace Nop.Core.Domain.Translation;

/// <summary>
/// Translation settings
/// </summary>
public partial class TranslationSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether pre-translation is allowed
    /// </summary>
    public bool AllowPreTranslate { get; set; }

    /// <summary>
    /// Gets or sets a language to translate from
    /// </summary>
    public int TranslateFromLanguageId { get; set; }

    /// <summary>
    /// Gets or sets a list of languages which is not allowed to pre-translate
    /// </summary>
    public List<int> NotTranslateLanguages { get; set; } = new();

    /// <summary>
    /// Gets or sets the Google Translate API key
    /// </summary>
    public string GoogleApiKey { get; set; }

    /// <summary>
    /// Gets or sets the DeepL Auth key
    /// </summary>
    public string DeepLAuthKey { get; set; }

    /// <summary>
    /// Gets or sets a translation service type id
    /// </summary>
    public int TranslationServiceId { get; set; }
}