namespace Nop.Plugin.Misc.PunchOut.Domain;

/// <summary>
/// Represents a PunchOut message type enumeration
/// </summary>
public enum PunchOutMessageType
{
    /// <summary>
    /// PunchOutSetupRequest
    /// </summary>
    SetupRequest = 1,

    /// <summary>
    /// PunchOutSetupResponse
    /// </summary>
    SetupResponse = 2,

    /// <summary>
    /// PunchOutOrderMessage
    /// </summary>
    OrderMessage = 3
}
