using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Shipping.Date
{
    /// <summary>
    /// Represents the date range service
    /// </summary>
    public partial class DateRangeService : IDateRangeService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;

        #endregion

        #region Ctor

        public DateRangeService(ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository)
        {
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
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
        public virtual DeliveryDate GetDeliveryDateById(int deliveryDateId)
        {
            if (deliveryDateId == 0)
                return null;

            return _deliveryDateRepository.ToCachedGetById(deliveryDateId);
        }

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>Delivery dates</returns>
        public virtual IList<DeliveryDate> GetAllDeliveryDates()
        {
            var query = from dd in _deliveryDateRepository.Table
                        orderby dd.DisplayOrder, dd.Id
                        select dd;
            var deliveryDates = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopShippingDefaults.DeliveryDatesAllCacheKey));

            return deliveryDates;
        }

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public virtual void InsertDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            _deliveryDateRepository.Insert(deliveryDate);

            //event notification
            _eventPublisher.EntityInserted(deliveryDate);
        }

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public virtual void UpdateDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            _deliveryDateRepository.Update(deliveryDate);

            //event notification
            _eventPublisher.EntityUpdated(deliveryDate);
        }

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        public virtual void DeleteDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            _deliveryDateRepository.Delete(deliveryDate);

            //event notification
            _eventPublisher.EntityDeleted(deliveryDate);
        }

        #endregion

        #region Product availability ranges

        /// <summary>
        /// Get a product availability range
        /// </summary>
        /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
        /// <returns>Product availability range</returns>
        public virtual ProductAvailabilityRange GetProductAvailabilityRangeById(int productAvailabilityRangeId)
        {
            return productAvailabilityRangeId != 0 ? _productAvailabilityRangeRepository.ToCachedGetById(productAvailabilityRangeId) : null;
        }

        /// <summary>
        /// Get all product availability ranges
        /// </summary>
        /// <returns>Product availability ranges</returns>
        public virtual IList<ProductAvailabilityRange> GetAllProductAvailabilityRanges()
        {
            var query = from par in _productAvailabilityRangeRepository.Table
                        orderby par.DisplayOrder, par.Id
                        select par;

            return query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopShippingDefaults.ProductAvailabilityAllCacheKey));
        }

        /// <summary>
        /// Insert the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual void InsertProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            _productAvailabilityRangeRepository.Insert(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityInserted(productAvailabilityRange);
        }

        /// <summary>
        /// Update the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual void UpdateProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            _productAvailabilityRangeRepository.Update(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityUpdated(productAvailabilityRange);
        }

        /// <summary>
        /// Delete the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public virtual void DeleteProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            _productAvailabilityRangeRepository.Delete(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityDeleted(productAvailabilityRange);
        }

        #endregion

        #endregion
    }
}