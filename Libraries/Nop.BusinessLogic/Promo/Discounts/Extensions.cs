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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the collection contains the specified discount.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="discountName">The discount name</param>
        /// <returns>true if the collection contains a discount with the specified name; otherwise, false.</returns>
        public static bool ContainsDiscount(this List<Discount> source,
            string discountName)
        {
            bool result = false;
            foreach (var discount in source)
                if (discount.Name == discountName)
                {
                    result = true;
                    break;
                }
            return result;
        }

        /// <summary>
        /// Finds the specified discount.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="couponCode">The coupon code</param>
        /// <returns>Found discount</returns>
        public static Discount FindByCouponCode(this List<Discount> source,
            string couponCode)
        {
            if (String.IsNullOrEmpty(couponCode))
                return null;

            foreach (var discount in source)
                if (discount.CouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    return discount;

            return null;
        }


        /// <summary>
        /// Gets a value indicating whether the discount is active now
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>A value indicating whether the discount is active now</returns>
        public static bool IsActive(this Discount discount)
        {
            return IsActive(discount, string.Empty);
        }

        /// <summary>
        /// Gets a value indicating whether the discount is active now
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="couponCodeToValidate">Coupon code to validate</param>
        /// <returns>A value indicating whether the discount is active now</returns>
        public static bool IsActive(this Discount discount, string couponCodeToValidate)
        {
            if (discount.RequiresCouponCode && !String.IsNullOrEmpty(discount.CouponCode))
            {
                if (!discount.CouponCode.Equals(couponCodeToValidate, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            DateTime now = DateTime.UtcNow;
            DateTime startDate = DateTime.SpecifyKind(discount.StartDate, DateTimeKind.Utc);
            DateTime endDate = DateTime.SpecifyKind(discount.EndDate, DateTimeKind.Utc);
            bool isActive = (!discount.Deleted) &&
                (startDate.CompareTo(now) < 0) &&
                (endDate.CompareTo(now) > 0);
            return isActive;
        }

        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="price">Price</param>
        /// <returns>The discount amount</returns>
        public static decimal GetDiscountAmount(this Discount discount, decimal price)
        {
            decimal result = decimal.Zero;
            if (discount.UsePercentage)
                result = Math.Round((decimal)((((float)price) * ((float)discount.DiscountPercentage)) / 100f), 2);
            else
                result = Math.Round(discount.DiscountAmount, 2);

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Checks requirements for customer
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Value indicating whether all requirements are met</returns>
        public static bool CheckDiscountRequirements(this Discount discount, Customer customer)
        {
            switch (discount.DiscountRequirement)
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
                            var assignedRoles = discount.CustomerRoles;
                            foreach (CustomerRole customerRole in customerRoles)
                                foreach (CustomerRole assignedRole in assignedRoles)
                                {
                                    if (customerRole.Name == assignedRole.Name)
                                    {
                                        return true;
                                    }
                                }
                        }
                    }
                    break;
                case DiscountRequirementEnum.MustBeRegistered:
                    {
                        //customer should be registered
                        bool result = customer != null && !customer.IsGuest;
                        return result;
                    }
                    break;
                case DiscountRequirementEnum.HasAllOfTheseProductVariantsInTheCart:
                    {
                        if (customer != null)
                        {
                            CustomerSession customerSession = IoC.Resolve<ICustomerService>().GetCustomerSessionByCustomerId(customer.CustomerId);
                            if (customerSession != null)
                            {
                                var restrictedProductVariants = IoC.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(discount.DiscountId);
                                var cart = IoC.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.ShoppingCart, customerSession.CustomerSessionGuid);

                                bool allFound = true;
                                foreach (ProductVariant restrictedPv in restrictedProductVariants)
                                {
                                    bool found1 = false;
                                    foreach (ShoppingCartItem sci in cart)
                                    {
                                        if (restrictedPv.ProductVariantId == sci.ProductVariantId)
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
                            CustomerSession customerSession = IoC.Resolve<ICustomerService>().GetCustomerSessionByCustomerId(customer.CustomerId);
                            if (customerSession != null)
                            {
                                var restrictedProductVariants = IoC.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(discount.DiscountId);
                                var cart = IoC.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.ShoppingCart, customerSession.CustomerSessionGuid);

                                bool found = false;
                                foreach (ProductVariant restrictedPv in restrictedProductVariants)
                                {
                                    foreach (ShoppingCartItem sci in cart)
                                    {
                                        if (restrictedPv.ProductVariantId == sci.ProductVariantId)
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
                        //customer should be registered
                        if (customer != null && !customer.IsGuest)
                        {
                            var restrictedProductVariants = IoC.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(discount.DiscountId);
                            var purchasedProductVariants = IoC.Resolve<IOrderService>().GetAllOrderProductVariants(null, customer.CustomerId, null, null, OrderStatusEnum.Complete, null, null);

                            bool allFound = true;
                            foreach (ProductVariant restrictedPv in restrictedProductVariants)
                            {
                                bool found1 = false;
                                foreach (OrderProductVariant purchasedPv in purchasedProductVariants)
                                {
                                    if (restrictedPv.ProductVariantId == purchasedPv.ProductVariantId)
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
                        //customer should be registered
                        if (customer != null && !customer.IsGuest)
                        {
                            var restrictedProductVariants = IoC.Resolve<IProductService>().GetProductVariantsRestrictedByDiscountId(discount.DiscountId);
                            var purchasedProductVariants = IoC.Resolve<IOrderService>().GetAllOrderProductVariants(null, customer.CustomerId, null, null, OrderStatusEnum.Complete, null, null);

                            bool found = false;
                            foreach (ProductVariant restrictedPv in restrictedProductVariants)
                            {
                                foreach (OrderProductVariant purchasedPv in purchasedProductVariants)
                                {
                                    if (restrictedPv.ProductVariantId == purchasedPv.ProductVariantId)
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
                        //customer should be registered
                        if (customer != null && !customer.IsGuest)
                        {
                            var orders = customer.Orders.FindAll(o => o.OrderStatus == OrderStatusEnum.Complete);
                            decimal spentAmount = orders.Sum(o => o.OrderTotal);
                            return spentAmount > discount.RequirementSpentAmount;
                        }
                    }
                    break;
                case DiscountRequirementEnum.BillingCountryIs:
                    {
                        if (customer != null &&
                            customer.BillingAddress != null &&
                            customer.BillingAddress.CountryId > 0 &&
                            discount.RequirementBillingCountryIs > 0)
                        {
                            return customer.BillingAddress.CountryId == discount.RequirementBillingCountryIs;
                        }
                    }
                    break;
                case DiscountRequirementEnum.ShippingCountryIs:
                    {
                        if (customer != null &&
                            customer.ShippingAddress != null &&
                            customer.ShippingAddress.CountryId > 0 &&
                            discount.RequirementShippingCountryIs > 0)
                        {
                            return customer.ShippingAddress.CountryId == discount.RequirementShippingCountryIs;
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
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Value indicating whether discount can be used</returns>
        public static bool CheckDiscountLimitations(this Discount discount, Customer customer)
        {
            switch (discount.DiscountLimitation)
            {
                case DiscountLimitationEnum.Unlimited:
                    {
                        return true;
                    }
                case DiscountLimitationEnum.OneTimeOnly:
                    {
                        var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, null, null);
                        return usageHistory.Count < 1;
                    }
                case DiscountLimitationEnum.NTimesOnly:
                    {
                        var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, null, null);
                        return usageHistory.Count < discount.LimitationTimes;
                    }
                case DiscountLimitationEnum.OneTimePerCustomer:
                    {
                        if (customer != null && !customer.IsGuest)
                        {
                            //registered customer
                            var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, customer.CustomerId, null);
                            return usageHistory.Count < 1;
                        }
                        else
                        {
                            //guest
                            return true;
                        }
                    }
                case DiscountLimitationEnum.NTimesPerCustomer:
                    {
                        if (customer != null && !customer.IsGuest)
                        {
                            //registered customer
                            var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, customer.CustomerId, null);
                            return usageHistory.Count < discount.LimitationTimes;
                        }
                        else
                        {
                            //guest
                            return true;
                        }
                    }
                default:
                    break;
            }
            return false;
        }


        /// <summary>
        /// Get discount limitation name
        /// </summary>
        /// <param name="dl">Discount limitation</param>
        /// <returns>Discount limitation name</returns>
        public static string GetDiscountLimitationName(this DiscountLimitationEnum dl)
        {
            string name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString(
                string.Format("DiscountLimitation.{0}", (int)dl),
                NopContext.Current.WorkingLanguage.LanguageId,
                true,
                CommonHelper.ConvertEnum(dl.ToString()));

            return name;
        }

        /// <summary>
        /// Get discount requirement name
        /// </summary>
        /// <param name="dr">Discount requirement</param>
        /// <returns>Discount requirement name</returns>
        public static string GetDiscountRequirementName(this DiscountRequirementEnum dr)
        {
            string name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString(
                string.Format("DiscountRequirement.{0}", (int)dr),
                NopContext.Current.WorkingLanguage.LanguageId,
                true,
                CommonHelper.ConvertEnum(dr.ToString()));

            return name;
        }

        /// <summary>
        /// Get discount type name
        /// </summary>
        /// <param name="dt">Discount type</param>
        /// <returns>Discount type name</returns>
        public static string GetDiscountTypeName(this DiscountTypeEnum dt)
        {
            string name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString(
                string.Format("DiscountType.{0}", (int)dt),
                NopContext.Current.WorkingLanguage.LanguageId,
                true,
                CommonHelper.ConvertEnum(dt.ToString()));

            return name;
        }
    }
}
