using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Shipping
{
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
        public bool AllowPickUpInStore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether display a pickup points in the map
        /// </summary>
        public bool DisplayPickupPointsOnMap { get; set; }

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
        /// Gets or sets a value indicating whether 'Estimate shipping' option is enabled
        /// </summary>
        public bool EstimateShippingEnabled { get; set; }

        /// <summary>
        /// A value indicating whether customers should see shipment events on their order details pages
        /// </summary>
        public bool DisplayShipmentEventsToCustomers { get; set; }
        /// <summary>
        /// A value indicating whether store owner should see shipment events on the shipment details pages
        /// </summary>
        public bool DisplayShipmentEventsToStoreOwner { get; set; }

        /// <summary>
        /// Gets or sets shipping origin address
        /// </summary>
        public int ShippingOriginAddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should return valid options if there are any (no matter of the errors returned by other shipping rate compuation methods).
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
    }
}