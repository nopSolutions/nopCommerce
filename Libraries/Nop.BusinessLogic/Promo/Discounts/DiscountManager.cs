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
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Discount manager
    /// </summary>
    public partial class DiscountManager : IDiscountManager
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

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public DiscountManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        #region Discounts

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="discounts">Discounts to analyze</param>
        /// <param name="amount">Amount</param>
        /// <returns>Preferred discount</returns>
        public Discount GetPreferredDiscount(List<Discount> discounts, 
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
        public Discount GetDiscountById(int discountId)
        {
            if (discountId == 0)
                return null;

            string key = string.Format(DISCOUNTS_BY_ID_KEY, discountId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Discount)obj2;
            }

            
            var query = from d in _context.Discounts
                        where d.DiscountId == discountId
                        select d;
            var discount = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, discount);
            }
            return discount;
        }

        /// <summary>
        /// Marks discount as deleted
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        public void MarkDiscountAsDeleted(int discountId)
        {
            Discount discount = GetDiscountById(discountId);
            if (discount != null)
            {
                discount.Deleted = true;
                UpdateDiscount(discount);
            }
        }

        /// <summary>
        /// Get a value indicating whether discounts that require coupon code exist
        /// </summary>
        /// <returns>A value indicating whether discounts that require coupon code exist</returns>
        public bool HasDiscountsWithCouponCode()
        {
            var discounts = GetAllDiscounts(null);
            return discounts.Find(d => d.RequiresCouponCode) != null;
        }

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <returns>Discount collection</returns>
        public List<Discount> GetAllDiscounts(DiscountTypeEnum? discountType)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_ALL_KEY, showHidden, discountType);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            int? discountTypeId = null;
            if (discountType.HasValue)
                discountTypeId = (int)discountType.Value;
            
            
            var query = (IQueryable<Discount>)_context.Discounts;
            if (!showHidden)
                query = query.Where(d => d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow);
            if (discountTypeId.HasValue && discountTypeId.Value > 0)
                query = query.Where(d => d.DiscountTypeId == discountTypeId);
            query = query.Where(d => !d.Deleted);
            query = query.OrderByDescending(d => d.StartDate);

            var discounts = query.ToList();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, discounts);
            }
            return discounts;
        }

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public void InsertDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            if (discount.StartDate.CompareTo(discount.EndDate) >= 0)
                throw new NopException("Start date should be less then expiration date");

            if (discount.RequiresCouponCode && String.IsNullOrEmpty(discount.CouponCode))
            {
                throw new NopException("Discount requires coupon code. Coupon code could not be empty.");
            }

            discount.Name = CommonHelper.EnsureNotNull(discount.Name);
            discount.Name = CommonHelper.EnsureMaximumLength(discount.Name, 100);
            discount.CouponCode = CommonHelper.EnsureNotNull(discount.CouponCode);
            discount.CouponCode = CommonHelper.EnsureMaximumLength(discount.CouponCode, 100);

            

            _context.Discounts.AddObject(discount);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public void UpdateDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            if (discount.StartDate.CompareTo(discount.EndDate) >= 0)
                throw new NopException("Start date should be less then expiration date");

            if (discount.RequiresCouponCode && String.IsNullOrEmpty(discount.CouponCode))
            {
                throw new NopException("Discount requires coupon code. Coupon code could not be empty.");
            }

            discount.Name = CommonHelper.EnsureNotNull(discount.Name);
            discount.Name = CommonHelper.EnsureMaximumLength(discount.Name, 100);
            discount.CouponCode = CommonHelper.EnsureNotNull(discount.CouponCode);
            discount.CouponCode = CommonHelper.EnsureMaximumLength(discount.CouponCode, 100);

            
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Adds a discount to a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void AddDiscountToProductVariant(int productVariantId, int discountId)
        {
            var productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(productVariant))
                _context.ProductVariants.Attach(productVariant);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            //ensure that navigation property is loaded
            if (productVariant.NpDiscounts == null)
                _context.LoadProperty(productVariant, pv => pv.NpDiscounts);

            productVariant.NpDiscounts.Add(discount);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountFromProductVariant(int productVariantId, int discountId)
        {
            var productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(productVariant))
                _context.ProductVariants.Attach(productVariant);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            productVariant.NpDiscounts.Remove(discount);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a discount collection of a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Discount collection</returns>
        public List<Discount> GetDiscountsByProductVariantId(int productVariantId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_BY_PRODUCTVARIANTID_KEY, productVariantId, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            
            var query = from d in _context.Discounts
                        from pv in d.NpProductVariants
                        where (showHidden || (d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)) &&
                            !d.Deleted &&
                            pv.ProductVariantId == productVariantId
                        orderby d.StartDate descending
                        select d;
            var discounts = query.ToList();

            if (this.CacheEnabled)
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
        public void AddDiscountToCategory(int categoryId, int discountId)
        {
            var category = IoCFactory.Resolve<ICategoryManager>().GetCategoryById(categoryId);
            if (category == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(category))
                _context.Categories.Attach(category);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            //ensure that navigation property is loaded
            if (category.NpDiscounts == null)
                _context.LoadProperty(category, c => c.NpDiscounts);

            category.NpDiscounts.Add(discount);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountFromCategory(int categoryId, int discountId)
        {
            var category = IoCFactory.Resolve<ICategoryManager>().GetCategoryById(categoryId);
            if (category == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(category))
                _context.Categories.Attach(category);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            //ensure that navigation property is loaded
            if (category.NpDiscounts == null)
                _context.LoadProperty(category, c => c.NpDiscounts);

            category.NpDiscounts.Remove(discount);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a discount collection of a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Discount collection</returns>
        public List<Discount> GetDiscountsByCategoryId(int categoryId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(DISCOUNTS_BY_CATEGORYID_KEY, categoryId, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<Discount>)obj2;
            }

            
            var query = from d in _context.Discounts
                        from c in d.NpCategories
                        where (showHidden || (d.StartDate <= DateTime.UtcNow  && d.EndDate >= DateTime.UtcNow)) &&
                            !d.Deleted &&
                            c.CategoryId == categoryId
                        orderby d.StartDate descending
                        select d;
            var discounts = query.ToList();

            if (this.CacheEnabled)
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
        public void AddDiscountRestriction(int productVariantId, int discountId)
        {
            var productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(productVariant))
                _context.ProductVariants.Attach(productVariant);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            //ensure that navigation property is loaded
            if (discount.NpRestrictedProductVariants == null)
                _context.LoadProperty(discount, d => d.NpRestrictedProductVariants);

            discount.NpRestrictedProductVariants.Add(productVariant);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes discount requirement
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountRestriction(int productVariantId, int discountId)
        {
            var productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            var discount = GetDiscountById(discountId);
            if (discount == null)
                return;

            
            if (!_context.IsAttached(productVariant))
                _context.ProductVariants.Attach(productVariant);
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            //ensure that navigation property is loaded
            if (discount.NpRestrictedProductVariants == null)
                _context.LoadProperty(discount, d => d.NpRestrictedProductVariants);

            discount.NpRestrictedProductVariants.Remove(productVariant);
            _context.SaveChanges();
        }

        #endregion

        #region Etc

        /// <summary>
        /// Gets all discount requirements
        /// </summary>
        /// <returns>Discount requirement collection</returns>
        public List<DiscountRequirement> GetAllDiscountRequirements()
        {
            string key = string.Format(DISCOUNTREQUIREMENT_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountRequirement>)obj2;
            }

            
            var query = from dr in _context.DiscountRequirements
                        orderby dr.DiscountRequirementId
                        select dr;
            var discountRequirements = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, discountRequirements);
            }
            return discountRequirements;
        }

        /// <summary>
        /// Gets all discount types
        /// </summary>
        /// <returns>Discount type collection</returns>
        public List<DiscountType> GetAllDiscountTypes()
        {
            string key = string.Format(DISCOUNTTYPES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountType>)obj2;
            }

            
            var query = from dt in _context.DiscountTypes
                        orderby dt.DiscountTypeId
                        select dt;
            var discountTypes = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, discountTypes);
            }
            return discountTypes;
        }
        
        /// <summary>
        /// Gets all discount limitations
        /// </summary>
        /// <returns>Discount limitation collection</returns>
        public List<DiscountLimitation> GetAllDiscountLimitations()
        {
            string key = string.Format(DISCOUNTLIMITATION_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<DiscountLimitation>)obj2;
            }

            
            var query = from dl in _context.DiscountLimitations
                        orderby dl.DiscountLimitationId
                        select dl;
            var discountLimitations = query.ToList();

            if (this.CacheEnabled)
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
        public void DeleteDiscountUsageHistory(int discountUsageHistoryId)
        {
            var discountUsageHistory = GetDiscountUsageHistoryById(discountUsageHistoryId);
            if (discountUsageHistory == null)
                return;

            
            if (!_context.IsAttached(discountUsageHistory))
                _context.DiscountUsageHistory.Attach(discountUsageHistory);
            _context.DeleteObject(discountUsageHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history entry identifier</param>
        /// <returns>Discount usage history entry</returns>
        public DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId)
        {
            if (discountUsageHistoryId == 0)
                return null;

            
            var query = from duh in _context.DiscountUsageHistory
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
        public List<DiscountUsageHistory> GetAllDiscountUsageHistoryEntries(int? discountId,
            int? customerId, int? orderId)
        {
            
            var discountUsageHistoryEntries = _context.Sp_DiscountUsageHistoryLoadAll(discountId,
                customerId, orderId).ToList();

            return discountUsageHistoryEntries;
        }

        /// <summary>
        /// Inserts a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history entry</param>
        public void InsertDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException("discountUsageHistory");

            
            
            _context.DiscountUsageHistory.AddObject(discountUsageHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history entry</param>
        public void UpdateDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException("discountUsageHistory");

            
            if (!_context.IsAttached(discountUsageHistory))
                _context.DiscountUsageHistory.Attach(discountUsageHistory);

            _context.SaveChanges();
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.DiscountManager.CacheEnabled");
            }
        }

        #endregion
    }
}
