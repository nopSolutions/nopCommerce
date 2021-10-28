using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Directory;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the country model factory
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
        #region Fields

        protected ICountryService CountryService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IWorkContext workContext)
        {
            CountryService = countryService;
            LocalizationService = localizationService;
            StateProvinceService = stateProvinceService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of identifiers and names of states and provinces
        /// </returns>
        public virtual async Task<IList<StateProvinceModel>> GetStatesByCountryIdAsync(string countryId, bool addSelectStateItem)
        {
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var country = await CountryService.GetCountryByIdAsync(Convert.ToInt32(countryId));
            var states = (await StateProvinceService
                .GetStateProvincesByCountryIdAsync(country?.Id ?? 0, (await WorkContext.GetWorkingLanguageAsync()).Id))
                .ToList();
            var result = new List<StateProvinceModel>();
            foreach (var state in states)
                result.Add(new StateProvinceModel
                {
                    id = state.Id,
                    name = await LocalizationService.GetLocalizedAsync(state, x => x.Name)
                });

            if (country == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem)
                {
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = await LocalizationService.GetResourceAsync("Address.SelectState")
                    });
                }
                else
                {
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = await LocalizationService.GetResourceAsync("Address.Other")
                    });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = await LocalizationService.GetResourceAsync("Address.Other")
                    });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = await LocalizationService.GetResourceAsync("Address.SelectState")
                        });
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
