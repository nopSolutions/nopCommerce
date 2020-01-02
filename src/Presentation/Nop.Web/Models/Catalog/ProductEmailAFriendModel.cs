using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductEmailAFriendModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [NopResourceDisplayName("Products.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [NopResourceDisplayName("Products.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}