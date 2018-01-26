using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart
{
    public partial class ShoppingCartModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }
    }

    public partial class ShoppingCartTypeModel : BaseNopModel
    {
        public ShoppingCartTypeModel()
        {
            AvailableShoppingCartTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
        public ShoppingCartType ShoppingCartType { get; set; }
        public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }
    }
}