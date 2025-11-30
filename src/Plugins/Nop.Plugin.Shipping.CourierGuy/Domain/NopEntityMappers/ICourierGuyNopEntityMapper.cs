using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.CourierGuy.Domain.NopEntityMappers;

public interface ICourierGuyNopEntityMapper
{
    Task<List<CourierGuyRateRequest>> NopCourierGuyRateRequest(GetShippingOptionRequest getShippingOptionRequest);
    Task<GetShippingOptionResponse> MapToShippingOptionResponse(List<CourierGuyRateResponse> responseContent);
}