using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent
    /// </summary>
    public partial class GdprConsent : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the message displayed to customers
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or setsa value indicating whether the consent is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the message displayed to customers when he/she doesn't agree to this consent
        /// </summary>
        public string RequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this consent is displayed during registrations
        /// </summary>
        public bool DisplayDuringRegistration { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this consent is displayed on my account page (customer info)
        /// </summary>
        public bool DisplayOnCustomerInfoPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}