using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Services;

/// <summary>
/// Represents service shipping by weight service implementation
/// </summary>
public class ShippingByWeightByTotalService : IShippingByWeightByTotalService
{
    #region Fields

    protected readonly FixedByWeightByTotalSettings _pluginSettings;
    protected readonly IRepository<ShippingByWeightByTotalRecord> _sbwtRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ShippingByWeightByTotalService(FixedByWeightByTotalSettings pluginSettings,
        IRepository<ShippingByWeightByTotalRecord> sbwtRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager)
    {
        _pluginSettings = pluginSettings;
        _sbwtRepository = sbwtRepository;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get filtered shipping by weight records
    /// </summary>
    /// <param name="shippingMethodId">Shipping method identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="warehouseId">Warehouse identifier</param>
    /// <param name="countryId">Country identifier</param>
    /// <param name="stateProvinceId">State identifier</param>
    /// <param name="zip">Zip postal code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the shipping by weight record
    /// </returns>
    private async Task<IList<ShippingByWeightByTotalRecord>> GetRecordsAsync(int shippingMethodId,
        int storeId,
        int warehouseId,
        int countryId,
        int stateProvinceId,
        string zip)
    {
        var rez = await _shortTermCacheManager.GetAsync(async ()=> await _sbwtRepository.GetAllAsync(async query =>
        {
            var data = _pluginSettings.LoadAllRecord
                ? (await _shortTermCacheManager.GetAsync(async () => await _sbwtRepository.GetAllAsync(q => q), FixedByWeightByTotalDefaults.ShippingByWeightByTotalCacheKey, null, null, null, null, null, null)).AsQueryable()
                : query;

            //filter by shipping method
            data = data.Where(sbw => sbw.ShippingMethodId == shippingMethodId);

            //filter by store
            data = storeId == 0
                ? data
                : data.Where(r => r.StoreId == storeId || r.StoreId == 0);

            //filter by warehouse
            data = warehouseId == 0
                ? data
                : data.Where(r => r.WarehouseId == warehouseId || r.WarehouseId == 0);

            //filter by country
            data = countryId == 0
                ? data
                : data.Where(r => r.CountryId == countryId || r.CountryId == 0);

            //filter by state/province
            data = stateProvinceId == 0
                ? data
                : data.Where(r => r.StateProvinceId == stateProvinceId || r.StateProvinceId == 0);

            zip = zip?.Trim() ?? string.Empty;

            //filter by zip
            data = string.IsNullOrEmpty(zip)
                ? data
                : data.Where(r => string.IsNullOrEmpty(r.Zip) || r.Zip.Equals(zip));

            data = data.OrderBy(sbw => sbw.StoreId)
                .ThenBy(sbw => sbw.CountryId)
                .ThenBy(sbw => sbw.StateProvinceId)
                .ThenBy(sbw => sbw.Zip)
                .ThenBy(sbw => sbw.ShippingMethodId)
                .ThenBy(sbw => sbw.WeightFrom)
                .ThenBy(sbw => sbw.OrderSubtotalFrom);

            return data;
        }), FixedByWeightByTotalDefaults.ShippingByWeightByTotalCacheKey, shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip);
            
        return rez;
    }

    #endregion

    #region Methods

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the shipping by weight record
    /// </returns>
    public virtual async Task<IPagedList<ShippingByWeightByTotalRecord>> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,
        int countryId, int stateProvinceId, string zip, decimal? weight, decimal? orderSubtotal, int pageIndex, int pageSize)
    {
        //filter by weight
        var existingRates =
            (await GetRecordsAsync(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip))
            .Where(sbw => !weight.HasValue || weight >= sbw.WeightFrom && weight <= sbw.WeightTo);
                
        //filter by order subtotal
        existingRates = !orderSubtotal.HasValue ? existingRates :
            existingRates.Where(sbw => orderSubtotal >= sbw.OrderSubtotalFrom && orderSubtotal <= sbw.OrderSubtotalTo);

        //sort from particular to general, more particular cases will be the first
        existingRates = existingRates
            .OrderBy(r => r.StoreId == 0)
            .ThenBy(r => r.WarehouseId == 0)
            .ThenBy(r => r.CountryId == 0)
            .ThenBy(r => r.StateProvinceId == 0)
            .ThenBy(r => string.IsNullOrEmpty(r.Zip));

        var records = new PagedList<ShippingByWeightByTotalRecord>(existingRates.ToList(), pageIndex, pageSize);

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight record
    /// </returns>
    public virtual async Task<ShippingByWeightByTotalRecord> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,
        int countryId, int stateProvinceId, string zip, decimal weight, decimal orderSubtotal)
    {
        var foundRecords = await FindRecordsAsync(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip, weight, orderSubtotal, 0, int.MaxValue);

        return foundRecords.FirstOrDefault();
    }

    /// <summary>
    /// Get a shipping by weight record by identifier
    /// </summary>
    /// <param name="shippingByWeightRecordId">Record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight record
    /// </returns>
    public virtual async Task<ShippingByWeightByTotalRecord> GetByIdAsync(int shippingByWeightRecordId)
    {
        return await _sbwtRepository.GetByIdAsync(shippingByWeightRecordId);
    }

    /// <summary>
    /// Insert the shipping by weight record
    /// </summary>
    /// <param name="shippingByWeightRecord">Shipping by weight record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        await _sbwtRepository.InsertAsync(shippingByWeightRecord, false);

        await _staticCacheManager.RemoveByPrefixAsync(FixedByWeightByTotalDefaults.ShippingByWeightByTotalCachePrefix);
    }

    /// <summary>
    /// Update the shipping by weight record
    /// </summary>
    /// <param name="shippingByWeightRecord">Shipping by weight record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        await _sbwtRepository.UpdateAsync(shippingByWeightRecord, false);

        await _staticCacheManager.RemoveByPrefixAsync(FixedByWeightByTotalDefaults.ShippingByWeightByTotalCachePrefix);
    }

    /// <summary>
    /// Delete the shipping by weight record
    /// </summary>
    /// <param name="shippingByWeightRecord">Shipping by weight record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        await _sbwtRepository.DeleteAsync(shippingByWeightRecord, false);

        await _staticCacheManager.RemoveByPrefixAsync(FixedByWeightByTotalDefaults.ShippingByWeightByTotalCachePrefix);
    }

    #endregion
}