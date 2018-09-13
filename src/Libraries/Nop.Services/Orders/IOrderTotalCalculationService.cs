using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Discounts;
using Nop.Services.Shipping;

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
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
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
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates);

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
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods,
            out decimal taxRate);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods, out SortedDictionary<decimal, decimal> taxRates, 
            bool usePaymentMethodAdditionalFee = true);

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
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart);

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        decimal ConvertRewardPointsToAmount(int rewardPoints);

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        int ConvertAmountToRewardPoints(decimal amount);

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints);

        /// <summary>
        /// Calculate how order total (maximum amount) for which reward points could be earned/reduced
        /// </summary>
        /// <param name="orderShippingInclTax">Order shipping (including tax)</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>Applicable order total</returns>
        decimal CalculateApplicableOrderTotalForRewardPoints(decimal orderShippingInclTax, decimal orderTotal);

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>Number of reward points</returns>
        int CalculateRewardPoints(Customer customer, decimal amount);
    }
}
