using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record VendorProductReviewModel : BaseNopModel
{
    public int CustomerId { get; set; }
    public string CustomerAvatarUrl { get; set; }
    public string CustomerName { get; set; }
    public string ProductSeName { get; set; }
    public string ProductName { get; set; }
    public string Title { get; set; }
    public string ReviewText { get; set; }
    public string ReplyText { get; set; }
    public int Rating { get; set; }
    public int HelpfulYesTotal { get; set; }
    public int HelpfulNoTotal { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public ProductReviewHelpfulnessModel Helpfulness { get; set; }
    public bool AllowViewingProfiles { get; set; }
}
