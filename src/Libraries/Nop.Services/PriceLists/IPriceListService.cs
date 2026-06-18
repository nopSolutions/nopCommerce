using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;

namespace Nop.Services.PriceLists;

/// <summary>
/// Price list service interface
/// </summary>
public partial interface IPriceListService
{
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
    Task<IList<PriceList>> GetAllPriceListsAsync(int[] customerRoleIds = null, int[] customerIds = null, bool? isActive = null);

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
    Task<IPagedList<PriceList>> SearchPriceListsAsync(int[] customerRoleIds = null, int[] customerIds = null, bool? isActive = null,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Gets list of customer roles
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of customer roles
    /// </returns>
    Task<IList<CustomerRole>> GetCustomerRolesAsync(PriceList priceList);

    /// <summary>
    /// Get customer role identifiers
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    Task<int[]> GetCustomerRoleIdsAsync(PriceList priceList);

    /// <summary>
    /// Inserts price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertPriceListAsync(PriceList priceList);

    /// <summary>
    /// Gets a price list
    /// </summary>
    /// <param name="priceListId">Price list identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list
    /// </returns>
    Task<PriceList> GetPriceListByIdAsync(int priceListId);

    /// <summary>
    /// Get price lists by identifiers
    /// </summary>
    /// <param name="priceListIds">Price list identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price lists
    /// </returns>
    Task<IList<PriceList>> GetPriceListsByIdsAsync(int[] priceListIds);

    /// <summary>
    /// Updates the price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdatePriceListAsync(PriceList priceList);

    /// <summary>
    /// Delete price list
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeletePriceListAsync(PriceList priceList);

    /// <summary>
    /// Gets price list product mapping collection
    /// </summary>
    /// <param name="priceListId">Price list identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list product mapping collection
    /// </returns>
    Task<IPagedList<PriceListItem>> GetPriceListItemsByPriceListIdAsync(int priceListId,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Calculates the adjusted price for a product based on the specified price list adjustment
    /// </summary>
    /// <param name="product">The product</param>
    /// <param name="priceList">The price list</param>
    /// <returns>
    /// The calculated price after applying the adjustment defined in the price list.
    /// </returns>
    decimal ApplyAdjustmentPrice(Product product, PriceList priceList);

    /// <summary>
    /// Gets a price list item
    /// </summary>
    /// <param name="priceListItemId">Price list item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item
    /// </returns>
    Task<PriceListItem> GetPriceListItemByIdAsync(int priceListItemId);

    /// <summary>
    /// Deletes a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeletePriceListItemAsync(PriceListItem priceListItem);

    /// <summary>
    /// Update a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdatePriceListItemAsync(PriceListItem priceListItem);

    /// <summary>
    /// Inserts a price list item
    /// </summary>
    /// <param name="priceListItem">Price list item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertPriceListItemAsync(PriceListItem priceListItem);

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
    Task<IPagedList<PriceListCustomer>> GetPriceListCustomersByPriceListIdAsync(int priceListId,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Gets a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomerId">Price list customer mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item
    /// </returns>
    Task<PriceListCustomer> GetPriceListCustomerByIdAsync(int priceListCustomerId);

    /// <summary>
    /// Deletes a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomer">Price list customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeletePriceListCustomerAsync(PriceListCustomer priceListCustomer);

    /// <summary>
    /// Inserts a price list customer mapping
    /// </summary>
    /// <param name="priceListCustomer">Price list customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertPriceListCustomerAsync(PriceListCustomer priceListCustomer);

    /// <summary>
    /// Add a price list-customer role mapping
    /// </summary>
    /// <param name="roleMapping">Price list-customer role mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddCustomerRoleMappingAsync(PriceListCustomerRole roleMapping);

    /// <summary>
    /// Remove a price list-customer role mapping
    /// </summary>
    /// <param name="priceList">Price list</param>
    /// <param name="customerRole">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task RemoveCustomerRoleMappingAsync(PriceList priceList, CustomerRole customerRole);
}
