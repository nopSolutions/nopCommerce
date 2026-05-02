namespace Nop.Web.Framework.Models.Translation;

/// <summary>
/// Represents translation supported model
/// </summary>
public partial interface ITranslationSupportedModel
{
    /// <summary>
    /// Gets or sets the value whether pre-translation is available
    /// </summary>
    bool PreTranslationAvailable { get; set; }
}