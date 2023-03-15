using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service
    /// </summary>
    public class StorePickupPointService : IStorePickupPointService
    {
        #region Constants

        /// <summary>
        /// Cache key for pickup points
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        protected readonly CacheKey _pickupPointAllKey = new("Nop.pickuppoint.all-{0}", PICKUP_POINT_PATTERN_KEY);
        protected const string PICKUP_POINT_PATTERN_KEY = "Nop.pickuppoint.";

        #endregion

        #region Fields

        protected readonly IRepository<StorePickupPoint> _storePickupPointRepository;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="storePickupPointRepository">Store pickup point repository</param>
        /// <param name="staticCacheManager">Cache manager</param>
        public StorePickupPointService(IRepository<StorePickupPoint> storePickupPointRepository,
            IStaticCacheManager staticCacheManager)
        {
            _storePickupPointRepository = storePickupPointRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

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
        public virtual async Task<IPagedList<StorePickupPoint>> GetAllStorePickupPointsAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rez = await _storePickupPointRepository.GetAllAsync(query =>
            {
                if (storeId > 0)
                    query = query.Where(point => point.StoreId == storeId || point.StoreId == 0);
                query = query.OrderBy(point => point.DisplayOrder).ThenBy(point => point.Name);

                return query;
            }, cache => cache.PrepareKeyForShortTermCache(_pickupPointAllKey, storeId));

            return new PagedList<StorePickupPoint>(rez, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup point
        /// </returns>
        public virtual async Task<StorePickupPoint> GetStorePickupPointByIdAsync(int pickupPointId)
        {
            return await _storePickupPointRepository.GetByIdAsync(pickupPointId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertStorePickupPointAsync(StorePickupPoint pickupPoint)
        {
            await _storePickupPointRepository.InsertAsync(pickupPoint, false);
            await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateStorePickupPointAsync(StorePickupPoint pickupPoint)
        {
            await _storePickupPointRepository.UpdateAsync(pickupPoint, false);
            await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteStorePickupPointAsync(StorePickupPoint pickupPoint)
        {
            await _storePickupPointRepository.DeleteAsync(pickupPoint, false);
            await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
        }

        #endregion
    }
}