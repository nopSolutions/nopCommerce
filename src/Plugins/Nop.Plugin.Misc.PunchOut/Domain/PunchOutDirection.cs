namespace Nop.Plugin.Misc.PunchOut.Domain;

/// <summary>
/// Represents a PunchOut message direction type enumeration
/// </summary>
public enum PunchOutDirection
{
    /// <summary>
    /// ERP to nopCommerce
    /// </summary>
    Inbound = 1,

    /// <summary>
    /// nopCommerce to ERP
    /// </summary>
    Outbound = 2
}
