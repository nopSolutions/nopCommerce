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
        /// <returns>Shipping provider search model</returns>
        ShippingProviderSearchModel PrepareShippingProviderSearchModel(ShippingProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipping provider list model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>Shipping provider list model</returns>
        ShippingProviderListModel PrepareShippingProviderListModel(ShippingProviderSearchModel searchModel);

        /// <summary>
        /// Prepare pickup point provider search model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>Pickup point provider search model</returns>
        PickupPointProviderSearchModel PreparePickupPointProviderSearchModel(PickupPointProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged pickup point provider list model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>Pickup point provider list model</returns>
        PickupPointProviderListModel PreparePickupPointProviderListModel(PickupPointProviderSearchModel searchModel);

        /// <summary>
        /// Prepare shipping method search model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>Shipping method search model</returns>
        ShippingMethodSearchModel PrepareShippingMethodSearchModel(ShippingMethodSearchModel searchModel);

        /// <summary>
        /// Prepare paged shipping method list model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>Shipping method list model</returns>
        ShippingMethodListModel PrepareShippingMethodListModel(ShippingMethodSearchModel searchModel);

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="model">Shipping method model</param>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Shipping method model</returns>
        ShippingMethodModel PrepareShippingMethodModel(ShippingMethodModel model,
            ShippingMethod shippingMethod, bool excludeProperties = false);

        /// <summary>
        /// Prepare dates and ranges search model
        /// </summary>
        /// <param name="searchModel">Dates and ranges search model</param>
        /// <returns>Dates and ranges search model</returns>
        DatesRangesSearchModel PrepareDatesRangesSearchModel(DatesRangesSearchModel searchModel);

        /// <summary>
        /// Prepare paged delivery date list model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>Delivery date list model</returns>
        DeliveryDateListModel PrepareDeliveryDateListModel(DeliveryDateSearchModel searchModel);

        /// <summary>
        /// Prepare delivery date model
        /// </summary>
        /// <param name="model">Delivery date model</param>
        /// <param name="deliveryDate">Delivery date</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Delivery date model</returns>
        DeliveryDateModel PrepareDeliveryDateModel(DeliveryDateModel model, DeliveryDate deliveryDate, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product availability range list model
        /// </summary>
        /// <param name="searchModel">Product availability range search model</param>
        /// <returns>Product availability range list model</returns>
        ProductAvailabilityRangeListModel PrepareProductAvailabilityRangeListModel(ProductAvailabilityRangeSearchModel searchModel);

        /// <summary>
        /// Prepare product availability range model
        /// </summary>
        /// <param name="model">Product availability range model</param>
        /// <param name="productAvailabilityRange">Product availability range</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product availability range model</returns>
        ProductAvailabilityRangeModel PrepareProductAvailabilityRangeModel(ProductAvailabilityRangeModel model,
            ProductAvailabilityRange productAvailabilityRange, bool excludeProperties = false);

        /// <summary>
        /// Prepare warehouse search model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>Warehouse search model</returns>
        WarehouseSearchModel PrepareWarehouseSearchModel(WarehouseSearchModel searchModel);

        /// <summary>
        /// Prepare paged warehouse list model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>Warehouse list model</returns>
        WarehouseListModel PrepareWarehouseListModel(WarehouseSearchModel searchModel);

        /// <summary>
        /// Prepare warehouse model
        /// </summary>
        /// <param name="model">Warehouse model</param>
        /// <param name="warehouse">Warehouse</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Warehouse model</returns>
        WarehouseModel PrepareWarehouseModel(WarehouseModel model, Warehouse warehouse, bool excludeProperties = false);

        /// <summary>
        /// Prepare shipping method restriction model
        /// </summary>
        /// <param name="model">Shipping method restriction model</param>
        /// <returns>Shipping method restriction model</returns>
        ShippingMethodRestrictionModel PrepareShippingMethodRestrictionModel(ShippingMethodRestrictionModel model);
    }
}