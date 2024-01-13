namespace Nop.Services.Affiliates;

/// <summary>
/// Represents default values related to affiliate services
/// </summary>
public static partial class NopAffiliateDefaults
{
    /// <summary>
    /// Gets a query parameter name to add affiliate friendly name to URL
    /// </summary>
    public static string AffiliateQueryParameter => "affiliate";

    /// <summary>
    /// Gets a query parameter name to add affiliate identifier to URL
    /// </summary>
    public static string AffiliateIdQueryParameter => "affiliateid";

    /// <summary>
    /// Gets a max length of affiliate friendly name
    /// </summary>
    /// <remarks>For long URLs we can get the following error: 
    /// "the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters", 
    /// that's why we limit it to 200</remarks>
    public static int FriendlyUrlNameLength => 200;
}