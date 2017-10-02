using System;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an information related to level two processing.
    /// </summary>
    public class LevelTwoData
    {
        /// <summary>
        /// Gets or sets a date of the transaction.
        /// </summary>
        [JsonProperty("orderDate")]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gets or sets a purchase order number associated with the transaction
        /// </summary>
        [JsonProperty("purchaseOrder")]
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// Gets or sets a lane number at which the transaction was completed in a retail environment.
        /// </summary>
        [JsonProperty("retailLaneNumber")]
        public int RetailLaneNumber { get; set; }

        /// <summary>
        /// Gets or sets a duty amount included in the transaction.
        /// </summary>
        [JsonProperty("dutyAmount")]
        public decimal DutyAmount { get; set; }

        /// <summary>
        /// Gets or sets a freight amount included in the transaction.
        /// </summary>
        [JsonProperty("freightAmount")]
        public decimal FreightAmount { get; set; }

        /// <summary>
        /// Gets or sets a tax amount included in the transaction.
        /// </summary>
        [JsonProperty("taxAmount")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets a tax status of the transaction.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("status")]
        public TaxStatusType? TaxStatus { get; set; }
    }
}