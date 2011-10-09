using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Localization
{
    /// <summary>
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity
    {
        private ICollection<LocaleStringResource> _localeStringResources;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public virtual string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the unique SEO code
        /// </summary>
        public virtual string UniqueSeoCode { get; set; }
        
        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public virtual string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language supports "Right-to-left"
        /// </summary>
        public virtual bool Rtl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources
        {
            get { return _localeStringResources ?? (_localeStringResources = new List<LocaleStringResource>()); }
            protected set { _localeStringResources = value; }
        }
    }
}
