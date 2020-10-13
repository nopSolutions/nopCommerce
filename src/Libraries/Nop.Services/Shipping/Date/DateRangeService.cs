using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Data;

namespace Nop.Services.Shipping.Date
{
    /// <summary>
    /// Represents the date range service
    /// </summary>
    public partial class DateRangeService : IDateRangeService
    {
        #region Fields

        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;

        #endregion

        #region Ctor

        public DateRangeService(IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository)
        {
            _deliveryDateRepository = deliveryDateRepository;
            _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
        }

        #endregion

        #region Methods

        #region Delivery dates

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>Delivery date</returns>
        public virtual async Task<DeliveryDate> GetDeliveryDateById(int deliveryDateId)
        {
            return await _deliveryDateRepository.GetById(deliveryDateId, cache => default);
        }

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>Delivery dates</returns>
        public virtual async Task<IList<DeliveryDate>> GetAllDeliveryDates()
        {
            var deliveryDates = await _deliveryDateRepository.GetAll(query =>
            {
                return from dd in query
                    orderby dd.DisplayOrder, dd.Id
                    select dd;
            }, cache => default);

            return deliveryDates;
        }

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public virtual async Task InsertDeliveryDate(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.Insert(deliveryDate);
        }

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public virtual async Task UpdateDeliveryDate(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.Update(deliveryDate);
        }

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        public virtual async Task DeleteDeliveryDate(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.Delete(deliveryDate);
        }

        #endregion

        #region Product availability ranges

        /// <summary>
        /// Get a product availability range
        /// </summary>
        /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
        /// <returns>Product availability range</returns>
        public virtual async Task<ProductAvailabilityRange> GetProductAvailabilityRangeById(int productAvailabilityRangeId)
        {
            return productAvailabilityRangeId != 0 ? await _productAvailabilityRangeRepository.GetById(productAvailabilityRangeId, cache => default) : null;
        }

        /// <summary>
        /// Get all product availability ranges
        /// </summary>
        /// <returns>Product availability ranges</returns>
        public virtual async Task<IList<ProductAvailabilityRange>> GetAllProductAvailabilityRanges()
        {
            return await _productAvailabilityRangeRepository.GetAll(query =>
            {
                return from par in query
                    orderby par.DisplayOrder, par.Id
                    select par;
            }, cache => default);
        }

        /// <summary>
        /// Insert the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual async Task InsertProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            await _productAvailabilityRangeRepository.Insert(productAvailabilityRange);
        }

        /// <summary>
        /// Update the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual async Task UpdateProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            await _productAvailabilityRangeRepository.Update(productAvailabilityRange);
        }

        /// <summary>
        /// Delete the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual async Task DeleteProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            await _productAvailabilityRangeRepository.Delete(productAvailabilityRange);
        }

        #endregion

        #endregion
    }
}