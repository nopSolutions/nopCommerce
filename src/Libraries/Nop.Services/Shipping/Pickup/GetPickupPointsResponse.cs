﻿using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Pickup;

/// <summary>
/// Represents a response of getting pickup points
/// </summary>
public partial class GetPickupPointsResponse
{
    public GetPickupPointsResponse()
    {
        Errors = new List<string>();
        PickupPoints = new List<PickupPoint>();
    }

    /// <summary>
    /// Gets or sets a list of pickup points
    /// </summary>
    public IList<PickupPoint> PickupPoints { get; set; }

    /// <summary>
    /// Gets or sets errors
    /// </summary>
    public IList<string> Errors { get; set; }

    /// <summary>
    /// Gets a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

    /// <summary>
    /// Add error
    /// </summary>
    /// <param name="error">Error</param>
    public virtual void AddError(string error)
    {
        Errors.Add(error);
    }
}