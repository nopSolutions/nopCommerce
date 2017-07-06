using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart
{
    public partial class ShoppingCartItemModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Store")]
        public string Store { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Product")]
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Product")]
        public string ProductName { get; set; }
        public string AttributeInfo { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.UnitPrice")]
        public string UnitPrice { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Quantity")]
        public int Quantity { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Total")]
        public string Total { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}