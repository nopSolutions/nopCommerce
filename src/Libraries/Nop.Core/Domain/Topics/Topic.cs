using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Topics;

/// <summary>
/// Represents a topic
/// </summary>
public partial class Topic : BaseEntity, ILocalizedEntity, ISlugSupported, IStoreMappingSupported, IAclSupported
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in sitemap
    /// </summary>
    public bool IncludeInSitemap { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in top menu
    /// </summary>
    public bool IncludeInTopMenu { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
    /// </summary>
    public bool IncludeInFooterColumn1 { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
    /// </summary>
    public bool IncludeInFooterColumn2 { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
    /// </summary>
    public bool IncludeInFooterColumn3 { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic is accessible when a store is closed
    /// </summary>
    public bool AccessibleWhenStoreClosed { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether this topic is password protected
    /// </summary>
    public bool IsPasswordProtected { get; set; }

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the body
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value of used topic template identifier
    /// </summary>
    public int TopicTemplateId { get; set; }

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
    /// Gets or sets a value indicating whether the entity is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }
}