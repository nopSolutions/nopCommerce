using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceStore
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
