using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Discounts;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public partial interface IOrderTotalCalculationService
    {
        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount);

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
        void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out TaxSummary taxSummary,
            out decimal subTotalEarnedRewardPointsBaseAmount);





        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Adjusted shipping rate</returns>
        decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, out List<DiscountForCaching> appliedDiscounts);

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>A value indicating whether shipping is free</returns>
        bool IsFreeShipping(IList<ShoppingCartItem> cart, decimal? subTotal = null);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts);






        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxSummary">Tax Summary</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(IList<ShoppingCartItem> cart, out TaxSummary taxSummary, bool usePaymentMethodAdditionalFee = true);

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
        decimal GetTaxTotal(IList<ShoppingCartItem> cart,
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
            int? rewardPointsOfOrder = null);



        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalAppliedDiscounts">Applied subtotal discounts</param>
        /// <param name="shippingAppliedDiscounts">Applied shipping discounts</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="redeemedRewardPoints">Array of reward points to redeem: earned, purchased, total</param>
        /// <param name="taxSummary">Tax summary</param>
        /// <param name="earnedRewardPointsBaseAmount">Base amount for reward points to earn</param>
        /// <param name="includingTax">A value indicating whether calculated amounts should include tax</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<DiscountForCaching> subTotalAppliedDiscounts,
            out List<DiscountForCaching> shippingAppliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out RewardPoints redeemedRewardPoints,
            out TaxSummary taxSummary,
            out decimal earnedRewardPointsBaseAmount,
            bool? includingTax = null,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);




        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart);

    }
}
