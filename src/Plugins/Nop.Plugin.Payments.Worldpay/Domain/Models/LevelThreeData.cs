using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an information related to level three processing.
    /// </summary>
    public class LevelThreeData
    {
        /// <summary>
        /// Gets or sets a details of the products that make up the total transaction.
        /// </summary>
        [JsonProperty("products")]
        public IList<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets a value added tax (VAT) information.
        /// </summary>
        [JsonProperty("vatData")]
        public VatTaxData VatTaxData { get; set; }

        /// <summary>
        /// Gets or sets a date of the transaction.
        /// </summary>
        [JsonProperty("orderDate")]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gets or sets a destination address of the purchased items.
        /// </summary>
        [JsonProperty("destinationAddress")]
        public Address DestinationAddress { get; set; }

        /// <summary>
        /// Gets or sets an origin address of the purchased items.
        /// </summary>
        [JsonProperty("originAddress")]
        public Address OriginAddress { get; set; }

        /// <summary>
        /// Gets or sets an amount of discount applied to the purchased items.
        /// </summary>
        [JsonProperty("discountAmount")]
        public decimal DiscountAmount { get; set; }
    }
}