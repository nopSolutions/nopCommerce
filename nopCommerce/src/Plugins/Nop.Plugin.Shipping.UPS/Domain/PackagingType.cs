namespace Nop.Plugin.Shipping.UPS.Domain;

/// <summary>
/// Represents packaging type
/// </summary>
/// <remarks>
/// Updated at January 7, 2019
/// </remarks>
public enum PackagingType
{
    /// <summary>
    /// Unknown
    /// </summary>
    [UPSCode("00")]
    Unknown,

    /// <summary>
    /// UPS Letter
    /// </summary>
    [UPSCode("01")]
    Letter,

    /// <summary>
    /// Customer supplied package
    /// </summary>
    [UPSCode("02")]
    CustomerSuppliedPackage,

    /// <summary>
    /// Tube
    /// </summary>
    [UPSCode("03")]
    Tube,

    /// <summary>
    /// PAK
    /// </summary>
    [UPSCode("04")]
    PAK,

    /// <summary>
    /// Express Box
    /// </summary>
    [UPSCode("21")]
    ExpressBox,

    /// <summary>
    /// 25 Kg Box
    /// </summary>
    [UPSCode("24")]
    _25KgBox,

    /// <summary>
    /// 10 Kg Box
    /// </summary>
    [UPSCode("25")]
    _10KgBox,

    /// <summary>
    /// Pallet
    /// </summary>
    [UPSCode("30")]
    Pallet,

    /// <summary>
    /// Small Express Box
    /// </summary>
    [UPSCode("2a")]
    SmallExpressBox,

    /// <summary>
    /// Medium Express Box
    /// </summary>
    [UPSCode("2b")]
    MediumExpressBox,

    /// <summary>
    /// Large Express Box
    /// </summary>
    [UPSCode("2c")]
    LargeExpressBox

}