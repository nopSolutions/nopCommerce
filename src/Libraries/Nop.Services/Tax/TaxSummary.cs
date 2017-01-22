
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nop.Services.Tax
{
    /// <summary>
    /// TaxRate entry to store amounts per vatrate
    /// all values are per rate and most of them calculated
    /// only VatRate, ItemAmount, ShippingAmount and PaymentFeeAmount can be set
    /// </summary>
    public partial class TaxRateEntry
    {
        public decimal VatRate { get; set; }
        public decimal VatRateWeight { get; internal set; } //weight of VatRate used for product sets [it's (sum of associated products price per VatRate) / (total price of assoc. prod.) ]
        public decimal SubtotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal PaymentFeeAmount { get; set; } //can be negative when discounted
        public decimal Amount { get; internal set; } //sum of above amounts
        public decimal SubTotalDiscAmount { get; internal set; }
        public decimal InvoiceDiscountAmount { get; internal set; }
        public decimal DiscountAmount { get; internal set; } //sum of above discounts
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
        public string GenerateTaxRateString()
        {
            return this.TaxRates.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2}:{3}:{4}:{5}:{6};   ", current,
                    next.Key.ToString(CultureInfo.InvariantCulture),
                    next.Value.Amount.ToString(CultureInfo.InvariantCulture),
                    next.Value.DiscountAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.BaseAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.VatAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.AmountIncludingVAT.ToString(CultureInfo.InvariantCulture)
                 )
            );
        }
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
        /// <summary>
        ///
        /// </summary>
        /// <param name="attribAmount">Total Amount of Attributes</param>
        /// <param name="attributeTaxWeight">dictionary of taxRate weights on that amount</param>
        public void AddAttributeRate(decimal attribAmount, SortedDictionary<decimal, decimal> attributeTaxWeight)
        {
            var dummyResult = ParseOrAddAttributeRate(attribAmount, attributeTaxWeight, doAdd: true);
        }

        public SortedDictionary<decimal, decimal> ParseAttributeRate(decimal attribAmount, SortedDictionary<decimal, decimal> attributeTaxWeight)
        {
            return ParseOrAddAttributeRate(attribAmount, attributeTaxWeight, doAdd: false);
        }
        public SortedDictionary<decimal, decimal> ParseOrAddAttributeRate(decimal attribAmount, SortedDictionary<decimal, decimal> attributeTaxWeight, bool doAdd = true)
        {
            var result = new SortedDictionary<decimal, decimal>();
            int i = 0;
            int c = attributeTaxWeight.Count();
            decimal reminder = attribAmount;
            foreach (KeyValuePair<decimal, decimal> kvp in attributeTaxWeight)
            {
                i += 1;
                decimal vatpercentage = kvp.Key;
                decimal rateWeight = kvp.Value;
                var attribAmountWeighted = RoundingHelper.RoundAmount(attribAmount * rateWeight);
                if (i < c)
                {
                    if (doAdd)
                        AddRate(vatpercentage, attribAmountWeighted);
                    else
                        result.Add(kvp.Key, attribAmountWeighted);
                    reminder -= attribAmountWeighted;
                }
                else
                {
                    if (doAdd)
                        AddRate(vatpercentage, reminder);
                    else
                        result.Add(kvp.Key, reminder);
                }
            }

            return result;
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

                taxrate.Amount = taxrate.SubtotalAmount + taxrate.ShippingAmount + taxrate.PaymentFeeAmount;
                taxrate.DiscountAmount = taxrate.SubTotalDiscAmount + taxrate.InvoiceDiscountAmount;

                //VAT: always round VAT first
                decimal rateamount = taxrate.Amount - taxrate.DiscountAmount;
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
            int i = 0;
            int c = TaxRates.Count();
            decimal totWeight = decimal.Zero;
            foreach (KeyValuePair<decimal, TaxRateEntry> kvp in this.TaxRates)
            {
                i++;
                decimal vatpercentage = kvp.Key;
                TaxRateEntry taxrate = kvp.Value;
                if (i < c)
                {
                    taxrate.VatRateWeight = (this.PricesIncludeTax ? (taxrate.AmountIncludingVAT / this.TotalAmountIncludingVAT) : (taxrate.BaseAmount / this.TotalAmount));
                    totWeight += taxrate.VatRateWeight;
                }
                else
                {
                    taxrate.VatRateWeight = decimal.One - totWeight; //assure sum of VatRateWeight = 1
                }
            }
        }
        #endregion
    }
}
