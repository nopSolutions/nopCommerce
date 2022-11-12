<<<<<<< HEAD
﻿using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record HeaderLinksModel : BaseNopModel
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
        public UserRegistrationType RegistrationType { get; set; }
    }
=======
﻿using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record HeaderLinksModel : BaseNopModel
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
        public UserRegistrationType RegistrationType { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}