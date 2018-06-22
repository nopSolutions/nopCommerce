using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a value added tax (VAT) information.
    /// </summary>
    public class VatTaxData
    {
        /// <summary>
        /// Gets or sets a Vat number used for the purchase.
        /// </summary>
        [JsonProperty("purchaserVatNumber")]
        public string PurchaserVatNumber { get; set; }

        /// <summary>
        /// Gets or sets a merchant Vat number.
        /// </summary>
        [JsonProperty("merchantVatNumber")]
        public string MerchantVatNumber { get; set; }

        /// <summary>
        /// Gets or sets a tax rate.
        /// </summary>
        [JsonProperty("taxRate")]
        public decimal TaxRate { get; set; }
    }
}