using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer
{
    public partial record SubscriptionModel
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionDate { get; set; }
        public string SubscriptionProduct { get; set; }

        public int AllottedCreditCount { get; set; }
        public int UsedCreditCount { get; set; }
        public int BalanceCreditCount { get; set; }
        public string OrderItem { get; set; }
        public bool IsPaidCustomer { get; set; }

    }
}
