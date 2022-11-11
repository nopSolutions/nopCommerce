using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Services
{
    /// <summary>
    /// Represents service shipping by weight service
    /// </summary>
    public interface IShippingByWeightByTotalService
    {
        /// <summary>
        /// Get all shipping by weight records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the shipping by weight record
        /// </returns>
        Task<IPagedList<ShippingByWeightByTotalRecord>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);

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
        Task<ShippingByWeightByTotalRecord> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,  
            int countryId, int stateProvinceId, string zip, decimal weight, decimal orderSubtotal);

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
        Task<IPagedList<ShippingByWeightByTotalRecord>> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,
            int countryId, int stateProvinceId, string zip, decimal? weight, decimal? orderSubtotal, int pageIndex, int pageSize);

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="shippingByWeightRecordId">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping by weight record
        /// </returns>
        Task<ShippingByWeightByTotalRecord> GetByIdAsync(int shippingByWeightRecordId);

        /// <summary>
        /// Insert the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord);

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord);

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteShippingByWeightRecordAsync(ShippingByWeightByTotalRecord shippingByWeightRecord);
    }
}
