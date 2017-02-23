
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nop.Services.Tax
{
    #region taxrateEntry
    /// <summary>
    /// TaxRate entry to store amounts per taxrate
    /// all values are per rate and most of them calculated
    /// only TaxRate, ItemAmount, ShippingAmount and PaymentFeeAmount can be set
    /// </summary>
    public partial class TaxRateEntry
    {
        public decimal TaxRate { get; set; }
        public decimal TaxRateWeight { get; internal set; } //weight of TaxRate used for product sets [it's (sum of associated products price per TaxRate) / (total price of assoc. prod.) ]
        public decimal SubtotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal PaymentFeeAmount { get; set; } //when set as fee
        public decimal EntryAmount //sum of above amounts
        {
            get { return SubtotalAmount + ShippingAmount + PaymentFeeAmount; }
        }
        public decimal SubTotalDiscountAmount { get; internal set; }
        public decimal InvoiceDiscountAmount { get; internal set; }
        public decimal PaymentFeeDiscountAmount { get; set; } //when set as discount or surcharge
        public decimal RewardPointsDiscountAmount { get; set; } //earned reward points
        public decimal DiscountAmount//sum of above discount amounts
        {
            get { return  SubTotalDiscountAmount + InvoiceDiscountAmount + PaymentFeeDiscountAmount + RewardPointsDiscountAmount; }
        }
        public decimal BaseAmount { get; internal set; } //base amount excl. tax
        public decimal TaxAmount { get; internal set; } //tax
        public decimal AmountIncludingTax { get; internal set; } //base + tax
    }

    #endregion

    /// <summary>
    /// TaxSummary to store TaxRates and overall totals
    /// </summary>
    public partial class TaxSummary
    {
        //private
        decimal _totalSubTotalAmount;
        decimal? _totalShippingAmountTaxable;
        decimal _totalAmount;
        decimal _totalAmountTax;
        decimal _totalAmountIncludingTax;
        decimal _totalPaymentFeeDiscAmount;
        decimal _totalPaymentFeeAmount;
        decimal _totalRewardPointsAmountTaxable;
        decimal _totalSubTotalDiscAmount;
        decimal _totalInvDiscAmount;

        //externally set amounts
        decimal? _totalShippingAmountNonTaxable;
        decimal? _totalRewardPointsAmountNonTaxable;
        decimal? _totalPaymentFeeAmountNonTaxable;
        decimal? _totalRewardPointsAmountPurchased;
        decimal? _totalGiftcardsAmount;

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pricesIncludingTax">Indicates if given amounts do include tax</param>
        public TaxSummary(bool pricesIncludingTax)
        {
            TaxRates = new SortedDictionary<decimal, TaxRateEntry>();
            PricesIncludeTax = pricesIncludingTax;
        }
        #endregion

        public SortedDictionary<decimal, TaxRateEntry> TaxRates { get; private set; }
        /// <summary>
        /// Indicates if supplied prices and amounts do include tax
        /// </summary>
        public bool PricesIncludeTax { get;  }
        public decimal PercentageInvDiscount { get; private set; } = decimal.Zero; //set via sub
        public decimal PercentageSubTotalDiscount { get; private set; } = decimal.Zero; //set via sub
        public decimal PercentageRewardPointsDiscount { get; private set; } = decimal.Zero; //earned points, set via sub
        public decimal PercentagePaymentFeeOrDiscount { get; private set; } = decimal.Zero; //set via sub
        /// <summary>
        /// Total subtotal of taxrates
        /// </summary>
        public decimal TotalSubTotalAmount { get { if (HasChanges) CalculateAmounts(); return _totalSubTotalAmount; } }
        /// <summary>
        /// Total shipping of taxrates
        /// </summary>
        public decimal? TotalShippingAmountTaxable { get { if (HasChanges) CalculateAmounts(); return _totalShippingAmountTaxable; } }
        /// <summary>
        /// Total shipping not taxable
        /// </summary>
        public decimal? TotalShippingAmountNonTaxable
        {
            get { return _totalShippingAmountNonTaxable; }
            set
            {
                _totalShippingAmountNonTaxable = value;
                if (value.HasValue)
                {
                    _totalShippingAmountNonTaxable = RoundingHelper.RoundAmount(value.Value);
                    HasShipping = true;
                }
            }
        }
        /// <summary>
        /// Total shipping amount (taxable + nontaxable)
        /// </summary>
        public decimal? TotalShippingAmount { get { return GetSum(SumType.TotalShippingAmount); } } //sum of shipping amounts
        /// <summary>
        /// Total reward points amount (taxable) = earned points
        /// </summary>
        public decimal TotalRewardPointsAmountTaxable { get { if (HasChanges) CalculateAmounts(); return _totalRewardPointsAmountTaxable; } } //set via percentage
        /// <summary>
        /// Total reward points amount (nontaxable) = earned points set as nontaxable
        /// </summary>
        public decimal? TotalRewardPointsAmountNonTaxable
        {
            get { return _totalRewardPointsAmountNonTaxable; }
            set { _totalRewardPointsAmountNonTaxable = value.HasValue ? RoundingHelper.RoundAmount(value.Value) : value; }
        }
        /// <summary>
        /// Total payment fee amount (taxable)
        /// </summary>
        public decimal TotalPaymentFeeAmount { get { if (HasChanges) CalculateAmounts(); return _totalPaymentFeeAmount; } } //set via percentage
        /// <summary>
        /// Total payment fee amount (nontaxable)
        /// </summary>
        public decimal? TotalPaymentFeeAmountNonTaxable
        {
            get { return _totalPaymentFeeAmountNonTaxable; }
            set { _totalPaymentFeeAmountNonTaxable = value.HasValue ? RoundingHelper.RoundAmount(value.Value) : value; }
        }
        /// <summary>
        /// Total payment fee discount amount (taxable)
        /// </summary>
        public decimal TotalPaymentFeeDiscAmount { get { if (HasChanges) CalculateAmounts(); return _totalPaymentFeeDiscAmount; } } //set via percentage
        /// <summary>
        /// Total payment fee + discount amount (taxable)
        /// </summary>
        public decimal TotalPaymentFeeAmountTaxable { get { return GetSum(SumType.TotalPaymentFeeAmountTaxable) ?? decimal.Zero; } } //sum of payment fees
        /// <summary>
        /// Total subtotal discount of taxrates
        /// </summary>
        public decimal TotalSubTotalDiscAmount { get { if (HasChanges) CalculateAmounts(); return _totalSubTotalDiscAmount; } } //set via percentage
        /// <summary>
        /// Total invoice discount of taxrates
        /// </summary>
        public decimal TotalInvDiscAmount { get { if (HasChanges) CalculateAmounts(); return _totalInvDiscAmount; } } //set via percentage
        /// <summary>
        /// Total base amount (excl. tax=
        /// </summary>
        public decimal TotalAmount { get { if (HasChanges) CalculateAmounts(); return _totalAmount; } }
        /// <summary>
        /// Total tax amount
        /// </summary>
        public decimal TotalAmountTax { get { if (HasChanges) CalculateAmounts(); return _totalAmountTax; } }
        /// <summary>
        /// Total amount incl. tax (base + tax)
        /// </summary>
        public decimal TotalAmountIncludingTax { get { if (HasChanges) CalculateAmounts(); return _totalAmountIncludingTax; } }
        /// <summary>
        /// Total purchased reward points amount (nontaxable)
        /// </summary>
        public decimal? TotalRewardPointsAmountPurchased
        {
            get { return _totalRewardPointsAmountPurchased; }
            set { _totalRewardPointsAmountPurchased = value.HasValue ? RoundingHelper.RoundAmount(value.Value) : value; }
        }
        /// <summary>
        /// Total gift cards amount (nontaxable)
        /// </summary>
        public decimal? TotalGiftcardsAmount
        {
            get { return _totalGiftcardsAmount; }
            set { _totalGiftcardsAmount = value.HasValue ? RoundingHelper.RoundAmount(value.Value) : value; }
        }
        /// <summary>
        /// Total to pay (Amount incl. vat + non tax payment fee + non tax shipp - rewardpoints - gift cards)
        /// </summary>
        public decimal TotalPaymentAmount { get { return GetSum(SumType.TotalPaymentAmount) ?? decimal.Zero; } }
        /// <summary>
        /// Total base amount for payment fee calculation (total amount or total incl.)
        /// </summary>
        public decimal TotalBaseAmountForPaymentFeeCalculation { get { return GetSum(SumType.TotalBaseAmountForPaymentFeeCalculation) ?? decimal.Zero; } }
        /// <summary>
        /// Total order amount (base + fee (nontax) + shipp (nontax) - reward (nontax)
        /// </summary>
        public decimal TotalOrderAmountExcl { get { return GetSum(SumType.TotalOrderAmount) ?? decimal.Zero; } }
        /// <summary>
        /// Total order amount (base + tax + fee (nontax) + shipp (nontax) - reward (nontax)
        /// </summary>
        public decimal TotalOrderAmountIncl { get { return GetSum(SumType.TotalOrderAmountIncl) ?? decimal.Zero; } }


        private bool HasChanges;
        private bool HasPaymentFeeTax
        {
            get
            {
                foreach (var tr in TaxRates)
                {
                    if (tr.Value.PaymentFeeAmount != decimal.Zero)
                        return true;
                }
                return false;
            }
        }
        private bool HasShipping { get; set; } = false;


        #region sums
        private enum SumType { TotalShippingAmount, TotalPaymentFeeAmountTaxable, TotalPaymentAmount, TotalBaseAmountForPaymentFeeCalculation, TotalOrderAmount, TotalOrderAmountIncl };
        private decimal? GetSum(SumType type)
        {

            if (HasChanges)
                CalculateAmounts();

            switch (type)
            {
                case SumType.TotalShippingAmount:
                    if (TotalShippingAmountTaxable.HasValue && !TotalShippingAmountNonTaxable.HasValue)
                        return TotalShippingAmountTaxable;
                    if (!TotalShippingAmountTaxable.HasValue && TotalShippingAmountNonTaxable.HasValue)
                        return TotalShippingAmountNonTaxable;

                    return TotalShippingAmountTaxable + TotalShippingAmountNonTaxable;

                case SumType.TotalPaymentFeeAmountTaxable:
                    return TotalPaymentFeeAmount - TotalPaymentFeeDiscAmount;

                case SumType.TotalPaymentAmount:
                    decimal amountPay = TotalAmountIncludingTax
                            + (TotalShippingAmountNonTaxable ?? decimal.Zero)
                            + (TotalPaymentFeeAmountNonTaxable ?? decimal.Zero)
                            - (TotalRewardPointsAmountNonTaxable ?? decimal.Zero)
                            - (TotalGiftcardsAmount ?? decimal.Zero)
                            - (TotalRewardPointsAmountPurchased ?? decimal.Zero);

                    if (amountPay < decimal.Zero)
                        amountPay = decimal.Zero;

                    return amountPay;

                case SumType.TotalBaseAmountForPaymentFeeCalculation:
                    return PricesIncludeTax ? TotalOrderAmountIncl : TotalOrderAmountExcl;

                case SumType.TotalOrderAmount:
                    decimal amountOrder = TotalAmount
                            + (TotalShippingAmountNonTaxable ?? decimal.Zero)
                            + (TotalPaymentFeeAmountNonTaxable ?? decimal.Zero)
                            - (TotalRewardPointsAmountNonTaxable ?? decimal.Zero);

                    if (amountOrder < decimal.Zero)
                        amountOrder = decimal.Zero;
                    return amountOrder;

                case SumType.TotalOrderAmountIncl:
                    decimal amountOrderIncl = TotalAmountIncludingTax
                            + (TotalShippingAmountNonTaxable ?? decimal.Zero)
                            + (TotalPaymentFeeAmountNonTaxable ?? decimal.Zero)
                            - (TotalRewardPointsAmountNonTaxable ?? decimal.Zero);

                    if (amountOrderIncl < decimal.Zero)
                        amountOrderIncl = decimal.Zero;
                    return amountOrderIncl;

                default:
                    return decimal.Zero;
            }
        }

        #endregion

        #region utilities
        public string GenerateTaxRateString()
        {
            return this.TaxRates.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2}:{3}:{4}:{5}:{6};   ", current,
                    next.Value.TaxRate.ToString(CultureInfo.InvariantCulture),
                    next.Value.EntryAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.DiscountAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.BaseAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.TaxAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.AmountIncludingTax.ToString(CultureInfo.InvariantCulture)
                 )
            );
        }
        public SortedDictionary<decimal, decimal> GenerateOldTaxrateDict()
        {
            return new SortedDictionary<decimal, decimal>(TaxRates.ToDictionary(x => x.Value.TaxRate, x => x.Value.TaxAmount));

        }

        protected decimal CalcDiscountPercentage(ref decimal discAmount, decimal baseAmount)
        {
            decimal PercentageDiscount;

            if (baseAmount == decimal.Zero)
            {
                PercentageDiscount = decimal.Zero;
                discAmount = decimal.Zero;
            }
            else
            {
                if (discAmount > baseAmount)
                    discAmount = baseAmount;

                PercentageDiscount = discAmount / baseAmount * 100;
            }
            return PercentageDiscount;
        }

        #endregion

        #region methods

        #region rates
        /// <summary>
        /// Add amounts to taxrate
        /// </summary>
        /// <param name="taxRate">Vat %</param>
        /// <param name="itemAmount">Item amount</param>
        /// <param name="shippingAmount">Shipping amount</param>
        /// <param name="paymentFeeAmount"> Payment method fee amount</param>
        public void AddRate(decimal taxRate, decimal itemAmount = decimal.Zero, decimal? shippingAmount = null, decimal paymentFeeAmount = decimal.Zero)
        {
            if (shippingAmount.HasValue)
                HasShipping = true;

            if (!TaxRates.ContainsKey(taxRate))
                TaxRates.Add(taxRate, new TaxRateEntry()
                {
                    TaxRate = taxRate,
                    SubtotalAmount = itemAmount,
                    ShippingAmount = shippingAmount ?? decimal.Zero,
                    PaymentFeeAmount = paymentFeeAmount
                });
            else
            {
                TaxRates[taxRate].SubtotalAmount += itemAmount;
                TaxRates[taxRate].ShippingAmount += shippingAmount ?? decimal.Zero;
                TaxRates[taxRate].PaymentFeeAmount += paymentFeeAmount;
            }

            HasChanges = true;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="amount">Total Amount of Attributes</param>
        /// <param name="attributeTaxWeight">dictionary of taxRate weights on that amount</param>
        public void AddAttributeRate(decimal amount, SortedDictionary<decimal, decimal> attributeTaxWeight)
        {
            var dummyResult = ApplyOrAddAttributeRate(amount, attributeTaxWeight, doAdd: true);
        }
        /// <summary>
        /// Parses taxWeights and calculates respective amounts
        /// </summary>
        /// <param name="amount">amount to weight</param>
        /// <param name="attributeTaxWeight">tax weights</param>
        /// <returns>SortedDictionary of weighted amounts</returns>
        public SortedDictionary<decimal, decimal> ApplyAttributeRate(decimal amount, SortedDictionary<decimal, decimal> attributeTaxWeight)
        {
            return ApplyOrAddAttributeRate(amount, attributeTaxWeight, doAdd: false);
        }
        #endregion

        protected SortedDictionary<decimal, decimal> ApplyOrAddAttributeRate(decimal amount, SortedDictionary<decimal, decimal> attributeTaxWeight, bool doAdd = true)
        {
            var result = new SortedDictionary<decimal, decimal>();
            int i = 0;
            var c = attributeTaxWeight.Count();
            decimal reminder = amount;
            foreach (KeyValuePair<decimal, decimal> kvp in attributeTaxWeight)
            {
                i += 1;
                decimal vatpercentage = kvp.Key;
                decimal rateWeight = kvp.Value;
                var amountWeighted = RoundingHelper.RoundAmount(amount * rateWeight);
                if (i < c)
                {
                    if (doAdd)
                        AddRate(vatpercentage, amountWeighted);
                    else
                        result.Add(vatpercentage, amountWeighted);
                    reminder -= amountWeighted;
                }
                else
                {
                    reminder = RoundingHelper.RoundAmount(reminder);
                    if (doAdd)
                        AddRate(vatpercentage, reminder);
                    else
                        result.Add(vatpercentage, reminder);
                }
            }

            return result;
        }

        #region discount percentages
        /// <summary>
        /// Set subtotal discount amount. Converted internally to percentage
        /// </summary>
        /// <param name="totalSubTotalDiscAmount">Subtotal discount amount</param>
        /// <param name="baseAmount">Base amount to which apply discount</param>
        public void SetSubtotalDiscAmount(decimal totalSubTotalDiscAmount, decimal baseAmount = 0)
        {
            totalSubTotalDiscAmount = RoundingHelper.RoundAmount(totalSubTotalDiscAmount);
            if (baseAmount == decimal.Zero)
                baseAmount = TaxRates.Sum(x => x.Value.SubtotalAmount);

            totalSubTotalDiscAmount = Math.Min(totalSubTotalDiscAmount, baseAmount);
            PercentageSubTotalDiscount = CalcDiscountPercentage(ref totalSubTotalDiscAmount, baseAmount);
            _totalSubTotalDiscAmount = totalSubTotalDiscAmount;

            HasChanges = true;
        }

        /// <summary>
        /// Set redeemed reward points amount. Converted internally to a percentage
        /// </summary>
        /// <param name="totalRewardPointsAmount">Redeemed reward points amount</param>
        /// <param name="baseAmount">Base amount to which apply discount</param>
        public void SetRewardPointsDiscAmount(decimal totalRewardPointsAmount, decimal baseAmount = 0)
        {
            totalRewardPointsAmount = RoundingHelper.RoundAmount(totalRewardPointsAmount);
            totalRewardPointsAmount = Math.Min(totalRewardPointsAmount, baseAmount);

            PercentageRewardPointsDiscount = CalcDiscountPercentage(ref totalRewardPointsAmount, baseAmount);
            _totalRewardPointsAmountTaxable = totalRewardPointsAmount;

            HasChanges = true;
        }

        /// <summary>
        /// Set PaymentFee as discount (negative amount) or surcharge (positive amount) (used for taxable fee as discount or surcharge)
        /// </summary>
        /// <param name="totalPaymentFeeOrDiscAmount">Fee Amount</param>
        /// <param name="baseAmount">Base amount to which apply discount fee</param>
        public void SetPaymentFeeOrDiscAmount(decimal totalPaymentFeeOrDiscAmount, decimal baseAmount)
        {
            totalPaymentFeeOrDiscAmount = RoundingHelper.RoundAmount(totalPaymentFeeOrDiscAmount);
            totalPaymentFeeOrDiscAmount = Math.Min(totalPaymentFeeOrDiscAmount, baseAmount);

            //totalPaymentFeeDiscAmount is passed as negative number
            var sign = Math.Sign(totalPaymentFeeOrDiscAmount);
            totalPaymentFeeOrDiscAmount = Math.Abs(totalPaymentFeeOrDiscAmount);
            _totalPaymentFeeAmount = decimal.Zero;
            _totalPaymentFeeDiscAmount = decimal.Zero;

            PercentagePaymentFeeOrDiscount = CalcDiscountPercentage(ref totalPaymentFeeOrDiscAmount, baseAmount);
            if (sign == -1)
                _totalPaymentFeeDiscAmount = totalPaymentFeeOrDiscAmount;
            else
                _totalPaymentFeeAmount = totalPaymentFeeOrDiscAmount;

            HasChanges = true;
        }

        /// <summary>
        /// Set total invoice discount amount, it will be converted to an internal discount percentage
        /// </summary>
        /// <param name="totalInvDiscAmount">invoice discount amount</param>
        /// <param name="baseAmount">Base amount to which apply discount</param>
        public void SetTotalInvDiscAmount(decimal totalInvDiscAmount, decimal baseAmount)
        {
            totalInvDiscAmount = RoundingHelper.RoundAmount(totalInvDiscAmount);
            totalInvDiscAmount = Math.Min(totalInvDiscAmount, baseAmount);

            PercentageInvDiscount = CalcDiscountPercentage(ref totalInvDiscAmount, baseAmount);
            _totalInvDiscAmount = totalInvDiscAmount;

            HasChanges = true;
        }
        #endregion

        /// <summary>
        /// Calculate amounts and tax
        /// </summary>
        private void CalculateAmounts()
        {
            if (!HasChanges)
                return;

            //reset totals
            _totalSubTotalAmount = decimal.Zero;
            if (HasShipping)
                _totalShippingAmountTaxable = decimal.Zero;
            else
                _totalShippingAmountTaxable = null;
            _totalAmount = decimal.Zero;
            _totalAmountTax = decimal.Zero;
            _totalAmountIncludingTax = decimal.Zero;

            HasChanges = false;

            //payment fee with tax
            if (HasPaymentFeeTax)
            {
                _totalPaymentFeeAmount = decimal.Zero;
                _totalPaymentFeeDiscAmount = decimal.Zero;
                PercentagePaymentFeeOrDiscount = decimal.Zero;
            }

            //init remainder
            decimal remainderSubTotalDisc = decimal.Zero; decimal sumSubTotalDisc = decimal.Zero;
            decimal remainderInvDisc = decimal.Zero; decimal sumInvDisc = decimal.Zero;
            decimal remainderRewardPointsDisc = decimal.Zero; decimal sumRewardPointsDisc = decimal.Zero;
            decimal remainderPaymentFeeOrDisc = decimal.Zero; decimal sumPaymentFeeOrDisc = decimal.Zero;

            var n = TaxRates.Count();
            var i = 0;

            //calc and sum up tax
            foreach (KeyValuePair<decimal, TaxRateEntry> kvp in TaxRates)
            {
                decimal vatpercentage = kvp.Key;
                TaxRateEntry taxrate = kvp.Value;
                i += 1;

                //subtotal discount
                if (PercentageSubTotalDiscount != 0)
                {
                    if (i == n)
                    {
                        var diff = TotalSubTotalDiscAmount - sumSubTotalDisc;
                        taxrate.SubTotalDiscountAmount = diff;
                    }
                    else
                    {
                        var discountbase = taxrate.SubtotalAmount;
                        remainderSubTotalDisc += discountbase * PercentageSubTotalDiscount / 100;
                        taxrate.SubTotalDiscountAmount = RoundingHelper.RoundAmount(remainderSubTotalDisc);
                        remainderSubTotalDisc -= taxrate.SubTotalDiscountAmount;
                        sumSubTotalDisc += taxrate.SubTotalDiscountAmount;
                    }
                }

                //Invoice discount is in sequence to other discounts, i.e. applied to already discounted amounts
                if (PercentageInvDiscount != 0)
                {
                    if (i == n)
                    {
                        var diff = TotalInvDiscAmount - sumInvDisc;
                        taxrate.InvoiceDiscountAmount = diff;
                    }
                    else
                    {
                        var discountbase = taxrate.SubtotalAmount - taxrate.SubTotalDiscountAmount + taxrate.ShippingAmount;
                        remainderInvDisc += discountbase * PercentageInvDiscount / 100;
                        taxrate.InvoiceDiscountAmount = RoundingHelper.RoundAmount(remainderInvDisc);
                        remainderInvDisc -= taxrate.InvoiceDiscountAmount;
                        sumInvDisc += taxrate.InvoiceDiscountAmount;
                    }
                }

                //Reward points are in sequence to other discounts, i.e. applied to already discounted amounts
                if (PercentageRewardPointsDiscount != 0)
                {
                    if (i == n)
                    {
                        var diff =TotalRewardPointsAmountTaxable - sumRewardPointsDisc;
                        taxrate.RewardPointsDiscountAmount = diff;
                    }
                    else
                    {
                        var discountbase = taxrate.SubtotalAmount - taxrate.SubTotalDiscountAmount + taxrate.ShippingAmount - taxrate.InvoiceDiscountAmount;
                        remainderRewardPointsDisc += discountbase * PercentageRewardPointsDiscount / 100;
                        taxrate.RewardPointsDiscountAmount = RoundingHelper.RoundAmount(remainderRewardPointsDisc);
                        remainderRewardPointsDisc -= taxrate.RewardPointsDiscountAmount;
                        sumRewardPointsDisc += taxrate.RewardPointsDiscountAmount;
                    }
                }

                //percentage payment fee discount.
                if (PercentagePaymentFeeOrDiscount != 0)
                {
                    if (i == n)
                    {
                        var diff = TotalPaymentFeeAmount + TotalPaymentFeeDiscAmount - sumPaymentFeeOrDisc; //TotalPaymentFeeAmount and TotalPaymentFeeDiscAmount are mutual zero
                        taxrate.PaymentFeeAmount = TotalPaymentFeeAmount != decimal.Zero ? diff : decimal.Zero;
                        taxrate.PaymentFeeDiscountAmount = TotalPaymentFeeDiscAmount != decimal.Zero ? diff : decimal.Zero;

                    }
                    else
                    {
                        var discountbase = taxrate.SubtotalAmount - taxrate.SubTotalDiscountAmount + taxrate.ShippingAmount - taxrate.InvoiceDiscountAmount - taxrate.RewardPointsDiscountAmount;
                        remainderPaymentFeeOrDisc += discountbase * PercentagePaymentFeeOrDiscount / 100;
                        taxrate.PaymentFeeAmount = decimal.Zero; taxrate.PaymentFeeDiscountAmount = decimal.Zero;
                        if (TotalPaymentFeeAmount != decimal.Zero)
                            taxrate.PaymentFeeAmount = RoundingHelper.RoundAmount(remainderPaymentFeeOrDisc);
                        else
                            taxrate.PaymentFeeDiscountAmount = RoundingHelper.RoundAmount(remainderPaymentFeeOrDisc);
                        remainderPaymentFeeOrDisc -= taxrate.PaymentFeeAmount + taxrate.PaymentFeeDiscountAmount;
                        sumPaymentFeeOrDisc += taxrate.PaymentFeeAmount + taxrate.PaymentFeeDiscountAmount;
                    }
                }

                //TAX: always round tax first
                decimal rateamount = taxrate.EntryAmount - taxrate.DiscountAmount;
                if (PricesIncludeTax)
                {
                    taxrate.AmountIncludingTax = rateamount;
                    taxrate.TaxAmount = RoundingHelper.RoundTax(taxrate.AmountIncludingTax  / (100 + vatpercentage) * vatpercentage); // this is  (1+p/100) * p/100
                    taxrate.BaseAmount = taxrate.AmountIncludingTax - taxrate.TaxAmount;
                }
                else
                {
                    taxrate.BaseAmount = rateamount;
                    taxrate.TaxAmount = RoundingHelper.RoundTax(taxrate.BaseAmount * vatpercentage / 100);
                    taxrate.AmountIncludingTax = taxrate.BaseAmount + taxrate.TaxAmount;
                }


                //taxrate totals
                _totalSubTotalAmount += taxrate.SubtotalAmount;
                if (HasShipping) //to maintain null
                    _totalShippingAmountTaxable += taxrate.ShippingAmount;

                if (PercentagePaymentFeeOrDiscount == 0) //sum only when not set as discount or surcharge
                    _totalPaymentFeeAmount += taxrate.PaymentFeeAmount;

                _totalAmount += taxrate.BaseAmount;
                _totalAmountTax += taxrate.TaxAmount;
                _totalAmountIncludingTax += taxrate.AmountIncludingTax;
            }
        }
        /// <summary>
        /// Calculate tax weights used in attributes
        /// </summary>
        public void CalculateWeights()
        {
            if (HasChanges)
                CalculateAmounts();

            //calculate tax weights
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
                    if (this.TotalAmount != 0)
                    {
                        taxrate.TaxRateWeight = PricesIncludeTax ? taxrate.AmountIncludingTax / this.TotalAmountIncludingTax : taxrate.BaseAmount / this.TotalAmount;
                    }
                    else
                    {
                        taxrate.TaxRateWeight = 0;
                    }
                    totWeight += taxrate.TaxRateWeight;
                }
                else
                {
                    //assure sum of TaxRateWeight = 1
                    taxrate.TaxRateWeight = decimal.One - totWeight;
                }
            }
        }
        #endregion
    }
}
