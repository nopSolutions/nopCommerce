using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an additional data for remote orders. 
    /// </summary>
    public class MailOrTelephoneData
    {
        /// <summary>
        /// Gets or sets a type of transaction.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("type")]
        public MotoTransactionType? TransactionType { get; set; }

        /// <summary>
        /// Gets or sets a total number of installments. 
        /// Required if type is INSTALLMENT.
        /// </summary>
        [JsonProperty("totalNumberOfInstallments")]
        public string TotalNumberOfInstallments { get; set; }

        /// <summary>
        /// Gets or sets a current installment number. 
        /// Required if type is INSTALLMENT.
        /// </summary>
        [JsonProperty("currentInstallment")]
        public string CurrentInstallment { get; set; }
    }
}