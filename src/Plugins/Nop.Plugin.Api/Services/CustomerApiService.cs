using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Caching;
using System.Threading.Tasks;
using Nop.Services.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Common;
using Nop.Services.Directory;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private static readonly string FIRST_NAME = "FirstName".ToLowerInvariant();
        private static readonly string LAST_NAME = "LastName".ToLowerInvariant();
        private static readonly string LANGUAGE_ID = "LanguageId".ToLowerInvariant();
        private static readonly string CURRENCY_ID = "CurrencyId".ToLowerInvariant();
        private static readonly string DATE_OF_BIRTH = "DateOfBirth".ToLowerInvariant();
        private static readonly string GENDER = "Gender".ToLowerInvariant();
        private static readonly string VAT_NUMBER = "VatNumber".ToLowerInvariant();
        private static readonly string VAT_NUMBER_STATUS_ID = "VatNumberStatusId".ToLowerInvariant();
        private static readonly string EU_COOKIE_LAW_ACCEPTED = "EuCookieLawAccepted".ToLowerInvariant();
        private static readonly string COMPANY = "Company".ToLowerInvariant();

        private readonly IStaticCacheManager _cacheManager;
		private readonly IAddressApiService _addressApiService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly ICurrencyService _currencyService;
		private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ILanguageService _languageService;

        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        
		public CustomerApiService(
            IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IStoreContext storeContext,
            ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStaticCacheManager staticCacheManager,
            IAddressApiService addressApiService,
            IGenericAttributeService genericAttributeService,
            ICurrencyService currencyService)
        {
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _subscriptionRepository = subscriptionRepository;
			_storeContext = storeContext;
            _languageService = languageService;
            _storeMappingService = storeMappingService;
            _cacheManager = staticCacheManager;
			_addressApiService = addressApiService;
			_genericAttributeService = genericAttributeService;
			_currencyService = currencyService;
		}

        public async Task<IList<CustomerDto>> GetCustomersDtosAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = Constants.Configurations.DefaultLimit,
            int page = Constants.Configurations.DefaultPageValue, int sinceId = Constants.Configurations.DefaultSinceId)
        {
            var query = GetCustomersQuery(createdAtMin, createdAtMax, sinceId);

            var result = await HandleCustomerGenericAttributesAsync(null, query, limit, page);

            await SetNewsletterSubscriptionStatusAsync(result);

            foreach (CustomerDto customerDto in result)
            {
                var customer = await query.Where(x => x.Id == customerDto.Id).FirstOrDefaultAsync();

                await SetCustomerAddressesAsync(customer, customerDto);
            }

            return result;
        }

        public Task<int> GetCustomersCountAsync()
        {
            return _customerRepository.Table.CountAsync(customer => !customer.Deleted
                                                               && (customer.RegisteredInStoreId == 0 ||
                                                                   customer.RegisteredInStoreId == _storeContext.GetCurrentStore().Id));
        }

        // Need to work with dto object so we can map the first and last name from generic attributes table.
        public async Task<IList<CustomerDto>> SearchAsync(
            string queryParams = "", string order = Constants.Configurations.DefaultOrder,
            int page = Constants.Configurations.DefaultPageValue, int limit = Constants.Configurations.DefaultLimit)
        {
            IList<CustomerDto> result = new List<CustomerDto>();

            var searchParams = EnsureSearchQueryIsValid(queryParams, ParseSearchQuery);

            if (searchParams != null)
            {
                var query = _customerRepository.Table.Where(customer => !customer.Deleted);

                foreach (var searchParam in searchParams)
                {
                    // Skip non existing properties.
                    if (ReflectionHelper.HasProperty(searchParam.Key, typeof(Customer)))
                    {
                        // @0 is a placeholder used by dynamic linq and it is used to prevent possible sql injections.
                        query = query.Where(string.Format("{0} = @0 || {0}.Contains(@0)", searchParam.Key), searchParam.Value);
                    }
                    // The code bellow will search in customer addresses as well.
                    //else if (HasProperty(searchParam.Key, typeof(Address)))
                    //{
                    //    query = query.Where(string.Format("Addresses.Where({0} == @0).Any()", searchParam.Key), searchParam.Value);
                    //}
                }

                result = await HandleCustomerGenericAttributesAsync(searchParams, query, limit, page, order);

                foreach (CustomerDto customerDto in result)
                {
                    var customer = await query.Where(x => x.Id == customerDto.Id).FirstOrDefaultAsync();

                    await SetCustomerAddressesAsync(customer, customerDto);
                }
            }            

            return result;
        }

        public Task<Dictionary<string, string>> GetFirstAndLastNameByCustomerIdAsync(int customerId)
        {
            return _genericAttributeRepository.Table.Where(
                                                           x =>
                                                               x.KeyGroup == nameof(Customer) && x.EntityId == customerId &&
                                                               (x.Key == FIRST_NAME || x.Key == LAST_NAME))
                                              .ToDictionaryAsync(x => x.Key.ToLowerInvariant(), y => y.Value);
        }

        public async Task<Customer> GetCustomerEntityByIdAsync(int id)
        {
            var customer = await _customerRepository.Table.FirstOrDefaultAsync(c => c.Id == id && !c.Deleted);

            return customer;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id, bool showDeleted = false)
        {
            if (id == 0)
            {
                return null;
            }

            // Here we expect to get two records, one for the first name and one for the last name.
            var customerAttributeMappings = await (from c in _customerRepository.Table //NoTracking
                                             join a in _genericAttributeRepository.Table //NoTracking
                                                 on c.Id equals a.EntityId
                                             where c.Id == id &&
                                                   a.KeyGroup == nameof(Customer)
                                                   select new CustomerAttributeMappingDto
                                             {
                                                 Attribute = a,
                                                 Customer = c
                                             }).ToListAsync();

            Customer customer = null;
            CustomerDto customerDto = null;

            // This is in case we have first and last names set for the customer.
            if (customerAttributeMappings.Count > 0)
            {
                customer = customerAttributeMappings.First().Customer;
                // The customer object is the same in all mappings.
                customerDto = customer.ToDto();

                var defaultStoreLanguageId = await GetDefaultStoreLangaugeIdAsync();

                // If there is no Language Id generic attribute create one with the default language id.
                if (!customerAttributeMappings.Any(cam => cam?.Attribute != null &&
                                                          cam.Attribute.Key.Equals(LANGUAGE_ID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var languageId = new GenericAttribute
                    {
                        Key = LANGUAGE_ID,
                        Value = defaultStoreLanguageId.ToString()
                    };

                    var customerAttributeMappingDto = new CustomerAttributeMappingDto
                    {
                        Customer = customer,
                        Attribute = languageId
                    };

                    customerAttributeMappings.Add(customerAttributeMappingDto);
                }

                foreach (var mapping in customerAttributeMappings)
                {
                    if (!showDeleted && mapping.Customer.Deleted)
                    {
                        continue;
                    }

                    if (mapping.Attribute != null)
                    {
                        if (mapping.Attribute.Key.Equals(FIRST_NAME, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.FirstName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LAST_NAME, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.LastName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LANGUAGE_ID, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(mapping.Attribute.Value, out int languageId))
                        {
                            customerDto.LanguageId = languageId;
                        }
                        else if (mapping.Attribute.Key.Equals(CURRENCY_ID, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(mapping.Attribute.Value, out int currencyId))
                        {
                            customerDto.CurrencyId = currencyId;
                        }
                        else if (mapping.Attribute.Key.Equals(DATE_OF_BIRTH, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.DateOfBirth = string.IsNullOrEmpty(mapping.Attribute.Value)
                                                          ? (DateTime?)null
                                                          : DateTime.Parse(mapping.Attribute.Value);
                        }
                        else if (mapping.Attribute.Key.Equals(GENDER, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.Gender = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(VAT_NUMBER, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.VatNumber = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(VAT_NUMBER_STATUS_ID, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.VatNumberStatusId = string.IsNullOrWhiteSpace(mapping.Attribute.Value)
                                                                ? (int?)null
                                                                : int.Parse(mapping.Attribute.Value);
                        }
                        else if (mapping.Attribute.Key.Equals(COMPANY, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.Company = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(EU_COOKIE_LAW_ACCEPTED, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.EuCookieLawAccepted = string.IsNullOrWhiteSpace(mapping.Attribute.Value)
                                                                ? (bool?)null
                                                                : bool.Parse(mapping.Attribute.Value);
                        }
                    }
                }
            }
            else
            {
                // This is when we do not have first and last name set.
                customer = _customerRepository.Table.FirstOrDefault(c => c.Id == id);

                if (customer != null)
                {
                    if (showDeleted || !customer.Deleted)
                    {
                        customerDto = customer.ToDto();
                    }
                }
            }

            await SetNewsletterSubscriptionStatusAsync(customerDto);

            await SetCustomerAddressesAsync(customer, customerDto);

            return customerDto;
        }

		private Dictionary<string, string> EnsureSearchQueryIsValid(string query, Func<string, Dictionary<string, string>> parseSearchQuery)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return parseSearchQuery(query);
            }

            return null;
        }

        private Dictionary<string, string> ParseSearchQuery(string query)
        {
            var parsedQuery = new Dictionary<string, string>();

            var splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != string.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (var i = 0;
                 i < fieldValueList.Count;
                 i += 2)
            {
                var field = fieldValueList[i];
                var value = fieldValueList[i + 1];

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    field = field.Replace("_", string.Empty);
                    parsedQuery.Add(field.Trim(), value.Trim());
                }
            }

            return parsedQuery;
        }

        /// <summary>
        ///     The idea of this method is to get the first and last name from the GenericAttribute table and to set them in the
        ///     CustomerDto object.
        /// </summary>
        /// <param name="searchParams">
        ///     Search parameters is used to shrinc the range of results from the GenericAttibutes table
        ///     to be only those with specific search parameter (i.e. currently we focus only on first and last name).
        /// </param>
        /// <param name="query">
        ///     Query parameter represents the current customer records which we will join with GenericAttributes
        ///     table.
        /// </param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<IList<CustomerDto>> HandleCustomerGenericAttributesAsync(
            IReadOnlyDictionary<string, string> searchParams, IQueryable<Customer> query,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            string order = Constants.Configurations.DefaultOrder)
        {
            // Here we join the GenericAttribute records with the customers and making sure that we are working only with the attributes
            // that are in the customers keyGroup and their keys are either first or last name.
            // We are returning a collection with customer record and attribute record. 
            // It will look something like:
            // customer data for customer 1
            //      attribute that contains the first name of customer 1
            //      attribute that contains the last name of customer 1
            // customer data for customer 2, 
            //      attribute that contains the first name of customer 2
            //      attribute that contains the last name of customer 2
            // etc.

            var allRecords =
                 from customer in query
                 from attribute in _genericAttributeRepository.Table
                                                              .Where(attr => attr.EntityId == customer.Id &&
                                                                             attr.KeyGroup == nameof(Customer)).DefaultIfEmpty()
                 select new CustomerAttributeMappingDto
                 {
                     Attribute = attribute,
                     Customer = customer
                 };

            if (searchParams != null && searchParams.Count > 0)
            {
                if (searchParams.ContainsKey(FIRST_NAME))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, FIRST_NAME, searchParams[FIRST_NAME]);
                }

                if (searchParams.ContainsKey(LAST_NAME))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, LAST_NAME, searchParams[LAST_NAME]);
                }

                if (searchParams.ContainsKey(LANGUAGE_ID))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, LANGUAGE_ID, searchParams[LANGUAGE_ID]);
                }

                if (searchParams.ContainsKey(DATE_OF_BIRTH))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, DATE_OF_BIRTH, searchParams[DATE_OF_BIRTH]);
                }

                if (searchParams.ContainsKey(GENDER))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, GENDER, searchParams[GENDER]);
                }
                if (searchParams.ContainsKey(VAT_NUMBER))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, VAT_NUMBER, searchParams[VAT_NUMBER]);
                }
                if (searchParams.ContainsKey(VAT_NUMBER_STATUS_ID))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, VAT_NUMBER_STATUS_ID, searchParams[VAT_NUMBER_STATUS_ID]);
                }
                if (searchParams.ContainsKey(EU_COOKIE_LAW_ACCEPTED))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, EU_COOKIE_LAW_ACCEPTED, searchParams[EU_COOKIE_LAW_ACCEPTED]);
                }
                if (searchParams.ContainsKey(COMPANY))
                {
                    allRecords = GetCustomerAttributesMappingsByKey(allRecords, COMPANY, searchParams[COMPANY]);
                }
            }

            var allRecordsGroupedByCustomerId = allRecords
                .AsEnumerable<CustomerAttributeMappingDto>() // convert to IEnumerable (materialize the query) as LinqToDb does not support GroupBy
                .GroupBy(x => x.Customer.Id) // do grouping in memory on materialized sequence
                .AsQueryable(); // convert back to queryable just to be accepted by a following method

            var result = await GetFullCustomerDtosAsync(allRecordsGroupedByCustomerId, page, limit, order);

            return result;
        }

        /// <summary>
        ///     This method is responsible for getting customer dto records with first and last names set from the attribute
        ///     mappings.
        /// </summary>
        private async Task<IList<CustomerDto>> GetFullCustomerDtosAsync(
            IQueryable<IGrouping<int, CustomerAttributeMappingDto>> customerAttributesMappings,
            int page = Constants.Configurations.DefaultPageValue, int limit = Constants.Configurations.DefaultLimit,
            string order = Constants.Configurations.DefaultOrder)
        {
            var customerDtos = new List<CustomerDto>();

            customerAttributesMappings = customerAttributesMappings.OrderBy(x => x.Key);

            IList<IGrouping<int, CustomerAttributeMappingDto>> customerAttributeGroupsList =
                new ApiList<IGrouping<int, CustomerAttributeMappingDto>>(customerAttributesMappings, page - 1, limit);

            // Get the default language id for the current store.
            var defaultLanguageId = await GetDefaultStoreLangaugeIdAsync();

            foreach (var group in customerAttributeGroupsList)
            {
                IList<CustomerAttributeMappingDto> mappingsForMerge = group.Select(x => x).ToList();

                var customerDto = Merge(mappingsForMerge, defaultLanguageId);

                customerDtos.Add(customerDto);
            }

            // Needed so we can apply the order parameter
            return customerDtos.AsQueryable().OrderBy(order).ToList();
        }

        private static CustomerDto Merge(IList<CustomerAttributeMappingDto> mappingsForMerge, int defaultLanguageId)
        {
            // We expect the customer to be always set.
            var customerDto = mappingsForMerge.First().Customer.ToDto();

            var attributes = mappingsForMerge.Select(x => x.Attribute).ToList();

            // If there is no Language Id generic attribute create one with the default language id.
            if (!attributes.Any(atr => atr != null && atr.Key.Equals(LANGUAGE_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                var languageId = new GenericAttribute
                {
                    Key = LANGUAGE_ID,
                    Value = defaultLanguageId.ToString()
                };

                attributes.Add(languageId);
            }

            foreach (var attribute in attributes)
            {
                if (attribute != null)
                {
                    if (attribute.Key.Equals(FIRST_NAME, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.FirstName = attribute.Value;
                    }
                    else if (attribute.Key.Equals(LAST_NAME, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.LastName = attribute.Value;
                    }
                    else if (attribute.Key.Equals(LANGUAGE_ID, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(attribute.Value, out int languageId))
                    {
                        customerDto.LanguageId = languageId;
                    }
                    else if (attribute.Key.Equals(CURRENCY_ID, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(attribute.Value, out int currencyId))
                    {
                        customerDto.CurrencyId = currencyId;
                    }
                    else if (attribute.Key.Equals(DATE_OF_BIRTH, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.DateOfBirth = string.IsNullOrEmpty(attribute.Value)
                                                      ? (DateTime?)null
                                                      : DateTime.Parse(attribute.Value);
                    }
                    else if (attribute.Key.Equals(GENDER, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.Gender = attribute.Value;
                    }
                    else if (attribute.Key.Equals(VAT_NUMBER, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.VatNumber = attribute.Value;
                    }
                    else if (attribute.Key.Equals(VAT_NUMBER_STATUS_ID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.VatNumberStatusId = string.IsNullOrWhiteSpace(attribute.Value)
                                                            ? (int?)null
                                                            : int.Parse(attribute.Value);
                    }
                    else if (attribute.Key.Equals(COMPANY, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.Company = attribute.Value;
                    }
                    else if (attribute.Key.Equals(EU_COOKIE_LAW_ACCEPTED, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.EuCookieLawAccepted = string.IsNullOrWhiteSpace(attribute.Value)
                                                            ? (bool?)null
                                                            : bool.Parse(attribute.Value);
                    }
                }
            }

            return customerDto;
        }

        private IQueryable<CustomerAttributeMappingDto> GetCustomerAttributesMappingsByKey(
            IQueryable<CustomerAttributeMappingDto> customerAttributes, string key, string value)
        {
            // Here we filter the customerAttributesGroups to be only the ones that have the passed key parameter as a key.
            var filteredCustomerAttributes = from a in customerAttributes
                                                 where a.Attribute.Key.Equals(key) && a.Attribute.Value.Equals(value)
                                                 select a;

            return filteredCustomerAttributes;
        }

        private IQueryable<Customer> GetCustomersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int sinceId = 0)
        {
            int currentStoreId = _storeContext.GetCurrentStore().Id;

            var query = _customerRepository.Table.Where(customer => !customer.Deleted && !customer.IsSystemAccount && customer.Active);

            //query = query.Where(customer =>
            //                        !customer.CustomerCustomerRoleMappings.Any(ccrm => ccrm.CustomerRole.Active &&
            //                                                                           ccrm.CustomerRole.SystemName == NopCustomerDefaults.GuestsRoleName)
            //                        && (customer.RegisteredInStoreId == 0 || customer.RegisteredInStoreId == _storeContext.CurrentStore.Id));

            query = query.Where(customer => (customer.RegisteredInStoreId == 0 || customer.RegisteredInStoreId == currentStoreId));

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            query = query.OrderBy(customer => customer.Id);

            if (sinceId > 0)
            {
                query = query.Where(customer => customer.Id > sinceId);
            }

            return query;
        }

        private async Task<int> GetDefaultStoreLangaugeIdAsync()
        {
            // Get the default language id for the current store.
            var defaultLanguageId = _storeContext.GetCurrentStore().DefaultLanguageId;

            if (defaultLanguageId == 0)
            {
                var allLanguages = await _languageService.GetAllLanguagesAsync();

                int currentStoreId = _storeContext.GetCurrentStore().Id;

                var storeLanguages = await allLanguages.WhereAwait(async l => await _storeMappingService.AuthorizeAsync(l, currentStoreId)).ToListAsync();

                // If there is no language mapped to the current store, get all of the languages,
                // and use the one with the first display order. This is a default nopCommerce workflow.
                if (storeLanguages.Count == 0)
                {
                    storeLanguages = allLanguages.ToList();
                }

                var defaultLanguage = storeLanguages.OrderBy(l => l.DisplayOrder).First();

                defaultLanguageId = defaultLanguage.Id;
            }

            return defaultLanguageId;
        }

        private async Task SetNewsletterSubscriptionStatusAsync(IList<CustomerDto> customerDtos)
        {
            if (customerDtos == null)
            {
                return;
            }

            var allNewsletterCustomerEmail = await GetAllNewsletterCustomersEmailsAsync();

            foreach (var customerDto in customerDtos)
            {
                await SetNewsletterSubscriptionStatusAsync(customerDto, allNewsletterCustomerEmail);
            }
        }

        private async Task SetNewsletterSubscriptionStatusAsync(BaseCustomerDto customerDto, IEnumerable<string> allNewsletterCustomerEmail = null)
        {
            if (customerDto == null || string.IsNullOrEmpty(customerDto.Email))
            {
                return;
            }

            if (allNewsletterCustomerEmail == null)
            {
                allNewsletterCustomerEmail = await GetAllNewsletterCustomersEmailsAsync();
            }

            if (allNewsletterCustomerEmail != null && allNewsletterCustomerEmail.Contains(customerDto.Email.ToLowerInvariant()))
            {
                customerDto.SubscribedToNewsletter = true;
            }
        }

        private Task<List<string>> GetAllNewsletterCustomersEmailsAsync()
        {
            int currentStoreId = _storeContext.GetCurrentStore().Id;
            return _cacheManager.GetAsync(Constants.Configurations.NEWSLETTER_SUBSCRIBERS_KEY, async () =>
            {
                var subscriberEmails = await (from nls in _subscriptionRepository.Table
                                              where nls.StoreId == currentStoreId && nls.Active
                                              select nls.Email).ToListAsync();
                return subscriberEmails.Where(e => !string.IsNullOrEmpty(e)).Select(e => e.ToLowerInvariant()).ToList();
            });
        }

        private async Task SetCustomerAddressesAsync(Customer customer, CustomerDto customerDto)
        {
            customerDto.Addresses = await _addressApiService.GetAddressesByCustomerIdAsync(customer.Id);
            if (customer.BillingAddressId != null)
                customerDto.BillingAddress = await _addressApiService.GetCustomerAddressAsync(customer.Id, customer.BillingAddressId.Value);
            else
                customerDto.BillingAddress = null;
            if (customer.ShippingAddressId != null)
                customerDto.ShippingAddress = await _addressApiService.GetCustomerAddressAsync(customer.Id, customer.ShippingAddressId.Value);
            else
                customerDto.ShippingAddress = null;
        }

		public async Task<Language> GetCustomerLanguageAsync(Customer customer)
		{
            //var store = await _storeContext.GetCurrentStoreAsync();

            //get current customer language identifier
            var customerLanguageId = await _genericAttributeService.GetAttributeAsync<int>(customer, "LanguageId"/*, store.Id*/);

            var customerLanguage = await _languageService.GetLanguageByIdAsync(customerLanguageId);
            return customerLanguage;

            // the following code tries to find the default language if attribute is not found >>>

            //var allStoreLanguages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);

            ////check customer language availability
            //var customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == customerLanguageId);
            //if (customerLanguage == null)
            //{
            //    //it not found, then try to get the default language for the current store (if specified)
            //    customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == store.DefaultLanguageId);
            //}

            ////if the default language for the current store not found, then try to get the first one
            //if (customerLanguage == null)
            //{
            //    customerLanguage = allStoreLanguages.FirstOrDefault();
            //}

            ////if there are no languages for the current store try to get the first one regardless of the store
            //if (customerLanguage == null)
            //{
            //    customerLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
            //}

            //return customerLanguage;
        }

        public async Task SetCustomerLanguageAsync(Customer customer, Language language)
		{
            //var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(customer, "LanguageId", language?.Id ?? 0/*, store.Id*/);
        }

		public async Task<Currency> GetCustomerCurrencyAsync(Customer customer)
		{
            //var store = await _storeContext.GetCurrentStoreAsync();

            //find a currency previously selected by a customer
            var customerCurrencyId = await _genericAttributeService.GetAttributeAsync<int>(customer, "CurrencyId"/*, store.Id*/);

            var customerCurrency = await _currencyService.GetCurrencyByIdAsync(customerCurrencyId);
            return customerCurrency;

            // the following code tries to find the default currency if attribute is not found >>>

            //var allStoreCurrencies = await _currencyService.GetAllCurrenciesAsync(storeId: store.Id);

            ////check customer currency availability
            //var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customerCurrencyId);
            //if (customerCurrency == null)
            //{
            //    //it not found, then try to get the default currency for the current language (if specified)
            //    var language = await GetCustomerLanguageAsync(customer);
            //    customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
            //}

            ////if the default currency for the current store not found, then try to get the first one
            //if (customerCurrency == null)
            //{
            //    customerCurrency = allStoreCurrencies.FirstOrDefault();
            //}

            ////if there are no currencies for the current store try to get the first one regardless of the store
            //if (customerCurrency == null)
            //{
            //    customerCurrency = (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();
            //}

            //return customerCurrency;
        }

		public async Task SetCustomerCurrencyAsync(Customer customer, Currency currency)
		{
            //var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(customer, "CurrencyId", currency?.Id ?? 0/*, store.Id*/);
        }
	}
}