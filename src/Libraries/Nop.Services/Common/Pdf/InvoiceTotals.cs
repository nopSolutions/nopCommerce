using System.ComponentModel;

namespace Nop.Services.Common.Pdf
{
    /// <summary>
    /// Represents totals for an invoice
    /// </summary>
    public class InvoiceTotals
    {
        #region Ctor

        public InvoiceTotals()
        {
            GiftCards = new();
            TaxRates = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the order subtotal (include tax)
        /// </summary>
        [DisplayName("Pdf.SubTotal")]
        public string SubTotal { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (exclude tax)
        /// </summary>
        [DisplayName("Pdf.Discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the gift card text
        /// </summary>
        public List<string> GiftCards { get; set; }

        /// <summary>
        /// Gets or sets the shipping price
        /// </summary>
        [DisplayName("Pdf.Shipping")]
        public string Shipping { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee
        /// </summary>
        [DisplayName("Pdf.PaymentMethodAdditionalFee")]
        public string PaymentMethodAdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets the order tax
        /// </summary>
        [DisplayName("Pdf.Tax")]
        public string Tax { get; set; }

        /// <summary>
        /// Gets or sets a collection of the tax rates
        /// </summary>
        public List<string> TaxRates { get; set; }

        /// <summary>
        /// Gets or sets the reward points text
        /// </summary>
        public string RewardPoints { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public string OrderTotal { get; set; }

        #endregion
    }
}