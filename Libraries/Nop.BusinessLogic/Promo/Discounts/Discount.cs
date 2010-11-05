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
using NopSolutions.NopCommerce.BusinessLogic.IoC;



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

        #region Methods

        /// <summary>
        /// Gets a value indicating whether the discount is active now
        /// </summary>
        /// <returns>A value indicating whether the discount is active now</returns>
        public bool IsActive()
        {
            return IsActive(string.Empty);
        }

        /// <summary>
        /// Gets a value indicating whether the discount is active now
        /// </summary>
        /// <param name="couponCodeToValidate">Coupon code to validate</param>
        /// <returns>A value indicating whether the discount is active now</returns>
        public bool IsActive(string couponCodeToValidate)
        {
            if (this.RequiresCouponCode && !String.IsNullOrEmpty(this.CouponCode))
            {
                if (!this.CouponCode.Equals(couponCodeToValidate,StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            DateTime _now = DateTime.UtcNow;
            DateTime _startDate = DateTime.SpecifyKind(this.StartDate, DateTimeKind.Utc);
            DateTime _endDate = DateTime.SpecifyKind(this.EndDate, DateTimeKind.Utc);
            bool isActive = (!this.Deleted) && 
                (_startDate.CompareTo(_now) < 0) && 
                (_endDate.CompareTo(_now) > 0);
            return isActive;
        }
        
        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>The discount amount</returns>
        public decimal GetDiscountAmount(decimal price)
        {
            decimal result = decimal.Zero;
            if (UsePercentage)
                result = Math.Round((decimal)((((float)price) * ((float)this.DiscountPercentage)) / 100f), 2);
            else
                result = Math.Round(this.DiscountAmount, 2);

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Checks requirements for customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Value indicating whether all requirements are met</returns>
        public bool CheckDiscountRequirements(Customer customer)
        {
            switch (this.DiscountRequirement)
            {
                case DiscountRequirementEnum.None:
                    {
                        return true;
                    }
                    break;
                case DiscountRequirementEnum.MustBeAssignedToCustomerRole:
                    {
                        if (customer != null)
                        {
                            var customerRoles = customer.CustomerRoles;
                            var assignedRoles = this.CustomerRoles;
                            foreach (CustomerRole _customerRole in customerRoles)
                                foreach (CustomerRole _assignedRole in assignedRoles)
                                {
                                    if (_customerRole.Name == _assignedRole.Name)
                                    {
                                        return true;
                                    }
                                }
                        }
                    }
                    break;
                case DiscountRequirementEnum.MustBeRegistered:
                    {
                        bool result = customer != null && !customer.IsGuest;
                        return result;
                    }
                    break;
                case DiscountRequirementEnum.HasAllOfTheseProductVariantsInTheCart:
                    {
                        if (customer != null)
                        {
                            CustomerSession customerSession = IoCFactory.Resolve<ICustomerService>().GetCustomerSessionByCustomerId(customer.CustomerId);
                            if (customerSession != null)
                            {
                                var restrictedProductVariants = IoCFactory.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(this.DiscountId);
                                var cart = IoCFactory.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.ShoppingCart, customerSession.CustomerSessionGuid);

                                bool allFound = true;
                                foreach (ProductVariant restrictedPV in restrictedProductVariants)
                                {
                                    bool found1 = false;
                                    foreach (ShoppingCartItem sci in cart)
                                    {
                                        if (restrictedPV.ProductVariantId == sci.ProductVariantId)
                                        {
                                            found1 = true;
                                            break;
                                        }
                                    }

                                    if (!found1)
                                    {
                                        allFound = false;
                                        break;
                                    }
                                }

                                if (allFound)
                                    return true;

                            }
                        }
                    }
                    break;
                case DiscountRequirementEnum.HasOneOfTheseProductVariantsInTheCart:
                    {
                        if (customer != null)
                        {
                            CustomerSession customerSession = IoCFactory.Resolve<ICustomerService>().GetCustomerSessionByCustomerId(customer.CustomerId);
                            if (customerSession != null)
                            {
                                var restrictedProductVariants = IoCFactory.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(this.DiscountId);
                                var cart = IoCFactory.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.ShoppingCart, customerSession.CustomerSessionGuid);

                                bool found = false;
                                foreach (ProductVariant restrictedPV in restrictedProductVariants)
                                {
                                    foreach (ShoppingCartItem sci in cart)
                                    {
                                        if (restrictedPV.ProductVariantId == sci.ProductVariantId)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (found)
                                    {
                                        break;
                                    }
                                }

                                if (found)
                                    return true;
                            }
                        }
                    }
                    break;
                case DiscountRequirementEnum.HadPurchasedAllOfTheseProductVariants:
                    {
                        if (customer != null)
                        {
                            var restrictedProductVariants = IoCFactory.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(this.DiscountId);
                            var purchasedProductVariants = IoCFactory.Resolve<IOrderService>().GetAllOrderProductVariants(null, customer.CustomerId, null, null, OrderStatusEnum.Complete, null, null);

                            bool allFound = true;
                            foreach (ProductVariant restrictedPV in restrictedProductVariants)
                            {
                                bool found1 = false;
                                foreach (OrderProductVariant purchasedPV in purchasedProductVariants)
                                {
                                    if (restrictedPV.ProductVariantId == purchasedPV.ProductVariantId)
                                    {
                                        found1 = true;
                                        break;
                                    }
                                }

                                if (!found1)
                                {
                                    allFound = false;
                                    break;
                                }
                            }

                            if (allFound)
                                return true;
                        }
                    }
                    break;
                case DiscountRequirementEnum.HadPurchasedOneOfTheseProductVariants:
                    {
                        if (customer != null)
                        {
                            var restrictedProductVariants = IoCFactory.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(this.DiscountId);
                            var purchasedProductVariants = IoCFactory.Resolve<IOrderService>().GetAllOrderProductVariants(null, customer.CustomerId, null, null, OrderStatusEnum.Complete, null, null);

                            bool found = false;
                            foreach (ProductVariant restrictedPV in restrictedProductVariants)
                            {
                                foreach (OrderProductVariant purchasedPV in purchasedProductVariants)
                                {
                                    if (restrictedPV.ProductVariantId == purchasedPV.ProductVariantId)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (found)
                                {
                                    break;
                                }
                            }

                            if (found)
                                return true;
                        }
                    }
                    break;
                case DiscountRequirementEnum.HadSpentAmount:
                    {
                        if (customer != null)
                        {
                            var orders = customer.Orders.FindAll(o=>o.OrderStatus == OrderStatusEnum.Complete);
                            decimal spentAmount = orders.Sum(o=>o.OrderTotal);
                            return spentAmount > this.RequirementSpentAmount;
                        }
                    }
                    break;
                case DiscountRequirementEnum.BillingCountryIs:
                    {
                        if (customer != null && 
                            customer.BillingAddress!=null &&
                            customer.BillingAddress.CountryId > 0 &&
                            this.RequirementBillingCountryIs > 0)
                        {
                            return customer.BillingAddress.CountryId == this.RequirementBillingCountryIs;
                        }
                    }
                    break;
                case DiscountRequirementEnum.ShippingCountryIs:
                    {
                        if (customer != null &&
                            customer.ShippingAddress != null &&
                            customer.ShippingAddress.CountryId > 0 &&
                            this.RequirementShippingCountryIs > 0)
                        {
                            return customer.ShippingAddress.CountryId == this.RequirementShippingCountryIs;
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// Checks discount limitations for customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Value indicating whether discount can be used</returns>
        public bool CheckDiscountLimitations(Customer customer)
        {
            switch (this.DiscountLimitation)
            {
                case DiscountLimitationEnum.Unlimited:
                    {
                        return true;
                    }
                case DiscountLimitationEnum.OneTimeOnly:
                    {
                        var usageHistory = IoCFactory.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(this.DiscountId, null, null);
                        return usageHistory.Count < 1;
                    }
                case DiscountLimitationEnum.NTimesOnly:
                    {
                        var usageHistory = IoCFactory.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(this.DiscountId, null, null);
                        return usageHistory.Count < this.LimitationTimes;
                    }
                case DiscountLimitationEnum.OneTimePerCustomer:
                    {
                        if (customer != null)
                        {
                            var usageHistory = IoCFactory.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(this.DiscountId, customer.CustomerId, null);
                            return usageHistory.Count < 1;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case DiscountLimitationEnum.NTimesPerCustomer:
                    {
                        if (customer != null)
                        {
                            var usageHistory = IoCFactory.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(this.DiscountId, customer.CustomerId, null);
                            return usageHistory.Count < this.LimitationTimes;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    break;
            }
            return false;
        }
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
                return IoCFactory.Resolve<ICustomerService>().GetCustomerRolesByDiscountId(this.DiscountId);
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
