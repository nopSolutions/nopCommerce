using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.TireDeals;

public record TireDealModel : BaseNopEntityModel
{
    public int Id { get; set; }

    [NopResourceDisplayName("Admin.Promotions.TireDeals.Edit.Title")]
    public string Title { get; set; }
    [NopResourceDisplayName("Admin.Promotions.TireDeals.Edit.LongDescription")]
    public string LongDescription { get; set; }
    [NopResourceDisplayName("Admin.Promotions.TireDeals.Edit.ShortDescription")]
    public string ShortDescription { get; set; }
    [NopResourceDisplayName("Admin.Promotions.TireDeals.Edit.IsActive")]
    public bool IsActive { get; set; }
    public int ActiveStoreScopeConfiguration { get; set; }
    [NopResourceDisplayName("Admin.Promotions.TireDeals.Edit.Picture")]
    [UIHint("Picture")]
    public int BackgroundPictureId { get; set; }
    public string BackgroundPictureUrl { get; set; }
    public bool BackgroundPictureId_OverrideForStore { get; set; }
    
}