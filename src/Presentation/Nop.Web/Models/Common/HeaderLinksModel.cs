using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerName { get; set; }
        
        public bool ShoppingCartEnabled { get; set; }
        public int ShoppingCartItems { get; set; }
        
        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}