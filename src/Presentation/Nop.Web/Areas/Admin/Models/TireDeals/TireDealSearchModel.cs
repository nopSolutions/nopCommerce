using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.TireDeals;

public record TireDealSearchModel : BaseSearchModel
{
    public TireDealSearchModel()
    {
        AvailableActiveOptions = new List<SelectListItem>();
    }
    
    [NopResourceDisplayName("Admin.Promotions.TireDeals.Search.Title")]
    public string SearchTireDealTitle { get; set; }

    [NopResourceDisplayName("Admin.Promotions.TireDeals.Search.Id")]
    public string SearchTireDealId { get; set; }

    public string SearchTireDealIsActive { get; set; }

    [NopResourceDisplayName("Admin.Promotions.TireDeals.Search.IsActive")]

    public IList<SelectListItem> AvailableActiveOptions { get; set; }
}