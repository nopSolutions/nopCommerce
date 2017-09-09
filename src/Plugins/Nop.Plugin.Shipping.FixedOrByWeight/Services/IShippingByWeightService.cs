using Nop.Core;
using Nop.Plugin.Shipping.FixedOrByWeight.Domain;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Services
{
    /// <summary>
    /// Represents service shipping by weight service
    /// </summary>
    public partial interface IShippingByWeightService
    {
        /// <summary>
        /// Get all shipping by weight records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of the shipping by weight record</returns>
        IPagedList<ShippingByWeightRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

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
        /// <returns>Shipping by weight record</returns>
        ShippingByWeightRecord FindRecord(int shippingMethodId, int storeId, int warehouseId,  
            int countryId, int stateProvinceId, string zip, decimal weight);

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="shippingByWeightRecordId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>
        ShippingByWeightRecord GetById(int shippingByWeightRecordId);

        /// <summary>
        /// Insert the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        void InsertShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        void UpdateShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        void DeleteShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);
    }
}
