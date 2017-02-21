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
        private readonly IProductAttributeParser _productAttributeParser;
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
        /// <param name="productAttributeParser">Product Attribute Parser</param>
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
            CatalogSettings catalogSettings,
            IProductAttributeParser productAttributeParser)
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
            this._productAttributeParser = productAttributeParser;
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

            return RoundingHelper.RoundAmount(discountAmount); //round amount as it is displayed in this way and rounding errors my happen
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

            return RoundingHelper.RoundAmount(discountAmount); //round amount as it is displayed in this way and rounding errors my happen
        }

        protected virtual RewardPoints GetRedeemableRewardPoints(decimal rewardpointsbase, RewardPoints redeemedRewardPoints, Customer customer, bool? useRewardPoints = null, int ? rewardPointsOfOrder = null)
        {
            if (_rewardPointsSettings.Enabled && rewardpointsbase > decimal.Zero)
            {
                if (!rewardPointsOfOrder.HasValue)
                {
                    if (!useRewardPoints.HasValue)
                        useRewardPoints = customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, _genericAttributeService, _storeContext.CurrentStore.Id);
                    if (useRewardPoints.Value)
                    {
                        var rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);

                        //taxable points.
                        if ( _rewardPointService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance.Points))
                        {
                            if (rewardpointsbase > rewardPointsBalance.Amount)
                            {
                                redeemedRewardPoints.Points = rewardPointsBalance.Points;
                            }
                            else
                            {
                                if (rewardpointsbase > decimal.Zero)
                                    redeemedRewardPoints.Points = _rewardPointService.ConvertAmountToRewardPoints(rewardpointsbase);
                            }
                            rewardpointsbase -= redeemedRewardPoints.Amount;
                        }
                        //purchased reward points (can be always spent)
                        if (rewardpointsbase > rewardPointsBalance.AmountPurchased)
                        {
                            redeemedRewardPoints.PointsPurchased = rewardPointsBalance.PointsPurchased;
                        }
                        else
                        {
                            if (rewardpointsbase > decimal.Zero)
                                redeemedRewardPoints.PointsPurchased = _rewardPointService.ConvertAmountToRewardPoints(rewardpointsbase);
                        }
                    }
                }
                else
                {   //return corrected taxable points (only for updateorder total). Purchased points not needed here.
                    redeemedRewardPoints.Points = _rewardPointService.ConvertAmountToRewardPoints(Math.Min(_rewardPointService.ConvertRewardPointsToAmount(rewardPointsOfOrder ?? 0), rewardpointsbase));
                }
            }

            return redeemedRewardPoints;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
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
            decimal subTotalRewardPointsBaseAmount;

            GetShoppingCartSubTotal(cart, includingTax,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxSummary,
                out subTotalRewardPointsBaseAmount);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxSummary">Tax rates summary (of order sub total)</param>
        /// <param name="subTotalEarnedRewardPointsBaseAmount">Subtotal base amount for earned reward points calculation</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out TaxSummary taxSummary,
            out decimal subTotalEarnedRewardPointsBaseAmount)
        {
            taxSummary = new TaxSummary(includingTax);
            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            subTotalEarnedRewardPointsBaseAmount = decimal.Zero;

            decimal subTotalAmount = decimal.Zero;

            if (!cart.Any())
                return;

            //get the customer
            Customer customer = cart.GetCustomer();

            //sub totals
             foreach (var shoppingCartItem in cart)
            {
                decimal itemAmount = decimal.Zero;
                decimal taxRate = decimal.Zero;

                //restored cartitem
                if (shoppingCartItem.TaxRate.HasValue && shoppingCartItem.SubTotalExclTax != null && shoppingCartItem.SubTotalInclTax != null)
                {
                    itemAmount = includingTax ? shoppingCartItem.SubTotalInclTax ?? 0 : shoppingCartItem.SubTotalExclTax ?? 0;
                    taxRate = shoppingCartItem.TaxRate ?? 0;
                    subTotalAmount += itemAmount;
                }
                //normal item
                else
                {
                    decimal sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem);
                    var newAttributesXml = shoppingCartItem.AttributesXml;
                    itemAmount = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, out taxRate, ref newAttributesXml, includingTax);
                    shoppingCartItem.AttributesXml = newAttributesXml;
                    subTotalAmount += itemAmount;
                }

                //reward points base
                subTotalEarnedRewardPointsBaseAmount += !shoppingCartItem.Product.ExcludeFromRewardPoints ? itemAmount : decimal.Zero;

                //tax rates
                string attributesXml = shoppingCartItem.AttributesXml;
                SortedDictionary<decimal, decimal> attributeTaxWeight = !String.IsNullOrEmpty(attributesXml) ? _productAttributeParser.ParseTaxAttribute(attributesXml) : null;
                if (attributeTaxWeight == null || !attributeTaxWeight.Any())
                {
                    if (itemAmount > decimal.Zero) //MF 22.11.16 taxRate > decimal.Zero &&: VatBase is need also when tax is 0 to show up in summary
                        taxSummary.AddRate(taxRate, itemAmount);
                }
                else //tax in attributes
                {
                    taxSummary.AddAttributeRate(itemAmount, attributeTaxWeight);
                }
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
                        subTotalEarnedRewardPointsBaseAmount += itemAmount;
                        //tax rates
                        if (itemAmount > decimal.Zero) //taxRate > decimal.Zero &&
                            taxSummary.AddRate(taxRate, itemAmount);
                    }
                }
            }

            //subtotal discount
            var subDisc = RoundingHelper.RoundAmount(GetOrderSubtotalDiscount(customer, subTotalAmount, out appliedDiscounts)); //round discount, otherwise rounding errors may happen
            if (subDisc != decimal.Zero)
            {
                taxSummary.SetSubtotalDiscAmount(subDisc);

                //adjust earned reward points base amount
                if (subTotalAmount != decimal.Zero)
                    subTotalEarnedRewardPointsBaseAmount = subTotalEarnedRewardPointsBaseAmount * (1 - subDisc / subTotalAmount);
                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    subTotalEarnedRewardPointsBaseAmount = RoundingHelper.RoundAmount(subTotalEarnedRewardPointsBaseAmount);
            }


            //overall totals
            discountAmount = taxSummary.TotalSubTotalDiscAmount;
            subTotalWithoutDiscount = taxSummary.TotalSubTotalAmount;
            subTotalWithDiscount = includingTax ? taxSummary.TotalAmountIncludingTax : taxSummary.TotalAmount;

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
            List<AppliedGiftCard> appliedGiftCards;
            TaxSummary taxSummaryIncl;
            TaxSummary taxSummaryExcl;

            RewardPoints redeemedRewardPointsIncl;
            RewardPoints redeemedRewardPointsExcl;
            bool earnedRewardPointsAreTaxable = _rewardPointsSettings.EarnedRewardPointsAreTaxable;

            //reward points of order
            var rewardPointsOfOrder = _rewardPointService.GetRewardPointsHistory(customer.Id, true).FirstOrDefault(history => history.UsedWithOrder == updatedOrder);
            var redeemedPointsToOrderAmount = rewardPointsOfOrder != null && earnedRewardPointsAreTaxable ? -rewardPointsOfOrder.Points : 0;
            decimal earnedRewardPointsBaseAmountIncl;
            decimal earnedRewardPointsBaseAmountExcl;

            decimal shoppingCartTaxExcl = GetTaxTotal(restoredCart, false, out taxSummaryExcl, out orderAppliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, out appliedGiftCards, out redeemedRewardPointsExcl, out earnedRewardPointsBaseAmountExcl, useRewardPoints: false, rewardPointsOfOrder: redeemedPointsToOrderAmount);
            decimal shoppingCartTaxIncl = GetTaxTotal(restoredCart, true, out taxSummaryIncl, out orderAppliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, out appliedGiftCards, out redeemedRewardPointsIncl, out earnedRewardPointsBaseAmountIncl, useRewardPoints: false, rewardPointsOfOrder: redeemedPointsToOrderAmount);

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
            updatedOrder.OrderShippingExclTax = taxSummaryExcl.TotalShippingAmountTaxable ?? decimal.Zero;
            updatedOrder.OrderShippingInclTax = taxSummaryIncl.TotalShippingAmountTaxable ?? decimal.Zero;
            updatedOrder.OrderShippingNonTaxable = includingTax ? (taxSummaryIncl.TotalShippingAmountNonTaxable ?? decimal.Zero) : (taxSummaryExcl.TotalShippingAmountNonTaxable ?? decimal.Zero);
            updatedOrder.OrderTax = includingTax ? shoppingCartTaxIncl : shoppingCartTaxExcl;
            updatedOrder.OrderDiscount = taxSummaryExcl.TotalInvDiscAmount;
            updatedOrder.OrderDiscountIncl = taxSummaryIncl.TotalInvDiscAmount;
            updatedOrder.OrderAmount = includingTax ? taxSummaryIncl.TotalAmount : taxSummaryExcl.TotalAmount;
            updatedOrder.OrderAmountIncl = includingTax ? taxSummaryIncl.TotalAmountIncludingTax : taxSummaryExcl.TotalAmountIncludingTax;
            updatedOrder.EarnedRewardPointsBaseAmountExcl = earnedRewardPointsBaseAmountExcl;
            updatedOrder.EarnedRewardPointsBaseAmountIncl = earnedRewardPointsBaseAmountIncl;
            updatedOrder.PaymentMethodAdditionalFeeInclTax = taxSummaryIncl.TotalPaymentFeeAmountTaxable;
            updatedOrder.PaymentMethodAdditionalFeeExclTax = taxSummaryExcl.TotalPaymentFeeAmountTaxable;
            updatedOrder.PaymentMethodAdditionalFeeNonTaxable = includingTax ? (taxSummaryIncl.TotalPaymentFeeAmountNonTaxable ?? decimal.Zero) : (taxSummaryExcl.TotalPaymentFeeAmountNonTaxable ?? decimal.Zero);

            var total = includingTax ? taxSummaryIncl.TotalAmountIncludingTax : taxSummaryExcl.TotalAmountIncludingTax; //gets updated later on

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
                updatedOrderItem.TaxRate = restoredCart.FirstOrDefault(item => item.Id == updatedOrderItem.Id).TaxRate ?? 0;
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
            updatedOrder.TaxRates = taxrates.GenerateTaxRateString();

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
            if (rewardPointsOfOrder != null)
            {
                var rewardPoints = new RewardPoints(_rewardPointService)
                {
                    Points = -rewardPointsOfOrder.Points,
                    PointsPurchased = -rewardPointsOfOrder.PointsPurchased
                };

                var redeemedRewardPoints = includingTax ? redeemedRewardPointsIncl : redeemedRewardPointsExcl;

                if (earnedRewardPointsAreTaxable && redeemedRewardPoints.Amount != rewardPoints.Amount)
                {
                    rewardPoints.Points = redeemedRewardPoints.Points;
                    var corrtotal = Math.Max(total - redeemedRewardPoints.Amount, decimal.Zero);

                    if (corrtotal < rewardPoints.AmountPurchased)
                    {
                        rewardPoints.PointsPurchased = _rewardPointService.ConvertAmountToRewardPoints(corrtotal);
                    }
                }
                else
                {
                    //purchased points have been applied to order total. To get base for reward points add them again.
                    var corrtotal = Math.Max(total - rewardPoints.AmountPurchased, decimal.Zero);
                    if (corrtotal < rewardPoints.Amount)
                    {
                        rewardPoints.Points = _rewardPointService.ConvertAmountToRewardPoints(corrtotal);
                    }
                    corrtotal = Math.Max(total - rewardPoints.Amount, decimal.Zero);
                    if (corrtotal < rewardPoints.AmountPurchased)
                    {
                        rewardPoints.PointsPurchased = _rewardPointService.ConvertAmountToRewardPoints(corrtotal);
                    }
                }

                total += -rewardPoints.Amount + -rewardPoints.AmountPurchased;

                var corrRewardPoints = new RewardPoints(_rewardPointService);
                //uncomment here for the return unused reward points if new order total less redeemed reward points amount
                if (rewardPoints.Points < -rewardPointsOfOrder.Points || rewardPoints.PointsPurchased < -rewardPointsOfOrder.PointsPurchased)
                {
                    corrRewardPoints.Points = -rewardPointsOfOrder.Points - rewardPoints.Points;
                    corrRewardPoints.PointsPurchased = -rewardPointsOfOrder.PointsPurchased - rewardPoints.PointsPurchased;
                    _rewardPointService.AddRewardPointsHistoryEntry(customer, corrRewardPoints, _storeContext.CurrentStore.Id, "Returned unused reward points due to order correction.");
                }
                //comment end

                if (rewardPoints.Amount != rewardPointsOfOrder.UsedAmount || rewardPoints.AmountPurchased != rewardPointsOfOrder.UsedAmountPurchased)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPoints.Amount;
                    rewardPointsOfOrder.Points = -rewardPoints.Points;
                    rewardPointsOfOrder.UsedAmountPurchased = rewardPoints.AmountPurchased;
                    rewardPointsOfOrder.PointsPurchased = -rewardPoints.PointsPurchased;
                    rewardPointsOfOrder.PointsBalance += -corrRewardPoints.Points;
                    rewardPointsOfOrder.PointsBalancePurchased = -corrRewardPoints.PointsPurchased;
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
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
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
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
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
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
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
            {
                //we always need a taxrate, get it from GetShippingPrice with price Zero
                return _taxService.GetShippingPrice(decimal.Zero, includingTax, customer, out taxRate);
            }

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
            List<AppliedGiftCard> appliedGiftCards;
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            RewardPoints redeemedRewardPoints;
            decimal earnedRewardPointsBaseAmount;
            return GetTaxTotal(cart, includingTax, out taxSummary, out appliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, out appliedGiftCards, out redeemedRewardPoints, out earnedRewardPointsBaseAmount, usePaymentMethodAdditionalFee);
        }
        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="taxSummary">Tax rates summary</param>
        /// <param name="appliedDiscounts">Applied invoice discounts</param>
        /// <param name="subTotalAppliedDiscounts">Applied subtotal discounts</param>
        /// <param name="shippingAppliedDiscounts">Applied shipping discounts</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="redeemedRewardPoints">Taxable reward points to redeem</param>
        /// <param name="earnedRewardPointsBaseAmount">Reward points base amount for earned points</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="rewardPointsOfOrder">Only in the case of updating a stored order: give the reward points amount used in the order.</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out TaxSummary taxSummary,
            out List<DiscountForCaching> appliedDiscounts,
            out List<DiscountForCaching> subTotalAppliedDiscounts,
            out List<DiscountForCaching> shippingAppliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out RewardPoints redeemedRewardPoints,
            out decimal earnedRewardPointsBaseAmount,
            bool usePaymentMethodAdditionalFee = true,
            bool? useRewardPoints = null,
            int? rewardPointsOfOrder = null
            )

        {
            redeemedRewardPoints = new RewardPoints(_rewardPointService);

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
                out taxSummary, out earnedRewardPointsBaseAmount);

            decimal taxRate;

            //shipping
            decimal? shippingAmount = GetShoppingCartShippingTotal(cart, includingTax, out taxRate, out shippingAppliedDiscounts);
            if ((shippingAmount ?? decimal.Zero) >= decimal.Zero) //pass also zero, otherwise taxsummary does not know that there is a shipping
                if (_taxSettings.ShippingIsTaxable)
                {
                    taxSummary.AddRate(taxRate, shippingAmount: shippingAmount ?? decimal.Zero);                    
                }
                else
                {
                    taxSummary.TotalShippingAmountNonTaxable = shippingAmount; //no need to recalulate without tax. GetShippingPrice returns price without tax when _taxSettings.ShippingIsTaxable = false.                    
                }

            if (_rewardPointsSettings.AwardPointsIncludeShipping)
                earnedRewardPointsBaseAmount += shippingAmount ?? decimal.Zero; 


            //Order total invoice discount

            //note if shipping is not taxable then no ivDiscount is calculated for it. It would be possible to do it separately. It can't be added here as tax amount would get changed.
            //please note also, then when considering it, ivDiscount must be split
            decimal discountbase = includingTax ? taxSummary.TotalAmountIncludingTax : taxSummary.TotalAmount;
            var ivDiscount = GetOrderTotalDiscount(customer, discountbase, out appliedDiscounts);
            if (ivDiscount != decimal.Zero)
                taxSummary.SetTotalInvDiscAmount(Math.Min(ivDiscount, discountbase), discountbase);

            earnedRewardPointsBaseAmount -= ivDiscount;


            #region reward points
            //redeemed reward points applied to order amount (taxable) and payment amount
            //we anticipate non taxable reward points here for taxable payment fee calculation
            //if hasRewardPointsProduct, then don't redeem points. Would lead to discount plus it generates new reward points when buying a reward points product and paying with purchased points.
            if (cart.HasRewardPointsProduct())
                earnedRewardPointsBaseAmount = decimal.Zero;

            if (_rewardPointsSettings.Enabled && earnedRewardPointsBaseAmount > decimal.Zero)
            {
                if (!useRewardPoints.HasValue)
                    useRewardPoints = customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, _genericAttributeService, _storeContext.CurrentStore.Id);

                if (useRewardPoints.Value)
                {
                    redeemedRewardPoints = GetRedeemableRewardPoints(earnedRewardPointsBaseAmount, redeemedRewardPoints, customer, useRewardPoints, rewardPointsOfOrder);
                    decimal consumedEarnedPointsAmount = Math.Min(redeemedRewardPoints.Amount, earnedRewardPointsBaseAmount);
                    if (_rewardPointsSettings.EarnedRewardPointsAreTaxable && redeemedRewardPoints.Amount != decimal.Zero)
                        taxSummary.SetRewardPointsDiscAmount(consumedEarnedPointsAmount, earnedRewardPointsBaseAmount);
                    else
                        taxSummary.TotalRewardPointsAmountNonTaxable = consumedEarnedPointsAmount;

                    earnedRewardPointsBaseAmount -= consumedEarnedPointsAmount;

                    taxSummary.TotalRewardPointsAmountPurchased = redeemedRewardPoints.AmountPurchased;

                    if (!_rewardPointsSettings.AwardPointsExcludePurchasedRewardPoints)
                        earnedRewardPointsBaseAmount -= redeemedRewardPoints.AmountPurchased;
                }
            }

            #endregion

            //we anticipate gift cards here for taxable payment fee calculation
            #region Applied gift cards

            //let's apply gift cards now (gift cards that can be used)
            decimal giftCardBase = taxSummary.TotalPaymentAmount;
            appliedGiftCards = new List<AppliedGiftCard>();
            if (!cart.IsRecurring())
            {
                //we don't apply gift cards for recurring products
                var giftCards = _giftCardService.GetActiveGiftCardsAppliedByCustomer(customer);
                if (giftCards != null)
                    foreach (var gc in giftCards)
                        if (giftCardBase > decimal.Zero)
                        {
                            decimal remainingAmount = gc.GetGiftCardRemainingAmount();
                            decimal amountCanBeUsed = giftCardBase > remainingAmount ?
                                remainingAmount :
                                giftCardBase;

                            //round
                            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                amountCanBeUsed = RoundingHelper.RoundAmount(amountCanBeUsed);

                            //reduce base
                            giftCardBase -= amountCanBeUsed;
                            taxSummary.TotalGiftcardsAmount += amountCanBeUsed;

                            if (!_rewardPointsSettings.AwardPointsExcludeGiftCard)
                                earnedRewardPointsBaseAmount -= amountCanBeUsed;

                            var appliedGiftCard = new AppliedGiftCard();
                            appliedGiftCard.GiftCard = gc;
                            appliedGiftCard.AmountCanBeUsed = amountCanBeUsed;
                            appliedGiftCards.Add(appliedGiftCard);
                        }
            }
            if (!_rewardPointsSettings.AwardPointsExcludeGiftCard)
                earnedRewardPointsBaseAmount -= taxSummary.TotalGiftcardsAmount ?? decimal.Zero;

            #endregion

            #region Payment Fee
            //payment method additional fee
            if (usePaymentMethodAdditionalFee && !String.IsNullOrEmpty(paymentMethodSystemName))
            {
                decimal additionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFee = _taxService.GetPaymentMethodAdditionalFee(additionalFee, includingTax, customer, out taxRate); //if not taxable returns price

                if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
                {
                    if (paymentMethodAdditionalFee != decimal.Zero)
                        //case of normal taxable fee > 0, taxcategory != 0
                        if (paymentMethodAdditionalFee > decimal.Zero && _taxSettings.PaymentMethodAdditionalFeeTaxClassId != 0)
                            taxSummary.AddRate(taxRate, paymentFeeAmount: paymentMethodAdditionalFee);
                        //case of fee < 0  considered as discount or > 0 considered as surcharge when taxcategory is not set
                        else
                        {
                            decimal discountbaseFee = taxSummary.TotalBaseAmountForPaymentFeeCalculation;
                            taxSummary.SetPaymentFeeOrDiscAmount(paymentMethodAdditionalFee, discountbaseFee);
                        }
                }
                else
                    taxSummary.TotalPaymentFeeAmountNonTaxable = paymentMethodAdditionalFee;

                if (_rewardPointsSettings.AwardPointsIncludePaymentMethodAdditionalFee && paymentMethodAdditionalFee > decimal.Zero)
                    earnedRewardPointsBaseAmount += paymentMethodAdditionalFee;
                else
                    //independent of AwardPointsIncludePaymentMethodAdditionalFee setting, reduce for a possible discount
                    earnedRewardPointsBaseAmount += paymentMethodAdditionalFee;
            }
            #endregion

            //add at least one tax rate (0%)
            if (!taxSummary.TaxRates.Any())
                taxSummary.AddRate(0, 0);

            //total tax
            decimal taxTotal = taxSummary.TotalAmountTax;

            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            //ensure that base amount for earning reward points is not negative
            if (earnedRewardPointsBaseAmount < decimal.Zero)
                earnedRewardPointsBaseAmount = decimal.Zero;

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
            RewardPoints redeemedRewardPoints;
            List<AppliedGiftCard> appliedGiftCards;
            TaxSummary taxSummary;
            decimal earnedRewardPointsBaseAmount;
            bool includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;

            return GetShoppingCartTotal(cart,
                out discountAmount,
                out appliedDiscounts,
                out subTotalAppliedDiscounts,
                out shippingAppliedDiscounts,
                out appliedGiftCards,
                out redeemedRewardPoints,
                out taxSummary,
                out earnedRewardPointsBaseAmount,
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
        /// <param name="redeemedRewardPoints">Redeemed reward points</param>
        /// <param name="taxSummary">Tax summary</param>
        /// <param name="earnedRewardPointsBaseAmount">Base amount for reward points to earn</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<DiscountForCaching> subTotalAppliedDiscounts,
            out List<DiscountForCaching> shippingAppliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out RewardPoints redeemedRewardPoints,
            out TaxSummary taxSummary,
            out decimal earnedRewardPointsBaseAmount,
            bool? includingTax = null,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {

            redeemedRewardPoints = new RewardPoints(_rewardPointService);

            //order totals and tax
            decimal shoppingCartTax = GetTaxTotal(cart, includingTax ?? _workContext.TaxDisplayType == TaxDisplayType.IncludingTax, out taxSummary, out appliedDiscounts, out subTotalAppliedDiscounts, out shippingAppliedDiscounts, out appliedGiftCards, out redeemedRewardPoints, out earnedRewardPointsBaseAmount, usePaymentMethodAdditionalFee, useRewardPoints);
            discountAmount = taxSummary.TotalInvDiscAmount;


            //No shipping method found, hence total can't be calculated
            if (cart.RequiresShipping() && !taxSummary.TotalShippingAmount.HasValue)
            {
                //we have no total -> display total on checkout
                return null;
            }

            decimal orderTotal = taxSummary.TotalPaymentAmount;


            //due to rounding of reward points and different reward points base amount corrections, ordertotal and earnedRewardPointsBaseAmount can be negative
            if (orderTotal < decimal.Zero)
                orderTotal = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                orderTotal = RoundingHelper.RoundAmount(orderTotal);
                earnedRewardPointsBaseAmount = RoundingHelper.RoundAmount(earnedRewardPointsBaseAmount);
            }
            return orderTotal;
        }


        #endregion
    }
}
