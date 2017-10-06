using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Discounts
{
    public class Discount_AppliedToManufacturers
    {
        public int DiscountId { get; set; }
        public virtual Discount Discount { get; set; }

        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
    }
}
