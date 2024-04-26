using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News;

/// <summary>
/// Represents a news item
/// </summary>
public partial class NewsItem : BaseEntity, ISlugSupported, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the language identifier
    /// </summary>
    public int LanguageId { get; set; }

    /// <summary>
    /// Gets or sets the news title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the short text
    /// </summary>
    public string Short { get; set; }

    /// <summary>
    /// Gets or sets the full text
    /// </summary>
    public string Full { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the news item is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the news item start date and time
    /// </summary>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the news item end date and time
    /// </summary>
    public DateTime? EndDateUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the news post comments are allowed 
    /// </summary>
    public bool AllowComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

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

    /// <summary>
    /// Gets or sets the date and time of entity creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}