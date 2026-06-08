using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;
using Nop.Data;

namespace Nop.Services.PriceLists;

/// <summary>
/// Price list service
/// </summary>
public partial class PriceListService : IPriceListService
{
    #region Fields

    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<CustomerRole> _customerRoleRepository;
    protected readonly IRepository<PriceList> _priceListRepository;
    protected readonly IRepository<PriceListCustomer> _priceListCustomerRepository;
    protected readonly IRepository<PriceListCustomerRole> _priceListCustomerRoleRepository;
    protected readonly IRepository<PriceListItem> _priceListItemRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public PriceListService(IRepository<Customer> customerRepository,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<PriceList> priceListRepository,
        IRepository<PriceListCustomer> priceListCustomerRepository,
        IRepository<PriceListCustomerRole> priceListCustomerRoleRepository,
        IRepository<PriceListItem> priceListItemRepository,
        IRepository<Product> productRepository,
        IStaticCacheManager staticCacheManager)
    {
        _customerRepository = customerRepository;
        _customerRoleRepository = customerRoleRepository;
        _priceListRepository = priceListRepository;
        _priceListCustomerRepository = priceListCustomerRepository;
        _priceListCustomerRoleRepository = priceListCustomerRoleRepository;
        _priceListItemRepository = priceListItemRepository;
        _productRepository = productRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a dictionary of all customer roles mapped by ID.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation and contains a dictionary of all customer roles mapped by ID.
    /// </returns>
    protected virtual async Task<IDictionary<int, CustomerRole>> GetAllCustomerRolesDictionaryAsync()
    {
        return await _staticCacheManager.GetAsync(
            _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<CustomerRole>.AllCacheKey),
            async () => await _customerRoleRepository.Table.ToDictionaryAsync(cr => cr.Id));
    }

    /// <summary>
    /// Apply price list filters to a query
    /// </summary>
    /// <param name="query">Initial query</param>
    /// <param name="customerRoleIds">Customer role identifiers to filter by</param>
    /// <param name="customerIds">Customer identifiers to filter by</param>
    /// <param name="isActive">Price list is active</param>
    /// <returns>Filtered query</returns>
    protected virtual IQueryable<PriceList> ApplyPriceListFilters(IQueryable<PriceList> query, int[] customerRoleIds = null, int[] customerIds = null, bool? isActive = null)
    {
        if (isActive.HasValue)
            query = query.Where(c => c.Active == isActive.Value);

        var hasCustomerRoleFilter = customerRoleIds != null && customerRoleIds.Length > 0;
        var hasCustomerFilter = customerIds != null && customerIds.Length > 0;

        if (hasCustomerRoleFilter || hasCustomerFilter)
        {
            IQueryable<PriceList> roleFilteredQuery = null;
            IQueryable<PriceList> customerFilteredQuery = null;
            IQueryable<PriceList> universalPriceListQuery = null;

            //get price lists that have no mappings (apply to all users)
            var priceListsWithMappings = _priceListRepository.Table
                .Join(_priceListCustomerRoleRepository.Table, x => x.Id, y => y.PriceListId, (x, y) => x.Id)
                .Union(_priceListRepository.Table
                    .Join(_priceListCustomerRepository.Table, x => x.Id, y => y.PriceListId, (x, y) => x.Id))
                .Distinct();

            universalPriceListQuery = query.Where(x => !priceListsWithMappings.Contains(x.Id));

            if (hasCustomerRoleFilter)
            {
                roleFilteredQuery = query.Join(_priceListCustomerRoleRepository.Table, x => x.Id, y => y.PriceListId,
                        (x, y) => new { PriceList = x, Mapping = y })
                    .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                    .Select(z => z.PriceList);
            }

            if (hasCustomerFilter)
            {
                customerFilteredQuery = query.Join(_priceListCustomerRepository.Table, x => x.Id, y => y.PriceListId,
                        (x, y) => new { PriceList = x, Mapping = y })
                    .Where(z => customerIds.Contains(z.Mapping.CustomerId))
                    .Select(z => z.PriceList);
            }

            if (hasCustomerRoleFilter && hasCustomerFilter)
                query = roleFilteredQuery.Union(customerFilteredQuery).Union(universalPriceListQuery);
            else if (hasCustomerRoleFilter)
                query = roleFilteredQuery.Union(universalPriceListQuery);
            else
                query = customerFilteredQuery.Union(universalPriceListQuery);

            query = query.Distinct();
        }

        query = query.OrderByDescending(c => c.Priority);

        return query;
    }

    #endregion

    #region Methods

    #region Price lists

    /// <summary>
    /// Get all price lists 
    /// </summary>
    /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all price lists; </param>
    /// <param name="customerIds">A list of customer identifiers to filter by (at least one match); pass null or empty list in order to load all price lists; </param>
    /// <param name="isActive">Price list is active; null to load all price lists</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price lists
    /// </returns>
    public virtual async Task<IList<PriceList>> GetAllPriceListsAsync(int[] customerRoleIds = null, int[] customerIds = null, bool? isActive = null)
    {
        return await _priceListRepository.GetAllAsync(query =>
        {
            return ApplyPriceListFilters(query, customerRoleIds, customerIds, isActive);
        }, cache => cache.PrepareKeyForDefaultCache(NopPriceListDefaults.PriceListAllCacheKey, customerRoleIds, customerIds, isActive));
    }

    /// <summary>
    /// Search price lists 
    /// </summary>
    /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all price lists; </param>
    /// <param name="customerIds">A list of customer identifiers to filter by (at least one match); pass null or empty list in order to load all price lists; </param>
    /// <param name="isActive">Price list is active; null to load all price lists</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price lists
    /// </returns>
    public virtual async Task<IPagedList<PriceList>> SearchPriceListsAsync(int[] customerRoleIds = null, int[] customerIds = null, bool? isActive = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var priceLists = await _priceListRepository.GetAllPagedAsync(query =>
        {
            return ApplyPriceListFilters(query, customerRoleIds, customerIds, isActive);
        }, pageIndex, pageSize);

        return priceLists;
    }

    /// <summary>
    /// Gets list of customer roles
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of customer roles
    /// </returns>
    public virtual async Task<IList<CustomerRole>> GetCustomerRolesAsync(PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(priceList);

        var allRolesById = await GetAllCustomerRolesDictionaryAsync();
        var mappings = await _priceListCustomerRoleRepository.GetAllAsync(query => query.Where(crm => crm.PriceListId == priceList.Id));

        var result = mappings.Select(mapping => allRolesById.TryGetValue(mapping.CustomerRoleId, out var role) ? role : null)
            .Where(cr => cr != null)
            .ToList();

        return result;
    }

    /// <summary>
    /// Get customer role identifiers
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    public virtual async Task<int[]> GetCustomerRoleIdsAsync(PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(priceList);

        return (await GetCustomerRolesAsync(priceList))
            .Select(cr => cr.Id)
            .ToArray();
    }

    /// <summary>
    /// Inserts price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPriceListAsync(PriceList priceList)
    {
        await _priceListRepository.InsertAsync(priceList);
    }

    /// <summary>
    /// Gets a price list
    /// </summary>
    /// <param name="priceListId">Price list identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list
    /// </returns>
    public virtual async Task<PriceList> GetPriceListByIdAsync(int priceListId)
    {
        return await _priceListRepository.GetByIdAsync(priceListId);
    }

    /// <summary>
    /// Get price lists by identifiers
    /// </summary>
    /// <param name="priceListIds">Price list identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price lists
    /// </returns>
    public virtual async Task<IList<PriceList>> GetPriceListsByIdsAsync(int[] priceListIds)
    {
        return await _priceListRepository.GetByIdsAsync(priceListIds, _ => default);
    }

    /// <summary>
    /// Updates the price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePriceListAsync(PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(priceList);

        await _priceListRepository.UpdateAsync(priceList);
    }

    /// <summary>
    /// Delete price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePriceListAsync(PriceList priceList)
    {
        await _priceListRepository.DeleteAsync(priceList);
    }

    #endregion

    #region Price list items

    /// <summary>
    /// Gets price list item collection
    /// </summary>
    /// <param name="priceListId">Price list identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item collection
    /// </returns>
    public virtual async Task<IPagedList<PriceListItem>> GetPriceListItemsByPriceListIdAsync(int priceListId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (priceListId == 0)
            return new PagedList<PriceListItem>(new List<PriceListItem>(), pageIndex, pageSize);

        var query =
            from pc in _priceListItemRepository.Table
            join p in _productRepository.Table on pc.ProductId equals p.Id
            where pc.PriceListId == priceListId && !p.Deleted
            orderby pc.Id
            select pc;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Calculates the adjusted price for a product based on the specified price list adjustment
    /// </summary>
    /// <param name="product">The product</param>
    /// <param name="priceList">The price list</param>
    /// <returns>
    /// The calculated price after applying the adjustment defined in the price list.
    /// </returns>
    public virtual decimal ApplyAdjustmentPrice(Product product, PriceList priceList)
    {
        var adjustmentType = (PriceCalculationTypeEnum)priceList.PriceCalculationTypeId;
        var value = priceList.PriceCalculationValue;
        var originalPrice = product.Price;

        return adjustmentType switch
        {
            PriceCalculationTypeEnum.PercentageDecrease => originalPrice - (originalPrice * value / 100m),
            PriceCalculationTypeEnum.PercentageIncrease => originalPrice + (originalPrice * value / 100m),
            PriceCalculationTypeEnum.AmountDecrease => originalPrice - value,
            PriceCalculationTypeEnum.AmountIncrease => originalPrice + value,
            PriceCalculationTypeEnum.FixedPrice => value,
            _ => throw new ArgumentOutOfRangeException(nameof(adjustmentType)),
        };
    }

    /// <summary>
    /// Gets a price list item
    /// </summary>
    /// <param name="priceListItemId">Price list item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item
    /// </returns>
    public virtual async Task<PriceListItem> GetPriceListItemByIdAsync(int priceListItemId)
    {
        return await _priceListItemRepository.GetByIdAsync(priceListItemId);
    }

    /// <summary>
    /// Deletes a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePriceListItemAsync(PriceListItem priceListItem)
    {
        await _priceListItemRepository.DeleteAsync(priceListItem);
    }

    /// <summary>
    /// Update a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdatePriceListItemAsync(PriceListItem priceListItem)
    {
        await _priceListItemRepository.UpdateAsync(priceListItem);
    }

    /// <summary>
    /// Inserts a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPriceListItemAsync(PriceListItem priceListItem)
    {
        await _priceListItemRepository.InsertAsync(priceListItem);
    }

    #endregion

    #region Customer mapping

    /// <summary>
    /// Gets price list customer mapping collection
    /// </summary>
    /// <param name="priceListId">Price list identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item collection
    /// </returns>
    public virtual async Task<IPagedList<PriceListCustomer>> GetPriceListCustomersByPriceListIdAsync(int priceListId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (priceListId == 0)
            return new PagedList<PriceListCustomer>(new List<PriceListCustomer>(), pageIndex, pageSize);

        var query =
            from plc in _priceListCustomerRepository.Table
            join c in _customerRepository.Table on plc.CustomerId equals c.Id
            where plc.PriceListId == priceListId && !c.Deleted
            orderby plc.Id
            select plc;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomerId">Price list customer mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list customer
    /// </returns>
    public virtual async Task<PriceListCustomer> GetPriceListCustomerByIdAsync(int priceListCustomerId)
    {
        return await _priceListCustomerRepository.GetByIdAsync(priceListCustomerId);
    }

    /// <summary>
    /// Deletes a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomer">Price list customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePriceListCustomerAsync(PriceListCustomer priceListCustomer)
    {
        await _priceListCustomerRepository.DeleteAsync(priceListCustomer);
    }

    /// <summary>
    /// Inserts a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomer">Price list customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPriceListCustomerAsync(PriceListCustomer priceListCustomer)
    {
        await _priceListCustomerRepository.InsertAsync(priceListCustomer);
    }

    #endregion

    #region Customer role mapping

    /// <summary>
    /// Add a price list-customer role mapping
    /// </summary>
    /// <param name="roleMapping">Price list-customer role mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AddCustomerRoleMappingAsync(PriceListCustomerRole roleMapping)
    {
        await _priceListCustomerRoleRepository.InsertAsync(roleMapping);
    }

    /// <summary>
    /// Remove a price list-customer role mapping
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <param name="customerRole">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveCustomerRoleMappingAsync(PriceList priceList, CustomerRole customerRole)
    {
        ArgumentNullException.ThrowIfNull(priceList);

        ArgumentNullException.ThrowIfNull(customerRole);

        var mapping = await _priceListCustomerRoleRepository.Table
            .SingleOrDefaultAsync(ccrm => ccrm.PriceListId == priceList.Id && ccrm.CustomerRoleId == customerRole.Id);

        if (mapping != null)
            await _priceListCustomerRoleRepository.DeleteAsync(mapping);
    }

    #endregion

    #endregion
}