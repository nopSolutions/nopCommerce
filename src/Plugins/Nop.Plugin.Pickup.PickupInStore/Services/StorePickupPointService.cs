using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service
    /// </summary>
    public partial class StorePickupPointService : IStorePickupPointService
    {
        #region Constants

        /// <summary>
        /// Cache key for pickup points
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        private readonly CacheKey _pickupPointAllKey = new CacheKey("Nop.pickuppoint.all-{0}");
        private const string PICKUP_POINT_PATTERN_KEY = "Nop.pickuppoint.";

        #endregion

        #region Fields

        private readonly IRepository<StorePickupPoint> _storePickupPointRepository;
        private readonly IStaticCacheManager _staticCacheManager;

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
        /// <returns>Pickup points</returns>
        public virtual IPagedList<StorePickupPoint> GetAllStorePickupPoints(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rez = _storePickupPointRepository.GetAll(query =>
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
        /// <returns>Pickup point</returns>
        public virtual StorePickupPoint GetStorePickupPointById(int pickupPointId)
        {
            return _storePickupPointRepository.GetById(pickupPointId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void InsertStorePickupPoint(StorePickupPoint pickupPoint)
        {
            _storePickupPointRepository.Insert(pickupPoint, false);
            _staticCacheManager.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void UpdateStorePickupPoint(StorePickupPoint pickupPoint)
        {
            _storePickupPointRepository.Update(pickupPoint, false);
            _staticCacheManager.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void DeleteStorePickupPoint(StorePickupPoint pickupPoint)
        {
            _storePickupPointRepository.Delete(pickupPoint, false);
            _staticCacheManager.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        #endregion
    }
}