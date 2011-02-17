
namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents a shipping rate computation method type
    /// </summary>
    public enum ShippingRateComputationMethodType : int
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Offline
        /// </summary>
        Offline = 10,
        /// <summary>
        /// Realtime
        /// </summary>
        Realtime = 20
    }
}
