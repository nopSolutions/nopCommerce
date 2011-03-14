using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public interface IExportManager
    {
        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        string ExportCustomersToXml(List<Customer> customers);

        /// <summary>
        /// Export customer list to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="customers">Customers</param>
        void ExportCustomersToXls(string filePath, List<Customer> customers);

        /// <summary>
        /// Export manufacturer list to xml
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Result in XML format</returns>
        string ExportManufacturersToXml(List<Manufacturer> manufacturers);

        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportCategoriesToXml();

        /// <summary>
        /// Export product list to xml
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        string ExportProductsToXml(List<Product> products);

        /// <summary>
        /// Export products to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="products">Products</param>
        void ExportProductsToXls(string filePath, List<Product> products);

        /// <summary>
        /// Export order list to xml
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        string ExportOrdersToXml(List<Order> orders);

        /// <summary>
        /// Export orders to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="orders">Orders</param>
        void ExportOrdersToXls(string filePath, List<Order> orders);

        /// <summary>
        /// Export message tokens to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportMessageTokensToXml();
    }
}
