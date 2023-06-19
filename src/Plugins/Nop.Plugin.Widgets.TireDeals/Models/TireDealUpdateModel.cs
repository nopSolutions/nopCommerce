namespace Nop.Plugin.Widgets.Deals.Models;

public record TireDealUpdateModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; }
    public int BrandPictureId { get; set; }
    public int BackgroundPictureId { get; set; }
}