using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate commission list model
/// </summary>
public partial record AffiliateCommissionListModel : BasePagedListModel<AffiliateCommissionModel>;