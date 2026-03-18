using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;

namespace Nop.Services.ExportImport;

/// <summary>
/// Export manager interface
/// </summary>
public partial interface IExportManager
{
    /// <summary>
    /// Export manufacturer list to XML
    /// </summary>
    /// <param name="manufacturers">Manufacturers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportManufacturersToXmlAsync(IList<Manufacturer> manufacturers);

    /// <summary>
    /// Export manufacturers to XLSX
    /// </summary>
    /// <param name="manufacturers">Manufactures</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportManufacturersToXlsxAsync(IList<Manufacturer> manufacturers);

    /// <summary>
    /// Export category list to XML
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportCategoriesToXmlAsync();

    /// <summary>
    /// Export categories to XLSX
    /// </summary>
    /// <param name="categories">Categories</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories);

    /// <summary>
    /// Export product list to XML
    /// </summary>
    /// <param name="products">Products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportProductsToXmlAsync(IList<Product> products);

    /// <summary>
    /// Export products to XLSX
    /// </summary>
    /// <param name="products">Products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportProductsToXlsxAsync(IEnumerable<Product> products);

    /// <summary>
    /// Export order list to XML
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportOrdersToXmlAsync(IList<Order> orders);

    /// <summary>
    /// Export orders to XLSX
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders);

    /// <summary>
    /// Export sales summary report to XML
    /// </summary>
    /// <param name="salesSummaries">Sales summaries</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportSalesSummaryToXmlAsync(IList<SalesSummaryReportLine> salesSummaries);

    /// <summary>
    /// Export sales summary report to XLSX
    /// </summary>
    /// <param name="salesSummaries">Sales Summaries</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportSalesSummaryToXlsxAsync(IList<SalesSummaryReportLine> salesSummaries);

    /// <summary>
    /// Export low stock report to XML.
    /// </summary>
    /// <param name="products">Low stock products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportLowStockToXmlAsync(IList<LowStockProductReportLine> products);

    /// <summary>
    /// Export low stock report to XLSX
    /// </summary>
    /// <param name="products">Low stock products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportLowStockToXlsxAsync(IList<LowStockProductReportLine> products);

    /// <summary>
    /// Export best sellers report to XML
    /// </summary>
    /// <param name="products">Best sellers products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportBestSellersToXmlAsync(IList<BestsellersReportLine> products);

    /// <summary>
    /// Export best sellers report to XLSX
    /// </summary>
    /// <param name="products">Best sellers products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportBestSellersToXlsxAsync(IList<BestsellersReportLine> products);

    /// <summary>
    /// Export never sold report to XML
    /// </summary>
    /// <param name="products">Never sold products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportNeverSoldToXmlAsync(IList<Product> products);

    /// <summary>
    /// Export never sold report to XLSX
    /// </summary>
    /// <param name="products">Never sold products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportNeverSoldToXlsxAsync(IList<Product> products);

    /// <summary>
    /// Export country sales report to XML
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportCountrySalesToXmlAsync(IList<OrderByCountryReportLine> orders);

    /// <summary>
    /// Export country sales report to XLSX
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportCountrySalesToXlsxAsync(IList<OrderByCountryReportLine> orders);

    /// <summary>
    /// Export registered customers report to XML
    /// </summary>
    /// <param name="customers">Registered customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportRegisteredCustomersToXmlAsync(IList<RegisteredCustomersReportLine> customers);

    /// <summary>
    /// Export registered customers report to XLSX
    /// </summary>
    /// <param name="customers">Registered customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportRegisteredCustomersToXlsxAsync(IList<RegisteredCustomersReportLine> customers);

    /// <summary>
    /// Export best customers by order total report to XML
    /// </summary>
    /// <param name="customers">Best customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportBestCustomersByOrderTotalToXmlAsync(IList<BestCustomerReportLine> customers);

    /// <summary>
    /// Export best customers by order total report to XLSX
    /// </summary>
    /// <param name="customers">Best customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportBestCustomersByOrderTotalToXlsxAsync(IList<BestCustomerReportLine> customers);

    /// <summary>
    /// Export best customers by number of orders report to XML
    /// </summary>
    /// <param name="customers">Best customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportBestCustomersByNumberOfOrdersToXmlAsync(IList<BestCustomerReportLine> customers);

    /// <summary>
    /// Export best customers by number of orders report to XLSX
    /// </summary>
    /// <param name="customers">Best customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportBestCustomersByNumberOfOrdersToXlsxAsync(IList<BestCustomerReportLine> customers);

    /// <summary>
    /// Export customer list to XLSX
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportCustomersToXlsxAsync(IList<Customer> customers);

    /// <summary>
    /// Export customer list to XML
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportCustomersToXmlAsync(IList<Customer> customers);

    /// <summary>
    /// Export newsletter subscribers to TXT
    /// </summary>
    /// <param name="subscriptions">Subscriptions</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in TXT (string) format
    /// </returns>
    Task<string> ExportNewsLetterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions);

    /// <summary>
    /// Export states to TXT
    /// </summary>
    /// <param name="states">States</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in TXT (string) format
    /// </returns>
    Task<string> ExportStatesToTxtAsync(IList<StateProvince> states);

    /// <summary>
    /// Export customer info (GDPR request) to XLSX 
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer GDPR info
    /// </returns>
    Task<byte[]> ExportCustomerGdprInfoToXlsxAsync(Customer customer, int storeId);

    /// <summary>
    /// Export filter level values to XLSX
    /// </summary>
    /// <param name="filterLevelValues">Filter level values</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<byte[]> ExportFilterLevelValuesToXlsxAsync(IList<FilterLevelValue> filterLevelValues);
}