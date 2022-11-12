<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record BackInStockSubscribeModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }

        public bool IsCurrentCustomerRegistered { get; set; }
        public bool SubscriptionAllowed { get; set; }
        public bool AlreadySubscribed { get; set; }

        public int MaximumBackInStockSubscriptions { get; set; }
        public int CurrentNumberOfBackInStockSubscriptions { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record BackInStockSubscribeModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }

        public bool IsCurrentCustomerRegistered { get; set; }
        public bool SubscriptionAllowed { get; set; }
        public bool AlreadySubscribed { get; set; }

        public int MaximumBackInStockSubscriptions { get; set; }
        public int CurrentNumberOfBackInStockSubscriptions { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}