using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer product model
    /// </summary>
    public partial class ManufacturerProductModel : BaseNopEntityModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.Product")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.IsFeaturedProduct")]
        public bool IsFeaturedProduct { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}