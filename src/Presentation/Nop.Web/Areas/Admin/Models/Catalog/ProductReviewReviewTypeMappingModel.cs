using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping model
    /// </summary>
    public partial record ProductReviewReviewTypeMappingModel : BaseNopEntityModel
    {
        #region Properties

        public int ProductReviewId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.VisibleToAllCustomers")]
        public bool VisibleToAllCustomers { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Rating")]
        public int Rating { get; set; }

        #endregion
    }
}
