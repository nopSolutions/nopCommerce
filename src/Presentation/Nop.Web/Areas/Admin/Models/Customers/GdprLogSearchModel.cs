using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a GDPR log search model
/// </summary>
public partial record GdprLogSearchModel : BaseSearchModel
{
    #region Ctor

    public GdprLogSearchModel()
    {
        AvailableRequestTypes = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchEmail")]
    [DataType(DataType.EmailAddress)]
    public string SearchEmail { get; set; }

    [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchRequestType")]
    public int SearchRequestTypeId { get; set; }

    public IList<SelectListItem> AvailableRequestTypes { get; set; }

    #endregion
}