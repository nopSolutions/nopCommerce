using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcStatusApi.Models
{
    public class OrderStatusItem
    {
        public string OrderId { get; set; }
        public int OrderStatus { get; set; }
    }
}
