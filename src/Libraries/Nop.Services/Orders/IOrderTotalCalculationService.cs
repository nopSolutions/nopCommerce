using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the applied discount amount. Applied discounts. Sub total (without discount). Sub total (with discount). Tax rates (of order sub total)
        /// </returns>
        Task<(decimal discountAmount, List<Discount> appliedDiscounts, decimal subTotalWithoutDiscount, decimal subTotalWithDiscount, SortedDictionary<decimal, decimal> taxRates)> GetShoppingCartSubTotalAsync(IList<ShoppingCartItem> cart,
            bool includingTax);

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the applied discount amount. Applied discounts. Sub total (without discount). Sub total (with discount). Tax rates (of order sub total)
        /// </returns>
        Task<(decimal discountAmountInclTax, decimal discountAmountExclTax, List<Discount> appliedDiscounts, decimal subTotalWithoutDiscountInclTax, decimal subTotalWithoutDiscountExclTax, decimal subTotalWithDiscountInclTax, decimal subTotalWithDiscountExclTax, SortedDictionary<decimal, decimal> taxRates)> GetShoppingCartSubTotalsAsync(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="applyToPickupInStore">Adjust shipping rate to pickup in store shipping option rate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the adjusted shipping rate. Applied discounts
        /// </returns>
        Task<(decimal adjustedShippingRate, List<Discount> appliedDiscounts)> AdjustShippingRateAsync(decimal shippingRate,
            IList<ShoppingCartItem> cart, bool applyToPickupInStore = false);

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether shipping is free
        /// </returns>
        Task<bool> IsFreeShippingAsync(IList<ShoppingCartItem> cart, decimal? subTotal = null);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total
        /// </returns>
        Task<decimal?> GetShoppingCartShippingTotalAsync(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Applied tax rate. Applied discounts
        /// </returns>
        Task<(decimal? shippingTotal, decimal taxRate, List<Discount> appliedDiscounts)> GetShoppingCartShippingTotalAsync(
            IList<ShoppingCartItem> cart, bool includingTax);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Applied tax rate. Applied discounts
        /// </returns>
        Task<(decimal? shippingTotalInclTax, decimal? shippingTotaExclTax, decimal taxRate, List<Discount> appliedDiscounts)> GetShoppingCartShippingTotalsAsync(
            IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total, Tax rates
        /// </returns>
        Task<(decimal taxTotal, SortedDictionary<decimal, decimal> taxRates)> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart total;Null if shopping cart total couldn't be calculated now. Applied gift cards. Applied discount amount. Applied discounts. Reward points to redeem. Reward points amount in primary store currency to redeem
        /// </returns>
        Task<(decimal? shoppingCartTotal, decimal discountAmount, List<Discount> appliedDiscounts, List<AppliedGiftCard> appliedGiftCards, int redeemedRewardPoints, decimal redeemedRewardPointsAmount)> GetShoppingCartTotalAsync(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Calculate payment method fee
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="fee">Fee value</param>
        /// <param name="usePercentage">Is fee amount specified as percentage or fixed value?</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<decimal> CalculatePaymentAdditionalFeeAsync(IList<ShoppingCartItem> cart, decimal fee, bool usePercentage);

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateOrderTotalsAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart);

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertRewardPointsToAmountAsync(int rewardPoints);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of reward points
        /// </returns>
        Task<int> CalculateRewardPointsAsync(Customer customer, decimal amount);
    }
}
