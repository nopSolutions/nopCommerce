using Nop.Core.Domain.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shipping model factory
    /// </summary>
    public partial interface IShippingModelFactory
    {
        /// <summary>
        /// Prepare shipping provider search model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping provider search model
        /// </returns>
        Task<ShippingProviderSearchModel> PrepareShippingProviderSearchModelAsync(ShippingProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipping provider list model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping provider list model
        /// </returns>
        Task<ShippingProviderListModel> PrepareShippingProviderListModelAsync(ShippingProviderSearchModel searchModel);

        /// <summary>
        /// Prepare pickup point provider search model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup point provider search model
        /// </returns>
        Task<PickupPointProviderSearchModel> PreparePickupPointProviderSearchModelAsync(PickupPointProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged pickup point provider list model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup point provider list model
        /// </returns>
        Task<PickupPointProviderListModel> PreparePickupPointProviderListModelAsync(PickupPointProviderSearchModel searchModel);

        /// <summary>
        /// Prepare shipping method search model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping method search model
        /// </returns>
        Task<ShippingMethodSearchModel> PrepareShippingMethodSearchModelAsync(ShippingMethodSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipping method list model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping method list model
        /// </returns>
        Task<ShippingMethodListModel> PrepareShippingMethodListModelAsync(ShippingMethodSearchModel searchModel);

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="model">Shipping method model</param>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping method model
        /// </returns>
        Task<ShippingMethodModel> PrepareShippingMethodModelAsync(ShippingMethodModel model,
            ShippingMethod shippingMethod, bool excludeProperties = false);

        /// <summary>
        /// Prepare dates and ranges search model
        /// </summary>
        /// <param name="searchModel">Dates and ranges search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dates and ranges search model
        /// </returns>
        Task<DatesRangesSearchModel> PrepareDatesRangesSearchModelAsync(DatesRangesSearchModel searchModel);

        /// <summary>
        /// Prepare paged delivery date list model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery date list model
        /// </returns>
        Task<DeliveryDateListModel> PrepareDeliveryDateListModelAsync(DeliveryDateSearchModel searchModel);

        /// <summary>
        /// Prepare delivery date model
        /// </summary>
        /// <param name="model">Delivery date model</param>
        /// <param name="deliveryDate">Delivery date</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery date model
        /// </returns>
        Task<DeliveryDateModel> PrepareDeliveryDateModelAsync(DeliveryDateModel model, DeliveryDate deliveryDate, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product availability range list model
        /// </summary>
        /// <param name="searchModel">Product availability range search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product availability range list model
        /// </returns>
        Task<ProductAvailabilityRangeListModel> PrepareProductAvailabilityRangeListModelAsync(ProductAvailabilityRangeSearchModel searchModel);

        /// <summary>
        /// Prepare product availability range model
        /// </summary>
        /// <param name="model">Product availability range model</param>
        /// <param name="productAvailabilityRange">Product availability range</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product availability range model
        /// </returns>
        Task<ProductAvailabilityRangeModel> PrepareProductAvailabilityRangeModelAsync(ProductAvailabilityRangeModel model,
            ProductAvailabilityRange productAvailabilityRange, bool excludeProperties = false);

        /// <summary>
        /// Prepare warehouse search model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warehouse search model
        /// </returns>
        Task<WarehouseSearchModel> PrepareWarehouseSearchModelAsync(WarehouseSearchModel searchModel);

        /// <summary>
        /// Prepare paged warehouse list model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warehouse list model
        /// </returns>
        Task<WarehouseListModel> PrepareWarehouseListModelAsync(WarehouseSearchModel searchModel);

        /// <summary>
        /// Prepare warehouse model
        /// </summary>
        /// <param name="model">Warehouse model</param>
        /// <param name="warehouse">Warehouse</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warehouse model
        /// </returns>
        Task<WarehouseModel> PrepareWarehouseModelAsync(WarehouseModel model, Warehouse warehouse, bool excludeProperties = false);

        /// <summary>
        /// Prepare shipping method restriction model
        /// </summary>
        /// <param name="model">Shipping method restriction model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping method restriction model
        /// </returns>
        Task<ShippingMethodRestrictionModel> PrepareShippingMethodRestrictionModelAsync(ShippingMethodRestrictionModel model);
    }
}