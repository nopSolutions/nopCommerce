using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.ShoppingCart;

namespace Nop.Web.Models.ShoppingCart
{
    [Validator(typeof(WishlistEmailAFriendValidator))]
    public partial class WishlistEmailAFriendModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Wishlist.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Wishlist.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Wishlist.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}