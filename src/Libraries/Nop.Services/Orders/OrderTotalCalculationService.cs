using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IRewardPointService _rewardPointService;
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
        /// <param name="rewardPointService">Reward point service</param>
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
            IRewardPointService rewardPointService,
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
            this._rewardPointService = rewardPointService;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._shippingSettings = shippingSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderSubtotalDiscount(Customer customer,
            decimal orderSubTotal, out List<Discount> appliedDiscounts)
        {
            appliedDiscounts = new List<Discount>();
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                               discount.DiscountType == DiscountType.AssignedToOrderSubTotal &&
                               !allowedDiscounts.ContainsDiscount(discount))
                        allowedDiscounts.Add(discount);

            appliedDiscounts = allowedDiscounts.GetPreferredDiscount(orderSubTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return discountAmount;
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping discount</returns>
        protected virtual decimal GetShippingDiscount(Customer customer, decimal shippingTotal, out List<Discount> appliedDiscounts)
        {
            appliedDiscounts = new List<Discount>();
            decimal shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return shippingDiscountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                               discount.DiscountType == DiscountType.AssignedToShipping &&
                               !allowedDiscounts.ContainsDiscount(discount))
                        allowedDiscounts.Add(discount);

            appliedDiscounts = allowedDiscounts.GetPreferredDiscount(shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = RoundingHelper.RoundPrice(shippingDiscountAmount);

            return shippingDiscountAmount;
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out List<Discount> appliedDiscounts)
        {
            appliedDiscounts = new List<Discount>();
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                               discount.DiscountType == DiscountType.AssignedToOrderTotal &&
                               !allowedDiscounts.ContainsDiscount(discount))
                        allowedDiscounts.Add(discount);

            appliedDiscounts = allowedDiscounts.GetPreferredDiscount(orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = RoundingHelper.RoundPrice(discountAmount);

            return discountAmount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, 
            bool includingTax,
            out decimal discountAmount, out List<Discount> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            SortedDictionary<decimal, decimal> taxRates;
            GetShoppingCartSubTotal(cart, includingTax, 
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<Discount> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates)
        {
            discountAmount = decimal.Zero;
            appliedDiscounts = new List<Discount>();
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            if (!cart.Any())
                return;

            //get the customer 
            Customer customer = cart.GetCustomer();
            
            //sub totals
            decimal subTotalExclTaxWithoutDiscount = decimal.Zero;
            decimal subTotalInclTaxWithoutDiscount = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                decimal sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem);

                decimal taxRate;
                decimal sciExclTax = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, false, customer, out taxRate);
                decimal sciInclTax = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, true, customer, out taxRate);
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
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        decimal taxRate;

                        decimal caExclTax = _taxService.GetCheckoutAttributePrice(attributeValue, false, customer, out taxRate);
                        decimal caInclTax = _taxService.GetCheckoutAttributePrice(attributeValue, true, customer, out taxRate);
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
            subTotalWithoutDiscount = includingTax ? subTotalInclTaxWithoutDiscount : subTotalExclTaxWithoutDiscount;
            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithoutDiscount = RoundingHelper.RoundPrice(subTotalWithoutDiscount);

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            decimal discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTaxWithoutDiscount, out appliedDiscounts);
            if (subTotalExclTaxWithoutDiscount < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTaxWithoutDiscount;
            decimal discountAmountInclTax = discountAmountExclTax;
            //subtotal with discount (excl tax)
            decimal subTotalExclTaxWithDiscount = subTotalExclTaxWithoutDiscount - discountAmountExclTax;
            decimal subTotalInclTaxWithDiscount = subTotalExclTaxWithDiscount;

            //add tax for shopping items & checkout attributes
            var tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
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
                            taxValue = RoundingHelper.RoundPrice(taxValue);
                        taxRates[taxRate] = taxValue;
                    }

                    //subtotal with discount (incl tax)
                    subTotalInclTaxWithDiscount += taxValue;
                }
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                discountAmountInclTax = RoundingHelper.RoundPrice(discountAmountInclTax);
                discountAmountExclTax = RoundingHelper.RoundPrice(discountAmountExclTax);
            }

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

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithDiscount = RoundingHelper.RoundPrice(subTotalWithDiscount);
        }

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        public virtual void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

            //get the customer 
            var customer = restoredCart.GetCustomer();

            #region Sub total

            var subTotalExclTax = decimal.Zero;
            var subTotalInclTax = decimal.Zero;
            var subTotalTaxRates = new SortedDictionary<decimal, decimal>();

            foreach (var shoppingCartItem in restoredCart)
            {
                var itemSubTotalExclTax = decimal.Zero;
                var itemSubTotalInclTax = decimal.Zero;
                var taxRate = decimal.Zero;
                var itemDiscounts = new List<Discount>();

                //calculate subtotal for the updated order item
                if (shoppingCartItem.Id == updatedOrderItem.Id)
                {
                    //update order item 
                    updatedOrderItem.UnitPriceExclTax = updateOrderParameters.PriceExclTax;
                    updatedOrderItem.UnitPriceInclTax = updateOrderParameters.PriceInclTax;
                    updatedOrderItem.DiscountAmountExclTax = updateOrderParameters.DiscountAmountExclTax;
                    updatedOrderItem.DiscountAmountInclTax = updateOrderParameters.DiscountAmountInclTax;
                    updatedOrderItem.PriceExclTax = itemSubTotalExclTax = updateOrderParameters.SubTotalExclTax;
                    updatedOrderItem.PriceInclTax = itemSubTotalInclTax = updateOrderParameters.SubTotalInclTax;
                    updatedOrderItem.Quantity = shoppingCartItem.Quantity;

                    taxRate = Math.Round((100 * (itemSubTotalInclTax - itemSubTotalExclTax)) / itemSubTotalExclTax, 3);
                }
                else
                {
                    //get the already calculated subtotal from the order item
                    itemSubTotalExclTax = updatedOrder.OrderItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceExclTax;
                    itemSubTotalInclTax = updatedOrder.OrderItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceInclTax;
                    taxRate = Math.Round((100 * (itemSubTotalInclTax - itemSubTotalExclTax)) / itemSubTotalExclTax, 3);
                }

                foreach (var discount in itemDiscounts)
                    if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                        updateOrderParameters.AppliedDiscounts.Add(discount);

                subTotalExclTax += itemSubTotalExclTax;
                subTotalInclTax += itemSubTotalInclTax;

                //tax rates
                var itemTaxValue = itemSubTotalInclTax - itemSubTotalExclTax;
                if (taxRate > decimal.Zero && itemTaxValue > decimal.Zero)
                {
                    if (!subTotalTaxRates.ContainsKey(taxRate))
                        subTotalTaxRates.Add(taxRate, itemTaxValue);
                    else
                        subTotalTaxRates[taxRate] = subTotalTaxRates[taxRate] + itemTaxValue;
                }
            }

            if (subTotalExclTax < decimal.Zero)
                subTotalExclTax = decimal.Zero;

            if (subTotalInclTax < decimal.Zero)
                subTotalInclTax = decimal.Zero;

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            List<Discount> subTotalDiscounts;
            var discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTax, out subTotalDiscounts);
            if (subTotalExclTax < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTax;
            var discountAmountInclTax = discountAmountExclTax;

            //add tax for shopping items
            var tempTaxRates = new Dictionary<decimal, decimal>(subTotalTaxRates);
            foreach (var kvp in tempTaxRates)
            {
                if (kvp.Value != decimal.Zero && subTotalExclTax > decimal.Zero)
                {
                    var discountTaxValue = kvp.Value * (discountAmountExclTax / subTotalExclTax);
                    discountAmountInclTax += discountTaxValue;
                    subTotalTaxRates[kvp.Key] = kvp.Value - discountTaxValue;
                }
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalExclTax = RoundingHelper.RoundPrice(subTotalExclTax);
                subTotalInclTax = RoundingHelper.RoundPrice(subTotalInclTax);
                discountAmountExclTax = RoundingHelper.RoundPrice(discountAmountExclTax);
                discountAmountInclTax = RoundingHelper.RoundPrice(discountAmountInclTax);
            }

            updatedOrder.OrderSubtotalExclTax = subTotalExclTax;
            updatedOrder.OrderSubtotalInclTax = subTotalInclTax;
            updatedOrder.OrderSubTotalDiscountExclTax = discountAmountExclTax;
            updatedOrder.OrderSubTotalDiscountInclTax = discountAmountInclTax;

            foreach (var discount in subTotalDiscounts)
                if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            #endregion

            #region Shipping

            var shippingTotalExclTax = decimal.Zero;
            var shippingTotalInclTax = decimal.Zero;
            var shippingTaxRate = decimal.Zero;

            if (restoredCart.RequiresShipping())
            {
                if (!IsFreeShipping(restoredCart, _shippingSettings.FreeShippingOverXIncludingTax ? subTotalInclTax : subTotalExclTax))
                {
                    var shippingTotal = decimal.Zero;
                    if (!string.IsNullOrEmpty(updatedOrder.ShippingRateComputationMethodSystemName))
                    {
                        //in the updated order were shipping items
                        if (updatedOrder.PickUpInStore)
                        {
                            //customer chose pickup in store method, try to get chosen pickup point
                            if (_shippingSettings.AllowPickUpInStore)
                            {
                                var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress,
                                    updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                                if (pickupPointsResponse.Success)
                                {
                                    var selectedPickupPoint = pickupPointsResponse.PickupPoints.FirstOrDefault(point => updatedOrder.ShippingMethod.Contains(point.Name));
                                    if (selectedPickupPoint != null)
                                        shippingTotal = selectedPickupPoint.PickupFee;
                                    else
                                        updateOrderParameters.Warnings.Add(string.Format("Shipping method {0} could not be loaded", updatedOrder.ShippingMethod));
                                }
                                else
                                    updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
                            }
                            else
                                updateOrderParameters.Warnings.Add("Pick up in store is not available");
                        }
                        else
                        {
                            //customer chose shipping to address, try to get chosen shipping option
                            var shippingOptionsResponse = _shippingService.GetShippingOptions(restoredCart,
                                updatedOrder.ShippingAddress, updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                            if (shippingOptionsResponse.Success)
                            {
                                var shippingOption = shippingOptionsResponse.ShippingOptions.FirstOrDefault(option => updatedOrder.ShippingMethod.Contains(option.Name));
                                if (shippingOption != null)
                                    shippingTotal = shippingOption.Rate;
                                else
                                    updateOrderParameters.Warnings.Add(string.Format("Shipping method {0} could not be loaded", updatedOrder.ShippingMethod));
                            }
                            else
                                updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
                        }
                    }
                    else
                    {
                        //before updating order was without shipping
                        if (_shippingSettings.AllowPickUpInStore)
                        {
                            //try to get the cheapest pickup point
                            var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress, null, _storeContext.CurrentStore.Id);
                            if (pickupPointsResponse.Success)
                            {
                                updateOrderParameters.PickupPoint = pickupPointsResponse.PickupPoints.OrderBy(point => point.PickupFee).First();
                                shippingTotal = updateOrderParameters.PickupPoint.PickupFee;
                            }
                            else
                                updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
                        }
                        else
                            updateOrderParameters.Warnings.Add("Pick up in store is not available");

                        if (updateOrderParameters.PickupPoint == null)
                        {
                            //or try to get the cheapest shipping option for the shipping to the customer address 
                            var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods(_storeContext.CurrentStore.Id);
                            if (shippingRateComputationMethods.Any())
                            {
                                var shippingOptionsResponse = _shippingService.GetShippingOptions(restoredCart, customer.ShippingAddress, null, _storeContext.CurrentStore.Id);
                                if (shippingOptionsResponse.Success)
                                {
                                    var shippingOption = shippingOptionsResponse.ShippingOptions.OrderBy(option => option.Rate).First();
                                    updatedOrder.ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName;
                                    updatedOrder.ShippingMethod = shippingOption.Name;
                                    updatedOrder.ShippingAddress = (Address)customer.ShippingAddress.Clone();
                                    shippingTotal = shippingOption.Rate;
                                }
                                else
                                    updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
                            }
                            else
                                updateOrderParameters.Warnings.Add("Shipping rate computation method could not be loaded");
                        }
                    }

                    //additional shipping charge
                    shippingTotal += restoredCart.Where(item => item.IsShipEnabled && !item.IsFreeShipping).Sum(item => item.Product.AdditionalShippingCharge);

                    //shipping discounts
                    List<Discount> shippingTotalDiscounts;
                    shippingTotal -= GetShippingDiscount(customer, shippingTotal, out shippingTotalDiscounts);
                    if (shippingTotal < decimal.Zero)
                        shippingTotal = decimal.Zero;

                    shippingTotalExclTax = _taxService.GetShippingPrice(shippingTotal, false, customer);
                    shippingTotalInclTax = _taxService.GetShippingPrice(shippingTotal, true, customer, out shippingTaxRate);

                    //rounding
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    {
                        shippingTotalExclTax = RoundingHelper.RoundPrice(shippingTotalExclTax);
                        shippingTotalInclTax = RoundingHelper.RoundPrice(shippingTotalInclTax);
                    }

                    //change shipping status
                    if (updatedOrder.ShippingStatus == ShippingStatus.ShippingNotRequired || updatedOrder.ShippingStatus == ShippingStatus.NotYetShipped)
                        updatedOrder.ShippingStatus = ShippingStatus.NotYetShipped;
                    else
                        updatedOrder.ShippingStatus = ShippingStatus.PartiallyShipped;

                    foreach (var discount in shippingTotalDiscounts)
                        if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                            updateOrderParameters.AppliedDiscounts.Add(discount);
                }
            }
            else
                updatedOrder.ShippingStatus = ShippingStatus.ShippingNotRequired;

            updatedOrder.OrderShippingExclTax = shippingTotalExclTax;
            updatedOrder.OrderShippingInclTax = shippingTotalInclTax;

            #endregion

            #region Tax rates

            var taxRates = new SortedDictionary<decimal, decimal>();

            //order subtotal taxes
            var subTotalTax = decimal.Zero;
            foreach (var kvp in subTotalTaxRates)
            {
                subTotalTax += kvp.Value;
                if (kvp.Key > decimal.Zero && kvp.Value > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(kvp.Key))
                        taxRates.Add(kvp.Key, kvp.Value);
                    else
                        taxRates[kvp.Key] = taxRates[kvp.Key] + kvp.Value;
                }
            }

            //shipping taxes
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                shippingTax = shippingTotalInclTax - shippingTotalExclTax;
                if (shippingTax < decimal.Zero)
                    shippingTax = decimal.Zero;

                if (shippingTaxRate > decimal.Zero && shippingTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(shippingTaxRate))
                        taxRates.Add(shippingTaxRate, shippingTax);
                    else
                        taxRates[shippingTaxRate] = taxRates[shippingTaxRate] + shippingTax;
                }
            }

            //payment method additional fee tax
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                paymentMethodAdditionalFeeTax = updatedOrder.PaymentMethodAdditionalFeeInclTax - updatedOrder.PaymentMethodAdditionalFeeExclTax;
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                if (updatedOrder.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    var paymentTaxRate = Math.Round(100 * paymentMethodAdditionalFeeTax / updatedOrder.PaymentMethodAdditionalFeeExclTax, 3);
                    if (paymentTaxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(paymentTaxRate))
                            taxRates.Add(paymentTaxRate, paymentMethodAdditionalFeeTax);
                        else
                            taxRates[paymentTaxRate] = taxRates[paymentTaxRate] + paymentMethodAdditionalFeeTax;
                    }
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTax + shippingTax + paymentMethodAdditionalFeeTax;
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = RoundingHelper.RoundPrice(taxTotal);

            updatedOrder.OrderTax = taxTotal;
            updatedOrder.TaxRates = taxRates.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2};   ", current, next.Key.ToString(CultureInfo.InvariantCulture), next.Value.ToString(CultureInfo.InvariantCulture)));

            #endregion

            #region Total

            var total = (subTotalExclTax - discountAmountExclTax) + shippingTotalExclTax + updatedOrder.PaymentMethodAdditionalFeeExclTax + taxTotal;

            //get discounts for the order total
            List<Discount> orderAppliedDiscounts;
            var discountAmountTotal = GetOrderTotalDiscount(customer, total, out orderAppliedDiscounts);     
            if (total < discountAmountTotal)
                discountAmountTotal = total;
            total -= discountAmountTotal;

            //applied giftcards
            foreach (var giftCard in _giftCardService.GetAllGiftCards(usedWithOrderId: updatedOrder.Id))
            {
                if (total > decimal.Zero)
                {
                    var remainingAmount = giftCard.GiftCardUsageHistory.Where(history => history.UsedWithOrderId == updatedOrder.Id).Sum(history => history.UsedValue);
                    var amountCanBeUsed = total > remainingAmount ? remainingAmount : total;
                    total -= amountCanBeUsed;
                }
            }

            //reward points
            var rewardPointsOfOrder = _rewardPointService.GetRewardPointsHistory(customer.Id, true).FirstOrDefault(history => history.UsedWithOrder == updatedOrder);
            if (rewardPointsOfOrder != null)
            {
                var rewardPoints = -rewardPointsOfOrder.Points;
                var rewardPointsAmount = ConvertRewardPointsToAmount(rewardPoints);
                if (total < rewardPointsAmount)
                {
                    rewardPoints = ConvertAmountToRewardPoints(total);
                    rewardPointsAmount = total;
                }
                if (total > decimal.Zero)
                    total -= rewardPointsAmount;

                //uncomment here for the return unused reward points if new order total less redeemed reward points amount
                //if (rewardPoints < -rewardPointsOfOrder.Points)
                //    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfOrder.Points - rewardPoints, _storeContext.CurrentStore.Id, "Return unused reward points");

                if (rewardPointsAmount != rewardPointsOfOrder.UsedAmount)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPointsAmount;
                    rewardPointsOfOrder.Points = -rewardPoints;
                    _rewardPointService.UpdateRewardPointsHistoryEntry(rewardPointsOfOrder);
                }
            }

            //rounding
            if (total < decimal.Zero)
                total = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                total = RoundingHelper.RoundPrice(total);

            updatedOrder.OrderDiscount = discountAmountTotal;
            updatedOrder.OrderTotal = total;

            foreach (var discount in orderAppliedDiscounts)
                if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            #endregion
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
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>A value indicating whether shipping is free</returns>
        public virtual bool IsFreeShipping(IList<ShoppingCartItem> cart, decimal? subTotal = null)
        {
            if (!cart.RequiresShipping())
                return true;

            //check whether customer is in a customer role with free shipping applied
            var customer = cart.GetCustomer();
            if (customer != null && customer.CustomerRoles.Where(role => role.Active).Any(role => role.FreeShipping))
                return true;

            //check whether all shopping cart items are marked as free shipping
            if (cart.All(item => item.IsShipEnabled && item.IsFreeShipping))
                return true;

            //free shipping over $X
            if (_shippingSettings.FreeShippingOverXEnabled)
            {
                if (!subTotal.HasValue)
                {
                    decimal discountAmount;
                    List<Discount> appliedDiscounts;
                    decimal subTotalWithoutDiscount;
                    decimal subTotalWithDiscount;
                    GetShoppingCartSubTotal(cart, _shippingSettings.FreeShippingOverXIncludingTax, out discountAmount,
                        out appliedDiscounts, out subTotalWithoutDiscount, out subTotalWithDiscount);
                    subTotal = subTotalWithDiscount;
                }

                //check whether we have subtotal enough to have free shipping
                if (subTotal.Value > _shippingSettings.FreeShippingOverXValue)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Adjusted shipping rate</returns>
        public virtual decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, out List<Discount> appliedDiscounts)
        {
            appliedDiscounts = new List<Discount>();

            //free shipping
            if (IsFreeShipping(cart))
                return decimal.Zero;
            
            //additional shipping charges
            decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart);
            var adjustedRate = shippingRate + additionalShippingCharge;

            //discount
            var customer = cart.GetCustomer();
            decimal discountAmount = GetShippingDiscount(customer, adjustedRate, out appliedDiscounts);
            adjustedRate = adjustedRate - discountAmount;

            if (adjustedRate < decimal.Zero)
                adjustedRate = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = RoundingHelper.RoundPrice(adjustedRate);

            return adjustedRate;
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart)
        {
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
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
            decimal taxRate;
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
            List<Discount> appliedDiscounts;
            return GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out appliedDiscounts);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate, out List<Discount> appliedDiscounts)
        {
            decimal? shippingTotal = null;
            decimal? shippingTotalTaxed = null;
            appliedDiscounts = new List<Discount>();
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
                shippingTotal = AdjustShippingRate(shippingOption.Rate, cart, out appliedDiscounts);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = customer.ShippingAddress;

                var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods(_storeContext.CurrentStore.Id);
                if (!shippingRateComputationMethods.Any() && !_shippingSettings.AllowPickUpInStore)
                        throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];

                    bool shippingFromMultipleLocations;
                    var shippingOptionRequests = _shippingService.CreateShippingOptionRequests(cart, 
                        shippingAddress, 
                        _storeContext.CurrentStore.Id, 
                        out shippingFromMultipleLocations);
                    decimal? fixedRate = null;
                    foreach (var shippingOptionRequest in shippingOptionRequests)
                    {
                        //calculate fixed rates for each request-package
                        var fixedRateTmp = shippingRateComputationMethod.GetFixedRate(shippingOptionRequest);
                        if (fixedRateTmp.HasValue)
                        {
                            if (!fixedRate.HasValue)
                                fixedRate = decimal.Zero;

                            fixedRate += fixedRateTmp.Value;
                        }
                    }
                    
                    if (fixedRate.HasValue)
                    {
                        //adjust shipping rate
                        shippingTotal = AdjustShippingRate(fixedRate.Value, cart, out appliedDiscounts);
                    }
                }
            }

            if (shippingTotal.HasValue)
            {
                if (shippingTotal.Value < decimal.Zero)
                    shippingTotal = decimal.Zero;

                //round
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    shippingTotal = RoundingHelper.RoundPrice(shippingTotal.Value);

                shippingTotalTaxed = _taxService.GetShippingPrice(shippingTotal.Value,
                    includingTax,
                    customer,
                    out taxRate);
                
                //round
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    shippingTotalTaxed = RoundingHelper.RoundPrice(shippingTotalTaxed.Value);
            }

            return shippingTotalTaxed;
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

            SortedDictionary<decimal, decimal> taxRates;
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
            decimal orderSubTotalDiscountAmount;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            SortedDictionary<decimal, decimal> orderSubTotalTaxRates;
            GetShoppingCartSubTotal(cart, false, 
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscounts,
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
                decimal taxRate;
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
                decimal taxRate;
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
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            decimal taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;
            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = RoundingHelper.RoundPrice(taxTotal);
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
            decimal discountAmount;
            List<Discount> appliedDiscounts;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            List<AppliedGiftCard> appliedGiftCards;
            return GetShoppingCartTotal(cart, 
                out discountAmount,
                out appliedDiscounts,
                out appliedGiftCards,
                out redeemedRewardPoints, 
                out redeemedRewardPointsAmount, 
                ignoreRewardPonts,
                usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="ignoreRewardPonts">A value indicating whether we should ignore reward points (if enabled and a customer is going to use them)</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<Discount> appliedDiscounts,
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
            decimal orderSubTotalDiscountAmount;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            GetShoppingCartSubTotal(cart, false,
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscounts,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            //subtotal with discount
            decimal subtotalBase = subTotalWithDiscountBase;



            //shipping without tax
            decimal? shoppingCartShipping = GetShoppingCartShippingTotal(cart, false);



            //payment method additional fee without tax
            decimal paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !String.IsNullOrEmpty(paymentMethodSystemName))
            {
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart,
                    paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax =
                    _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
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
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

            #region Order total discount

            discountAmount = GetOrderTotalDiscount(customer, resultTemp, out appliedDiscounts);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

            #endregion

            #region Applied gift cards

            //let's apply gift cards now (gift cards that can be used)
            appliedGiftCards = new List<AppliedGiftCard>();
            if (!cart.IsRecurring())
            {
                //we don't apply gift cards for recurring products
                var giftCards = _giftCardService.GetActiveGiftCardsAppliedByCustomer(customer);
                if (giftCards != null)
                    foreach (var gc in giftCards)
                        if (resultTemp > decimal.Zero)
                        {
                            decimal remainingAmount = gc.GetGiftCardRemainingAmount();
                            decimal amountCanBeUsed = resultTemp > remainingAmount ? 
                                remainingAmount : 
                                resultTemp;

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
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

            if (!shoppingCartShipping.HasValue)
            {
                //we have errors
                return null;
            }

            decimal orderTotal = resultTemp;

            #region Reward points

            if (_rewardPointsSettings.Enabled &&
                !ignoreRewardPonts &&
                customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout,
                    _genericAttributeService, _storeContext.CurrentStore.Id))
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
                if (CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    decimal rewardPointsBalanceAmount = ConvertRewardPointsToAmount(rewardPointsBalance);
                    if (orderTotal > decimal.Zero)
                    {
                        if (orderTotal > rewardPointsBalanceAmount)
                        {
                            redeemedRewardPoints = rewardPointsBalance;
                            redeemedRewardPointsAmount = rewardPointsBalanceAmount;
                        }
                        else
                        {
                            redeemedRewardPointsAmount = orderTotal;
                            redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
                        }
                    }
                }
            }

            #endregion

            orderTotal = orderTotal - redeemedRewardPointsAmount;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                orderTotal = RoundingHelper.RoundPrice(orderTotal);
            return orderTotal;
        }





        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = RoundingHelper.RoundPrice(result);
            return result;
        }

        /// <summary>
        /// Converts an amount to reward points
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

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>umber of reward points</returns>
        public virtual int CalculateRewardPoints(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //Ensure that reward points are applied only to registered users
            if (customer == null || customer.IsGuest())
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
            return points;
        }

        #endregion
    }
}
