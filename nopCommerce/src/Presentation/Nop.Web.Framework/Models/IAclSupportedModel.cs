using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents a model which supports access control list (ACL)
/// </summary>
public partial interface IAclSupportedModel
{
    #region Properties

    /// <summary>
    /// Gets or sets identifiers of the selected customer roles
    /// </summary>
    [NopResourceDisplayName("Admin.IAclSupportedModel.Fields.AclCustomerRoles")]
    IList<int> SelectedCustomerRoleIds { get; set; }

    /// <summary>
    /// Gets or sets items for the all available customer roles
    /// </summary>
    IList<SelectListItem> AvailableCustomerRoles { get; set; }

    #endregion
}