namespace Nop.Plugin.Shipping.EasyPost
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class EasyPostDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Shipping.EasyPost";

        /// <summary>
        /// Gets the system name of the weight measure used
        /// </summary>
        public static string MeasureWeightSystemName => "ounce";

        /// <summary>
        /// Gets the system name of the dimension measure used
        /// </summary>
        public static string MeasureDimensionSystemName => "inches";

        /// <summary>
        /// Gets the code of the default currency used
        /// </summary>
        public static string CurrencyCode => "USD";

        #region Routes

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Shipping.EasyPost.Configure";

        /// <summary>
        /// Gets the update shipment route name
        /// </summary>
        public static string UpdateShipmentRouteName => "Plugin.Shipping.EasyPost.UpdateShipment";

        /// <summary>
        /// Gets the buy label route name
        /// </summary>
        public static string BuyLabelRouteName => "Plugin.Shipping.EasyPost.BuyLabel";

        /// <summary>
        /// Gets the download label route name
        /// </summary>
        public static string DownloadLabelRouteName => "Plugin.Shipping.EasyPost.DownloadLabel";

        /// <summary>
        /// Gets the download invoice route name
        /// </summary>
        public static string DownloadInvoiceRouteName => "Plugin.Shipping.EasyPost.DownloadInvoice";

        /// <summary>
        /// Gets the create pickup route name
        /// </summary>
        public static string CreatePickupRouteName => "Plugin.Shipping.EasyPost.CreatePickup";

        /// <summary>
        /// Gets the buy pickup route name
        /// </summary>
        public static string BuyPickupRouteName => "Plugin.Shipping.EasyPost.BuyPickup";

        /// <summary>
        /// Gets the cancel pickup route name
        /// </summary>
        public static string CancelPickupRouteName => "Plugin.Shipping.EasyPost.CancelPickup";

        /// <summary>
        /// Gets the webhook route name
        /// </summary>
        public static string WebhookRouteName => "Plugin.Shipping.EasyPost.Webhook";

        #endregion

        #region Attributes

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a created shipment id
        /// </summary>
        public static string ShipmentIdAttribute => "EasyPost.Shipment.Id";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a created pickup id
        /// </summary>
        public static string PickupIdAttribute => "EasyPost.Pickup.Id";

        /// <summary>
        /// Gets the name of the form key of a predefined package
        /// </summary>
        public static string ProductPredefinedPackageFormKey => "EasyPostPredefinedPackage";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a predefined package value for the product
        /// </summary>
        public static string ProductPredefinedPackageAttribute => "EasyPost.Product.PredefinedPackage";

        /// <summary>
        /// Gets the name of the form key of a predefined package
        /// </summary>
        public static string ProductHtsNumberFormKey => "EasyPostHtsNumber";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a predefined package value for the product
        /// </summary>
        public static string ProductHtsNumberAttribute => "EasyPost.Product.HtsNumber";

        /// <summary>
        /// Gets the name of the form key of a predefined package
        /// </summary>
        public static string ProductOriginCountryFormKey => "EasyPostOriginCountry";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a predefined package value for the product
        /// </summary>
        public static string ProductOriginCountryAttribute => "EasyPost.Product.OriginCountry";

        #endregion

        #region View components

        /// <summary>
        /// Gets the name of the view component to display an additional block on the product details page in the admin area
        /// </summary>
        public const string PRODUCT_DETAILS_VIEW_COMPONENT_NAME = "EasyPost.ProductDetailsBlock";

        /// <summary>
        /// Gets the name of the view component to display an additional block on the shipment details page in the admin area
        /// </summary>
        public const string SHIPMENT_DETAILS_VIEW_COMPONENT_NAME = "EasyPost.ShipmentDetailsBlock";

        /// <summary>
        /// Gets the name of the view component to display address verification warning on the opc shipping methods page in the public store
        /// </summary>
        public const string SHIPPING_METHODS_VIEW_COMPONENT_NAME = "EasyPost.OpcShippingMethods";

        #endregion
    }
}