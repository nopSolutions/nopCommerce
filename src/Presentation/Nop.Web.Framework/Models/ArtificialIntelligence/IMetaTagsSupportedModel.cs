namespace Nop.Web.Framework.Models.ArtificialIntelligence;

/// <summary>
/// Represents a model which supports meta-tags
/// </summary>
public partial interface IMetaTagsSupportedModel
{
    #region Properties

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public string MetaTitle { get; set; }

    #endregion
}