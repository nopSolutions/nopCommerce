using FluentValidation.Attributes;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Customer;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(GiftCardValidator))]
    public partial class CheckGiftCardBalanceModel : BaseNopModel
    {
        public string Result { get; set; }

        public string Message { get; set; }
        
        [NopResourceDisplayName("ShoppingCart.GiftCardCouponCode.Tooltip")]
        public string GiftCardCode { get; set; }
    }
}
