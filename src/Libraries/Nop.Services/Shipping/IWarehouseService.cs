using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping;

/// <summary>
/// Warehouse service interface
/// </summary>
public partial interface IWarehouseService
{
    /// <summary>
    /// Deletes a warehouse
    /// </summary>
    /// <param name="warehouse">The warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteWarehouseAsync(Warehouse warehouse);

    /// <summary>
    /// Gets a warehouse
    /// </summary>
    /// <param name="warehouseId">The warehouse identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse
    /// </returns>
    Task<Warehouse> GetWarehouseByIdAsync(int warehouseId);

    /// <summary>
    /// Gets all warehouses
    /// </summary>
    /// <param name="name">Warehouse name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouses
    /// </returns>
    Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null);

    /// <summary>
    /// Inserts a warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertWarehouseAsync(Warehouse warehouse);

    /// <summary>
    /// Updates the warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateWarehouseAsync(Warehouse warehouse);

    /// <summary>
    /// Get the nearest warehouse for the specified address
    /// </summary>
    /// <param name="address">Address</param>
    /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    Task<Warehouse> GetNearestWarehouseAsync(Address address, IList<Warehouse> warehouses = null);
}