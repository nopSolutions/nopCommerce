/* This table is populated from John */

using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class AbcDeliveryMap : BaseEntity
    {
        public int CategoryId { get; set; }

        public int FedEx { get; set; }

        public int DeliveryOnly { get; set; }

        public int DeliveryInstall { get; set; }

        public int DeliveryHaulway { get; set; }

        public int DeliveryHaulwayInstall { get; set; }

        public bool HasDeliveryOptions()
        {
            return FedEx != 0 ||
                   DeliveryOnly != 0 ||
                   DeliveryInstall != 0 ||
                   DeliveryHaulway != 0 ||
                   DeliveryHaulwayInstall != 0;
        }
    }
}