using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;

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
        /// <param name="order">Order</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        void PrintOrderToPdf(Order order, Language lang, string filePath);

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="filePath">File path</param>
        void PrintPackagingSlipsToPdf(IList<Order> orders, string filePath);

        
        /// <summary>
        /// Print product collection to PDF
        /// </summary>
        /// <param name="products">Products</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        void PrintProductsToPdf(IList<Product> products, Language lang, string filePath);
    }
}