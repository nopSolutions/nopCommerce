namespace Nop.Web.Framework.Models.ArtificialIntelligence;

public partial record MetaTagsModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the translations
    /// </summary>
    public GeneratedMetaTags MetaTags { get; set; } = new();

    /// <summary>
    /// Gets the value that indicates whether error(s) occurred during generation of meta tags
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(Error);

    /// <summary>
    /// Gets or sets the error text occurred during generation of meta tags
    /// </summary>
    public string Error { get; set; }
}