namespace Nop.Web.Framework.Models.Translation;

/// <summary>
/// Represents translated model
/// </summary>
public partial record TranslatedModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the input name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the translated value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the origin value
    /// </summary>
    public string OriginValue { get; set; }

    /// <summary>
    /// Gets or sets the language code
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the origin language code
    /// </summary>
    public string OriginLanguage { get; set; }
}