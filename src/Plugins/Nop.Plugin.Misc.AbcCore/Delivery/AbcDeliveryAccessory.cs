/* This table is populated from John */

using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class AbcDeliveryAccessory : BaseEntity
    {
        public int CategoryId { get; set; }

        public int AccessoryItemNumber { get; set; }

        public bool IsDeliveryInstall { get; set; }
    }
}