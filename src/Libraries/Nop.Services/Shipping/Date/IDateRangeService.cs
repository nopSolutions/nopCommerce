using System.Collections.Generic;
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
        void DeleteDeliveryDate(DeliveryDate deliveryDate);

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>Delivery date</returns>
        DeliveryDate GetDeliveryDateById(int deliveryDateId);

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>Delivery dates</returns>
        IList<DeliveryDate> GetAllDeliveryDates();

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        void InsertDeliveryDate(DeliveryDate deliveryDate);

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        void UpdateDeliveryDate(DeliveryDate deliveryDate);

        #endregion

        #region Product availability ranges

        /// <summary>
        /// Get a product availability range
        /// </summary>
        /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
        /// <returns>Product availability range</returns>
        ProductAvailabilityRange GetProductAvailabilityRangeById(int productAvailabilityRangeId);

        /// <summary>
        /// Get all product availability ranges
        /// </summary>
        /// <returns>Product availability ranges</returns>
        IList<ProductAvailabilityRange> GetAllProductAvailabilityRanges();

        /// <summary>
        /// Insert the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        void InsertProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange);

        /// <summary>
        /// Update the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        void UpdateProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange);

        /// <summary>
        /// Delete the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        void DeleteProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange);

        #endregion
    }
}
