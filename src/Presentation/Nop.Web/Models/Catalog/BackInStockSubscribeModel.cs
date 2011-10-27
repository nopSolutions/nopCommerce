using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class BackInStockSubscribeModel : BaseNopModel
    {
        public bool IsCurrentCustomerRegistered { get; set; }
        public bool SubscriptionAllowed { get; set; }
        public bool AlreadySubscribed { get; set; }

        public int MaximumBackInStockSubscriptions { get; set; }
        public int CurrentNumberOfBackInStockSubscriptions { get; set; }
    }
}