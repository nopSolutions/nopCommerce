using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;

namespace Nop.Services.Shipping;

/// <summary>
/// Warehouse service
/// </summary>
public partial class WarehouseService : IWarehouseService
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IRepository<Warehouse> _warehouseRepository;

    #endregion

    #region Ctor

    public WarehouseService(IAddressService addressService,
        IRepository<Warehouse> warehouseRepository)
    {
        _addressService = addressService;
        _warehouseRepository = warehouseRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a warehouse
    /// </summary>
    /// <param name="warehouse">The warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteWarehouseAsync(Warehouse warehouse)
    {
        await _warehouseRepository.DeleteAsync(warehouse);
    }

    /// <summary>
    /// Gets a warehouse
    /// </summary>
    /// <param name="warehouseId">The warehouse identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse
    /// </returns>
    public virtual async Task<Warehouse> GetWarehouseByIdAsync(int warehouseId)
    {
        return await _warehouseRepository.GetByIdAsync(warehouseId, _ => default);
    }

    /// <summary>
    /// Gets all warehouses
    /// </summary>
    /// <param name="name">Warehouse name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouses
    /// </returns>
    public virtual async Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(query =>
            from wh in query
            orderby wh.Name
            select wh, _ => default);

        if (!string.IsNullOrEmpty(name))
            warehouses = warehouses.Where(wh => wh.Name.Contains(name)).ToList();

        return warehouses;
    }

    /// <summary>
    /// Inserts a warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertWarehouseAsync(Warehouse warehouse)
    {
        await _warehouseRepository.InsertAsync(warehouse);
    }

    /// <summary>
    /// Updates the warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateWarehouseAsync(Warehouse warehouse)
    {
        await _warehouseRepository.UpdateAsync(warehouse);
    }

    /// <summary>
    /// Get the nearest warehouse for the specified address
    /// </summary>
    /// <param name="address">Address</param>
    /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public virtual async Task<Warehouse> GetNearestWarehouseAsync(Address address, IList<Warehouse> warehouses = null)
    {
        warehouses ??= await GetAllWarehousesAsync();

        //no address specified. return any
        if (address == null)
            return warehouses.FirstOrDefault();

        //of course, we should use some better logic to find nearest warehouse,
        //but we don't have a built-in geographic database which supports "distance" functionality
        //that's why we simply look for exact matches

        //find by country
        var matchedByCountry = new List<Warehouse>();
        foreach (var warehouse in warehouses)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
            if (warehouseAddress == null)
                continue;

            if (warehouseAddress.CountryId == address.CountryId)
                matchedByCountry.Add(warehouse);
        }
        //no country matches. return any
        if (!matchedByCountry.Any())
            return warehouses.FirstOrDefault();

        //find by state
        var matchedByState = new List<Warehouse>();
        foreach (var warehouse in matchedByCountry)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
            if (warehouseAddress == null)
                continue;

            if (warehouseAddress.StateProvinceId == address.StateProvinceId)
                matchedByState.Add(warehouse);
        }

        if (matchedByState.Any())
            return matchedByState.FirstOrDefault();

        //no state matches. return any
        return matchedByCountry.FirstOrDefault();
    }

    #endregion
}