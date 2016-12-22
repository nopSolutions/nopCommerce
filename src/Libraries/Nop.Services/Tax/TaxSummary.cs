
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Tax
{
    /// <summary>
    /// TaxRate entry to store amounts per vatrate
    /// all value are per rate and most of them calculated
    /// only VatRate, ItemAmount, ShippingAmount and PaymentFeeAmount can be set
    /// </summary>
    public partial class TaxRateEntry
    {
        public decimal VatRate { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal PaymentFeeAmount { get; set; } //can be negative when discounted
        public decimal SubTotalDiscAmount { get; internal set; }
        public decimal InvoiceDiscountAmount { get; internal set; }
        public decimal BaseAmount { get; internal set; }
        public decimal VatAmount { get; internal set; }
        public decimal AmountIncludingVAT { get; internal set; }
    }
    /// <summary>
    /// TaxSummary to store TaxRates and overall totals
    /// </summary>
    public partial class TaxSummary
    {
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="includingTax">Indicates if given amounts do include tax</param>
        public TaxSummary(bool includingTax)
        {
            TaxRates = new SortedDictionary<decimal, TaxRateEntry>();
            PricesIncludeTax = includingTax;
            PercentageInvDiscount = decimal.Zero;
            PercentageSubTotalDiscount = decimal.Zero;
            PercentagePaymentFeeDiscount = decimal.Zero;

            TotalSubTotalAmount = decimal.Zero;
            TotalShippingAmount = decimal.Zero;
            TotalPaymentFeeAmount = decimal.Zero;
            TotalAmount = decimal.Zero;
            TotalAmountVAT = decimal.Zero;
            TotalAmountIncludingVAT = decimal.Zero;
            TotalSubTotalDiscAmount = decimal.Zero;
            TotalInvDiscAmount = decimal.Zero;
        }
        #endregion

        public SortedDictionary<decimal, TaxRateEntry> TaxRates { get; set; }
        public bool PricesIncludeTax { get;  }
        public decimal PercentageInvDiscount { get; private set; }
        public decimal PercentageSubTotalDiscount { get; private set; }
        public decimal PercentagePaymentFeeDiscount { get; private set; }
        public decimal TotalSubTotalAmount { get; private set; }
        public decimal TotalShippingAmount { get; private set; }
        public decimal TotalPaymentFeeAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal TotalAmountVAT { get; private set; }
        public decimal TotalAmountIncludingVAT { get; private set; }
        public decimal TotalSubTotalDiscAmount { get; private set; }
        public decimal TotalInvDiscAmount { get; private set; }

        private bool HasChanged = false;

        #region utilities
        public SortedDictionary<decimal, decimal> GenerateOldTaxrateDict()
        {
            return new SortedDictionary<decimal, decimal>(TaxRates.ToDictionary(x => x.Key, x => x.Value.VatAmount));

        }
        #endregion

        #region methods
        /// <summary>
        /// Add amounts to taxrate
        /// </summary>
        /// <param name="vatRate">Vat %</param>
        /// <param name="itemAmount">Item amount</param>
        /// <param name="shippingAmount">Shipping amount</param>
        /// <param name="paymentfeeAmount"> Payment method fee amount</param>
        public void AddRate(decimal vatRate, decimal itemAmount = 0, decimal shippingAmount = 0, decimal paymentfeeAmount = 0)
        {
            if (!TaxRates.ContainsKey(vatRate))
                TaxRates.Add(vatRate, new TaxRateEntry()
                {
                    VatRate = vatRate,
                    SubtotalAmount = itemAmount,
                    ShippingAmount = shippingAmount,
                    PaymentFeeAmount = paymentfeeAmount
                });
            else
            {
                TaxRates[vatRate].SubtotalAmount += itemAmount;
                TaxRates[vatRate].ShippingAmount += shippingAmount;
                TaxRates[vatRate].PaymentFeeAmount += paymentfeeAmount;
            }
            HasChanged = true;
        }

        public void SetSubtotalDiscAmount(decimal totalSubTotalDiscAmount, decimal totalAmount = 0)
        {
            if (totalAmount == 0)
                totalAmount = TaxRates.Sum(x => x.Value.SubtotalAmount);
            if (totalSubTotalDiscAmount > totalAmount)
                totalSubTotalDiscAmount = totalAmount;

            if (totalAmount != decimal.Zero)
                PercentageSubTotalDiscount = totalSubTotalDiscAmount / totalAmount * 100;
            HasChanged = true;
        }
        /// <summary>
        /// set PaymentFee as discount (used for taxable fee as discount or surcharge)
        /// </summary>
        /// <param name="totalPaymentFeeDiscAmount">Fee Amount</param>
        /// <param name="totalAmount">Base amount to which apply fee</param>
        public void SetPaymentFeeDiscAmount(decimal totalPaymentFeeDiscAmount, decimal totalAmount)
        {
            PercentagePaymentFeeDiscount = totalPaymentFeeDiscAmount / totalAmount * 100;           
            HasChanged = true;
        }

        /// <summary>
        /// Set total invoice discount amount, it will be converted to an internal discount percentage
        /// </summary>
        /// <param name="totalInvDiscAmount">invoice discount amount</param>
        public void SetTotalInvDiscAmount(decimal totalInvDiscAmount, decimal totalAmount)
        {
            if (totalInvDiscAmount > totalAmount)
                totalInvDiscAmount = totalAmount;

            PercentageInvDiscount = totalInvDiscAmount / totalAmount * 100;
            HasChanged = true;
        }
        /// <summary>
        /// calculate amounts and VAT
        /// </summary>
        public void CalculateAmounts()
        {
            if (!HasChanged)
                return;

            //reset totals
            TotalShippingAmount = decimal.Zero;
            TotalPaymentFeeAmount = decimal.Zero;
            TotalAmount = decimal.Zero;
            TotalAmountVAT = decimal.Zero;
            TotalAmountIncludingVAT = decimal.Zero;
            TotalSubTotalAmount = decimal.Zero;
            TotalSubTotalDiscAmount = decimal.Zero;
            TotalInvDiscAmount = decimal.Zero;
            HasChanged = false;

            //init remainder
            decimal remainderSubTotalDisc = decimal.Zero;
            decimal remainderPaymentFeeDisc = decimal.Zero;
            decimal remainderInvDisc = decimal.Zero;

            //calc and sum up tax
            foreach (KeyValuePair<decimal, TaxRateEntry> kvp in TaxRates)
            {
                decimal vatpercentage = kvp.Key;
                TaxRateEntry taxrate = kvp.Value;

                //discounts
                if (PercentageSubTotalDiscount != 0)
                {
                    remainderSubTotalDisc += taxrate.SubtotalAmount * PercentageSubTotalDiscount / 100;
                    taxrate.SubTotalDiscAmount = RoundingHelper.RoundAmount(remainderSubTotalDisc);
                    remainderSubTotalDisc -= taxrate.SubTotalDiscAmount;
                }

                if (PercentagePaymentFeeDiscount != 0)
                {
                    remainderPaymentFeeDisc += (taxrate.SubtotalAmount + taxrate.ShippingAmount - taxrate.SubTotalDiscAmount) * PercentagePaymentFeeDiscount / 100;
                    taxrate.PaymentFeeAmount = RoundingHelper.RoundAmount(remainderPaymentFeeDisc);
                    remainderPaymentFeeDisc -= taxrate.PaymentFeeAmount;
                }

                //Invoice discount is in sequence to other discounts, i.e. applied to already discounted amounts
                if (PercentageInvDiscount != 0)
                {
                    remainderInvDisc += (taxrate.SubtotalAmount + taxrate.ShippingAmount + taxrate.PaymentFeeAmount - taxrate.SubTotalDiscAmount)
                                        * PercentageInvDiscount / 100;
                    taxrate.InvoiceDiscountAmount = RoundingHelper.RoundAmount(remainderInvDisc);
                    remainderInvDisc -= taxrate.InvoiceDiscountAmount;
                }

                //last remainder get's lost as it can't be considered anywhere else. This has no implication and only lowers or highers discount.

                //VAT: always round VAT first
                decimal rateamount = taxrate.SubtotalAmount + taxrate.ShippingAmount + taxrate.PaymentFeeAmount - taxrate.SubTotalDiscAmount - taxrate.InvoiceDiscountAmount;
                if (PricesIncludeTax)
                {
                    taxrate.AmountIncludingVAT = rateamount;
                    taxrate.VatAmount = RoundingHelper.RoundAmount(taxrate.AmountIncludingVAT  / (100 + vatpercentage) * vatpercentage); // this is  (1+p/100) * p/100
                    taxrate.BaseAmount = taxrate.AmountIncludingVAT - taxrate.VatAmount;
                }
                else
                {
                    taxrate.BaseAmount = rateamount;
                    taxrate.VatAmount = RoundingHelper.RoundAmount(taxrate.BaseAmount * vatpercentage / 100);
                    taxrate.AmountIncludingVAT = taxrate.BaseAmount + taxrate.VatAmount;
                }


                //totals
                TotalSubTotalAmount += taxrate.SubtotalAmount;
                TotalShippingAmount += taxrate.ShippingAmount;
                TotalPaymentFeeAmount += taxrate.PaymentFeeAmount;

                if (PercentageSubTotalDiscount != 0)
                    TotalSubTotalDiscAmount += taxrate.SubTotalDiscAmount;
                if (PercentageInvDiscount != 0)
                    TotalInvDiscAmount += taxrate.InvoiceDiscountAmount;

                TotalAmount += taxrate.BaseAmount;
                TotalAmountVAT += taxrate.VatAmount;
                TotalAmountIncludingVAT += taxrate.AmountIncludingVAT;

            }
        }
        #endregion
    }
}
