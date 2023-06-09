using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Stores
{
    /// <summary>
    /// Represents a store
    /// </summary>
    public partial class Store : BaseEntity, ILocalizedEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string DefaultMetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string DefaultMetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string DefaultTitle { get; set; }

        /// <summary>
        /// Home page title
        /// </summary>
        public string HomepageTitle { get; set; }

        /// <summary>
        /// Home page description
        /// </summary>
        public string HomepageDescription { get; set; }

        /// <summary>
        /// Gets or sets the store URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the comma separated list of possible HTTP_HOST values
        /// </summary>
        public string Hosts { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the default language for this store; 0 is set when we use the default language display order
        /// </summary>
        public int DefaultLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company address
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the store phone number
        /// </summary>
        public string CompanyPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the company VAT (used in Europe Union countries)
        /// </summary>
        public string CompanyVat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }
    }
}
