using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Shipping.FixedOrByWeight.Domain;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Services
{
    public partial class ShippingByWeightService : IShippingByWeightService
    {
        #region Constants

        private const string SHIPPINGBYWEIGHT_ALL_KEY = "Nop.shippingbyweight.all-{0}-{1}";
        private const string SHIPPINGBYWEIGHT_PATTERN_KEY = "Nop.shippingbyweight.";

        #endregion

        #region Fields

        private readonly IRepository<ShippingByWeightRecord> _sbwRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public ShippingByWeightService(ICacheManager cacheManager,
            IRepository<ShippingByWeightRecord> sbwRepository)
        {
            this._cacheManager = cacheManager;
            this._sbwRepository = sbwRepository;
        }

        #endregion

        #region Methods

        public virtual void DeleteShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwRepository.Delete(shippingByWeightRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual IPagedList<ShippingByWeightRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYWEIGHT_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            orderby sbw.StoreId, sbw.CountryId, sbw.StateProvinceId, sbw.Zip, sbw.ShippingMethodId, sbw.From
                            select sbw;

                var records = new PagedList<ShippingByWeightRecord>(query, pageIndex, pageSize);
                return records;
            });
        }

        public virtual ShippingByWeightRecord FindRecord(int shippingMethodId,
            int storeId, int warehouseId, 
            int countryId, int stateProvinceId, string zip, decimal weight)
        {
            if (zip == null)
                zip = string.Empty;
            zip = zip.Trim();

            //filter by weight and shipping method
            var existingRates = GetAll()
                .Where(sbw => sbw.ShippingMethodId == shippingMethodId && weight >= sbw.From && weight <= sbw.To)
                .ToList();

            //filter by store
            var matchedByStore = storeId == 0
                ? existingRates
                : existingRates.Where(r => r.StoreId == storeId || r.StoreId == 0);
           
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

            return foundRecords.FirstOrDefault();
        }

        public virtual ShippingByWeightRecord GetById(int shippingByWeightRecordId)
        {
            if (shippingByWeightRecordId == 0)
                return null;

            return _sbwRepository.GetById(shippingByWeightRecordId);
        }

        public virtual void InsertShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwRepository.Insert(shippingByWeightRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual void UpdateShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwRepository.Update(shippingByWeightRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        #endregion
    }
}
