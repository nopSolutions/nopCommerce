using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Services
{
    /// <summary>
    /// Represents service shipping by weight service implementation
    /// </summary>
    public partial class ShippingByWeightByTotalService : IShippingByWeightByTotalService
    {
        #region Constants

        /// <summary>
        /// Key for caching all records
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string SHIPPINGBYWEIGHTBYTOTAL_ALL_KEY = "Nop.shippingbyweightbytotal.all-{0}-{1}";
        private const string SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY = "Nop.shippingbyweightbytotal.";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ShippingByWeightByTotalRecord> _sbwtRepository;

        #endregion

        #region Ctor

        public ShippingByWeightByTotalService(IRepository<ShippingByWeightByTotalRecord> sbwtRepository,
            ICacheManager cacheManager)
        {
            _sbwtRepository = sbwtRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all shipping by weight records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of the shipping by weight record</returns>
        public virtual IPagedList<ShippingByWeightByTotalRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(SHIPPINGBYWEIGHTBYTOTAL_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbw in _sbwtRepository.Table
                            orderby sbw.StoreId, sbw.CountryId, sbw.StateProvinceId, sbw.Zip, sbw.ShippingMethodId, sbw.WeightFrom, sbw.OrderSubtotalFrom
                            select sbw;

                var records = new PagedList<ShippingByWeightByTotalRecord>(query, pageIndex, pageSize);
                return records;
            });
        }

        /// <summary>
        /// Filter Shipping Weight Records
        /// </summary>
        /// <param name="shippingMethodId">Shipping method identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State identifier</param>
        /// <param name="zip">Zip postal code</param>
        /// <param name="weight">Weight</param>
        /// <param name="orderSubtotal">Order subtotal</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of the shipping by weight record</returns>
        public virtual IPagedList<ShippingByWeightByTotalRecord> FindRecords(int shippingMethodId, int storeId, int warehouseId,
            int countryId, int stateProvinceId, string zip, decimal? weight, decimal? orderSubtotal, int pageIndex, int pageSize)
        {
            zip = zip?.Trim() ?? string.Empty;

            //filter by weight and shipping method
            var existingRates = GetAll()
                .Where(sbw => sbw.ShippingMethodId == shippingMethodId && (!weight.HasValue || weight >= sbw.WeightFrom && weight <= sbw.WeightTo))
                .ToList();

            //filter by order subtotal
            var matchedBySubtotal = !orderSubtotal.HasValue ? existingRates :
                existingRates.Where(sbw => orderSubtotal >= sbw.OrderSubtotalFrom && orderSubtotal <= sbw.OrderSubtotalTo);

            //filter by store
            var matchedByStore = storeId == 0
                ? matchedBySubtotal
                : matchedBySubtotal.Where(r => r.StoreId == storeId || r.StoreId == 0);

            //filter by warehouse
            var matchedByWarehouse = warehouseId == 0
                ? matchedByStore
                : matchedByStore.Where(r => r.WarehouseId == warehouseId || r.WarehouseId == 0);

            //filter by country
            var matchedByCountry = countryId == 0
                ? matchedByWarehouse
                : matchedByWarehouse.Where(r => r.CountryId == countryId || r.CountryId == 0);

            //filter by state/province
            var matchedByStateProvince = stateProvinceId == 0
                ? matchedByCountry
                : matchedByCountry.Where(r => r.StateProvinceId == stateProvinceId || r.StateProvinceId == 0);

            //filter by zip
            var matchedByZip = string.IsNullOrEmpty(zip)
                ? matchedByStateProvince
                : matchedByStateProvince.Where(r => string.IsNullOrEmpty(r.Zip) || r.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase));

            //sort from particular to general, more particular cases will be the first
            var foundRecords = matchedByZip.OrderBy(r => r.StoreId == 0).ThenBy(r => r.WarehouseId == 0)
                .ThenBy(r => r.CountryId == 0).ThenBy(r => r.StateProvinceId == 0)
                .ThenBy(r => string.IsNullOrEmpty(r.Zip));

            var records = new PagedList<ShippingByWeightByTotalRecord>(foundRecords.AsQueryable(), pageIndex, pageSize);
            return records;
        }

        /// <summary>
        /// Get a shipping by weight record by passed parameters
        /// </summary>
        /// <param name="shippingMethodId">Shipping method identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State identifier</param>
        /// <param name="zip">Zip postal code</param>
        /// <param name="weight">Weight</param>
        /// <param name="orderSubtotal">Order subtotal</param>
        /// <returns>Shipping by weight record</returns>
        public virtual ShippingByWeightByTotalRecord FindRecords(int shippingMethodId, int storeId, int warehouseId, 
            int countryId, int stateProvinceId, string zip, decimal weight, decimal orderSubtotal)
        {
            var foundRecords = FindRecords(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip, weight, orderSubtotal, 0, int.MaxValue);

            return foundRecords.FirstOrDefault();
        }

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="shippingByWeightRecordId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>
        public virtual ShippingByWeightByTotalRecord GetById(int shippingByWeightRecordId)
        {
            if (shippingByWeightRecordId == 0)
                return null;

            return _sbwtRepository.GetById(shippingByWeightRecordId);
        }

        /// <summary>
        /// Insert the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void InsertShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Insert(shippingByWeightRecord);

            _cacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void UpdateShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Update(shippingByWeightRecord);

            _cacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void DeleteShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Delete(shippingByWeightRecord);

            _cacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        #endregion
    }
}
