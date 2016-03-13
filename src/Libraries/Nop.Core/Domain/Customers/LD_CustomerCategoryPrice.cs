using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    public partial class LD_CustomerCategoryPrice : BaseEntity
    {
        public int CategoryId { get; set; }
        public int CustomerId { get; set; }
        public decimal Price { get; set; }
        public int WinningUnit { get; set; }
        public bool Active { get; set; }
        public DateTime BeginUsedDate { get; set; }
    }
}
