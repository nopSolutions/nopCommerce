using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review list model
    /// </summary>
    public partial record ProductReviewListModel : BasePagedListModel<ProductReviewModel>
    {
    }
}