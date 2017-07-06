using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class BulkEditProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.SKU")]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod")]
        public string ManageInventoryMethod { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Published")]
        public bool Published { get; set; }
    }
}