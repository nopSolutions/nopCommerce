using Nop.Core.Configuration;
using Nop.Plugin.Payments.Square.Domain;

namespace Nop.Plugin.Payments.Square
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class SquarePaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets OAuth2 application identifier
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 application secret
        /// </summary>
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox credentials
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets access token verification string
        /// </summary>
        public string AccessTokenVerificationString { get; set; }

        /// <summary>
        /// Gets or sets the transaction mode
        /// </summary>
        public TransactionMode TransactionMode { get; set; }

        /// <summary>
        /// Gets or sets the selected location identifier
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}