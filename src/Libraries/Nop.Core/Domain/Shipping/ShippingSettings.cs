using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipping settings
    /// </summary>
    public class ShippingSettings : ISettings
    {
        public ShippingSettings()
        {
            ActiveShippingRateComputationMethodSystemNames = new List<string>();
            ActivePickupPointProviderSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets system names of active shipping rate computation methods
        /// </summary>
        public List<string> ActiveShippingRateComputationMethodSystemNames { get; set; }

        /// <summary>
        /// Gets or sets system names of active pickup point providers
        /// </summary>
        public List<string> ActivePickupPointProviderSystemNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating "Ship to the same address" option is enabled
        /// </summary>
        public bool ShipToSameAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can choose "Pick Up in Store" option during checkout (displayed on the "billing address" checkout step)
        /// </summary>
        public bool AllowPickupInStore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether display a pickup points in the map
        /// </summary>
        public bool DisplayPickupPointsOnMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore additional shipping charge for pick up in store
        /// </summary>
        public bool IgnoreAdditionalShippingChargeForPickupInStore { get; set; }

        /// <summary>
        /// Gets or sets Google map API key
        /// </summary>
        public string GoogleMapsApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the system should use warehouse location when requesting shipping rates
        /// This is useful when you ship from multiple warehouses
        /// </summary>
        public bool UseWarehouseLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers should be notified when shipping is made from multiple locations (warehouses)
        /// </summary>
        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Free shipping over X' is enabled
        /// </summary>
        public bool FreeShippingOverXEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value of 'Free shipping over X' option
        /// </summary>
        public decimal FreeShippingOverXValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Free shipping over X' option
        /// should be evaluated over 'X' value including tax or not
        /// </summary>
        public bool FreeShippingOverXIncludingTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Estimate shipping' is enabled on the shopping cart page
        /// </summary>
        public bool EstimateShippingCartPageEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Estimate shipping' is enabled on the product details pages
        /// </summary>
        public bool EstimateShippingProductPageEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use city name on 'Estimate shipping' widget instead zip postal code
        /// </summary>
        public bool EstimateShippingCityNameEnabled { get; set; }

        /// <summary>
        /// A value indicating whether customers should see shipment events on their order details pages
        /// </summary>
        public bool DisplayShipmentEventsToCustomers { get; set; }

        /// <summary>
        /// A value indicating whether store owner should see shipment events on the shipment details pages
        /// </summary>
        public bool DisplayShipmentEventsToStoreOwner { get; set; }

        /// <summary>
        /// A value indicating whether should hide "Shipping total" label if shipping not required
        /// </summary>
        public bool HideShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets shipping origin address
        /// </summary>
        public int ShippingOriginAddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should return valid options if there are any (no matter of the errors returned by other shipping rate computation methods).
        /// </summary>
        public bool ReturnValidOptionsIfThereAreAny { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should bypass 'select shipping method' page if we have only one shipping method
        /// </summary>
        public bool BypassShippingMethodSelectionIfOnlyOne { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dimensions are calculated based on cube root of volume
        /// </summary>
        public bool UseCubeRootMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to consider associated products dimensions and weight on shipping, false if main product includes them
        /// </summary>
        public bool ConsiderAssociatedProductsDimensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send all the items of a product marked as "Ship Separately" separately; if false, all the items of a such product will be shipped in a single box, but separately from the other order items
        /// </summary>
        public bool ShipSeparatelyOneItemEach { get; set; }

        /// <summary>
        /// Gets or sets the request delay in the shipping calculation popup (on product page/shopping cart page) when user enter the shipping address.
        /// </summary>
        public int RequestDelay { get; set; }

        /// <summary>
        /// Gets or sets a value for sorting shipping methods (on the product/shopping cart page when the user selects a shipping method)
        /// </summary>
        public ShippingSortingEnum ShippingSorting { get; set; }
    }
}
