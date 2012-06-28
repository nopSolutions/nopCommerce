using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Common
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface IPdfService
    {
        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        void PrintOrdersToPdf(IList<Order> orders, Language lang, string filePath);

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="shipments">Shipments</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        void PrintPackagingSlipsToPdf(IList<Shipment> shipments, Language lang, string filePath);

        
        /// <summary>
        /// Print product collection to PDF
        /// </summary>
        /// <param name="products">Products</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        void PrintProductsToPdf(IList<Product> products, Language lang, string filePath);
    }
}