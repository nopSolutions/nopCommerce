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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Discount manager
    /// </summary>
    public partial class DiscountManager
    {
        #region Constants
        private const string DISCOUNTS_ALL_KEY = "Nop.discount.all-{0}-{1}";
        private const string DISCOUNTS_BY_ID_KEY = "Nop.discount.id-{0}";
        private const string DISCOUNTS_BY_PRODUCTVARIANTID_KEY = "Nop.discount.byproductvariantid-{0}-{1}";
        private const string DISCOUNTS_BY_CATEGORYID_KEY = "Nop.discount.bycategoryid-{0}-{1}";
        private const string DISCOUNTTYPES_ALL_KEY = "Nop.discounttype.all";
        private const string DISCOUNTREQUIREMENT_ALL_KEY = "Nop.discountrequirement.all";
        private const string DISCOUNTLIMITATION_ALL_KEY = "Nop.discountlimitation.all";
        private const string DISCOUNTS_PATTERN_KEY = "Nop.discount.";
        private const string DISCOUNTTYPES_PATTERN_KEY = "Nop.discounttype.";
        private const string DISCOUNTREQUIREMENT_PATTERN_KEY = "Nop.discountrequirement.";
        private const string DISCOUNTLIMITATION_PATTERN_KEY = "Nop.discountlimitation.";
        #endregion

        #region Methods

        #region Discounts

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="discounts">Discounts to analyze</param>
        /// <param name="amount">Amount</param>
        /// <returns>Preferred discount</returns>
        public static Discount GetPreferredDiscount(List<Discount> discounts, 
            decimal amount)
        {
            Discount preferredDiscount = null;
            decimal maximumDiscountValue = decimal.Zero;
            foreach (var _discount in discounts)
            {
                decimal currentDiscountValue = _discount.GetDiscountAmount(amount);
                if (currentDiscountValue > maximumDiscountValue)
                {
                    maximumDiscountValue = currentDiscountValue;
                    preferredDiscount = _discount;
                }
            }

            return preferredDiscount;
        }

        /// <summary>
        /// Gets a discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Discount</returns>
        public static Discount GetDiscountById(int discountId)
        {
            if (discountId == 0)
                return null;

            string key = string.Format(DISCOUNTS_BY_ID_KEY, discountId);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (Discount)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from d in context.Discounts
                        where d.DiscountId == discountId
                        select d;
            var discount = query.SingleOrDefault();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discount);
            }
            return discount;
        }

        /// <summary>
        /// Marks discount as deleted
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        public static void MarkDiscountAsDeleted(int discountId)
        {
            Discount discount = GetDiscountById(discountId);
            if (discount != null)
            {
                UpdateDiscount(discount.DiscountId, discount.DiscountType, 
                    discount.DiscountRequirement, discount.DiscountLimitation,
                    discount.Name, discount.UsePercentage, discount.DiscountPercentage,
                    discount.DiscountAmount, discount.StartDate,
                    discount.EndDate, discount.RequiresCouponCode,
                    discount.CouponCode, true);
            }
        }

        /// <summary>
        /// Get a value indicating whether discounts that require coupon code exist
        /// </summary>
        /// <returns>A value indicating whether discounts that require coupon code exist</returns>
        public static bool HasDiscountsWithCouponCode()
        {
            var discounts = GetAllDiscounts(null);
            return discounts.Find(d => d.RequiresCouponCode) != null;
        }

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <returns>Discount collection</returns>
        public static List<Discount> GetAllDiscounts(DiscountTypeEnum? discountType)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_ALL_KEY, showHidden, discountType);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            int? discountTypeId = null;
            if (discountType.HasValue)
                discountTypeId = (int)discountType.Value;
            
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<Discount>)context.Discounts;
            if (!showHidden)
                query = query.Where(d => d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow);
            if (discountTypeId.HasValue && discountTypeId.Value > 0)
                query = query.Where(d => d.DiscountTypeId == discountTypeId);
            query = query.Where(d => !d.Deleted);
            query = query.OrderByDescending(d => d.StartDate);

            var discounts = query.ToList();
            
            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discounts);
            }
            return discounts;
        }

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discountType">The discount type</param>
        /// <param name="discountRequirement">The discount requirement</param>
        /// <param name="discountLimitation">The discount limitation</param>
        /// <param name="name">The name</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="discountPercentage">The discount percentage</param>
        /// <param name="discountAmount">The discount amount</param>
        /// <param name="startDate">The discount start date and time</param>
        /// <param name="endDate">The discount end date and time</param>
        /// <param name="requiresCouponCode">The value indicating whether discount requires coupon code</param>
        /// <param name="couponCode">The coupon code</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <returns>Discount</returns>
        public static Discount InsertDiscount(DiscountTypeEnum discountType,
            DiscountRequirementEnum discountRequirement,
            DiscountLimitationEnum discountLimitation, string name, bool usePercentage, 
            decimal discountPercentage, decimal discountAmount,
            DateTime startDate, DateTime endDate, bool requiresCouponCode, 
            string couponCode, bool deleted)
        {
            if (startDate.CompareTo(endDate) >= 0)
                throw new NopException("Start date should be less then expiration date");

            if (requiresCouponCode && String.IsNullOrEmpty(couponCode))
            {
                throw new NopException("Discount requires coupon code. Coupon code could not be empty.");
            }

            name = CommonHelper.EnsureMaximumLength(name, 100);
            couponCode = CommonHelper.EnsureMaximumLength(couponCode, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var discount = context.Discounts.CreateObject();
            discount.DiscountTypeId = (int)discountType;
            discount.DiscountRequirementId = (int)discountRequirement;
            discount.DiscountLimitationId = (int)discountLimitation;
            discount.Name = name;
            discount.UsePercentage = usePercentage;
            discount.DiscountPercentage = discountPercentage;
            discount.DiscountAmount = discountAmount;
            discount.StartDate = startDate;
            discount.EndDate = endDate;
            discount.RequiresCouponCode = requiresCouponCode;
            discount.CouponCode = couponCode;
            discount.Deleted = deleted;

            context.Discounts.AddObject(discount);
            context.SaveChanges();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
            return discount;
        }

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountType">The discount type</param>
        /// <param name="discountRequirement">The discount requirement</param>
        /// <param name="discountLimitation">The discount limitation</param>
        /// <param name="name">The name</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="discountPercentage">The discount percentage</param>
        /// <param name="discountAmount">The discount amount</param>
        /// <param name="startDate">The discount start date and time</param>
        /// <param name="endDate">The discount end date and time</param>
        /// <param name="requiresCouponCode">The value indicating whether discount requires coupon code</param>
        /// <param name="couponCode">The coupon code</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <returns>Discount</returns>
        public static Discount UpdateDiscount(int discountId, DiscountTypeEnum discountType,
            DiscountRequirementEnum discountRequirement, DiscountLimitationEnum discountLimitation,
            string name, bool usePercentage, decimal discountPercentage, decimal discountAmount,
            DateTime startDate, DateTime endDate, bool requiresCouponCode, 
            string couponCode, bool deleted)
        {
            if (startDate.CompareTo(endDate) >= 0)
                throw new NopException("Start date should be less then expiration date");

            if (requiresCouponCode && String.IsNullOrEmpty(couponCode))
            {
                throw new NopException("Discount requires coupon code. Coupon code could not be empty.");
            }

            name = CommonHelper.EnsureMaximumLength(name, 100);
            couponCode = CommonHelper.EnsureMaximumLength(couponCode, 100);

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            discount.DiscountTypeId = (int)discountType;
            discount.DiscountRequirementId = (int)discountRequirement;
            discount.DiscountLimitationId = (int)discountLimitation;
            discount.Name = name;
            discount.UsePercentage = usePercentage;
            discount.DiscountPercentage = discountPercentage;
            discount.DiscountAmount = discountAmount;
            discount.StartDate = startDate;
            discount.EndDate = endDate;
            discount.RequiresCouponCode = requiresCouponCode;
            discount.CouponCode = couponCode;
            discount.Deleted = deleted;
            context.SaveChanges();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
            return discount;
        }

        /// <summary>
        /// Adds a discount to a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void AddDiscountToProductVariant(int productVariantId, int discountId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariant))
                context.ProductVariants.Attach(productVariant);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            productVariant.NpDiscounts.Add(discount);
            context.SaveChanges();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void RemoveDiscountFromProductVariant(int productVariantId, int discountId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariant))
                context.ProductVariants.Attach(productVariant);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            productVariant.NpDiscounts.Remove(discount);
            context.SaveChanges();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a discount collection of a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Discount collection</returns>
        public static List<Discount> GetDiscountsByProductVariantId(int productVariantId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_BY_PRODUCTVARIANTID_KEY, productVariantId, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from d in context.Discounts
                        from pv in d.NpProductVariants
                        where (showHidden || (d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)) &&
                            !d.Deleted &&
                            pv.ProductVariantId == productVariantId
                        orderby d.StartDate descending
                        select d;
            var discounts = query.ToList();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discounts);
            }
            return discounts;
        }

        /// <summary>
        /// Adds a discount to a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void AddDiscountToCategory(int categoryId, int discountId)
        {
            var category = CategoryManager.GetCategoryById(categoryId);
            if (category == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(category))
                context.Categories.Attach(category);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            category.NpDiscounts.Add(discount);
            context.SaveChanges();
            
            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void RemoveDiscountFromCategory(int categoryId, int discountId)
        {
            var category = CategoryManager.GetCategoryById(categoryId);
            if (category == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(category))
                context.Categories.Attach(category);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            category.NpDiscounts.Remove(discount);
            context.SaveChanges();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a discount collection of a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Discount collection</returns>
        public static List<Discount> GetDiscountsByCategoryId(int categoryId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_BY_CATEGORYID_KEY, categoryId, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from d in context.Discounts
                        from c in d.NpCategories
                        where (showHidden || (d.StartDate <= DateTime.UtcNow  && d.EndDate >= DateTime.UtcNow)) &&
                            !d.Deleted &&
                            c.CategoryId == categoryId
                        orderby d.StartDate descending
                        select d;
            var discounts = query.ToList();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discounts);
            }
            return discounts;
        }

        /// <summary>
        /// Adds a discount requirement
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void AddDiscountRestriction(int productVariantId, int discountId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariant))
                context.ProductVariants.Attach(productVariant);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            discount.NpRestrictedProductVariants.Add(productVariant);
            context.SaveChanges();
        }

        /// <summary>
        /// Removes discount requirement
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void RemoveDiscountRestriction(int productVariantId, int discountId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariant))
                context.ProductVariants.Attach(productVariant);
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);

            discount.NpRestrictedProductVariants.Remove(productVariant);
            context.SaveChanges();
        }

        #endregion

        #region Etc

        /// <summary>
        /// Gets all discount requirements
        /// </summary>
        /// <returns>Discount requirement collection</returns>
        public static List<DiscountRequirement> GetAllDiscountRequirements()
        {
            string key = string.Format(DISCOUNTREQUIREMENT_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountRequirement>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from dr in context.DiscountRequirements
                        orderby dr.DiscountRequirementId
                        select dr;
            var discountRequirements = query.ToList();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discountRequirements);
            }
            return discountRequirements;
        }

        /// <summary>
        /// Gets all discount types
        /// </summary>
        /// <returns>Discount type collection</returns>
        public static List<DiscountType> GetAllDiscountTypes()
        {
            string key = string.Format(DISCOUNTTYPES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountType>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from dt in context.DiscountTypes
                        orderby dt.DiscountTypeId
                        select dt;
            var discountTypes = query.ToList();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discountTypes);
            }
            return discountTypes;
        }
        
        /// <summary>
        /// Gets all discount limitations
        /// </summary>
        /// <returns>Discount limitation collection</returns>
        public static List<DiscountLimitation> GetAllDiscountLimitations()
        {
            string key = string.Format(DISCOUNTLIMITATION_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (DiscountManager.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountLimitation>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from dl in context.DiscountLimitations
                        orderby dl.DiscountLimitationId
                        select dl;
            var discountLimitations = query.ToList();

            if (DiscountManager.CacheEnabled)
            {
                NopRequestCache.Add(key, discountLimitations);
            }
            return discountLimitations;
        }

        #endregion

        #region Discount History

        /// <summary>
        /// Deletes a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history entry identifier</param>
        public static void DeleteDiscountUsageHistory(int discountUsageHistoryId)
        {
            var discountUsageHistory = GetDiscountUsageHistoryById(discountUsageHistoryId);
            if (discountUsageHistory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(discountUsageHistory))
                context.DiscountUsageHistory.Attach(discountUsageHistory);
            context.DeleteObject(discountUsageHistory);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history entry identifier</param>
        /// <returns>Discount usage history entry</returns>
        public static DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId)
        {
            if (discountUsageHistoryId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from duh in context.DiscountUsageHistory
                        where duh.DiscountUsageHistoryId == discountUsageHistoryId
                        select duh;
            var discountUsageHistory = query.SingleOrDefault();
            return discountUsageHistory;
        }

        /// <summary>
        /// Gets all discount usage history entries
        /// </summary>
        /// <param name="discountId">Discount type identifier; null to load all</param>
        /// <param name="customerId">Customer identifier; null to load all</param>
        /// <param name="orderId">Order identifier; null to load all</param>
        /// <returns>Discount usage history entries</returns>
        public static List<DiscountUsageHistory> GetAllDiscountUsageHistoryEntries(int? discountId,
            int? customerId, int? orderId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var discountUsageHistoryEntries = context.Sp_DiscountUsageHistoryLoadAll(discountId,
                customerId, orderId).ToList();

            return discountUsageHistoryEntries;
        }

        /// <summary>
        /// Inserts a discount usage history entry
        /// </summary>
        /// <param name="discountId">Discount type identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Discount usage history entry</returns>
        public static DiscountUsageHistory InsertDiscountUsageHistory(int discountId,
            int customerId, int orderId, DateTime createdOn)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var discountUsageHistory = context.DiscountUsageHistory.CreateObject();
            discountUsageHistory.DiscountId = discountId;
            discountUsageHistory.CustomerId = customerId;
            discountUsageHistory.OrderId = orderId;
            discountUsageHistory.CreatedOn = createdOn;

            context.DiscountUsageHistory.AddObject(discountUsageHistory);
            context.SaveChanges();

            return discountUsageHistory;
        }

        /// <summary>
        /// Updates the discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">discount usage history entry identifier</param>
        /// <param name="discountId">Discount type identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Discount</returns>
        public static DiscountUsageHistory UpdateDiscountUsageHistory(int discountUsageHistoryId, 
            int discountId, int customerId, int orderId, DateTime createdOn)
        {
            var discountUsageHistory = GetDiscountUsageHistoryById(discountUsageHistoryId);
            if (discountUsageHistory == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(discountUsageHistory))
                context.DiscountUsageHistory.Attach(discountUsageHistory);

            discountUsageHistory.DiscountId = discountId;
            discountUsageHistory.CustomerId = customerId;
            discountUsageHistory.OrderId = orderId;
            discountUsageHistory.CreatedOn = createdOn;
            context.SaveChanges();
            return discountUsageHistory;
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.DiscountManager.CacheEnabled");
            }
        }
        #endregion
    }
}
