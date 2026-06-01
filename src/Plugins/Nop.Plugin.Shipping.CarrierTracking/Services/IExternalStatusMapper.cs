using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public interface IExternalStatusMapper
{
    ShippingStatus? MapToInternal(string externalStatus);
}

public class ExternalStatusMapper : IExternalStatusMapper
{
    public ShippingStatus? MapToInternal(string externalStatus) =>
        externalStatus switch
        {
            "DELIVERED" => ShippingStatus.Delivered,
            "IN_TRANSIT" or "OUT_FOR_DELIVERY" or "PICKED_UP" => ShippingStatus.Shipped,
            _ => null
        };
}
