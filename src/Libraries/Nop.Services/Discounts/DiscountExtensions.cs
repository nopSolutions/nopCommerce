using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount extensions
    /// </summary>
    public static class DiscountExtensions
    {
        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public static decimal GetDiscountAmount(this DiscountForCaching discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //calculate discount amount
            decimal result;
            if (discount.UsePercentage)
                result = (decimal)((((float)amount) * ((float)discount.DiscountPercentage)) / 100f);
            else
                result = discount.DiscountAmount;

            //validate maximum discount amount
            if (discount.UsePercentage && 
                discount.MaximumDiscountAmount.HasValue &&
                result > discount.MaximumDiscountAmount.Value)
                result = discount.MaximumDiscountAmount.Value;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Get preferred discount (with maximum discount value)
        /// </summary>
        /// <param name="discounts">A list of discounts to check</param>
        /// <param name="amount">Amount (initial value)</param>
        /// <param name="discountAmount">Discount amount</param>
        /// <returns>Preferred discount</returns>
        public static List<DiscountForCaching> GetPreferredDiscount(this IList<DiscountForCaching> discounts,
            decimal amount, out decimal discountAmount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            var result = new List<DiscountForCaching>();
            discountAmount = decimal.Zero;
            if (!discounts.Any())
                return result;

            //first we check simple discounts
            foreach (var discount in discounts)
            {
                var currentDiscountValue = discount.GetDiscountAmount(amount);
                if (currentDiscountValue > discountAmount)
                {
                    discountAmount = currentDiscountValue;

                    result.Clear();
                    result.Add(discount);
                }
            }
            //now let's check cumulative discounts
            //right now we calculate discount values based on the original amount value
            //please keep it in mind if you're going to use discounts with "percentage"
            var cumulativeDiscounts = discounts.Where(x => x.IsCumulative).OrderBy(x => x.Name).ToList();
            if (cumulativeDiscounts.Count > 1)
            {
                var cumulativeDiscountAmount = cumulativeDiscounts.Sum(d => d.GetDiscountAmount(amount));
                if (cumulativeDiscountAmount > discountAmount)
                {
                    discountAmount = cumulativeDiscountAmount;

                    result.Clear();
                    result.AddRange(cumulativeDiscounts);
                }
            }

            return result;
        }

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount intance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        public static bool ContainsDiscount(this IList<DiscountForCaching> discounts,
            DiscountForCaching discount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            foreach (var dis1 in discounts)
                if (discount.Id == dis1.Id)
                    return true;

            return false;
        }

        /// <summary>
        /// Map a discount to the same class for caching
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Result</returns>
        public static DiscountForCaching MapDiscount(this Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return new DiscountForCaching
            {
                Id = discount.Id,
                Name = discount.Name,
                DiscountTypeId = discount.DiscountTypeId,
                UsePercentage = discount.UsePercentage,
                DiscountPercentage = discount.DiscountPercentage,
                DiscountAmount = discount.DiscountAmount,
                MaximumDiscountAmount = discount.MaximumDiscountAmount,
                StartDateUtc = discount.StartDateUtc,
                EndDateUtc = discount.EndDateUtc,
                RequiresCouponCode = discount.RequiresCouponCode,
                CouponCode = discount.CouponCode,
                IsCumulative = discount.IsCumulative,
                DiscountLimitationId = discount.DiscountLimitationId,
                LimitationTimes = discount.LimitationTimes,
                MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
                AppliedToSubCategories = discount.AppliedToSubCategories
            };
        }
    }
}
