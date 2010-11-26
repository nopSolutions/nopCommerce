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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Discount service
    /// </summary>
    public partial class DiscountService : IDiscountService
    {
        #region Constants
        private const string DISCOUNTS_ALL_KEY = "Nop.discount.all-{0}-{1}";
        private const string DISCOUNTS_BY_ID_KEY = "Nop.discount.id-{0}";
        private const string DISCOUNTS_BY_PRODUCTVARIANTID_KEY = "Nop.discount.byproductvariantid-{0}-{1}";
        private const string DISCOUNTS_BY_CATEGORYID_KEY = "Nop.discount.bycategoryid-{0}-{1}";
        private const string DISCOUNTS_PATTERN_KEY = "Nop.discount.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public DiscountService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
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
            object obj2 = _cacheManager.Get(key);
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
                _cacheManager.Add(key, discount);
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
            object obj2 = _cacheManager.Get(key);
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
                _cacheManager.Add(key, discounts);
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

            discount.Name = CommonHelper.EnsureNotNull(discount.Name);
            discount.Name = CommonHelper.EnsureMaximumLength(discount.Name, 100);
            discount.CouponCode = CommonHelper.EnsureNotNull(discount.CouponCode);
            discount.CouponCode = CommonHelper.EnsureMaximumLength(discount.CouponCode, 100);
            
            _context.Discounts.AddObject(discount);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
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

            discount.Name = CommonHelper.EnsureNotNull(discount.Name);
            discount.Name = CommonHelper.EnsureMaximumLength(discount.Name, 100);
            discount.CouponCode = CommonHelper.EnsureNotNull(discount.CouponCode);
            discount.CouponCode = CommonHelper.EnsureMaximumLength(discount.CouponCode, 100);
                        
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Adds a discount to a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void AddDiscountToProductVariant(int productVariantId, int discountId)
        {
            var productVariant = IoC.Resolve<IProductService>().GetProductVariantById(productVariantId);
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
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountFromProductVariant(int productVariantId, int discountId)
        {
            var productVariant = IoC.Resolve<IProductService>().GetProductVariantById(productVariantId);
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
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
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
            object obj2 = _cacheManager.Get(key);
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
                _cacheManager.Add(key, discounts);
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
            var category = IoC.Resolve<ICategoryService>().GetCategoryById(categoryId);
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
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Removes a discount from a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountFromCategory(int categoryId, int discountId)
        {
            var category = IoC.Resolve<ICategoryService>().GetCategoryById(categoryId);
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
                _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
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
            object obj2 = _cacheManager.Get(key);
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
                _cacheManager.Add(key, discounts);
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
            var productVariant = IoC.Resolve<IProductService>().GetProductVariantById(productVariantId);
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
            var productVariant = IoC.Resolve<IProductService>().GetProductVariantById(productVariantId);
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
            var query1 = from duh in _context.DiscountUsageHistory
                         from d in _context.Discounts
                         .Where(d => d.DiscountId == duh.DiscountId)
                         .DefaultIfEmpty()
                         from c in _context.Customers
                         .Where(c => c.CustomerId == duh.CustomerId)
                         .DefaultIfEmpty()
                         from o in _context.Orders
                         .Where(o => o.OrderId == duh.OrderId)
                         .DefaultIfEmpty()
                         where 
                         (!d.Deleted && !c.Deleted && !o.Deleted) &&
                         (!discountId.HasValue || discountId.Value ==0 || duh.DiscountId == discountId.Value) &&
                         (!customerId.HasValue || customerId.Value ==0 || duh.CustomerId == customerId.Value) &&
                         (!orderId.HasValue || orderId.Value ==0 || duh.OrderId == orderId.Value)
                         select duh.DiscountUsageHistoryId;

            var query2 = from duh in _context.DiscountUsageHistory
                         where query1.Contains(duh.DiscountUsageHistoryId)
                         select duh;
            
            var discountUsageHistoryEntries = query2.ToList();
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
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.DiscountManager.CacheEnabled");
            }
        }

        #endregion
    }
}
