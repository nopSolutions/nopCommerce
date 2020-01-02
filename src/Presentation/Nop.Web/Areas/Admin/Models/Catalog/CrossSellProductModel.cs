using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell product model
    /// </summary>
    public partial class CrossSellProductModel : BaseNopEntityModel
    {
        #region Properties

        public int ProductId2 { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.CrossSells.Fields.Product")]
        public string Product2Name { get; set; }

        #endregion
    }
}