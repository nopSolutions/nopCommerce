using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a request quote search model
/// </summary>
public record RequestQuoteSearchModel : BaseSearchModel
{
    #region Ctor

    public RequestQuoteSearchModel()
    {
        AvailableRequestQuoteStatuses = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerEmail")]
    public string CustomerEmail { get; set; }

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.AdminRequests.RequestQuoteStatus")]
    public int RequestQuoteStatus { get; set; }
    public List<SelectListItem> AvailableRequestQuoteStatuses { get; set; }

    #endregion
}
