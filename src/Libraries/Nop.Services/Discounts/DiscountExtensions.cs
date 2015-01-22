using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    public static class DiscountExtensions
    {
        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public static decimal GetDiscountAmount(this Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            //calculate discount amount
            decimal result;
            if (discount.UsePercentage)
                result = (decimal)((((float)amount) * ((float)discount.DiscountPercentage)) / 100f);
            else
                result = discount.DiscountAmount;

            //validate maximum disocunt amount
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
        /// <param name="amount">Amount</param>
        /// <returns>Preferred discount</returns>
        public static Discount GetPreferredDiscount(this IList<Discount> discounts,
            decimal amount)
        {
            Discount preferredDiscount = null;
            decimal maximumDiscountValue = decimal.Zero;
            foreach (var discount in discounts)
            {
                decimal currentDiscountValue = discount.GetDiscountAmount(amount);
                if (currentDiscountValue > maximumDiscountValue)
                {
                    maximumDiscountValue = currentDiscountValue;
                    preferredDiscount = discount;
                }
            }

            return preferredDiscount;
        }

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount intance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        public static bool ContainsDiscount(this IList<Discount> discounts,
            Discount discount)
        {
            if (discounts == null)
                throw new ArgumentNullException("discounts");

            if (discount == null)
                throw new ArgumentNullException("discount");

            foreach (var dis1 in discounts)
                if (discount.Id == dis1.Id)
                    return true;

            return false;
        }
    }
}
