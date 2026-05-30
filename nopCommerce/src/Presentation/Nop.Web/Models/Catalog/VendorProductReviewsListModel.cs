using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record VendorProductReviewsListModel : BaseNopModel
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string VendorUrl { get; set; }
    public VendorReviewsPagingFilteringModel PagingFilteringContext { get; set; } = new();
    public List<VendorProductReviewModel> Reviews { get; set; } = new();
}
