using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Deals.Models;

public record TireDealSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Plugins.TireDeal.List.Title")]
    public string SearchTireDealTitle { get; set; }

    [NopResourceDisplayName("Admin.Plugins.TireDeal.List.ShortDescription")]
    public string SearchTireDealShortDescription { get; set; }

    [NopResourceDisplayName("Admin.Plugins.TireDeal.List.LongDescription")]
    public string SearchTireDealLongDescription { get; set; }
    [NopResourceDisplayName("Admin.Plugins.TireDeal.List.IsActive")]
    public bool? SearchTireDealIsActive { get; set; }
}