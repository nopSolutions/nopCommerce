using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart
{
    /// <summary>
    /// Represents a shopping cart search model
    /// </summary>
    public partial class ShoppingCartSearchModel : BaseSearchModel
    {
        #region Ctor

        public ShoppingCartSearchModel()
        {
            AvailableShoppingCartTypes = new List<SelectListItem>();
            ShoppingCartItemSearchModel = new ShoppingCartItemSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
        public ShoppingCartType ShoppingCartType { get; set; }

        public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }

        public ShoppingCartItemSearchModel ShoppingCartItemSearchModel { get; set; }

        #endregion
    }
}