using Nop.Core.Configuration;
using Nop.Plugin.Payments.CyberSource.Domain;

namespace Nop.Plugin.Payments.CyberSource
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class CyberSourceSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets internal merchant id
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets key id
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// Gets or sets secret key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tokenization is enabled
        /// </summary>
        public bool TokenizationEnabled { get; set; }

        /// <summary>
        /// Gets or sets payment connection method
        /// </summary>
        public ConnectionMethodType PaymentConnectionMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payer authentication is enabled
        /// </summary>
        public bool PayerAuthenticationEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payer authentication is required
        /// </summary>
        public bool PayerAuthenticationRequired { get; set; }

        /// <summary>
        /// Gets or sets the transaction type
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CVV is required
        /// </summary>
        public bool CvvRequired { get; set; }

        /// <summary>
        /// Gets or sets the AVS action type
        /// </summary>
        public AvsActionType AvsActionType { get; set; }

        /// <summary>
        /// Gets or sets the CVN action type
        /// </summary>
        public CvnActionType CvnActionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether decision manager is enabled
        /// </summary>
        public bool DecisionManagerEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether conversion detail reporting is enabled
        /// </summary>
        public bool ConversionDetailReportingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes that conversion detail report download will be initiated frequently
        /// </summary>
        public int ConversionDetailReportingFrequency { get; set; }

        /// <summary>
        /// Gets or sets the number of days that refund can be initiated
        /// </summary>
        public int NumberOfDaysRefundAvailable { get; set; }

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out
        /// </summary>
        public int? RequestTimeout { get; set; }
    }
}