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
        private ICollection<LocalizedProperty> _localizedProperties;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public virtual string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public virtual string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets customers
        /// </summary>
        public virtual ICollection<Customer> Customers { get; set; }

        /// <summary>
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources
        {
            get { return _localeStringResources ?? (_localeStringResources = new List<LocaleStringResource>()); }
            protected set { _localeStringResources = value; }
        }

        /// <summary>
        /// Gets or sets localized properties
        /// </summary>
        public virtual ICollection<LocalizedProperty> LocalizedProperties
        {
            get { return _localizedProperties ?? (_localizedProperties = new List<LocalizedProperty>()); }
            protected set { _localizedProperties = value; }
        }
    }
}
