
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    public static class DiscountExtentions
    {
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
    }
}
