using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service interface
    /// </summary>
    public partial interface IShipmentService
    {
        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        void DeleteShipment(Shipment shipment);

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="shippedFrom">Date shipped from; null to load all records</param>
        /// <param name="shippedTo">Date shipped to; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        IPagedList<Shipment> GetAllShipments(DateTime? shippedFrom, DateTime? shippedTo, 
            int pageIndex, int pageSize);
        
        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        IList<Shipment> GetShipmentsByIds(int[] shipmentIds);

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>Shipment</returns>
        Shipment GetShipmentById(int shipmentId);

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        void InsertShipment(Shipment shipment);

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        void UpdateShipment(Shipment shipment);



        /// <summary>
        /// Deletes a shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        void DeleteShipmentOrderProductVariant(ShipmentOrderProductVariant sopv);

        /// <summary>
        /// Gets a shipment order product variant
        /// </summary>
        /// <param name="sopvId">Shipment order product variant identifier</param>
        /// <returns>Shipment order product variant</returns>
        ShipmentOrderProductVariant GetShipmentOrderProductVariantById(int sopvId);

        /// <summary>
        /// Inserts a shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        void InsertShipmentOrderProductVariant(ShipmentOrderProductVariant sopv);

        /// <summary>
        /// Updates the shipment order product variant
        /// </summary>
        /// <param name="sopv">Shipment order product variant</param>
        void UpdateShipmentOrderProductVariant(ShipmentOrderProductVariant sopv);
    }
}
