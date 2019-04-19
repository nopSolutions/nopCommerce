using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial interface IOrderModelFactory
    {
        /// <summary>
        /// Prepare order search model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order search model</returns>
        OrderSearchModel PrepareOrderSearchModel(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order list model</returns>
        OrderListModel PrepareOrderListModel(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare order aggregator model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order aggregator model</returns>
        OrderAggreratorModel PrepareOrderAggregatorModel(OrderSearchModel searchModel);

        /// <summary>
        /// Prepare order model
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Order model</returns>
        OrderModel PrepareOrderModel(OrderModel model, Order order, bool excludeProperties = false);

        /// <summary>
        /// Prepare upload license model
        /// </summary>
        /// <param name="model">Upload license model</param>
        /// <param name="order">Order</param>
        /// <param name="orderItem">Order item</param>
        /// <returns>Upload license model</returns>
        UploadLicenseModel PrepareUploadLicenseModel(UploadLicenseModel model, Order order, OrderItem orderItem);

        /// <summary>
        /// Prepare product search model to add to the order
        /// </summary>
        /// <param name="searchModel">Product search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>Product search model to add to the order</returns>
        AddProductToOrderSearchModel PrepareAddProductToOrderSearchModel(AddProductToOrderSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare paged product list model to add to the order
        /// </summary>
        /// <param name="searchModel">Product search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>Product search model to add to the order</returns>
        AddProductToOrderListModel PrepareAddProductToOrderListModel(AddProductToOrderSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare product model to add to the order
        /// </summary>
        /// <param name="model">Product model to add to the order</param>
        /// <param name="order">Order</param>
        /// <param name="product">Product</param>
        /// <returns>Product model to add to the order</returns>
        AddProductToOrderModel PrepareAddProductToOrderModel(AddProductToOrderModel model, Order order, Product product);

        /// <summary>
        /// Prepare order address model
        /// </summary>
        /// <param name="model">Order address model</param>
        /// <param name="order">Order</param>
        /// <param name="address">Address</param>
        /// <returns>Order address model</returns>
        OrderAddressModel PrepareOrderAddressModel(OrderAddressModel model, Order order, Address address);

        /// <summary>
        /// Prepare shipment search model
        /// </summary>
        /// <param name="searchModel">Shipment search model</param>
        /// <returns>Shipment search model</returns>
        ShipmentSearchModel PrepareShipmentSearchModel(ShipmentSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipment list model
        /// </summary>
        /// <param name="searchModel">Shipment search model</param>
        /// <returns>Shipment list model</returns>
        ShipmentListModel PrepareShipmentListModel(ShipmentSearchModel searchModel);

        /// <summary>
        /// Prepare shipment model
        /// </summary>
        /// <param name="model">Shipment model</param>
        /// <param name="shipment">Shipment</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Shipment model</returns>
        ShipmentModel PrepareShipmentModel(ShipmentModel model, Shipment shipment, Order order, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged order shipment list model
        /// </summary>
        /// <param name="searchModel">Order shipment search model</param>
        /// <param name="order">Order</param>
        /// <returns>Order shipment list model</returns>
        OrderShipmentListModel PrepareOrderShipmentListModel(OrderShipmentSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare paged shipment item list model
        /// </summary>
        /// <param name="searchModel">Shipment item search model</param>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment item list model</returns>
        ShipmentItemListModel PrepareShipmentItemListModel(ShipmentItemSearchModel searchModel, Shipment shipment);

        /// <summary>
        /// Prepare paged order note list model
        /// </summary>
        /// <param name="searchModel">Order note search model</param>
        /// <param name="order">Order</param>
        /// <returns>Order note list model</returns>
        OrderNoteListModel PrepareOrderNoteListModel(OrderNoteSearchModel searchModel, Order order);

        /// <summary>
        /// Prepare bestseller brief search model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>Bestseller brief search model</returns>
        BestsellerBriefSearchModel PrepareBestsellerBriefSearchModel(BestsellerBriefSearchModel searchModel);

        /// <summary>
        /// Prepare paged bestseller brief list model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>Bestseller brief list model</returns>
        BestsellerBriefListModel PrepareBestsellerBriefListModel(BestsellerBriefSearchModel searchModel);

        /// <summary>
        /// Prepare order average line summary report list model
        /// </summary>
        /// <param name="searchModel">Order average line summary report search model</param>
        /// <returns>Order average line summary report list model</returns>
        OrderAverageReportListModel PrepareOrderAverageReportListModel(OrderAverageReportSearchModel searchModel);

        /// <summary>
        /// Prepare incomplete order report list model
        /// </summary>
        /// <param name="searchModel">Incomplete order report search model</param>
        /// <returns>Incomplete order report list model</returns>
        OrderIncompleteReportListModel PrepareOrderIncompleteReportListModel(OrderIncompleteReportSearchModel searchModel);
    }
}