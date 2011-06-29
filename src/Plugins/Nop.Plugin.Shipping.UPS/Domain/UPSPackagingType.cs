
namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// UPSP packaging type
    /// </summary>
    public enum UPSPackagingType 
    {
        /// <summary>
        /// Customer supplied package
        /// </summary>
        CustomerSuppliedPackage,
        /// <summary>
        /// Letter
        /// </summary>
        Letter,
        /// <summary>
        /// Tube
        /// </summary>
        Tube,
        /// <summary>
        /// PAK
        /// </summary>
        PAK,
        /// <summary>
        /// ExpressBox
        /// </summary>
        ExpressBox,
        /// <summary>
        /// _10 Kg Box
        /// </summary>
        _10KgBox,
        /// <summary>
        /// _25 Kg Box
        /// </summary>
        _25KgBox
    }
}
