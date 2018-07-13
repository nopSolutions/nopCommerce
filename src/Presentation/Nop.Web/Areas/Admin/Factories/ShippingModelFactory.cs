using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shipping model factory implementation
    /// </summary>
    public partial class ShippingModelFactory : IShippingModelFactory
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IDateRangeService _dateRangeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public ShippingModelFactory(IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IDateRangeService dateRangeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IShippingService shippingService)
        {
            this._addressService = addressService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._countryService = countryService;
            this._dateRangeService = dateRangeService;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._shippingService = shippingService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.CountryEnabled = true;
            model.CountryRequired = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CityEnabled = true;
            model.StreetAddressEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;
            model.PhoneEnabled = true;

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare delivery date search model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>Delivery date search model</returns>
        protected virtual DeliveryDateSearchModel PrepareDeliveryDateSearchModel(DeliveryDateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare product availability range search model
        /// </summary>
        /// <param name="searchModel">Product availability range search model</param>
        /// <returns>Product availability range search model</returns>
        protected virtual ProductAvailabilityRangeSearchModel PrepareProductAvailabilityRangeSearchModel(ProductAvailabilityRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare shipping provider search model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>Shipping provider search model</returns>
        public virtual ShippingProviderSearchModel PrepareShippingProviderSearchModel(ShippingProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged shipping provider list model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>Shipping provider list model</returns>
        public virtual ShippingProviderListModel PrepareShippingProviderListModel(ShippingProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get shipping providers
            var shippingProviders = _shippingService.LoadAllShippingRateComputationMethods();

            //prepare grid model
            var model = new ShippingProviderListModel
            {
                Data = shippingProviders.PaginationByRequestModel(searchModel).Select(provider =>
                {
                    //fill in model values from the entity
                    var shippingProviderModel = provider.ToPluginModel<ShippingProviderModel>();

                    //fill in additional values (not existing in the entity)
                    shippingProviderModel.IsActive = _shippingService.IsShippingRateComputationMethodActive(provider);
                    shippingProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                    shippingProviderModel.LogoUrl = PluginManager.GetLogoUrl(provider.PluginDescriptor);

                    return shippingProviderModel;
                }),
                Total = shippingProviders.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare pickup point provider search model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>Pickup point provider search model</returns>
        public virtual PickupPointProviderSearchModel PreparePickupPointProviderSearchModel(PickupPointProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged pickup point provider list model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>Pickup point provider list model</returns>
        public virtual PickupPointProviderListModel PreparePickupPointProviderListModel(PickupPointProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get pickup point providers
            var pickupPointProviders = _shippingService.LoadAllPickupPointProviders();

            //prepare grid model
            var model = new PickupPointProviderListModel
            {
                Data = pickupPointProviders.PaginationByRequestModel(searchModel).Select(provider =>
                {
                    //fill in model values from the entity
                    var pickupPointProviderModel = provider.ToPluginModel<PickupPointProviderModel>();

                    //fill in additional values (not existing in the entity)
                    pickupPointProviderModel.IsActive = _shippingService.IsPickupPointProviderActive(provider);
                    pickupPointProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                    pickupPointProviderModel.LogoUrl = PluginManager.GetLogoUrl(provider.PluginDescriptor);

                    return pickupPointProviderModel;
                }),
                Total = pickupPointProviders.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare shipping method search model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>Shipping method search model</returns>
        public virtual ShippingMethodSearchModel PrepareShippingMethodSearchModel(ShippingMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged shipping method list model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>Shipping method list model</returns>
        public virtual ShippingMethodListModel PrepareShippingMethodListModel(ShippingMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get shipping methods
            var shippingMethods = _shippingService.GetAllShippingMethods();

            //prepare grid model
            var model = new ShippingMethodListModel
            {
                //fill in model values from the entity
                Data = shippingMethods.PaginationByRequestModel(searchModel).Select(method => method.ToModel<ShippingMethodModel>()),
                Total = shippingMethods.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="model">Shipping method model</param>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Shipping method model</returns>
        public virtual ShippingMethodModel PrepareShippingMethodModel(ShippingMethodModel model,
            ShippingMethod shippingMethod, bool excludeProperties = false)
        {
            Action<ShippingMethodLocalizedModel, int> localizedModelConfiguration = null;

            if (shippingMethod != null)
            {
                //fill in model values from the entity
                model = model ?? shippingMethod.ToModel<ShippingMethodModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(shippingMethod, entity => entity.Name, languageId, false, false);
                    locale.Description = _localizationService.GetLocalized(shippingMethod, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare dates and ranges search model
        /// </summary>
        /// <param name="searchModel">Dates and ranges search model</param>
        /// <returns>Dates and ranges search model</returns>
        public virtual DatesRangesSearchModel PrepareDatesRangesSearchModel(DatesRangesSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            PrepareDeliveryDateSearchModel(searchModel.DeliveryDateSearchModel);
            PrepareProductAvailabilityRangeSearchModel(searchModel.ProductAvailabilityRangeSearchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged delivery date list model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>Delivery date list model</returns>
        public virtual DeliveryDateListModel PrepareDeliveryDateListModel(DeliveryDateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get delivery dates
            var deliveryDates = _dateRangeService.GetAllDeliveryDates();

            //prepare grid model
            var model = new DeliveryDateListModel
            {
                //fill in model values from the entity
                Data = deliveryDates.PaginationByRequestModel(searchModel).Select(date => date.ToModel<DeliveryDateModel>()),
                Total = deliveryDates.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare delivery date model
        /// </summary>
        /// <param name="model">Delivery date model</param>
        /// <param name="deliveryDate">Delivery date</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Delivery date model</returns>
        public virtual DeliveryDateModel PrepareDeliveryDateModel(DeliveryDateModel model, DeliveryDate deliveryDate, bool excludeProperties = false)
        {
            Action<DeliveryDateLocalizedModel, int> localizedModelConfiguration = null;

            if (deliveryDate != null)
            {
                //fill in model values from the entity
                model = model ?? deliveryDate.ToModel<DeliveryDateModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(deliveryDate, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged product availability range list model
        /// </summary>
        /// <param name="searchModel">Product availability range search model</param>
        /// <returns>Product availability range list model</returns>
        public virtual ProductAvailabilityRangeListModel PrepareProductAvailabilityRangeListModel(ProductAvailabilityRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product availability ranges
            var productAvailabilityRanges = _dateRangeService.GetAllProductAvailabilityRanges();

            //prepare grid model
            var model = new ProductAvailabilityRangeListModel
            {
                //fill in model values from the entity
                Data = productAvailabilityRanges.PaginationByRequestModel(searchModel)
                    .Select(range => range.ToModel<ProductAvailabilityRangeModel>()),
                Total = productAvailabilityRanges.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare product availability range model
        /// </summary>
        /// <param name="model">Product availability range model</param>
        /// <param name="productAvailabilityRange">Product availability range</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product availability range model</returns>
        public virtual ProductAvailabilityRangeModel PrepareProductAvailabilityRangeModel(ProductAvailabilityRangeModel model,
            ProductAvailabilityRange productAvailabilityRange, bool excludeProperties = false)
        {
            Action<ProductAvailabilityRangeLocalizedModel, int> localizedModelConfiguration = null;

            if (productAvailabilityRange != null)
            {
                //fill in model values from the entity
                model = model ?? productAvailabilityRange.ToModel<ProductAvailabilityRangeModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(productAvailabilityRange, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare warehouse search model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>Warehouse search model</returns>
        public virtual WarehouseSearchModel PrepareWarehouseSearchModel(WarehouseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged warehouse list model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>Warehouse list model</returns>
        public virtual WarehouseListModel PrepareWarehouseListModel(WarehouseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get warehouses
            var warehouses = _shippingService.GetAllWarehouses();

            //prepare list model
            var model = new WarehouseListModel
            {
                //fill in model values from the entity
                Data = warehouses.PaginationByRequestModel(searchModel).Select(warehouse => new WarehouseModel
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name
                }),
                Total = warehouses.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare warehouse model
        /// </summary>
        /// <param name="model">Warehouse model</param>
        /// <param name="warehouse">Warehouse</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Warehouse model</returns>
        public virtual WarehouseModel PrepareWarehouseModel(WarehouseModel model, Warehouse warehouse, bool excludeProperties = false)
        {
            if (warehouse != null)
            {
                //fill in model values from the entity
                model = model ?? new WarehouseModel
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    AdminComment = warehouse.AdminComment
                };
            }

            //prepare address model
            var address = _addressService.GetAddressById(warehouse?.AddressId ?? 0);
            if (!excludeProperties && address != null)
                model.Address = address.ToModel(model.Address);
            PrepareAddressModel(model.Address, address);

            return model;
        }

        /// <summary>
        /// Prepare shipping method restriction model
        /// </summary>
        /// <param name="model">Shipping method restriction model</param>
        /// <returns>Shipping method restriction model</returns>
        public virtual ShippingMethodRestrictionModel PrepareShippingMethodRestrictionModel(ShippingMethodRestrictionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var countries = _countryService.GetAllCountries(showHidden: true);
            model.AvailableCountries = countries.Select(country => country.ToModel<CountryModel>()).ToList();

            foreach (var shippingMethod in _shippingService.GetAllShippingMethods())
            {
                model.AvailableShippingMethods.Add(shippingMethod.ToModel<ShippingMethodModel>());
                foreach (var country in countries)
                {
                    if (!model.Restricted.ContainsKey(country.Id))
                        model.Restricted[country.Id] = new Dictionary<int, bool>();

                    model.Restricted[country.Id][shippingMethod.Id] = _shippingService.CountryRestrictionExists(shippingMethod, country.Id);
                }
            }

            return model;
        }

        #endregion
    }
}