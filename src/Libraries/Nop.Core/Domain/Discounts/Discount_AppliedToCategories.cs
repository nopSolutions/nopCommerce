using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Discounts
{
    public class Discount_AppliedToCategories
    {
        public int DiscountId { get; set; }
        public virtual Discount Discount { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
