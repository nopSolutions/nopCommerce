using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a transaction data
    /// </summary>
    public class AuthorizedTransactionData
    {
        /// <summary>
        /// Gets or sets a date and time when the transaction took place.
        /// </summary>
        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets an amount of the transaction.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}