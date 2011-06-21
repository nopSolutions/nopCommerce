using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Nop.Services.Discounts;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Services.Tests.Discounts
{
    public partial class TestDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        public override PluginDescriptor PluginDescriptor
        {
            get
            {
                return new PluginDescriptor()
                {
                    Author = "nopCommerce team",
                    FriendlyName = "Test discount requirement rule",
                    SystemName = "TestDiscountRequirementRule",
                    Version = "1.00"
                };
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            return true;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            throw new NotImplementedException();
        }
    }
}
