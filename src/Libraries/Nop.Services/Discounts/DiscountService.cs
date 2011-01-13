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
using System.Reflection;
using Nop.Core.Caching;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service
    /// </summary>
    public partial class DiscountService : IDiscountService
    {
        #region Constants
        private const string DISCOUNTS_ALL_KEY = "Nop.discount.all-{0}-{1}";
        private const string DISCOUNTS_BY_ID_KEY = "Nop.discount.id-{0}";
        private const string DISCOUNTS_PATTERN_KEY = "Nop.discount.";
        #endregion

        #region Fields

        private readonly IRepository<Discount> _discountRepository;
        private readonly ICacheManager _cacheManager;
        
        #endregion

        #region Ctor

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="discountRepository">Discount repository</param>
        public DiscountService(ICacheManager cacheManager,
            IRepository<Discount> discountRepository)
        {
            this._cacheManager = cacheManager;
            this._discountRepository = discountRepository;
        }

        #endregion

        #endregion

        #region Utilities

        /// <summary>
        /// Checks discount limitation for customer
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Value indicating whether discount can be used</returns>
        public static bool CheckDiscountLimitations(Discount discount, Customer customer)
        {
            switch (discount.DiscountLimitation)
            {
                case DiscountLimitationType.Unlimited:
                    {
                        return true;
                    }
                case DiscountLimitationType.NTimesOnly:
                    {
                        //UNDONE implement
                        throw new NotImplementedException();
                        //var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, null, null);
                        //return usageHistory.Count < discount.LimitationTimes;
                    }
                case DiscountLimitationType.NTimesPerCustomer:
                    {
                        //UNDONE implement
                        throw new NotImplementedException();
                        //if (customer != null && !customer.IsGuest) 
                        //{
                        //    //registered customer
                        //    var usageHistory = IoC.Resolve<IDiscountService>().GetAllDiscountUsageHistoryEntries(discount.DiscountId, customer.CustomerId, null);
                        //    return usageHistory.Count < discount.LimitationTimes;
                        //}
                        //else
                        //{
                        //    //guest
                        //    return true;
                        //}
                    }
                default:
                    break;
            }
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public void DeleteDiscount(Discount discount)
        {
            if (discount == null)
                return;

            _discountRepository.Delete(discount);

            _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
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
            return _cacheManager.Get(key, () =>
            {
                var discount = _discountRepository.GetById(discountId);
                return discount;
            });
        }

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Discount collection</returns>
        public IList<Discount> GetAllDiscounts(DiscountType? discountType, bool showHidden = false)
        { 
            int? discountTypeId = null;
            if (discountType.HasValue)
                discountTypeId = (int)discountType.Value;

            string key = string.Format(DISCOUNTS_ALL_KEY, showHidden, discountTypeId.HasValue ? discountTypeId.Value : 0);
            return _cacheManager.Get(key, () =>
            {
                var query = _discountRepository.Table;
                if (!showHidden)
                {
                    query = query.Where(d => 
                        (!d.StartDateUtc.HasValue || d.StartDateUtc <= DateTime.UtcNow)
                        && (!d.EndDateUtc.HasValue || d.EndDateUtc >= DateTime.UtcNow)
                        );
                }
                if (discountTypeId.HasValue && discountTypeId.Value > 0)
                {
                    query = query.Where(d => d.DiscountTypeId == discountTypeId);
                }
                query = query.OrderByDescending(d => d.Id);
                
                var discounts = query.ToList();
                return discounts;
            });
        }

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public void InsertDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            _discountRepository.Insert(discount);

            _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public void UpdateDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            _discountRepository.Update(discount);

            _cacheManager.RemoveByPattern(DISCOUNTS_PATTERN_KEY);
        }

        /// <summary>
        /// Load discount requirement rule by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found discount requirement rule</returns>
        public IDiscountRequirementRule LoadDiscountRequirementRuleBySystemName(string systemName)
        {
            var rules = LoadAllDiscountRequirementRules();
            var rule = rules.SingleOrDefault(r => r.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return rule;
        }

        /// <summary>
        /// Load all discount requirement rules
        /// </summary>
        /// <returns>Discount requirement rules</returns>
        public IList<IDiscountRequirementRule> LoadAllDiscountRequirementRules()
        {
            var rules = new List<IDiscountRequirementRule>();

            //TODO search in all assemblies
            System.Type configType = typeof(BillingCountryDiscountRequirementRule);   //any of your IDiscountRequirementRule implementations here
            var typesToRegister = Assembly.GetAssembly(configType).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IDiscountRequirementRule)));
            foreach (var type in typesToRegister)
            {
                dynamic rule = Activator.CreateInstance(type);
                rules.Add(rule);
            }
            return rules;
        }

        /// <summary>
        /// Check discount requirements
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool IsDiscountValid(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            if (customer == null)
                throw new ArgumentNullException("customer");

            //check coupon code
            if (discount.RequiresCouponCode)
            {
                if (String.IsNullOrEmpty(discount.CouponCode))
                    return false;

                string couponCodeToValidate = customer.DiscountCouponCode;
                if (!discount.CouponCode.Equals(couponCodeToValidate, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            //check date range
            DateTime now = DateTime.UtcNow;
            if (discount.StartDateUtc.HasValue)
            {
                DateTime startDate = DateTime.SpecifyKind(discount.StartDateUtc.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                    return false;
            }
            if (discount.EndDateUtc.HasValue)
            {
                DateTime endDate = DateTime.SpecifyKind(discount.EndDateUtc.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                    return false;
            }

            //UNDONE Check discount limitations
            if (!CheckDiscountLimitations(discount, customer))
                return false;
            
            //discount requirements
            var requirements = discount.DiscountRequirements;
            foreach (var req in requirements)
            {
                var requirementRule = LoadDiscountRequirementRuleBySystemName(req.DiscountRequirementRuleSystemName);
                if (requirementRule == null)
                    continue;
                var request = new CheckDiscountRequirementRequest()
                {
                     DiscountRequirement = req,
                     Customer = customer
                };
                if (!requirementRule.CheckRequirement(request))
                    return false;
            }
            return true;
        }
        #endregion
    }
}
