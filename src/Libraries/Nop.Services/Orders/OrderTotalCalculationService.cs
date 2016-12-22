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
using System.Diagnostics;

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
            decimal orderSubTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !allowedDiscounts.ContainsDiscount(discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

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
        protected virtual decimal GetShippingDiscount(Customer customer, decimal shippingTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            decimal shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return shippingDiscountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !allowedDiscounts.ContainsDiscount(discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = allowedDiscounts.GetPreferredDiscount(shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = RoundingHelper.RoundAmount(shippingDiscountAmount);

            return shippingDiscountAmount;
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            decimal discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !allowedDiscounts.ContainsDiscount(discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = allowedDiscounts.GetPreferredDiscount(orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = RoundingHelper.RoundAmount(discountAmount);

            return discountAmount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            TaxSummary taxSummary;
            GetShoppingCartSubTotal(cart, includingTax,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxSummary);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxSummary">Tax rates summary (of order sub total)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out TaxSummary taxSummary)
        {
            taxSummary = new TaxSummary(includingTax);
            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;

            decimal subTotalAmount = decimal.Zero;

            if (!cart.Any())
                return;

            //get the customer
            Customer customer = cart.GetCustomer();

            //sub totals
             foreach (var shoppingCartItem in cart)
            {
                decimal itemAmount = decimal.Zero;
                decimal taxRate;

                //restored cartitem
                if (shoppingCartItem.VatRate.HasValue)
                {
                    itemAmount = includingTax ? shoppingCartItem.SubTotalInclTax ?? 0 : shoppingCartItem.SubTotalExclTax ?? 0;
                    taxRate = shoppingCartItem.VatRate ?? 0;
                    subTotalAmount += itemAmount;
                }
                //normal item
                else
                {
                    decimal sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem);
                    itemAmount = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, includingTax, customer, out taxRate);
                    subTotalAmount += itemAmount;
                }
                //tax rates
                if (itemAmount > decimal.Zero) //MF 22.11.16 taxRate > decimal.Zero &&: VatBase is need also when tax is 0 to show up in summary
                    taxSummary.AddRate(taxRate, itemAmount);
            }

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (attributeValues != null && attributeValues.Any()) //MF 16.11.16 any
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        decimal taxRate;
                        decimal itemAmount = _taxService.GetCheckoutAttributePrice(attributeValue, includingTax, customer, out taxRate);
                        subTotalAmount += itemAmount;
                        //tax rates
                        if (itemAmount > decimal.Zero) //taxRate > decimal.Zero &&
                            taxSummary.AddRate(taxRate, itemAmount);
                    }
                }
            }

            //subtotal discount
            var subDisc = GetOrderSubtotalDiscount(customer, subTotalAmount, out appliedDiscounts);
            if (subDisc != decimal.Zero)
                taxSummary.SetSubtotalDiscAmount(subDisc);

            //overall totals
            taxSummary.CalculateAmounts();
            discountAmount = taxSummary.TotalSubTotalDiscAmount;
            subTotalWithoutDiscount = taxSummary.TotalSubTotalAmount;
            subTotalWithDiscount = taxSummary.TotalAmountIncludingVAT;

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

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


            bool includingTax = updatedOrder.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;
            List<DiscountForCaching> orderAppliedDiscounts;
            List<DiscountForCaching> subTotalAppliedDiscounts;
            List<DiscountForCaching> shippingAppliedDiscounts;
            TaxSummary taxSummaryIncl;
            TaxSummary taxSummaryExcl;

            decimal shoppingCartTax = GetTaxTotal(restoredCart, false, out taxSummaryExcl, out orderAppliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts);
            shoppingCartTax = GetTaxTotal(restoredCart, true, out taxSummaryIncl, out orderAppliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts);

            #region discounts
            foreach (var discount in orderAppliedDiscounts)
                if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            foreach (var discount in subTotalAppliedDiscounts)
                if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            foreach (var discount in shippingAppliedDiscounts)
                if (!updateOrderParameters.AppliedDiscounts.ContainsDiscount(discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            #endregion

            #region order

            updatedOrder.OrderSubtotalExclTax = taxSummaryExcl.TotalSubTotalAmount;
            updatedOrder.OrderSubtotalInclTax = taxSummaryIncl.TotalSubTotalAmount;
            updatedOrder.OrderSubTotalDiscountExclTax = taxSummaryExcl.TotalSubTotalDiscAmount;
            updatedOrder.OrderSubTotalDiscountInclTax = taxSummaryIncl.TotalSubTotalDiscAmount;
            updatedOrder.OrderShippingExclTax = taxSummaryExcl.TotalShippingAmount;
            updatedOrder.OrderShippingInclTax = taxSummaryIncl.TotalShippingAmount;
            updatedOrder.OrderTax = shoppingCartTax;
            updatedOrder.OrderDiscount = includingTax ? taxSummaryIncl.TotalInvDiscAmount : taxSummaryExcl.TotalInvDiscAmount;
            updatedOrder.OrderAmount = includingTax ? taxSummaryIncl.TotalAmount : taxSummaryExcl.TotalAmount;
            updatedOrder.OrderAmountIncl = includingTax ? taxSummaryIncl.TotalAmountIncludingVAT : taxSummaryExcl.TotalAmountIncludingVAT;

            var total = includingTax ? taxSummaryIncl.TotalAmountIncludingVAT : taxSummaryExcl.TotalAmountIncludingVAT; //gets updated later on

            //get shopping cart item which has been updated
            var updatedShoppingCartItem = restoredCart.FirstOrDefault(shoppingCartItem => shoppingCartItem.Id == updatedOrderItem.Id);
            var itemDeleted = updatedShoppingCartItem == null;
            if (!itemDeleted)
            {
                //update order item
                updatedOrderItem.UnitPriceExclTax = updateOrderParameters.PriceExclTax;
                updatedOrderItem.UnitPriceInclTax = updateOrderParameters.PriceInclTax;
                updatedOrderItem.DiscountAmountExclTax = updateOrderParameters.DiscountAmountExclTax;
                updatedOrderItem.DiscountAmountInclTax = updateOrderParameters.DiscountAmountInclTax;
                updatedOrderItem.PriceExclTax = updateOrderParameters.SubTotalExclTax;
                updatedOrderItem.PriceInclTax = updateOrderParameters.SubTotalInclTax;
                updatedOrderItem.Quantity = restoredCart.FirstOrDefault(item => item.Id == updatedOrderItem.Id).Quantity;
                updatedOrderItem.VatRate = restoredCart.FirstOrDefault(item => item.Id == updatedOrderItem.Id).VatRate ?? 0;
            }
            #endregion

            #region Shipping

            if (restoredCart.RequiresShipping())
            {
                if (!IsFreeShipping(restoredCart, _shippingSettings.FreeShippingOverXIncludingTax ? taxSummaryIncl.TotalSubTotalAmount : taxSummaryExcl.TotalSubTotalAmount))
                {
                    if (!string.IsNullOrEmpty(updatedOrder.ShippingRateComputationMethodSystemName))
                    {
                        //in the updated order were shipping items
                        if (updatedOrder.PickUpInStore)
                        {
                            //customer chose pickup in store method, try to get chosen pickup point
                            if (_shippingSettings.AllowPickUpInStore)
                            {
                                var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress, updatedOrder.Customer,
                                    updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                                if (pickupPointsResponse.Success)
                                {
                                    var selectedPickupPoint = pickupPointsResponse.PickupPoints.FirstOrDefault(point => updatedOrder.ShippingMethod.Contains(point.Name));
                                    if (selectedPickupPoint == null)
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
                                updatedOrder.ShippingAddress, updatedOrder.Customer, updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                            if (shippingOptionsResponse.Success)
                            {
                                var shippingOption = shippingOptionsResponse.ShippingOptions.FirstOrDefault(option => updatedOrder.ShippingMethod.Contains(option.Name));
                                if (shippingOption == null)
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
                            var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                            if (pickupPointsResponse.Success)
                            {
                                updateOrderParameters.PickupPoint = pickupPointsResponse.PickupPoints.OrderBy(point => point.PickupFee).First();
                            }
                            else
                                updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
                        }
                        else
                            updateOrderParameters.Warnings.Add("Pick up in store is not available");

                        if (updateOrderParameters.PickupPoint == null)
                        {
                            //or try to get the cheapest shipping option for the shipping to the customer address
                            var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                            if (shippingRateComputationMethods.Any())
                            {
                                var shippingOptionsResponse = _shippingService.GetShippingOptions(restoredCart, customer.ShippingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                                if (shippingOptionsResponse.Success)
                                {
                                    var shippingOption = shippingOptionsResponse.ShippingOptions.OrderBy(option => option.Rate).First();
                                    updatedOrder.ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName;
                                    updatedOrder.ShippingMethod = shippingOption.Name;
                                    updatedOrder.ShippingAddress = (Address)customer.ShippingAddress.Clone();
                                }
                                else
                                    updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
                            }
                            else
                                updateOrderParameters.Warnings.Add("Shipping rate computation method could not be loaded");
                        }
                    }

                    //change shipping status
                    if (updatedOrder.ShippingStatus == ShippingStatus.ShippingNotRequired || updatedOrder.ShippingStatus == ShippingStatus.NotYetShipped)
                        updatedOrder.ShippingStatus = ShippingStatus.NotYetShipped;
                    else
                        updatedOrder.ShippingStatus = ShippingStatus.PartiallyShipped;
                }
            }
            else
                updatedOrder.ShippingStatus = ShippingStatus.ShippingNotRequired;



            #endregion

            #region Tax rates
            //tax rates
            var taxrates = includingTax ? taxSummaryIncl : taxSummaryExcl;
            updatedOrder.TaxRates = taxrates.TaxRates.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2}:{3}:{4}:{5}:{6};   ", current,
                    next.Key.ToString(CultureInfo.InvariantCulture),
                    (next.Value.SubtotalAmount + next.Value.ShippingAmount + next.Value.PaymentFeeAmount).ToString(CultureInfo.InvariantCulture),
                    (next.Value.SubTotalDiscAmount + next.Value.InvoiceDiscountAmount).ToString(CultureInfo.InvariantCulture),
                    next.Value.BaseAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.VatAmount.ToString(CultureInfo.InvariantCulture),
                    next.Value.AmountIncludingVAT.ToString(CultureInfo.InvariantCulture)
                 )
            );

            #endregion

            #region total

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
                if (rewardPoints < -rewardPointsOfOrder.Points)
                    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfOrder.Points - rewardPoints, _storeContext.CurrentStore.Id, "Return unused reward points");
                //comment end

                if (rewardPointsAmount != rewardPointsOfOrder.UsedAmount)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPointsAmount;
                    rewardPointsOfOrder.Points = -rewardPoints;
                    _rewardPointService.UpdateRewardPointsHistoryEntry(rewardPointsOfOrder);
                }
            }

            updatedOrder.OrderTotal = total;
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
                    List<DiscountForCaching> appliedDiscounts;
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
            IList<ShoppingCartItem> cart, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();

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
                adjustedRate = RoundingHelper.RoundAmount(adjustedRate);

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
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
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
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate)
        {
            List<DiscountForCaching> appliedDiscounts;
            return GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out appliedDiscounts);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts)
        {
            decimal? shippingTotal = null;
            decimal? shippingTotalTaxed = null;
            appliedDiscounts = new List<DiscountForCaching>();
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

                var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
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
                    shippingTotal = RoundingHelper.RoundAmount(shippingTotal.Value);

                shippingTotalTaxed = _taxService.GetShippingPrice(shippingTotal.Value,
                    includingTax,
                    customer,
                    out taxRate);

                //round
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    shippingTotalTaxed = RoundingHelper.RoundAmount(shippingTotalTaxed.Value);
            }

            return shippingTotalTaxed;
        }





        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxSummary">Tax Summary</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, out TaxSummary taxSummary, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            List<DiscountForCaching> appliedDiscounts;
            List<DiscountForCaching> subTotalAppliedDiscounts;
            List<DiscountForCaching> shippingAppliedDiscounts;
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetTaxTotal(cart, includingTax, out taxSummary, out appliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, usePaymentMethodAdditionalFee);
        }
        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="taxSummary">Tax rates summary</param>
        /// <param name="appliedDiscounts">Applied invoice discounts</param>
        /// <param name="subTotalAppliedDiscounts">Applied subtotal discounts</param>
        /// <param name="shippingAppliedDiscounts">Applied shipping discounts</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out TaxSummary taxSummary,
            out List<DiscountForCaching> appliedDiscounts,
            out List<DiscountForCaching> subTotalAppliedDiscounts,
            out List<DiscountForCaching> shippingAppliedDiscounts,
            bool usePaymentMethodAdditionalFee = true)

        {
            if (cart == null)
                throw new ArgumentNullException("cart");

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
            decimal orderSubTotalDiscountAmount;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;

            GetShoppingCartSubTotal(cart, includingTax,
                out orderSubTotalDiscountAmount, out subTotalAppliedDiscounts,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase,
                out taxSummary);

            decimal taxRate;

            //shipping
            decimal? shippingAmount = GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out shippingAppliedDiscounts);
            if ((shippingAmount ?? 0) > decimal.Zero && _taxSettings.ShippingIsTaxable)
                taxSummary.AddRate(taxRate, shippingAmount: shippingAmount ?? 0);

            //payment method additional fee
            taxSummary.CalculateAmounts();
            decimal discountbaseFee = includingTax ? taxSummary.TotalAmountIncludingVAT : taxSummary.TotalAmount;

            if (usePaymentMethodAdditionalFee && !String.IsNullOrEmpty(paymentMethodSystemName))
            {
                decimal paymentMethodAdditionalFee = _taxService.GetPaymentMethodAdditionalFee(_paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName), includingTax, customer, out taxRate);

                if (paymentMethodAdditionalFee != decimal.Zero )
                    //tfc case of taxable fee > 0
                    if (paymentMethodAdditionalFee > decimal.Zero && _taxSettings.PaymentMethodAdditionalFeeIsTaxable) 
                        taxSummary.AddRate(taxRate, paymentfeeAmount: paymentMethodAdditionalFee);
                    //tfc case fee < 0 is considered as discount or as surcharge when not taxable
                    else
                        taxSummary.SetPaymentFeeDiscAmount(paymentMethodAdditionalFee, discountbaseFee);
            }

            //Order total invoice discount should be considered in tax calculation
            taxSummary.CalculateAmounts();
            decimal discountbase = includingTax ? taxSummary.TotalAmountIncludingVAT : taxSummary.TotalAmount;
            var ivDiscount = GetOrderTotalDiscount(customer, discountbase, out appliedDiscounts);
            if (ivDiscount != decimal.Zero)
                taxSummary.SetTotalInvDiscAmount(ivDiscount, discountbase);


            //add at least one tax rate (0%)
            if (!taxSummary.TaxRates.Any())
                taxSummary.AddRate(0, 0);

            taxSummary.CalculateAmounts();

            //summarize taxes
            decimal taxTotal = taxSummary.TotalAmountVAT;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            return taxTotal;
        }





        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null,  bool usePaymentMethodAdditionalFee = true)
        {
            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            List<DiscountForCaching> subTotalAppliedDiscounts;
            List<DiscountForCaching> shippingAppliedDiscounts;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            List<AppliedGiftCard> appliedGiftCards;
            TaxSummary taxSummary;
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;

            return GetShoppingCartTotal(cart,
                out discountAmount,
                out appliedDiscounts,
                out subTotalAppliedDiscounts,
                out shippingAppliedDiscounts,
                out appliedGiftCards,
                out redeemedRewardPoints,
                out redeemedRewardPointsAmount,
                out taxSummary,
                includingTax,
                useRewardPoints,
                usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalAppliedDiscounts">Applied subtotal discounts</param>
        /// <param name="shippingAppliedDiscounts">Applied shipping discounts</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="taxSummary">Tax summary</param>
        /// <param name="includingTax">A value indicating whether submitted prices do include tax</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<DiscountForCaching> subTotalAppliedDiscounts,
            out List<DiscountForCaching> shippingAppliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            out TaxSummary taxSummary,
            bool includingTax,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {

            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            #region order totals and tax
            //order totals and tax
            decimal shoppingCartTax = GetTaxTotal(cart, includingTax, out taxSummary, out appliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, usePaymentMethodAdditionalFee);
            decimal orderSubTotalDiscountAmount = taxSummary.TotalSubTotalDiscAmount;
            decimal subTotalWithoutDiscountBase = taxSummary.TotalSubTotalAmount;
            decimal subTotalWithDiscountBase = taxSummary.TotalAmountIncludingVAT;

            //shipping
            decimal? shoppingCartShipping = taxSummary.TotalShippingAmount;

            //payment method additional fee
            decimal paymentMethodAdditionalFee = taxSummary.TotalPaymentFeeAmount;

            //order total
            decimal resultTemp = taxSummary.TotalAmountIncludingVAT;
            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = RoundingHelper.RoundAmount(resultTemp);

            //Order total discount
            //it has reduced vat
            discountAmount = taxSummary.TotalInvDiscAmount;

            #endregion

            #region Applied gift cards

            var customer = cart.GetCustomer();
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
                resultTemp = RoundingHelper.RoundAmount(resultTemp);

            if (!shoppingCartShipping.HasValue)
            {
                //we have errors
                return null;
            }

            decimal orderTotal = resultTemp;

            #region Reward points

            if (_rewardPointsSettings.Enabled)
            {
                if (!useRewardPoints.HasValue)
                    useRewardPoints = customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, _genericAttributeService, _storeContext.CurrentStore.Id);
                if (useRewardPoints.Value)
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
            }

            #endregion

            orderTotal = orderTotal - redeemedRewardPointsAmount;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                orderTotal = RoundingHelper.RoundAmount(orderTotal);
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
                result = RoundingHelper.RoundAmount(result);
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
        /// Calculate how order total (maximum amount) for which reward points could be earned/reduced
        /// </summary>
        /// <param name="orderShippingInclTax">Order shipping (including tax)</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>Applicable order total</returns>
        public virtual decimal CalculateApplicableOrderTotalForRewardPoints(decimal orderShippingInclTax, decimal orderTotal)
        {
            //do you give reward points for order total? or do you exclude shipping?
            //since shipping costs vary some of store owners don't give reward points based on shipping total
            //you can put your custom logic here
            return orderTotal - orderShippingInclTax;
        }
        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>Number of reward points</returns>
        public virtual int CalculateRewardPoints(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || customer.IsGuest())
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
            return points;
        }

        #endregion
    }
}
