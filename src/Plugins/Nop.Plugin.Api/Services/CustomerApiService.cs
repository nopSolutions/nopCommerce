using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Core.Domain.Messages;
using Nop.Core.Caching;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string LanguageId = "languageid";
        private const string DateOfBirth = "dateofbirth";
        private const string Gender = "gender";
        private const string KeyGroup = "customer";

        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        private readonly IStaticCacheManager _cacheManager;

        public CustomerApiService(IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IStoreContext storeContext,
            ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStaticCacheManager staticCacheManager)
        {
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _subscriptionRepository = subscriptionRepository;
            _storeContext = storeContext;
            _languageService = languageService;
            _storeMappingService = storeMappingService;
            _cacheManager = staticCacheManager;
        }

        public IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetCustomersQuery(createdAtMin, createdAtMax, sinceId);

            var result = HandleCustomerGenericAttributes(null, query, limit, page);

            SetNewsletterSubscribtionStatus(result);

            return result;
        }

        public int GetCustomersCount()
        {
            return _customerRepository.Table.Count(customer => !customer.Deleted
                                      && (customer.RegisteredInStoreId == 0 || customer.RegisteredInStoreId == _storeContext.CurrentStore.Id));
        }

        // Need to work with dto object so we can map the first and last name from generic attributes table.
        public IList<CustomerDto> Search(string queryParams = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit)
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

                result = HandleCustomerGenericAttributes(searchParams, query, limit, page, order);
            }

            return result;
        }

        public Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId)
        {
            return _genericAttributeRepository.Table.Where(
                x =>
                    x.KeyGroup == KeyGroup && x.EntityId == customerId &&
                    (x.Key == FirstName || x.Key == LastName)).ToDictionary(x => x.Key.ToLowerInvariant(), y => y.Value);
        }

        public Customer GetCustomerEntityById(int id)
        {
            var customer = _customerRepository.Table.FirstOrDefault(c => c.Id == id && !c.Deleted);

            return customer;
        }

        public CustomerDto GetCustomerById(int id, bool showDeleted = false)
        {
            if (id == 0)
                return null;

            // Here we expect to get two records, one for the first name and one for the last name.
            var customerAttributeMappings = (from customer in _customerRepository.Table //NoTracking
                                             join attribute in _genericAttributeRepository.Table//NoTracking
                                                                                                on customer.Id equals attribute.EntityId
                                             where customer.Id == id &&
                                                   attribute.KeyGroup == "Customer"
                                             select new CustomerAttributeMappingDto
                                             {
                                                 Attribute = attribute,
                                                 Customer = customer
                                             }).ToList();

            CustomerDto customerDto = null;

            // This is in case we have first and last names set for the customer.
            if (customerAttributeMappings.Count > 0)
            {
                var customer = customerAttributeMappings.First().Customer;
                // The customer object is the same in all mappings.
                customerDto = customer.ToDto();

                var defaultStoreLanguageId = GetDefaultStoreLangaugeId();

                // If there is no Language Id generic attribute create one with the default language id.
                if (!customerAttributeMappings.Any(cam => cam?.Attribute != null && cam.Attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var languageId = new GenericAttribute
                    {
                        Key = LanguageId,
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
                        if (mapping.Attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.FirstName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.LastName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.LanguageId = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.DateOfBirth = string.IsNullOrEmpty(mapping.Attribute.Value) ? (DateTime?)null : DateTime.Parse(mapping.Attribute.Value);
                        }
                        else if (mapping.Attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.Gender = mapping.Attribute.Value;
                        }
                    }
                }
            }
            else
            {
                // This is when we do not have first and last name set.
                var currentCustomer = _customerRepository.Table.FirstOrDefault(customer => customer.Id == id);

                if (currentCustomer != null)
                {
                    if (showDeleted || !currentCustomer.Deleted)
                    {
                        customerDto = currentCustomer.ToDto();
                    }
                }
            }

            SetNewsletterSubscribtionStatus(customerDto);

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

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (var i = 0; i < fieldValueList.Count; i += 2)
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
        /// The idea of this method is to get the first and last name from the GenericAttribute table and to set them in the CustomerDto object.
        /// </summary>
        /// <param name="searchParams">Search parameters is used to shrinc the range of results from the GenericAttibutes table 
        /// to be only those with specific search parameter (i.e. currently we focus only on first and last name).</param>
        /// <param name="query">Query parameter represents the current customer records which we will join with GenericAttributes table.</param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private IList<CustomerDto> HandleCustomerGenericAttributes(IReadOnlyDictionary<string, string> searchParams, IQueryable<Customer> query,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, string order = Configurations.DefaultOrder)
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

            var allRecordsGroupedByCustomerId =
                (from customer in query
                 from attribute in _genericAttributeRepository.Table
                     .Where(attr => attr.EntityId == customer.Id &&
                                    attr.KeyGroup == "Customer").DefaultIfEmpty()
                 select new CustomerAttributeMappingDto
                 {
                     Attribute = attribute,
                     Customer = customer
                 }).GroupBy(x => x.Customer.Id);

            if (searchParams != null && searchParams.Count > 0)
            {
                if (searchParams.ContainsKey(FirstName))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, FirstName, searchParams[FirstName]);
                }

                if (searchParams.ContainsKey(LastName))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, LastName, searchParams[LastName]);
                }

                if (searchParams.ContainsKey(LanguageId))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, LanguageId, searchParams[LanguageId]);
                }

                if (searchParams.ContainsKey(DateOfBirth))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, DateOfBirth, searchParams[DateOfBirth]);
                }

                if (searchParams.ContainsKey(Gender))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, Gender, searchParams[Gender]);
                }
            }

            var result = GetFullCustomerDtos(allRecordsGroupedByCustomerId, page, limit, order);

            return result;
        }

        /// <summary>
        /// This method is responsible for getting customer dto records with first and last names set from the attribute mappings.
        /// </summary>
        private IList<CustomerDto> GetFullCustomerDtos(IQueryable<IGrouping<int, CustomerAttributeMappingDto>> customerAttributesMappings,
        int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit, string order = Configurations.DefaultOrder)
        {
            var customerDtos = new List<CustomerDto>();

            customerAttributesMappings = customerAttributesMappings.OrderBy(x => x.Key);

            IList<IGrouping<int, CustomerAttributeMappingDto>> customerAttributeGroupsList = new ApiList<IGrouping<int, CustomerAttributeMappingDto>>(customerAttributesMappings, page - 1, limit);

            // Get the default language id for the current store.
            var defaultLanguageId = GetDefaultStoreLangaugeId();

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
            if (!attributes.Any(atr => atr != null && atr.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase)))
            {
                var languageId = new GenericAttribute
                {
                    Key = LanguageId,
                    Value = defaultLanguageId.ToString()
                };

                attributes.Add(languageId);
            }

            foreach (var attribute in attributes)
            {
                if (attribute != null)
                {
                    if (attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.FirstName = attribute.Value;
                    }
                    else if (attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.LastName = attribute.Value;
                    }
                    else if (attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.LanguageId = attribute.Value;
                    }
                    else if (attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.DateOfBirth = string.IsNullOrEmpty(attribute.Value) ? (DateTime?)null : DateTime.Parse(attribute.Value);
                    }
                    else if (attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.Gender = attribute.Value;
                    }
                }
            }

            return customerDto;
        }

        private IQueryable<IGrouping<int, CustomerAttributeMappingDto>> GetCustomerAttributesMappingsByKey(
            IQueryable<IGrouping<int, CustomerAttributeMappingDto>> customerAttributesGroups, string key, string value)
        {
            // Here we filter the customerAttributesGroups to be only the ones that have the passed key parameter as a key.
            var customerAttributesMappingByKey = from @group in customerAttributesGroups
                                                 where @group.Select(x => x.Attribute)
                                                             .Any(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                                                                  x.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                                                 select @group;

            return customerAttributesMappingByKey;
        }

        private IQueryable<Customer> GetCustomersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int sinceId = 0)
        {
            var query = _customerRepository.Table //NoTracking
                                                  .Where(customer => !customer.Deleted && !customer.IsSystemAccount && customer.Active);

            query = query.Where(customer => !customer.CustomerCustomerRoleMappings.Any(ccrm => ccrm.CustomerRole.Active && ccrm.CustomerRole.SystemName == NopCustomerDefaults.GuestsRoleName)
            && (customer.RegisteredInStoreId == 0 || customer.RegisteredInStoreId == _storeContext.CurrentStore.Id));

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

        private int GetDefaultStoreLangaugeId()
        {
            // Get the default language id for the current store.
            var defaultLanguageId = _storeContext.CurrentStore.DefaultLanguageId;

            if (defaultLanguageId == 0)
            {
                var allLanguages = _languageService.GetAllLanguages();

                var storeLanguages = allLanguages.Where(l =>
                    _storeMappingService.Authorize(l, _storeContext.CurrentStore.Id)).ToList();

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

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void SetNewsletterSubscribtionStatus(IList<CustomerDto> customerDtos)
        {
            if (customerDtos == null)
            {
                return;
            }

            var allNewsletterCustomerEmail = GetAllNewsletterCustomersEmails();

            foreach (var customerDto in customerDtos)
            {
                SetNewsletterSubscribtionStatus(customerDto, allNewsletterCustomerEmail);
            }
        }

        private void SetNewsletterSubscribtionStatus(BaseCustomerDto customerDto, IEnumerable<String> allNewsletterCustomerEmail = null)
        {
            if (customerDto == null || String.IsNullOrEmpty(customerDto.Email))
            {
                return;
            }

            if (allNewsletterCustomerEmail == null)
            {
                allNewsletterCustomerEmail = GetAllNewsletterCustomersEmails();
            }

            if (allNewsletterCustomerEmail != null && allNewsletterCustomerEmail.Contains(customerDto.Email.ToLowerInvariant()))
            {
                customerDto.SubscribedToNewsletter = true;
            }
        }

        private IEnumerable<String> GetAllNewsletterCustomersEmails()
        {
            return _cacheManager.Get(Configurations.NEWSLETTER_SUBSCRIBERS_KEY, () =>
            {
                IEnumerable<String> subscriberEmails = (from nls in _subscriptionRepository.Table
                                                        where nls.StoreId == _storeContext.CurrentStore.Id
                                                              && nls.Active
                                                        select nls.Email).ToList();


                subscriberEmails = subscriberEmails.Where(e => !String.IsNullOrEmpty(e)).Select(e => e.ToLowerInvariant());

                return subscriberEmails.Where(e => !String.IsNullOrEmpty(e)).Select(e => e.ToLowerInvariant());
            });
        }
    }
}