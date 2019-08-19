using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Discounts
{
    public abstract partial class DiscountMapping : BaseEntity
    {
        public int DiscountId { get; set; }
    }
}
