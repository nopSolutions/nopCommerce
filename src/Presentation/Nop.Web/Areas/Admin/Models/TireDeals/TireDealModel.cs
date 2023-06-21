using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.TireDeals;

public record TireDealModel : BaseNopEntityModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; }
    public int ActiveStoreScopeConfiguration { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
    [UIHint("Picture")]
    public int BackgroundPictureId { get; set; }
    public bool BackgroundPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
    [UIHint("Picture")]
    public int BrandPictureId { get; set; }
    public bool BrandPictureId_OverrideForStore { get; set; }
}