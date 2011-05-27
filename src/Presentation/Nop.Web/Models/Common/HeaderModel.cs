using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class HeaderModel : BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerEmailUsername { get; set; }

        public bool DisplayAdminLink { get; set; }


        public int ShoppingCartItems { get; set; }
        
        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }
    }
}