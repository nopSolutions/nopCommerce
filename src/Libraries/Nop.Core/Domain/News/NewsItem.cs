using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsItem : BaseEntity
    {
        private ICollection<NewsComment> _newsComments;

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the news title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the short text
        /// </summary>
        public virtual string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text
        /// </summary>
        public virtual string Full { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news item is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news post comments are allowed 
        /// </summary>
        public virtual bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the news comments
        /// </summary>
        public virtual ICollection<NewsComment> NewsComments
        {
            get { return _newsComments ?? (_newsComments = new List<NewsComment>()); }
            protected set { _newsComments = value; }
        }
        
        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }
}