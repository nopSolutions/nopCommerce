using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.ShoppingCart
{
    public class ShoppingCartItemModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Product")]
        public int ProductVariantId { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Product")]
        public string FullProductName { get; set; }

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