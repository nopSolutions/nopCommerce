namespace Nop.Plugin.Widgets.Deals.Models;

public class TireDealCreateModel
{
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; } = true;
    public int BrandPictureId { get; set; }
    public int BackgroundPictureId { get; set; }
}