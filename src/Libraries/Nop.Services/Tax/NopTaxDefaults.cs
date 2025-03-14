namespace Nop.Services.Tax;

/// <summary>
/// Represents default values related to tax services
/// </summary>
public static partial class NopTaxDefaults
{
    /// <summary>
    /// Gets the URL for validate UK VAT number
    /// </summary>
    public static string HmrcVatValidateUrl => "/organisations/vat/check-vat-number/lookup/{0}";

    /// <summary>
    /// Gets the URL for validate UK VAT number
    /// </summary>
    public static string HmrcOauthTokenUrl => "/oauth/token";
}