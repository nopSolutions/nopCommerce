/* This table is populated from John */

using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class AbcDeliveryItem : BaseEntity
    {
        public int Item_Number { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}