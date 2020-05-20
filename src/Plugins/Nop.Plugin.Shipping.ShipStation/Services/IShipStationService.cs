using System;
using System.Collections.Generic;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.ShipStation.Services
{
    public interface IShipStationService
    {
        /// <summary>
        /// Gets all rates
        /// </summary>
        /// <param name="shippingOptionRequest"></param>
        /// <returns></returns>
        IList<ShipStationServiceRate> GetAllRates(GetShippingOptionRequest shippingOptionRequest);
        
        /// <summary>
        /// Create or upadete shipping
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="carrier"></param>
        /// <param name="service"></param>
        /// <param name="trackingNumber"></param>
        void CreateOrUpadeteShipping(string orderNumber, string carrier, string service, string trackingNumber);

        /// <summary>
        /// Get XML view of orders to sending to the ShipStation service
        /// </summary>
        /// <param name="startDate">Created date from (UTC); null to load all records</param>
        /// <param name="endDate">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>XML view of orders</returns>
        string GetXmlOrders(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);

        /// <summary>
        /// Gets a string that defines the required format of date time
        /// </summary>
        string DateFormat { get; }
    }
}
