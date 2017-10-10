using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a settlement data
    /// </summary>
    public class ChargedTransactionData
    {
        /// <summary>
        /// Gets or sets a date and time when the transaction was settled.
        /// </summary>
        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets an amount of the transaction.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets an identifier for the batch that the transaction belongs to.
        /// </summary>
        [JsonProperty("batchId")]
        public int BatchId { get; set; }
    }
}