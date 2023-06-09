using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record WishlistEmailAFriendModel : BaseNopModel
    {
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Wishlist.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Wishlist.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [NopResourceDisplayName("Wishlist.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}