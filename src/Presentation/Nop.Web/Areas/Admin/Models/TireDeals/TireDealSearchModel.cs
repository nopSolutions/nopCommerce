using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.TireDeals;

public record TireDealSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Plugins.TireDeal.List.Title")]
    public string SearchTireDealTitle { get; set; }
}