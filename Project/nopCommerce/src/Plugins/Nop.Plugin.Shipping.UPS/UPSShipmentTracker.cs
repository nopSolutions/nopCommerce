using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS;

/// <summary>
/// Represents the USP shipment tracker
/// </summary>
public class UPSShipmentTracker : IShipmentTracker
{
    #region Fields

    private readonly UPSService _upsService;

    #endregion

    #region Ctor

    public UPSShipmentTracker(UPSService upsService)
    {
        _upsService = upsService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get URL for a page to show tracking info (third party tracking page)
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL of a tracking page
    /// </returns>
    public Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
    {
        return Task.FromResult($"https://www.ups.com/track?&tracknum={trackingNumber}");
    }

    /// <summary>
    /// Get all shipment events
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of shipment events
    /// </returns>
    public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
    {
        var result = new List<ShipmentStatusEvent>();

        if (string.IsNullOrEmpty(trackingNumber))
            return result;

        result.AddRange(await _upsService.GetShipmentEventsAsync(trackingNumber));

        return result;
    }

    #endregion
}