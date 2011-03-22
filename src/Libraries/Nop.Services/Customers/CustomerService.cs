using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Data;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer service
    /// </summary>
    public partial class CustomerService : ICustomerService
    {
        #region Constants

        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        private const string CUSTOMERROLES_BY_ID_KEY = "Nop.customerrole.id-{0}";
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";
        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly CustomerSettings _customerSettings;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="customerRoleRepository">Customer role repository</param>
        /// <param name="customerAttributeRepository">Customer attribute repository</param>
        /// <param name="customerSettings">Customer settings</param>
        public CustomerService(ICacheManager cacheManager,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<CustomerAttribute> customerAttributeRepository,
            CustomerSettings customerSettings)
        {
            this._cacheManager = cacheManager;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._customerAttributeRepository = customerAttributeRepository;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region Methods

        #region Customers
        
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        public PagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, int pageIndex, int pageSize)
        {
            var query = from c in _customerRepository.Table
                        where
                        (!registrationFrom.HasValue || registrationFrom.Value <= c.CreatedOnUtc) &&
                        (!registrationTo.HasValue || registrationTo.Value >= c.CreatedOnUtc) &&
                        (!c.Deleted)
                        orderby c.CreatedOnUtc descending
                        select c;
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);

            return customers;
        }

        /// <summary>
        /// Gets all customers by customer role id
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer collection</returns>
        public IList<Customer> GetCustomersByCustomerRoleId(int customerRoleId, bool showHidden = false)
        {
            var query = from c in _customerRepository.Table
                        from cr in c.CustomerRoles
                        where (showHidden || c.Active) &&
                            !c.Deleted &&
                            cr.Id == customerRoleId
                        orderby c.CreatedOnUtc descending
                        select c;

            //var query = from c in _context.Customers
            //            where (showHidden || c.Active) && !c.Deleted
            //            && c.NpCustomerRoles.Any(cr => cr.CustomerRoleId == customerRoleId)
            //            orderby c.RegistrationDate descending
            //            select c;


            //var query = _context.CustomerRoles.Where(cr => cr.CustomerRoleId == customerRoleId)
            //    .SelectMany(cr => cr.NpCustomers);
            //if (!showHidden)
            //    query = query.Where(c => c.Active);
            //query = query.Where(c => !c.Deleted);
            //query = query.OrderByDescending(c => c.RegistrationDate);
            //var customers = query.ToList();

            var customers = query.ToList();
            return customers;
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            customer.Deleted = true;
            UpdateCustomer(customer);
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        public Customer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;
            
            var customer = _customerRepository.GetById(customerId);
            return customer;
        }

        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>A customer</returns>
        public Customer GetCustomerByGuid(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var query = from c in _customerRepository.Table
                        where c.CustomerGuid == customerGuid
                        orderby c.Id
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="associatedUserId">User identifier</param>
        /// <returns>A customer</returns>
        public Customer GetCustomerByAssociatedUserId(int associatedUserId)
        {
            if (associatedUserId == 0)
                return null;

            var query = from c in _customerRepository.Table
                        where c.AssociatedUserId == associatedUserId
                        orderby c.Id
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Customer</returns>
        public Customer InsertGuestCustomer()
        {
            //TODO save current language, currency, tax display type, etc
            var customer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = string.Empty,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");
            customer.CustomerRoles = new List<CustomerRole> { guestRole };

            _customerRepository.Insert(customer);

            return customer;
        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer</returns>
        public Customer RegisterCustomer(int customerId)
        {
            var customer = GetCustomerById(customerId);

            if (customer == null)
                throw new NopException(string.Format("Customer {0} could not be loaded", customerId));

            //TODO pass and save customer attributes as argument
            //TODO check CustomerRegistrationType (Disabled, EmailValidation, AdminApproval)
            
            //add to 'Registered' role
            var registeredRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");
            customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = customer.CustomerRoles.FirstOrDefault(cr=>cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                customer.CustomerRoles.Remove(guestRole);

            _customerRepository.Update(customer);

            //TODO add reward points for registration (if enabled)
            //TODO Send welcome message / email validation message
            return customer;
        }

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //var subscriptionOld = customer.NewsLetterSubscription;

            _customerRepository.Update(customer);

            //UNDONE update newsletter subscription
            //if (subscriptionOld != null && !customer.Email.ToLower().Equals(subscriptionOld.Email.ToLower()))
            //{
            //    subscriptionOld.Email = customer.Email;
            //    IoC.Resolve<IMessageService>().UpdateNewsLetterSubscription(subscriptionOld);
            //}
        }

        #endregion
        
        #region Customer roles

        /// <summary>
        /// Delete a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public void DeleteCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            if (customerRole.IsSystemRole)
                throw new NopException("System role could not be deleted");

            //clear many-to-many navigation property because EF doesn't allow to configure cascade delete for this type of associations
            customerRole.Customers.Clear();
            customerRole.PermissionRecords.Clear();

            _customerRoleRepository.Delete(customerRole);
            
            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        public CustomerRole GetCustomerRoleById(int customerRoleId)
        {
            if (customerRoleId == 0)
                return null;

            string key = string.Format(CUSTOMERROLES_BY_ID_KEY, customerRoleId);
            return _cacheManager.Get(key, () =>
            {
                var customerRole = _customerRoleRepository.GetById(customerRoleId);
                return customerRole;
            });
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="systemName">Customer role system name</param>
        /// <returns>Customer role</returns>
        public CustomerRole GetCustomerRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(CUSTOMERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var customerRole = query.FirstOrDefault();
                return customerRole;
            });
        }

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer role collection</returns>
        public IList<CustomerRole> GetAllCustomerRoles(bool showHidden = false)
        {
            string key = string.Format(CUSTOMERROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var customerRoles = query.ToList();
                return customerRoles;
            });
        }
        
        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer role collection</returns>
        public IList<CustomerRole> GetCustomerRolesByCustomerId(int customerId, bool showHidden = false)
        {
            if (customerId == 0)
                return new List<CustomerRole>();

            var query = from cr in _customerRoleRepository.Table
                        from c in cr.Customers
                        where (showHidden || cr.Active) &&
                            c.Id == customerId
                        orderby cr.Name descending
                        select cr;

            var customerRoles = query.ToList();
            return customerRoles;
        }

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public void InsertCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Insert(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public void UpdateCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Update(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
        }

        #endregion

        #region Customer attributes

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public void DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Delete(customerAttribute);
        }

        /// <summary>
        /// Gets a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>A customer attribute</returns>
        public CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return null;

            var customerAttribute = _customerAttributeRepository.GetById(customerAttributeId);
            return customerAttribute;
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Insert(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Update(customerAttribute);
        }

        /// <summary>
        /// Save customer attribute
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customer">Customer</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Customer attribute</returns>
        public CustomerAttribute SaveCustomerAttribute<T>(Customer customer,
            string key, T value)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (!TypeDescriptor.GetConverter(typeof(T)).CanConvertTo(typeof(string)))
                throw new NopException("Not supported customer attribute type");

            string valueStr = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value);
            //use the code below in order to support all serializable types (for example, ShippingOption)
            //or use custom TypeConverters like it's implemented for ISettings
            //using (var tr = new StringReader(customerAttribute.Value))
            //{
            //    var xmlS = new XmlSerializer(typeof(T));
            //    valueStr = (T)xmlS.Deserialize(tr);
            //}
            
            if (customer.CustomerAttributes == null)
                customer.CustomerAttributes = new List<CustomerAttribute>();
            var customerAttribute = customer.CustomerAttributes.FirstOrDefault(ca => ca.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (customerAttribute != null)
            {
                //update
                customerAttribute.Value = valueStr;
                UpdateCustomerAttribute(customerAttribute);
            }
            else
            {
                //insert
                customerAttribute = new CustomerAttribute()
                {
                    Customer = customer,
                    Key = key,
                    Value = valueStr,
                };
                InsertCustomerAttribute(customerAttribute);
            }

            return customerAttribute;
        }

        #endregion

        #endregion
    }
}