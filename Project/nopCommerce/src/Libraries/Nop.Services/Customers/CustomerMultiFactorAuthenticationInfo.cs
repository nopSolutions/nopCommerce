namespace Nop.Services.Customers;

/// <summary>
/// Customer multi-factor authentication info
/// </summary>
public partial class CustomerMultiFactorAuthenticationInfo
{
    public CustomerMultiFactorAuthenticationInfo()
    {
        CustomValues = new Dictionary<string, object>();
    }
    public string UserName { get; set; }

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; }

    /// <summary>
    /// You can store any custom value in this property
    /// </summary>
    public Dictionary<string, object> CustomValues { get; set; }
}