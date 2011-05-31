
using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Discounts;

namespace Nop.Core.Domain.Topics
{
    /// <summary>
    /// Represents a topic
    /// </summary>
    public partial class Topic : BaseEntity, ILocalizedEntity
    {
        //UNDONE add password protected topics support (like it was done in version 1.90)
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in sitemap
        /// </summary>
        public bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

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
}
