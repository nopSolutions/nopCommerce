using Nop.Core.Configuration;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class PayPalSmartPaymentButtonsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets client secret
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets the payment type
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets a webhook identifier
        /// </summary>
        public string WebhookId { get; set; }

        #region Advanced settings

        /// <summary>
        /// Gets or sets a value indicating whether to display buttons on the shopping cart page
        /// </summary>
        public bool DisplayButtonsOnShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display buttons on a product details page
        /// </summary>
        public bool DisplayButtonsOnProductDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display logo in header links
        /// </summary>
        public bool DisplayLogoInHeaderLinks { get; set; }

        /// <summary>
        /// Gets or sets the source code of logo in header links
        /// </summary>
        public string LogoInHeaderLinks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display logo in footer
        /// </summary>
        public bool DisplayLogoInFooter { get; set; }

        /// <summary>
        /// Gets or sets the source code of logo in footer
        /// </summary>
        public string LogoInFooter { get; set; }

        /// <summary>
        /// Gets or sets the disabled funding types (separated by comma)
        /// </summary>
        public string DisabledFunding { get; set; }

        /// <summary>
        /// Gets or sets the disabled card types (separated by comma)
        /// </summary>
        public string DisabledCards { get; set; }

        /// <summary>
        /// Gets or sets the layout option to determine the button layout when multiple buttons are available
        /// </summary>
        public string StyleLayout { get; set; }

        /// <summary>
        /// Gets or sets the color option
        /// </summary>
        public string StyleColor { get; set; }

        /// <summary>
        /// Gets or sets the shape option
        /// </summary>
        public string StyleShape { get; set; }

        /// <summary>
        /// Gets or sets the label option
        /// </summary>
        public string StyleLabel { get; set; }

        /// <summary>
        /// Gets or sets the tagline option
        /// </summary>
        public string StyleTagline { get; set; }

        #endregion
    }
}