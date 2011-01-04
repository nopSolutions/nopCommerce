//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Core;

namespace Nop.Services
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Constants
        private const string COUNTRIES_ALL_KEY = "Nop.country.all-{0}";
        private const string COUNTRIES_REGISTRATION_KEY = "Nop.country.registration-{0}";
        private const string COUNTRIES_BILLING_KEY = "Nop.country.billing-{0}";
        private const string COUNTRIES_SHIPPING_KEY = "Nop.country.shipping-{0}";
        private const string COUNTRIES_BY_ID_KEY = "Nop.country.id-{0}";
        private const string COUNTRIES_PATTERN_KEY = "Nop.country.";
        #endregion
        
        #region Fields
        
        private readonly IWorkContext _workContext;
        private readonly IRepository<Country> _countryRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="workContext">Work context</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        public CountryService(IWorkContext workContext,
            ICacheManager cacheManager,
            IRepository<Country> countryRepository)
        {
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._countryRepository = countryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public void DeleteCountry(Country country)
        {
            if (country == null)
                return;

            _countryRepository.Delete(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <returns>Country collection</returns>
        public IList<Country> GetAllCountries()
        {
            bool showHidden = _workContext.IsAdmin;
            string key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where showHidden || c.Published
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow registration
        /// </summary>
        /// <returns>Country collection</returns>
        public IList<Country> GetAllCountriesForRegistration()
        {
            bool showHidden = _workContext.IsAdmin;
            string key = string.Format(COUNTRIES_REGISTRATION_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where (showHidden || c.Published) && c.AllowsRegistration
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <returns>Country collection</returns>
        public IList<Country> GetAllCountriesForBilling()
        {
            bool showHidden = _workContext.IsAdmin;
            string key = string.Format(COUNTRIES_BILLING_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where (showHidden || c.Published) && c.AllowsBilling
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <returns>Country collection</returns>
        public IList<Country> GetAllCountriesForShipping()
        {
            bool showHidden = _workContext.IsAdmin;
            string key = string.Format(COUNTRIES_SHIPPING_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where (showHidden || c.Published) && c.AllowsShipping
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            string key = string.Format(COUNTRIES_BY_ID_KEY, countryId);
            return _cacheManager.Get(key, () =>
            {
                var category = _countryRepository.GetById(countryId);
                return category;
            });
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();

            return country;
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Insert(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);
        }

        #endregion
    }
}