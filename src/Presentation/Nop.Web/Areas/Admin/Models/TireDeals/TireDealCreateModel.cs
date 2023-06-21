namespace Nop.Web.Areas.Admin.Models.TireDeals;

public class TireDealCreateModel
{
    public int  Id { get; set; }
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; } = true;
    public int BrandPictureId { get; set; }
    public int BackgroundPictureId { get; set; }
}