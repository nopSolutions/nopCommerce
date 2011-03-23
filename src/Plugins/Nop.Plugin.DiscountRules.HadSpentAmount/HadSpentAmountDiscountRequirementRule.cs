using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Customers;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.HadSpentAmount
{
    public partial class HadSpentAmountDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return "Customer had spent x.xx amount";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get
            {
                return "DiscountRequirement.HadSpentAmount";
            }
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirement == null)
                throw new NopException("Discount requirement is not set");
            
            if (request.DiscountRequirement.SpentAmount == decimal.Zero)
                return true;

            if (request.Customer == null || request.Customer.IsGuest())
                return false;

            var orders = request.Customer.Orders.Where(o => !o.Deleted && o.OrderStatus == OrderStatus.Complete);
            decimal spentAmount = orders.Sum(o => o.OrderTotal);
            return spentAmount > request.DiscountRequirement.SpentAmount;
        }
    }
}