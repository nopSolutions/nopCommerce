//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;



namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Represents a discount
    /// </summary>
    public partial class Discount : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the Discount class
        /// </summary>
        public Discount()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the discount type identifier
        /// </summary>
        public int DiscountTypeId { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement identifier
        /// </summary>
        public int DiscountRequirementId { get; set; }

        /// <summary>
        /// Gets or sets the the discount requirement spent amount - customer has spent/purchased x.xx amount (used when requirement is set to "Customer had spent/purchased x.xx amount")
        /// </summary>
        public decimal RequirementSpentAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement - customer's billing country is... (used when requirement is set to "Billing country is")
        /// </summary>
        public int RequirementBillingCountryIs { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement - customer's shipping country is... (used when requirement is set to "Shipping country is")
        /// </summary>
        public int RequirementShippingCountryIs { get; set; }
        
        /// <summary>
        /// Gets or sets the discount limitation identifier
        /// </summary>
        public int DiscountLimitationId { get; set; }

        /// <summary>
        /// Gets or sets the discount limitation times (used when Limitation is set to "N Times Only" or "N Times Per Customer")
        /// </summary>
        public int LimitationTimes { get; set; }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use percentage
        /// </summary>
        public bool UsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount percentage
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount start date and time
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the discount end date and time
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discount requires coupon code
        /// </summary>
        public bool RequiresCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the coupon code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets or sets the discount type
        /// </summary>
        public DiscountTypeEnum DiscountType
        {
            get
            {
                return (DiscountTypeEnum)this.DiscountTypeId;
            }
            set
            {
                this.DiscountTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the discount requirement
        /// </summary>
        public DiscountRequirementEnum DiscountRequirement
        {
            get
            {
                return (DiscountRequirementEnum)this.DiscountRequirementId;
            }
            set
            {
                this.DiscountRequirementId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the discount limitation
        /// </summary>
        public DiscountLimitationEnum DiscountLimitation
        {
            get
            {
                return (DiscountLimitationEnum)this.DiscountLimitationId;
            }
            set
            {
                this.DiscountLimitationId = (int)value;
            }
        }

        /// <summary>
        /// Gets the customer role assigned to discount
        /// </summary>
        public List<CustomerRole> CustomerRoles
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerRolesByDiscountId(this.DiscountId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the categories
        /// </summary>
        public virtual ICollection<Category> NpCategories { get; set; }

        /// <summary>
        /// Gets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> NpCustomerRoles { get; set; }

        /// <summary>
        /// Gets the restricted product variants
        /// </summary>
        public virtual ICollection<ProductVariant> NpRestrictedProductVariants { get; set; }

        /// <summary>
        /// Gets the product variants
        /// </summary>
        public virtual ICollection<ProductVariant> NpProductVariants { get; set; }

        /// <summary>
        /// Gets the discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> NpDiscountUsageHistory { get; set; }

        #endregion
    }
}
