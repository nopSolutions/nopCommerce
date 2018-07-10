using System;
using System.Linq;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the country model factory implementation
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStateProvinceService stateProvinceService)
        {
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare state and province search model
        /// </summary>
        /// <param name="searchModel">State and province search model</param>
        /// <param name="country">Country</param>
        /// <returns>State and province search model</returns>
        protected virtual StateProvinceSearchModel PrepareStateProvinceSearchModel(StateProvinceSearchModel searchModel, Country country)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (country == null)
                throw new ArgumentNullException(nameof(country));

            searchModel.CountryId = country.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare country search model
        /// </summary>
        /// <param name="searchModel">Country search model</param>
        /// <returns>Country search model</returns>
        public virtual CountrySearchModel PrepareCountrySearchModel(CountrySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged country list model
        /// </summary>
        /// <param name="searchModel">Country search model</param>
        /// <returns>Country list model</returns>
        public virtual CountryListModel PrepareCountryListModel(CountrySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get countries
            var countries = _countryService.GetAllCountries(showHidden: true);

            //prepare list model
            var model = new CountryListModel
            {
                //fill in model values from the entity
                Data = countries.PaginationByRequestModel(searchModel).Select(country => country.ToModel<CountryModel>()),
                Total = countries.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare country model
        /// </summary>
        /// <param name="model">Country model</param>
        /// <param name="country">Country</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Country model</returns>
        public virtual CountryModel PrepareCountryModel(CountryModel model, Country country, bool excludeProperties = false)
        {
            Action<CountryLocalizedModel, int> localizedModelConfiguration = null;

            if (country != null)
            {
                //fill in model values from the entity
                model = model ?? country.ToModel<CountryModel>();

                //prepare nested search model
                PrepareStateProvinceSearchModel(model.StateProvinceSearchModel, country);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(country, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (country == null)
            {
                model.Published = true;
                model.AllowsBilling = true;
                model.AllowsShipping = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, country, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged state and province list model
        /// </summary>
        /// <param name="searchModel">State and province search model</param>
        /// <param name="country">Country</param>
        /// <returns>State and province list model</returns>
        public virtual StateProvinceListModel PrepareStateProvinceListModel(StateProvinceSearchModel searchModel, Country country)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (country == null)
                throw new ArgumentNullException(nameof(country));

            //get comments
            var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);

            //prepare list model
            var model = new StateProvinceListModel
            {
                //fill in model values from the entity
                Data = states.PaginationByRequestModel(searchModel).Select(state => state.ToModel<StateProvinceModel>()),
                Total = states.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare state and province model
        /// </summary>
        /// <param name="model">State and province model</param>
        /// <param name="country">Country</param>
        /// <param name="state">State or province</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>State and province model</returns>
        public virtual StateProvinceModel PrepareStateProvinceModel(StateProvinceModel model,
            Country country, StateProvince state, bool excludeProperties = false)
        {
            Action<StateProvinceLocalizedModel, int> localizedModelConfiguration = null;

            if (state != null)
            {
                //fill in model values from the entity
                model = model ?? state.ToModel<StateProvinceModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(country, entity => entity.Name, languageId, false, false);
                };
            }

            model.CountryId = country.Id;

            //set default values for the new model
            if (state == null)
                model.Published = true;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}