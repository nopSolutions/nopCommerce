using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;

/// <summary>
/// Represents GoogleAuthenticator search model
/// </summary>
public record GoogleAuthenticatorSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
    public string SearchEmail { get; set; }

    public bool HideSearchBlock { get; set; }
}