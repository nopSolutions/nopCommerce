
using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Localization
{
    /// <summary>
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity
    {
        public Language()
        {
            Customers = new List<Customer>();
            LocaleStringResources = new List<LocaleStringResource>();
            LocalizedProperties = new List<LocalizedProperty>();
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets customers
        /// </summary>
        public virtual ICollection<Customer> Customers { get; set; }

        /// <summary>
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources { get; set; }

        /// <summary>
        /// Gets or sets localized properties
        /// </summary>
        public virtual ICollection<LocalizedProperty> LocalizedProperties { get; set; }
    }
}
