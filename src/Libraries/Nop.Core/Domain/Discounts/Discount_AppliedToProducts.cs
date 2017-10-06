using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Discounts
{
    public class Discount_AppliedToProducts
    {
        public int DiscountId { get; set; }
        public virtual Discount Discount { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
