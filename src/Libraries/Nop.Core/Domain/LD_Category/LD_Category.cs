using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.LD_Category
{
    public partial class LD_Category : BaseEntity
    {
        public string Name { get; set; }
        public int WinningUnit { get; set; }
        public bool Active { get; set; }
        public int Price { get; set; }
        public int PriorityOrder { get; set; }
    }
}
