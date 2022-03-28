using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore
{
    public class GiftCardInfo
    {
        public GiftCard GiftCard { get; set; }
        public decimal AmountUsed { get; set; }
    }
}
