using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Deals.Models;

public record PublicInfoModel : BaseNopModel
{
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public bool IsActive { get; set; }
    public string ShortDescription { get; set; }
    public string BackgroundPictureUrl { get; set; }
    public string BrandPictureUrl { get; set; }
}