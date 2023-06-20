namespace Nop.Core.Domain.TireDeals;

public class TireDeal : BaseEntity
{
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; }
    public int BrandPictureId { get; set; }
    public int BackgroundPictureId { get; set; }
}