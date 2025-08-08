namespace Nop.Core.Domain.Common;

/// <summary>
/// Represents an entity which supports meta tags
/// </summary>
public partial interface IMetaTagsSupported
{
    /// <summary>
    /// Gets or sets the meta keywords
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta title
    /// </summary>
    public string MetaTitle { get; set; }
}

public partial class BaseMetaTagsSupportedEntity : IMetaTagsSupported
{
    /// <summary>
    /// Gets or sets the meta keywords
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta title
    /// </summary>
    public string MetaTitle { get; set; }

    public virtual void CopyFrom(IMetaTagsSupported other)
    {
        MetaTitle = other.MetaTitle;
        MetaKeywords = other.MetaKeywords;
        MetaDescription = other.MetaDescription;
    }
}