using Nop.Core.Configuration;
using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class PayPalCommerceSettings : ISettings
    {
        /// <summary>
        /// Gets or sets merchant email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the URL to sign up 
        /// </summary>
        public string SignUpUrl { get; set; }

        /// <summary>
        /// Gets or sets internal merchant id
        /// </summary>
        public string MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to manually set the credentials.
        /// For example, there is already an app created, or if the merchant wants to use the sandbox mode.
        /// </summary>
        public bool SetCredentialsManually { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets client secret
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the payment type
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets a webhook URL
        /// </summary>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out
        /// </summary>
        public int? RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets a minimum value that can be considered a real discount, and not a rounding error in calculations, 
        /// for example, if you are sure that the store will not have discounts less than a $1, set here 1.00
        /// </summary>
        public decimal MinDiscountAmount { get; set; }

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
        /// Gets or sets a value indicating whether to display Pay Later messages
        /// </summary>
        public bool DisplayPayLaterMessages { get; set; }

        /// <summary>
        /// Gets or sets the disabled funding sources for the transaction (separated by comma)
        /// By default, funding source eligibility is determined based on a variety of factors
        /// </summary>
        public string DisabledFunding { get; set; }

        /// <summary>
        /// Gets or sets the enabled funding sources for the transaction (separated by comma)
        /// Enable funding can be used to ensure a funding source is rendered, if eligible
        /// </summary>
        public string EnabledFunding { get; set; }

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