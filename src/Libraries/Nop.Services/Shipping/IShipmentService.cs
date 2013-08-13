using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Shipping;

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
        /// Search shipments
        /// </summary>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        IPagedList<Shipment> GetAllShipments(int vendorId,
            int shippingCountryId,
            int shippingStateId,
            string shippingCity,
            string trackingNumber,
            DateTime? createdFromUtc, DateTime? createdToUtc,
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
        /// Deletes a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        void DeleteShipmentItem(ShipmentItem shipmentItem);

        /// <summary>
        /// Gets a shipment item
        /// </summary>
        /// <param name="shipmentItemId">Shipment item identifier</param>
        /// <returns>Shipment item</returns>
        ShipmentItem GetShipmentItemById(int shipmentItemId);

        /// <summary>
        /// Inserts a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        void InsertShipmentItem(ShipmentItem shipmentItem);

        /// <summary>
        /// Updates the shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        void UpdateShipmentItem(ShipmentItem shipmentItem);
    }
}
