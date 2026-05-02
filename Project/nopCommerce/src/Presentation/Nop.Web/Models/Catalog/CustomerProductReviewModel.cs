using Nop.Web.Framework.Models;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Catalog;

public partial record CustomerProductReviewModel : BaseNopModel
{
    public CustomerProductReviewModel()
    {
        AdditionalProductReviewList = new List<ProductReviewReviewTypeMappingModel>();
    }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductSeName { get; set; }
    public string Title { get; set; }
    public string ReviewText { get; set; }
    public string ReplyText { get; set; }
    public int Rating { get; set; }
    public string WrittenOnStr { get; set; }
    public string ApprovalStatus { get; set; }
    public IList<ProductReviewReviewTypeMappingModel> AdditionalProductReviewList { get; set; }
}

public partial record CustomerProductReviewsModel : BaseNopModel
{
    public CustomerProductReviewsModel()
    {
        ProductReviews = new List<CustomerProductReviewModel>();
    }

    public IList<CustomerProductReviewModel> ProductReviews { get; set; }
    public PagerModel PagerModel { get; set; }

    #region Nested class

    /// <summary>
    /// record that has only page for route value. Used for (My Account) My Product Reviews pagination
    /// </summary>
    public partial record CustomerProductReviewsRouteValues : IRouteValues
    {
        public int PageNumber { get; set; }
    }

    #endregion
}