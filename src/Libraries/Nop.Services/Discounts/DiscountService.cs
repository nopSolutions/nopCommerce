using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts.Cache;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service
    /// </summary>
    public partial class DiscountService : IDiscountService
    {
        #region Fields

        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<DiscountRequirement> _discountRequirementRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Static cache manager</param>
        /// <param name="discountRepository">Discount repository</param>
        /// <param name="discountRequirementRepository">Discount requirement repository</param>
        /// <param name="discountUsageHistoryRepository">Discount usage history repository</param>
        /// <param name="categoryRepository">Category repository</param>
        /// <param name="manufacturerRepository">Manufacturer repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="categoryService">Category service</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public DiscountService(IStaticCacheManager cacheManager,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            ICategoryService categoryService,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._discountRepository = discountRepository;
            this._discountRequirementRepository = discountRequirementRepository;
            this._discountUsageHistoryRepository = discountUsageHistoryRepository;
            this._categoryRepository = categoryRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._productRepository = productRepository;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._categoryService = categoryService;
            this._pluginFinder = pluginFinder;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// DiscountRequirement (for caching)
        /// </summary>
        [Serializable]
        public class DiscountRequirementForCaching
        {
            public DiscountRequirementForCaching()
            {
                ChildRequirements = new List<DiscountRequirementForCaching>();
            }

            public int Id { get; set; }
            public string SystemName { get; set; }
            public bool IsGroup { get; set; }
            public RequirementGroupInteractionType? InteractionType { get; set; }
            public IList<DiscountRequirementForCaching> ChildRequirements { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get requirements for caching
        /// </summary>
        /// <param name="requirements">Collection of discount requirement</param>
        /// <returns>List of DiscountRequirementForCaching</returns>
        protected IList<DiscountRequirementForCaching> GetReqirementsForCaching(IEnumerable<DiscountRequirement> requirements)
        {
            var requirementForCaching = requirements.Select(requirement => new DiscountRequirementForCaching
            {
                Id = requirement.Id,
                IsGroup = requirement.IsGroup,
                SystemName = requirement.DiscountRequirementRuleSystemName,
                InteractionType = requirement.InteractionType,
                ChildRequirements = GetReqirementsForCaching(requirement.ChildRequirements)
            });
            
            return requirementForCaching.ToList();
        }

        /// <summary>
        /// Get discount validation result
        /// </summary>
        /// <param name="requirements">Collection of discount requirement</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <param name="customer">Customer</param>
        /// <param name="errors">Errors</param>
        /// <returns>True if result is valid; otherwise false</returns>
        protected bool GetValidationResult(IEnumerable<DiscountRequirementForCaching> requirements, 
            RequirementGroupInteractionType groupInteractionType, Customer customer, List<string> errors)
        {
            var result = false;

            foreach (var requirement in requirements)
            {
                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    var interactionType = requirement.InteractionType.HasValue 
                        ? requirement.InteractionType.Value : RequirementGroupInteractionType.And;
                    result = GetValidationResult(requirement.ChildRequirements, interactionType, customer, errors);
                }
                else
                {
                    //or try to get validation result for the requirement
                    var requirementRulePlugin = LoadDiscountRequirementRuleBySystemName(requirement.SystemName);
                    if (requirementRulePlugin == null)
                        continue;

                    if (!_pluginFinder.AuthorizedForUser(requirementRulePlugin.PluginDescriptor, customer))
                        continue;

                    if (!_pluginFinder.AuthenticateStore(requirementRulePlugin.PluginDescriptor, _storeContext.CurrentStore.Id))
                        continue;

                    var ruleResult = requirementRulePlugin.CheckRequirement(new DiscountRequirementValidationRequest
                    {
                        DiscountRequirementId = requirement.Id,
                        Customer = customer,
                        Store = _storeContext.CurrentStore
                    });

                    //add validation error
                    if (!ruleResult.IsValid)
                        errors.Add(ruleResult.UserError);

                    result = ruleResult.IsValid;
                }

                //all requirements must be met, so return false
                if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                    return result;

                //any of requirements must be met, so return true
                if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                    return result;
            }

            return result;
        }

        #endregion

        #region Methods

        #region Discounts

        /// <summary>
        /// Delete discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void DeleteDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Delete(discount);

            //event notification
            _eventPublisher.EntityDeleted(discount);
        }

        /// <summary>
        /// Gets a discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Discount</returns>
        public virtual Discount GetDiscountById(int discountId)
        {
            if (discountId == 0)
                return null;

            return _discountRepository.GetById(discountId);
        }

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; pass null to load all records</param>
        /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
        /// <param name="discountName">Discount name; pass null or empty to load all records</param>
        /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
        /// <param name="startDateUtc">Discount start date; pass null to load all records</param>
        /// <param name="endDateUtc">Discount end date; pass null to load all records</param>
        /// <returns>Discounts</returns>
        public virtual IList<Discount> GetAllDiscounts(DiscountType? discountType = null,
            string couponCode = null, string discountName = null, bool showHidden = false, 
            DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            var query = _discountRepository.Table;

            if (!showHidden)
            {
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact, that's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                query = query.Where(discount => 
                    (!discount.StartDateUtc.HasValue || discount.StartDateUtc <= nowUtc)  && 
                    (!discount.EndDateUtc.HasValue || discount.EndDateUtc >= nowUtc));
            }

            //filter by dates
            if (startDateUtc.HasValue)
                query = query.Where(discount => !discount.StartDateUtc.HasValue || discount.StartDateUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                query = query.Where(discount => !discount.EndDateUtc.HasValue || discount.EndDateUtc <= endDateUtc.Value);

            //filter by coupon code
            if (!string.IsNullOrEmpty(couponCode))
                query = query.Where(discount => discount.CouponCode == couponCode);

            //filter by name
            if (!string.IsNullOrEmpty(discountName))
                query = query.Where(discount => discount.Name.Contains(discountName));

            //filter by type
            if (discountType.HasValue)
                query = query.Where(discount => discount.DiscountTypeId == (int)discountType.Value);

            query = query.OrderBy(discount => discount.Name).ThenBy(discount => discount.Id);

            return query.ToList();
        }

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void InsertDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Insert(discount);

            //event notification
            _eventPublisher.EntityInserted(discount);
        }

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void UpdateDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Update(discount);

            //event notification
            _eventPublisher.EntityUpdated(discount);
        }

        /// <summary>
        /// Get categories for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted categories</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of categories</returns>
        public virtual IPagedList<Category> GetCategoriesWithAppliedDiscount(int? discountId = null, 
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var categories = _categoryRepository.Table;

            if (discountId.HasValue)
                categories = categories.Where(category => category.AppliedDiscounts.Any(discount => discount.Id == discountId.Value));

            if (!showHidden)
                categories = categories.Where(category => !category.Deleted);

            categories = categories.OrderBy(category => category.DisplayOrder).ThenBy(category => category.Id);

            return new PagedList<Category>(categories, pageIndex, pageSize);
        }

        /// <summary>
        /// Get manufacturers for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of manufacturers</returns>
        public virtual IPagedList<Manufacturer> GetManufacturersWithAppliedDiscount(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var manufacturers = _manufacturerRepository.Table;

            if (discountId.HasValue)
                manufacturers = manufacturers.Where(manufacturer => manufacturer.AppliedDiscounts.Any(discount => discount.Id == discountId.Value));

            if (!showHidden)
                manufacturers = manufacturers.Where(manufacturer => !manufacturer.Deleted);

            manufacturers = manufacturers.OrderBy(manufacturer => manufacturer.DisplayOrder).ThenBy(manufacturer => manufacturer.Id);

            return new PagedList<Manufacturer>(manufacturers, pageIndex, pageSize);

        }

        /// <summary>
        /// Get products for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of products</returns>
        public virtual IPagedList<Product> GetProductsWithAppliedDiscount(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var products = _productRepository.Table.Where(product => product.HasDiscountsApplied);

            if (discountId.HasValue)
                products = products.Where(product => product.AppliedDiscounts.Any(discount => discount.Id == discountId.Value));

            if (!showHidden)
                products = products.Where(product => !product.Deleted);

            products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return new PagedList<Product>(products, pageIndex, pageSize);
        }

        #endregion

        #region Discounts (caching)

        /// <summary>
        /// Gets all discounts (cachable models)
        /// </summary>
        /// <param name="discountType">Discount type; pass null to load all records</param>
        /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
        /// <param name="discountName">Discount name; pass null or empty to load all records</param>
        /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
        /// <returns>Discounts</returns>
        public virtual IList<DiscountForCaching> GetAllDiscountsForCaching(DiscountType? discountType = null,
            string couponCode = null, string discountName = null, bool showHidden = false)
        {
            //we cache discounts between requests. Otherwise, they will be loaded for almost each HTTP request
            //we have to use the following workaround with cachable model (DiscountForCaching) because
            //Entity Framework doesn't support 2-level caching

            //we load all discounts, and filter them using "discountType" parameter later (in memory)
            //we do it because we know that this method is invoked several times per HTTP request with distinct "discountType" parameter
            //that's why let's access the database only once
            var cacheKey = string.Format(DiscountEventConsumer.DISCOUNT_ALL_KEY, 
                showHidden, couponCode ?? string.Empty, discountName ?? string.Empty);
            var discounts = _cacheManager.Get(cacheKey, () =>
            {
                return GetAllDiscounts(couponCode: couponCode, discountName: discountName, showHidden: showHidden)
                    .Select(discount => discount.MapDiscount()).ToList();
            });

            //we know that this method is usually inkoved multiple times
            //that's why we filter discounts by type on the application layer
            if (discountType.HasValue)
                discounts = discounts.Where(discount => discount.DiscountType == discountType.Value).ToList();

            return discounts;
        }

        /// <summary>
        /// Get category identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Category identifiers</returns>
        public virtual IList<int> GetAppliedCategoryIds(DiscountForCaching discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var discountId = discount.Id;
            var cacheKey = string.Format(DiscountEventConsumer.DISCOUNT_CATEGORY_IDS_MODEL_KEY,
                discountId,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var result = _cacheManager.Get(cacheKey, () =>
            {
                var ids = new List<int>();
                var rootCategoryIds = _discountRepository.Table.Where(x => x.Id == discountId)
                        .SelectMany(x => x.AppliedToCategories.Select(c => c.Id))
                        .ToList();
                foreach (var categoryId in rootCategoryIds)
                {
                    if (!ids.Contains(categoryId))
                        ids.Add(categoryId);
                    if (discount.AppliedToSubCategories)
                    {
                        //include subcategories
                        foreach (var childCategoryId in _categoryService
                            .GetAllCategoriesByParentCategoryId(categoryId, false, true)
                            .Select(x => x.Id))
                        {
                            if (!ids.Contains(childCategoryId))
                                ids.Add(childCategoryId);
                        }
                    }
                }
                return ids;
            });

            return result;
        }

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Manufacturer identifiers</returns>
        public virtual IList<int> GetAppliedManufacturerIds(DiscountForCaching discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var discountId = discount.Id;
            var cacheKey = string.Format(DiscountEventConsumer.DISCOUNT_MANUFACTURER_IDS_MODEL_KEY,
                discountId,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var result = _cacheManager.Get(cacheKey, () =>
            {
                return _discountRepository.Table.Where(x => x.Id == discountId)
                    .SelectMany(x => x.AppliedToManufacturers.Select(c => c.Id))
                    .ToList();
            });

            return result;
        }

        #endregion

        #region Discount requirements

        /// <summary>
        /// Get all discount requirements
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="topLevelOnly">Whether to load top-level requirements only (without parent identifier)</param>
        /// <returns>Requirements</returns>
        public virtual IList<DiscountRequirement> GetAllDiscountRequirements(int discountId = 0, bool topLevelOnly = false)
        {
            var query = _discountRequirementRepository.Table;

            //filter by discount
            if (discountId > 0)
                query = query.Where(requirement => requirement.DiscountId == discountId);

            //filter by top-level
            if (topLevelOnly)
                query = query.Where(requirement => !requirement.ParentId.HasValue);

            query = query.OrderBy(requirement => requirement.Id);
            
            return query.ToList();
        }

        /// <summary>
        /// Delete discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        public virtual void DeleteDiscountRequirement(DiscountRequirement discountRequirement)
        {
            if (discountRequirement == null)
                throw new ArgumentNullException(nameof(discountRequirement));

            _discountRequirementRepository.Delete(discountRequirement);

            //event notification
            _eventPublisher.EntityDeleted(discountRequirement);
        }

        /// <summary>
        /// Load discount requirement rule by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found discount requirement rule</returns>
        public virtual IDiscountRequirementRule LoadDiscountRequirementRuleBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IDiscountRequirementRule>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IDiscountRequirementRule>();

            return null;
        }

        /// <summary>
        /// Load all discount requirement rules
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <returns>Discount requirement rules</returns>
        public virtual IList<IDiscountRequirementRule> LoadAllDiscountRequirementRules(Customer customer = null)
        {
            return _pluginFinder.GetPlugins<IDiscountRequirementRule>(customer: customer).ToList();
        }
        
        #endregion

        #region Validation

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return ValidateDiscount(discount.MapDiscount(), customer);
        }

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <param name="couponCodesToValidate">Coupon codes to validate</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(Discount discount, Customer customer, string[] couponCodesToValidate)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return ValidateDiscount(discount.MapDiscount(), customer, couponCodesToValidate);
        }

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(DiscountForCaching discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var couponCodesToValidate = customer.ParseAppliedDiscountCouponCodes();
            return ValidateDiscount(discount, customer, couponCodesToValidate);
        }

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <param name="couponCodesToValidate">Coupon codes to validate</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(DiscountForCaching discount, Customer customer, string[] couponCodesToValidate)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //invalid by default
            var result = new DiscountValidationResult();

            //check coupon code
            if (discount.RequiresCouponCode)
            {
                if (string.IsNullOrEmpty(discount.CouponCode))
                    return result;

                if (couponCodesToValidate == null)
                    return result;

                if (!couponCodesToValidate.Any(x => x.Equals(discount.CouponCode, StringComparison.InvariantCultureIgnoreCase)))
                    return result;
            }

            //Do not allow discounts applied to order subtotal or total when a customer has gift cards in the cart.
            //Otherwise, this customer can purchase gift cards with discount and get more than paid ("free money").
            if (discount.DiscountType == DiscountType.AssignedToOrderSubTotal ||
                discount.DiscountType == DiscountType.AssignedToOrderTotal)
            {
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                var hasGiftCards = cart.Any(x => x.Product.IsGiftCard);
                if (hasGiftCards)
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsedWithGiftCards") };
                    return result;
                }
            }

            //check date range
            var now = DateTime.UtcNow;
            if (discount.StartDateUtc.HasValue)
            {
                var startDate = DateTime.SpecifyKind(discount.StartDateUtc.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.NotStartedYet") };
                    return result;
                }
            }
            if (discount.EndDateUtc.HasValue)
            {
                var endDate = DateTime.SpecifyKind(discount.EndDateUtc.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.Expired") };
                    return result;
                }
            }

            //discount limitation
            switch (discount.DiscountLimitation)
            {
                case DiscountLimitationType.NTimesOnly:
                    {
                        var usedTimes = GetAllDiscountUsageHistory(discount.Id, null, null, 0, 1).TotalCount;
                        if (usedTimes >= discount.LimitationTimes)
                            return result;
                    }
                    break;
                case DiscountLimitationType.NTimesPerCustomer:
                    {
                        if (customer.IsRegistered())
                        {
                            var usedTimes = GetAllDiscountUsageHistory(discount.Id, customer.Id, null, 0, 1).TotalCount;
                            if (usedTimes >= discount.LimitationTimes)
                            {
                                result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsedAnymore") };
                                return result;
                            }
                        }
                    }
                    break;
                case DiscountLimitationType.Unlimited:
                default:
                    break;
            }

            //discount requirements
            var key = string.Format(DiscountEventConsumer.DISCOUNT_REQUIREMENT_MODEL_KEY, discount.Id);
            var requirementsForCaching = _cacheManager.Get(key, () =>
            {
                var requirements = GetAllDiscountRequirements(discount.Id, true);
                return GetReqirementsForCaching(requirements);
            });

            //get top-level group
            var topLevelGroup = requirementsForCaching.FirstOrDefault();
            if (topLevelGroup == null || (topLevelGroup.IsGroup && !topLevelGroup.ChildRequirements.Any()) || !topLevelGroup.InteractionType.HasValue)
            {
                //there are no requirements, so discount is valid
                result.IsValid = true;
                return result;
            }

            //requirements exist, let's check them
            var errors = new List<string>();
            result.IsValid = GetValidationResult(requirementsForCaching, topLevelGroup.InteractionType.Value, customer, errors);

            //set errors if result is not valid
            if (!result.IsValid)
                result.Errors = errors;

            return result;
        }

        #endregion

        #region Discount usage history

        /// <summary>
        /// Gets a discount usage history record
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history record identifier</param>
        /// <returns>Discount usage history</returns>
        public virtual DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId)
        {
            if (discountUsageHistoryId == 0)
                return null;

            return _discountUsageHistoryRepository.GetById(discountUsageHistoryId);
        }

        /// <summary>
        /// Gets all discount usage history records
        /// </summary>
        /// <param name="discountId">Discount identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Discount usage history records</returns>
        public virtual IPagedList<DiscountUsageHistory> GetAllDiscountUsageHistory(int? discountId = null,
            int? customerId = null, int? orderId = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var discountUsageHistory = _discountUsageHistoryRepository.Table;

            //filter by discount
            if (discountId.HasValue && discountId.Value > 0)
                discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.DiscountId == discountId.Value);

            //filter by customer
            if (customerId.HasValue && customerId.Value > 0)
                discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.Order != null && historyRecord.Order.CustomerId == customerId.Value);

            //filter by order
            if (orderId.HasValue && orderId.Value > 0)
                discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.OrderId == orderId.Value);

            //ignore deleted orders
            discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.Order != null && !historyRecord.Order.Deleted);

            //order
            discountUsageHistory = discountUsageHistory.OrderByDescending(historyRecord => historyRecord.CreatedOnUtc).ThenBy(historyRecord => historyRecord.Id);

            return new PagedList<DiscountUsageHistory>(discountUsageHistory, pageIndex, pageSize);
        }

        /// <summary>
        /// Insert discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void InsertDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Insert(discountUsageHistory);
            
            //event notification
            _eventPublisher.EntityInserted(discountUsageHistory);
        }

        /// <summary>
        /// Update discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void UpdateDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Update(discountUsageHistory);
            
            //event notification
            _eventPublisher.EntityUpdated(discountUsageHistory);
        }

        /// <summary>
        /// Delete discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void DeleteDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Delete(discountUsageHistory);
            
            //event notification
            _eventPublisher.EntityDeleted(discountUsageHistory);
        }

        #endregion

        #endregion
    }
}