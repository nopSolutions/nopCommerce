using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.ShipStation.Services
{
    public interface IShipStationService
    {
        /// <summary>
        /// Gets all rates
        /// </summary>
        /// <param name="shippingOptionRequest"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        Task<IList<ShipStationServiceRate>> GetAllRatesAsync(GetShippingOptionRequest shippingOptionRequest);
        
        /// <summary>
        /// Create or update shipping
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <param name="carrier">Carrier</param>
        /// <param name="service">Service</param>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task CreateOrUpdateShippingAsync(string orderNumber, string carrier, string service, string trackingNumber);

        /// <summary>
        /// Get XML view of orders to sending to the ShipStation service
        /// </summary>
        /// <param name="startDate">Created date from (UTC); null to load all records</param>
        /// <param name="endDate">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the xML view of orders
        /// </returns>
        Task<string> GetXmlOrdersAsync(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);

        /// <summary>
        /// Gets a string that defines the required format of date time
        /// </summary>
        string DateFormat { get; }
    }
}
