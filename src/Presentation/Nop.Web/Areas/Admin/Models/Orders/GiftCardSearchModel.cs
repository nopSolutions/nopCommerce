using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card search model
    /// </summary>
    public partial record GiftCardSearchModel : BaseSearchModel
    {
        #region Ctor

        public GiftCardSearchModel()
        {
            ActivatedList = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.GiftCards.List.CouponCode")]
        public string CouponCode { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.List.RecipientName")]
        public string RecipientName { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.List.Activated")]
        public int ActivatedId { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.List.Activated")]
        public IList<SelectListItem> ActivatedList { get; set; }

        #endregion
    }
}