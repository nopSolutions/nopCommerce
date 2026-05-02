namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public enum ActivityEnum
{
    /// <summary>
    /// Full HS code classification (country specific). This is the default activity if no value is specified in the request.
    /// </summary>
    HS_FULL,

    /// <summary>
    /// 6 digits HS code classification
    /// </summary>
    HS6
}