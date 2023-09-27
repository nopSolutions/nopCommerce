using Nop.Core;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public interface IStorePickupPointService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup points
        /// </returns>
        Task<IPagedList<StorePickupPoint>> GetAllStorePickupPointsAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup point
        /// </returns>
        Task<StorePickupPoint> GetStorePickupPointByIdAsync(int pickupPointId);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertStorePickupPointAsync(StorePickupPoint pickupPoint);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateStorePickupPointAsync(StorePickupPoint pickupPoint);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteStorePickupPointAsync(StorePickupPoint pickupPoint);
    }
}