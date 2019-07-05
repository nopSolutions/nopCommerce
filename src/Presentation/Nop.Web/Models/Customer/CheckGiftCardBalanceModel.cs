using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Models.Customer
{
    public partial class CheckGiftCardBalanceModel : BaseNopModel
    {
        public string Result { get; set; }

        public string Message { get; set; }
        
        [NopResourceDisplayName("ShoppingCart.GiftCardCouponCode.Tooltip")]
        public string GiftCardCode { get; set; }
    }
}
