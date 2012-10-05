using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    public static class DiscountExtentions
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


            decimal result = decimal.Zero;
            if (discount.UsePercentage)
                result = (decimal)((((float)amount) * ((float)discount.DiscountPercentage)) / 100f);
            else
                result = discount.DiscountAmount;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

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
