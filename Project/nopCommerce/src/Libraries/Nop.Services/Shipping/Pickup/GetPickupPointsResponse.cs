using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Pickup;

/// <summary>
/// Represents a response of getting pickup points
/// </summary>
public partial class GetPickupPointsResponse : BaseNopResult
{
    public GetPickupPointsResponse()
    {
        PickupPoints = new List<PickupPoint>();
    }

    /// <summary>
    /// Gets or sets a list of pickup points
    /// </summary>
    public IList<PickupPoint> PickupPoints { get; set; }
}