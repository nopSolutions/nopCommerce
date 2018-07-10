using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Pickup.PickupInStore.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Pickup.PickupInStore
{
    public class PickupInStoreProvider : BasePlugin, IPickupPointProvider
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStorePickupPointService _storePickupPointService;
        private readonly StorePickupPointObjectContext _objectContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public PickupInStoreProvider(IAddressService addressService,
            ICountryService countryService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStorePickupPointService storePickupPointService,
            StorePickupPointObjectContext objectContext,
            IWebHelper webHelper)
        {
            this._addressService = addressService;
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._stateProvinceService = stateProvinceService;
            this._storeContext = storeContext;
            this._storePickupPointService = storePickupPointService;
            this._objectContext = objectContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get { return null; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get pickup points for the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Represents a response of getting pickup points</returns>
        public GetPickupPointsResponse GetPickupPoints(Address address)
        {
            var result = new GetPickupPointsResponse();

            foreach (var point in _storePickupPointService.GetAllStorePickupPoints(_storeContext.CurrentStore.Id))
            {
                var pointAddress = _addressService.GetAddressById(point.AddressId);
                if (pointAddress == null)
                    continue;

                result.PickupPoints.Add(new PickupPoint
                {
                    Id = point.Id.ToString(),
                    Name = point.Name,
                    Description = point.Description,
                    Address = pointAddress.Address1,
                    City = pointAddress.City,
                    County = pointAddress.County,
                    StateAbbreviation = pointAddress.StateProvince?.Abbreviation ?? string.Empty,
                    CountryCode = pointAddress.Country?.TwoLetterIsoCode ?? string.Empty,
                    ZipPostalCode = pointAddress.ZipPostalCode,
                    OpeningHours = point.OpeningHours,
                    PickupFee = point.PickupFee,
                    DisplayOrder = point.DisplayOrder,
                    ProviderSystemName = PluginDescriptor.SystemName
                });
            }

            if (!result.PickupPoints.Any())
                result.AddError(_localizationService.GetResource("Plugins.Pickup.PickupInStore.NoPickupPoints"));

            return result;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PickupInStore/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //sample pickup point
            var country = _countryService.GetCountryByThreeLetterIsoCode("USA");
            var state = _stateProvinceService.GetStateProvinceByAbbreviation("NY", country?.Id);

            var address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                CountryId = country?.Id,
                StateProvinceId = state?.Id,
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressService.InsertAddress(address);

            var pickupPoint = new StorePickupPoint
            {
                Name = "New York store",
                AddressId = address.Id,
                OpeningHours = "10.00 - 19.00",
                PickupFee = 1.99m
            };
            _storePickupPointService.InsertStorePickupPoint(pickupPoint);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.AddNew", "Add a new pickup point");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Description", "Description");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Description.Hint", "Specify a description of the pickup point.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.DisplayOrder.Hint", "Specify the pickup point display order.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Name", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Name.Hint", "Specify a name of the pickup point.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.OpeningHours", "Opening hours");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.OpeningHours.Hint", "Specify opening hours of the pickup point (Monday - Friday: 09:00 - 19:00 for example).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.PickupFee", "Pickup fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.PickupFee.Hint", "Specify a fee for the shipping to the pickup point.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Store", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Store.Hint", "A store name for which this pickup point will be available.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Pickup.PickupInStore.NoPickupPoints", "No pickup points are available");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Description");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Description.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.DisplayOrder.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Name.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.OpeningHours");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.OpeningHours.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.PickupFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.PickupFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Store");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.Fields.Store.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Pickup.PickupInStore.NoPickupPoints");

            base.Uninstall();
        }

        #endregion
    }
}