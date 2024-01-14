using System;
using System.ComponentModel.DataAnnotations;
using Nop.Core;

namespace AO.Services.Domain
{
    public class AOOrderItemSetting : BaseEntity
    {
        [Key]
        public int OrderItemId { get; set; }

        public int IsTakenAside { get; set; }

        public int IsOrdered { get; set; }

        public DateTime IsTakenAsideDate { get; set; }

        public DateTime IsOrderedDate { get; set; }       
        
        public string Gtin { get; set; }
    }
}
