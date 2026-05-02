using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliated customer search model
/// </summary>
public partial record AffiliatedCustomerSearchModel : BaseSearchModel
{
    #region Properties

    public int AffliateId { get; set; }

    #endregion
}