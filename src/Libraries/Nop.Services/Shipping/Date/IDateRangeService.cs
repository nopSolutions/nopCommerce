using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Date
{
    /// <summary>
    /// Date range service interface
    /// </summary>
    public partial interface IDateRangeService
    {
        #region Delivery dates

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate);

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>Delivery date</returns>
        Task<DeliveryDate> GetDeliveryDateByIdAsync(int deliveryDateId);

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>Delivery dates</returns>
        Task<IList<DeliveryDate>> GetAllDeliveryDatesAsync();

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        Task InsertDeliveryDateAsync(DeliveryDate deliveryDate);

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate);

        #endregion

        #region Product availability ranges

        /// <summary>
        /// Get a product availability range
        /// </summary>
        /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
        /// <returns>Product availability range</returns>
        Task<ProductAvailabilityRange> GetProductAvailabilityRangeByIdAsync(int productAvailabilityRangeId);

        /// <summary>
        /// Get all product availability ranges
        /// </summary>
        /// <returns>Product availability ranges</returns>
        Task<IList<ProductAvailabilityRange>> GetAllProductAvailabilityRangesAsync();

        /// <summary>
        /// Insert the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        Task InsertProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange);

        /// <summary>
        /// Update the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        Task UpdateProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange);

        /// <summary>
        /// Delete the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        Task DeleteProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange);

        #endregion
    }
}
