using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Deals.Models;

public record TireDealModel : BaseNopModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; }
    
    public int ActiveStoreScopeConfiguration { get; set; }
    [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
    [UIHint("Picture")]
    public int PictureId { get; set; }
    public bool PictureId_OverrideForStore { get; set; }
}