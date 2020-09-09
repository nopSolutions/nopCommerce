using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public partial interface IStorePickupPointService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        Task<IPagedList<StorePickupPoint>> GetAllStorePickupPoints(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        Task<StorePickupPoint> GetStorePickupPointById(int pickupPointId);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        Task InsertStorePickupPoint(StorePickupPoint pickupPoint);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        Task UpdateStorePickupPoint(StorePickupPoint pickupPoint);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        Task DeleteStorePickupPoint(StorePickupPoint pickupPoint);
    }
}