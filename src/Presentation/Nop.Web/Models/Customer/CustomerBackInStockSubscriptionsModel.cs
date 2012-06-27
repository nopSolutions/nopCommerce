using System.Collections.Generic;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerBackInStockSubscriptionsModel
    {
        public CustomerBackInStockSubscriptionsModel()
        {
            this.Subscriptions = new List<BackInStockSubscriptionModel>();
        }

        public IList<BackInStockSubscriptionModel> Subscriptions { get; set; }
        public CustomerNavigationModel NavigationModel { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}