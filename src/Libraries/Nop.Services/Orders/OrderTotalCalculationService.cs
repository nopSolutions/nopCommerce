using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderTotalCalculationService : IOrderTotalCalculationService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IDiscountService _discountService;
        private readonly IGiftCardService _giftCardService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public OrderTotalCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IShippingService shippingService,
            IPaymentService paymentService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IDiscountService discountService,
            IGiftCardService giftCardService,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._discountService = discountService;
            this._giftCardService = giftCardService;
            this._genericAttributeService = genericAttributeService;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._shippingSettings = shippingSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            GetShoppingCartSubTotal(cart, includingTax,
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, 
            bool includingTax,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            SortedDictionary<decimal, decimal> taxRates = null;
            GetShoppingCartSubTotal(cart, includingTax, 
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates)
        {
            discountAmount = decimal.Zero;
            appliedDiscount = null;
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            if (cart.Count == 0)
                return;

            //get the customer 
            Customer customer = cart.GetCustomer();
            
            //sub totals
            decimal subTotalExclTaxWithoutDiscount = decimal.Zero;
            decimal subTotalInclTaxWithoutDiscount = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                decimal taxRate = decimal.Zero;
                decimal sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem, true);

                decimal sciExclTax = _taxService.GetProductPrice(shoppingCartItem.ProductVariant, sciSubTotal, false, customer, out taxRate);
                decimal sciInclTax = _taxService.GetProductPrice(shoppingCartItem.ProductVariant, sciSubTotal, true, customer, out taxRate);
                subTotalExclTaxWithoutDiscount += sciExclTax;
                subTotalInclTaxWithoutDiscount += sciInclTax;
                
                //tax rates
                decimal sciTax = sciInclTax - sciExclTax;
                if (taxRate > decimal.Zero && sciTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                    {
                        taxRates.Add(taxRate, sciTax);
                    }
                    else
                    {
                        taxRates[taxRate] = taxRates[taxRate] + sciTax;
                    }
                }
            }

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService);
                var caValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (caValues!=null)
                {
                    foreach (var caValue in caValues)
                    {
                        decimal taxRate = decimal.Zero;

                        decimal caExclTax = _taxService.GetCheckoutAttributePrice(caValue, false, customer, out taxRate);
                        decimal caInclTax = _taxService.GetCheckoutAttributePrice(caValue, true, customer, out taxRate);
                        subTotalExclTaxWithoutDiscount += caExclTax;
                        subTotalInclTaxWithoutDiscount += caInclTax;

                        //tax rates
                        decimal caTax = caInclTax - caExclTax;
                        if (taxRate > decimal.Zero && caTax > decimal.Zero)
                        {
                            if (!taxRates.ContainsKey(taxRate))
                            {
                                taxRates.Add(taxRate, caTax);
                            }
                            else
                            {
                                taxRates[taxRate] = taxRates[taxRate] + caTax;
                            }
                        }
                    }
                }
            }

            //subtotal without discount
            if (includingTax)
                subTotalWithoutDiscount = subTotalInclTaxWithoutDiscount;
            else
                subTotalWithoutDiscount = subTotalExclTaxWithoutDiscount;
            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithoutDiscount = Math.Round(subTotalWithoutDiscount, 2);

            /*We calculate discount amount on order subtotal excl tax (discount first)*/
            //calculate discount amount ('Applied to order subtotal' discount)
            decimal discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTaxWithoutDiscount, out appliedDiscount);
            if (subTotalExclTaxWithoutDiscount < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTaxWithoutDiscount;
            decimal discountAmountInclTax = discountAmountExclTax;
            //subtotal with discount (excl tax)
            decimal subTotalExclTaxWithDiscount = subTotalExclTaxWithoutDiscount - discountAmountExclTax;
            decimal subTotalInclTaxWithDiscount = subTotalExclTaxWithDiscount;

            //add tax for shopping items & checkout attributes
            Dictionary<decimal, decimal> tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
            foreach (KeyValuePair<decimal, decimal> kvp in tempTaxRates)
            {
                decimal taxRate = kvp.Key;
                decimal taxValue = kvp.Value;

                if (taxValue != decimal.Zero)
                {
                    //discount the tax amount that applies to subtotal items
                    if (subTotalExclTaxWithoutDiscount > decimal.Zero)
                    {
                        decimal discountTax = taxRates[taxRate] * (discountAmountExclTax / subTotalExclTaxWithoutDiscount);
                        discountAmountInclTax += discountTax;
                        taxValue = taxRates[taxRate] - discountTax;
                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            taxValue = Math.Round(taxValue, 2);
                        taxRates[taxRate] = taxValue;
                    }

                    //subtotal with discount (incl tax)
                    subTotalInclTaxWithDiscount += taxValue;
                }
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmountInclTax = Math.Round(discountAmountInclTax, 2);

            if (includingTax)
            {
                subTotalWithDiscount = subTotalInclTaxWithDiscount;
                discountAmount = discountAmountInclTax;
            }
            else
            {
                subTotalWithDiscount = subTotalExclTaxWithDiscount;
                discountAmount = discountAmountExclTax;
            }

            //round
            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithDiscount = Math.Round(subTotalWithDiscount, 2);
        }

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Order discount</returns>
        public virtual decimal GetOrderSubtotalDiscount(Customer customer,
            decimal orderSubTotal, out Discount appliedDiscount)
        {
            appliedDiscount = null;
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.IsDiscountValid(discount, customer) &&
                               discount.DiscountType == DiscountType.AssignedToOrderSubTotal &&
                               !allowedDiscounts.ContainsDiscount(discount))
                        allowedDiscounts.Add(discount);

            appliedDiscount = allowedDiscounts.GetPreferredDiscount(orderSubTotal);
            if (appliedDiscount != null)
                discountAmount = appliedDiscount.GetDiscountAmount(orderSubTotal);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return discountAmount;
        }





        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        public virtual decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart)
        {
            decimal additionalShippingCharge = decimal.Zero;

            bool isFreeShipping = IsFreeShipping(cart);
            if (isFreeShipping)
                return decimal.Zero;

            foreach (var sci in cart)
                if (sci.IsShipEnabled && !sci.IsFreeShipping)
                    additionalShippingCharge += sci.AdditionalShippingCharge;

            return additionalShippingCharge;
        }

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>A value indicating whether shipping is free</returns>
        public virtual bool IsFreeShipping(IList<ShoppingCartItem> cart)
        {
            Customer customer = cart.GetCustomer();
            if (customer != null)
            {
                //check whether customer is in a customer role with free shipping applied
                var customerRoles = customer.CustomerRoles.Where(cr => cr.Active);
                foreach (var customerRole in customerRoles)
                    if (customerRole.FreeShipping)
                        return true;
            }

            bool shoppingCartRequiresShipping = cart.RequiresShipping();
            if (!shoppingCartRequiresShipping)
                return true;

            //check whether all shopping cart items are marked as free shipping
            bool allItemsAreFreeShipping = true;
            foreach (var sc in cart)
            {
                if (sc.IsShipEnabled && !sc.IsFreeShipping)
                {
                    allItemsAreFreeShipping = false;
                    break;
                }
            }
            if (allItemsAreFreeShipping)
                return true;

            //free shipping over $X
            if (_shippingSettings.FreeShippingOverXEnabled)
            {
                //check whether we have subtotal enough to have free shipping
                decimal subTotalDiscountAmount = decimal.Zero;
                Discount subTotalAppliedDiscount = null;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                GetShoppingCartSubTotal(cart, _shippingSettings.FreeShippingOverXIncludingTax, out subTotalDiscountAmount,
                    out subTotalAppliedDiscount, out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithDiscountBase > _shippingSettings.FreeShippingOverXValue)
                    return true;
            }

            //otherwise, return false
            return false;
        }

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Adjusted shipping rate</returns>
        public virtual decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, out Discount appliedDiscount)
        {
            appliedDiscount = null;

            //free shipping
            if (IsFreeShipping(cart))
                return decimal.Zero;
            
            //additional shipping charges
            decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart);
            var adjustedRate = shippingRate + additionalShippingCharge;

            //discount
            var customer = cart.GetCustomer();
            decimal discountAmount = GetShippingDiscount(customer, adjustedRate, out appliedDiscount);
            adjustedRate = adjustedRate - discountAmount;

            if (adjustedRate < decimal.Zero)
                adjustedRate = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = Math.Round(adjustedRate, 2);

            return adjustedRate;
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShoppingCartShippingTotal(cart, includingTax);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax)
        {
            decimal taxRate = decimal.Zero;
            return GetShoppingCartShippingTotal(cart, includingTax, out taxRate);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate)
        {
            Discount appliedDiscount = null;
            return GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out appliedDiscount);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate, out Discount appliedDiscount)
        {
            decimal? shippingTotal = null;
            decimal? shippingTotalTaxed = null;
            appliedDiscount = null;
            taxRate = decimal.Zero;

            var customer = cart.GetCustomer();

            bool isFreeShipping = IsFreeShipping(cart);
            if (isFreeShipping)
                return decimal.Zero;

            ShippingOption shippingOption = null;
            if (customer != null)
                shippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _genericAttributeService, _storeContext.CurrentStore.Id);

            if (shippingOption != null)
            {
                //use last shipping option (get from cache)

                //adjust shipping rate
                shippingTotal = AdjustShippingRate(shippingOption.Rate, cart, out appliedDiscount);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = customer.ShippingAddress;

                var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods();
                if (shippingRateComputationMethods == null || shippingRateComputationMethods.Count == 0)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var getShippingOptionRequest = _shippingService.CreateShippingOptionRequest(cart, shippingAddress);

                    var shippingRateComputationMethod = shippingRateComputationMethods[0];
                    decimal? fixedRate = shippingRateComputationMethod.GetFixedRate(getShippingOptionRequest);
                    if (fixedRate.HasValue)
                    {
                        //adjust shipping rate
                        shippingTotal = AdjustShippingRate(fixedRate.Value, cart, out appliedDiscount);
                    }
                }
            }

            if (shippingTotal.HasValue)
            {
                if (shippingTotal.Value < decimal.Zero)
                    shippingTotal = decimal.Zero;

                //round
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    shippingTotal = Math.Round(shippingTotal.Value, 2);

                shippingTotalTaxed = _taxService.GetShippingPrice(shippingTotal.Value,
                    includingTax,
                    customer,
                    out taxRate);
                
                //round
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    shippingTotalTaxed = Math.Round(shippingTotalTaxed.Value, 2);
            }

            return shippingTotalTaxed;
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Shipping discount</returns>
        public virtual decimal GetShippingDiscount(Customer customer, decimal shippingTotal, out Discount appliedDiscount)
        {
            appliedDiscount = null;
            decimal shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return shippingDiscountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
            foreach (var discount in allDiscounts)
                if (_discountService.IsDiscountValid(discount, customer) &&
                           discount.DiscountType == DiscountType.AssignedToShipping &&
                           !allowedDiscounts.ContainsDiscount(discount))
                    allowedDiscounts.Add(discount);

            appliedDiscount = allowedDiscounts.GetPreferredDiscount(shippingTotal);
            if (appliedDiscount != null)
            {
                shippingDiscountAmount = appliedDiscount.GetDiscountAmount(shippingTotal);
            }

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;
            
            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                shippingDiscountAmount = Math.Round(shippingDiscountAmount, 2);

            return shippingDiscountAmount;
        }
        





        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            SortedDictionary<decimal, decimal> taxRates = null;
            return GetTaxTotal(cart, out taxRates, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart,
            out SortedDictionary<decimal, decimal> taxRates, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            taxRates = new SortedDictionary<decimal, decimal>();

            var customer = cart.GetCustomer();
            string paymentMethodSystemName = "";
            if (customer != null)
            {
                paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService,
                    _storeContext.CurrentStore.Id);
            }

            //order sub total (items + checkout attributes)
            decimal subTotalTaxTotal = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            SortedDictionary<decimal, decimal> orderSubTotalTaxRates = null;
            GetShoppingCartSubTotal(cart,  false, 
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase,
                out orderSubTotalTaxRates);
            foreach (KeyValuePair<decimal, decimal> kvp in orderSubTotalTaxRates)
            {
                decimal taxRate = kvp.Key;
                decimal taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate > decimal.Zero && taxValue > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, taxValue);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + taxValue;
                }
            }

            //shipping
            decimal shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                decimal taxRate = decimal.Zero;
                decimal? shippingExclTax = GetShoppingCartShippingTotal(cart, false, out taxRate);
                decimal? shippingInclTax = GetShoppingCartShippingTotal(cart, true, out taxRate);
                if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                {
                    shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                    //ensure that tax is equal or greater than zero
                    if (shippingTax < decimal.Zero)
                        shippingTax = decimal.Zero;

                    //tax rates
                    if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                            taxRates.Add(taxRate, shippingTax);
                        else
                            taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                    }
                }
            }

            //payment method additional fee
            decimal paymentMethodAdditionalFeeTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                decimal taxRate = decimal.Zero;

                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out taxRate);
                decimal paymentMethodAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                //ensure that tax is equal or greater than zero
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }

            //add at least one tax rate (0%)
            if (taxRates.Count == 0)
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            decimal taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;
            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                taxTotal = Math.Round(taxTotal, 2);
            return taxTotal;
        }





        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="ignoreRewardPonts">A value indicating whether we should ignore reward points (if enabled and a customer is going to use them)</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            bool ignoreRewardPonts = false, bool usePaymentMethodAdditionalFee = true)
        {
            decimal discountAmount = decimal.Zero;
            Discount appliedDiscount = null;
            
            int redeemedRewardPoints = 0;
            decimal redeemedRewardPointsAmount = decimal.Zero;
            List<AppliedGiftCard> appliedGiftCards = null;
            return GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards,
                out redeemedRewardPoints, out redeemedRewardPointsAmount, ignoreRewardPonts, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="ignoreRewardPonts">A value indicating whether we should ignore reward points (if enabled and a customer is going to use them)</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out Discount appliedDiscount,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool ignoreRewardPonts = false, bool usePaymentMethodAdditionalFee = true)
        {
            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            var customer = cart.GetCustomer();
            string paymentMethodSystemName = "";
            if (customer != null)
            {
                paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService,
                    _storeContext.CurrentStore.Id);
            }


            //subtotal without tax
            decimal subtotalBase = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            GetShoppingCartSubTotal(cart, false,
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            //subtotal with discount
            subtotalBase = subTotalWithDiscountBase;



            //shipping without tax
            decimal? shoppingCartShipping = GetShoppingCartShippingTotal(cart, false);



            //payment method additional fee without tax
            decimal paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !String.IsNullOrEmpty(paymentMethodSystemName))
            {
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
                    false, customer);
            }




            //tax
            decimal shoppingCartTax = GetTaxTotal(cart, usePaymentMethodAdditionalFee);




            //order total
            decimal resultTemp = decimal.Zero;
            resultTemp += subtotalBase;
            if (shoppingCartShipping.HasValue)
            {
                resultTemp += shoppingCartShipping.Value;
            }
            resultTemp += paymentMethodAdditionalFeeWithoutTax;
            resultTemp += shoppingCartTax;
            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                resultTemp = Math.Round(resultTemp, 2);

            #region Order total discount

            discountAmount = GetOrderTotalDiscount(customer, resultTemp, out appliedDiscount);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                resultTemp = Math.Round(resultTemp, 2);

            #endregion

            #region Applied gift cards

            //let's apply gift cards now (gift cards that can be used)
            appliedGiftCards = new List<AppliedGiftCard>();
            if (!cart.IsRecurring())
            {
                //we don't apply gift cards for recurring products
                var giftCards = _giftCardService.GetActiveGiftCardsAppliedByCustomer(customer);
                if (giftCards!=null)
                    foreach (var gc in giftCards)
                        if (resultTemp > decimal.Zero)
                        {
                            decimal remainingAmount = gc.GetGiftCardRemainingAmount();
                            decimal amountCanBeUsed = decimal.Zero;
                            if (resultTemp > remainingAmount)
                                amountCanBeUsed = remainingAmount;
                            else
                                amountCanBeUsed = resultTemp;

                            //reduce subtotal
                            resultTemp -= amountCanBeUsed;

                            var appliedGiftCard = new AppliedGiftCard();
                            appliedGiftCard.GiftCard = gc;
                            appliedGiftCard.AmountCanBeUsed = amountCanBeUsed;
                            appliedGiftCards.Add(appliedGiftCard);
                        }
            }

            #endregion

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = Math.Round(resultTemp, 2);

            decimal? orderTotal = null;
            if (!shoppingCartShipping.HasValue)
            {
                //return null if we have errors
                orderTotal = null;
                return orderTotal;
            }
            else
            {
                //return result if we have no errors
                orderTotal = resultTemp;
            }

            #region Reward points

            if (_rewardPointsSettings.Enabled && 
                !ignoreRewardPonts &&
                customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout,
                _genericAttributeService, _storeContext.CurrentStore.Id))
            {
                int rewardPointsBalance = customer.GetRewardPointsBalance();
                if (CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    decimal rewardPointsBalanceAmount = ConvertRewardPointsToAmount(rewardPointsBalance);
                    if (orderTotal.HasValue && orderTotal.Value > decimal.Zero)
                    {
                        if (orderTotal.Value > rewardPointsBalanceAmount)
                        {
                            redeemedRewardPoints = rewardPointsBalance;
                            redeemedRewardPointsAmount = rewardPointsBalanceAmount;
                        }
                        else
                        {
                            redeemedRewardPointsAmount = orderTotal.Value;
                            redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
                        }
                    }
                }
            }
            #endregion

            if (orderTotal.HasValue)
            {
                orderTotal = orderTotal.Value - redeemedRewardPointsAmount;
                if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                    orderTotal = Math.Round(orderTotal.Value, 2);
                return orderTotal;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Order discount</returns>
        public virtual decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out Discount appliedDiscount)
        {
            appliedDiscount = null;
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts!=null)
                foreach (var discount in allDiscounts)
                    if (_discountService.IsDiscountValid(discount, customer) &&
                               discount.DiscountType == DiscountType.AssignedToOrderTotal &&
                               !allowedDiscounts.ContainsDiscount(discount))
                        allowedDiscounts.Add(discount);

            appliedDiscount = allowedDiscounts.GetPreferredDiscount(orderTotal);
            if (appliedDiscount != null)
                discountAmount = appliedDiscount.GetDiscountAmount(orderTotal);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                discountAmount = Math.Round(discountAmount, 2);

            return discountAmount;
        }





        /// <summary>
        /// Converts reward points to amount primary store currency
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            decimal result = decimal.Zero;
            if (rewardPoints <= 0)
                return decimal.Zero;

            result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation) 
                result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts an amount in primary store currency to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            int result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }
 

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        #endregion
    }
}
