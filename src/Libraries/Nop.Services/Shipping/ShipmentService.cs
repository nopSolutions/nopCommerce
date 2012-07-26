using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentService : IShipmentService
    {
        #region Fields

        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentOrderProductVariant> _sopvRepository;
        private readonly IEventPublisher _eventPublisher;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipmentRepository">Shipment repository</param>
        /// <param name="sopvRepository">Shipment order product variant repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ShipmentService(IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentOrderProductVariant> sopvRepository,
            IEventPublisher eventPublisher)
        {
            this._shipmentRepository = shipmentRepository;
            this._sopvRepository = sopvRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void DeleteShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Delete(shipment);

            //event notification
            _eventPublisher.EntityDeleted(shipment);
        }
        
        /// <summary>
        /// Search shipments
        /// </summary>
        /// <param name="createdFrom">Created date from; null to load all records</param>
        /// <param name="createdTo">Created date to; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        public virtual IPagedList<Shipment> GetAllShipments(DateTime? createdFrom, DateTime? createdTo, 
            int pageIndex, int pageSize)
        {
            var query = _shipmentRepository.Table;
            if (createdFrom.HasValue)
                query = query.Where(s => createdFrom.Value <= s.CreatedOnUtc);
            if (createdTo.HasValue)
                query = query.Where(s => createdTo.Value >= s.CreatedOnUtc);
            query = query.Where(s => s.Order != null && !s.Order.Deleted);
            query = query.OrderByDescending(s => s.CreatedOnUtc);

            var shipments = new PagedList<Shipment>(query, pageIndex, pageSize);
            return shipments;
        }

        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        public virtual IList<Shipment> GetShipmentsByIds(int[] shipmentIds)
        {
            if (shipmentIds == null || shipmentIds.Length == 0)
                return new List<Shipment>();

            var query = from o in _shipmentRepository.Table
                        where shipmentIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Shipment>();
            foreach (int id in shipmentIds)
            {
                var shipment = shipments.Find(x => x.Id == id);
                if (shipment != null)
                    sortedOrders.Add(shipment);
            }
            return sortedOrders;
        }

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>Shipment</returns>
        public virtual Shipment GetShipmentById(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            var shipment = _shipmentRepository.GetById(shipmentId);
            return shipment;
        }

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void InsertShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Insert(shipment);

            //event notification
            _eventPublisher.EntityInserted(shipment);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void UpdateShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Update(shipment);

            //event notification
            _eventPublisher.EntityUpdated(shipment);
        }


        
        /// <summary>
        /// Deletes a shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        public virtual void DeleteShipmentOrderProductVariant(ShipmentOrderProductVariant sopv)
        {
            if (sopv == null)
                throw new ArgumentNullException("sopv");

            _sopvRepository.Delete(sopv);

            //event notification
            _eventPublisher.EntityDeleted(sopv);
        }

        /// <summary>
        /// Gets a shipment order product variant
        /// </summary>
        /// <param name="sopvId">Shipment order product variant identifier</param>
        /// <returns>Shipment order product variant</returns>
        public virtual ShipmentOrderProductVariant GetShipmentOrderProductVariantById(int sopvId)
        {
            if (sopvId == 0)
                return null;

            var sopv = _sopvRepository.GetById(sopvId);
            return sopv;
        }
        
        /// <summary>
        /// Inserts a shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        public virtual void InsertShipmentOrderProductVariant(ShipmentOrderProductVariant sopv)
        {
            if (sopv == null)
                throw new ArgumentNullException("sopv");

            _sopvRepository.Insert(sopv);

            //event notification
            _eventPublisher.EntityInserted(sopv);
        }

        /// <summary>
        /// Updates the shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        public virtual void UpdateShipmentOrderProductVariant(ShipmentOrderProductVariant sopv)
        {
            if (sopv == null)
                throw new ArgumentNullException("sopv");

            _sopvRepository.Update(sopv);

            //event notification
            _eventPublisher.EntityUpdated(sopv);
        }
        #endregion
    }
}
