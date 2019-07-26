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
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service
    /// </summary>
    public partial class DiscountService : IDiscountService
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountPluginManager _discountPluginManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<DiscountRequirement> _discountRequirementRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public DiscountService(ICategoryService categoryService,
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<Category> categoryRepository,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext)
        {
            _categoryService = categoryService;
            _customerService = customerService;
            _discountPluginManager = discountPluginManager;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _categoryRepository = categoryRepository;
            _discountRepository = discountRepository;
            _discountRequirementRepository = discountRequirementRepository;
            _discountUsageHistoryRepository = discountUsageHistoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
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
                    var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                    result = GetValidationResult(requirement.ChildRequirements, interactionType, customer, errors);
                }
                else
                {
                    //or try to get validation result for the requirement
                    var requirementRulePlugin = _discountPluginManager
                        .LoadPluginBySystemName(requirement.SystemName, customer, _storeContext.CurrentStore.Id);
                    if (requirementRulePlugin == null)
                        continue;

                    var ruleResult = requirementRulePlugin.CheckRequirement(new DiscountRequirementValidationRequest
                    {
                        DiscountRequirementId = requirement.Id,
                        Customer = customer,
                        Store = _storeContext.CurrentStore
                    });

                    //add validation error
                    if (!ruleResult.IsValid)
                    {
                        var userError = !string.IsNullOrEmpty(ruleResult.UserError)
                            ? ruleResult.UserError
                            : _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsed");
                        errors.Add(userError);
                    }

                    result = ruleResult.IsValid;
                }

                //all requirements must be met, so return false
                if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                    return false;

                //any of requirements must be met, so return true
                if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                    return true;
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
                query = query.Where(discount =>
                    (!discount.StartDateUtc.HasValue || discount.StartDateUtc <= DateTime.UtcNow) &&
                    (!discount.EndDateUtc.HasValue || discount.EndDateUtc >= DateTime.UtcNow));
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
                categories = categories.Where(category => category.DiscountCategoryMappings.Any(mapping => mapping.DiscountId == discountId.Value));

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
                manufacturers = manufacturers.Where(manufacturer => manufacturer.DiscountManufacturerMappings.Any(mapping => mapping.DiscountId == discountId.Value));

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
                products = products.Where(product => product.DiscountProductMappings.Any(mapping => mapping.DiscountId == discountId.Value));

            if (!showHidden)
                products = products.Where(product => !product.Deleted);

            products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return new PagedList<Product>(products, pageIndex, pageSize);
        }

        #endregion

        #region Discounts (caching)

        /// <summary>
        /// Gets all discounts (cacheable models)
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
            //we have to use the following workaround with cacheable model (DiscountForCaching) because
            //Entity Framework doesn't support 2-level caching

            //we load all discounts, and filter them using "discountType" parameter later (in memory)
            //we do it because we know that this method is invoked several times per HTTP request with distinct "discountType" parameter
            //that's why let's access the database only once
            var cacheKey = string.Format(NopDiscountDefaults.DiscountAllCacheKey,
                showHidden, couponCode ?? string.Empty, discountName ?? string.Empty);
            var discounts = _cacheManager.Get(cacheKey, () => GetAllDiscounts(couponCode: couponCode, discountName: discountName, showHidden: showHidden)
                .Select(MapDiscount).ToList());

            //we know that this method is usually invoked multiple times
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
            var cacheKey = string.Format(NopDiscountDefaults.DiscountCategoryIdsModelCacheKey,
                discountId,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var result = _cacheManager.Get(cacheKey, () =>
            {
                var ids = new List<int>();
                var rootCategoryIds = _discountRepository.Table.Where(x => x.Id == discountId)
                        .SelectMany(x => x.DiscountCategoryMappings.Select(mapping => mapping.CategoryId))
                        .ToList();
                foreach (var categoryId in rootCategoryIds)
                {
                    if (!ids.Contains(categoryId))
                        ids.Add(categoryId);

                    if (!discount.AppliedToSubCategories)
                        continue;

                    //include subcategories
                    foreach (var childCategoryId in _categoryService.GetChildCategoryIds(categoryId, _storeContext.CurrentStore.Id))
                    {
                        if (!ids.Contains(childCategoryId))
                            ids.Add(childCategoryId);
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
            var cacheKey = string.Format(NopDiscountDefaults.DiscountManufacturerIdsModelCacheKey,
                discountId,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var result = _cacheManager.Get(cacheKey, () =>
            {
                return _discountRepository.Table.Where(x => x.Id == discountId)
                    .SelectMany(x => x.DiscountManufacturerMappings.Select(mapping => mapping.ManufacturerId))
                    .ToList();
            });

            return result;
        }

        /// <summary>
        /// Map a discount to the same class for caching
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Result</returns>
        public virtual DiscountForCaching MapDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return new DiscountForCaching
            {
                Id = discount.Id,
                Name = discount.Name,
                DiscountTypeId = discount.DiscountTypeId,
                UsePercentage = discount.UsePercentage,
                DiscountPercentage = discount.DiscountPercentage,
                DiscountAmount = discount.DiscountAmount,
                MaximumDiscountAmount = discount.MaximumDiscountAmount,
                StartDateUtc = discount.StartDateUtc,
                EndDateUtc = discount.EndDateUtc,
                RequiresCouponCode = discount.RequiresCouponCode,
                CouponCode = discount.CouponCode,
                IsCumulative = discount.IsCumulative,
                DiscountLimitationId = discount.DiscountLimitationId,
                LimitationTimes = discount.LimitationTimes,
                MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
                AppliedToSubCategories = discount.AppliedToSubCategories
            };
        }

        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public virtual decimal GetDiscountAmount(DiscountForCaching discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //calculate discount amount
            decimal result;
            if (discount.UsePercentage)
                result = (decimal)((float)amount * (float)discount.DiscountPercentage / 100f);
            else
                result = discount.DiscountAmount;

            //validate maximum discount amount
            if (discount.UsePercentage &&
                discount.MaximumDiscountAmount.HasValue &&
                result > discount.MaximumDiscountAmount.Value)
                result = discount.MaximumDiscountAmount.Value;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Get preferred discount (with maximum discount value)
        /// </summary>
        /// <param name="discounts">A list of discounts to check</param>
        /// <param name="amount">Amount (initial value)</param>
        /// <param name="discountAmount">Discount amount</param>
        /// <returns>Preferred discount</returns>
        public virtual List<DiscountForCaching> GetPreferredDiscount(IList<DiscountForCaching> discounts,
            decimal amount, out decimal discountAmount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            var result = new List<DiscountForCaching>();
            discountAmount = decimal.Zero;
            if (!discounts.Any())
                return result;

            //first we check simple discounts
            foreach (var discount in discounts)
            {
                var currentDiscountValue = GetDiscountAmount(discount, amount);
                if (currentDiscountValue <= discountAmount)
                    continue;

                discountAmount = currentDiscountValue;

                result.Clear();
                result.Add(discount);
            }
            //now let's check cumulative discounts
            //right now we calculate discount values based on the original amount value
            //please keep it in mind if you're going to use discounts with "percentage"
            var cumulativeDiscounts = discounts.Where(x => x.IsCumulative).OrderBy(x => x.Name).ToList();
            if (cumulativeDiscounts.Count <= 1)
                return result;

            var cumulativeDiscountAmount = cumulativeDiscounts.Sum(d => GetDiscountAmount(d, amount));
            if (cumulativeDiscountAmount <= discountAmount)
                return result;

            discountAmount = cumulativeDiscountAmount;

            result.Clear();
            result.AddRange(cumulativeDiscounts);

            return result;
        }

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount instance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        public virtual bool ContainsDiscount(IList<DiscountForCaching> discounts, DiscountForCaching discount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            foreach (var dis1 in discounts)
                if (discount.Id == dis1.Id)
                    return true;

            return false;
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

            return ValidateDiscount(MapDiscount(discount), customer);
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

            return ValidateDiscount(MapDiscount(discount), customer, couponCodesToValidate);
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

            var couponCodesToValidate = _customerService.ParseAppliedDiscountCouponCodes(customer);
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
                var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
                var cart = shoppingCartService.GetShoppingCart(customer,
                    ShoppingCartType.ShoppingCart, storeId: _storeContext.CurrentStore.Id);

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
            var key = string.Format(NopDiscountDefaults.DiscountRequirementModelCacheKey, discount.Id);
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