namespace Nop.Web.Framework.Models.Translation;

/// <summary>
/// Represents translation model
/// </summary>
public partial record TranslationModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the translations
    /// </summary>
    public List<TranslatedModel> Translations { get; set; } = new();

    /// <summary>
    /// Gets or sets the value that indicates whether error(s) occurred during translation
    /// </summary>
    public bool HasErrors { get; set; }
}