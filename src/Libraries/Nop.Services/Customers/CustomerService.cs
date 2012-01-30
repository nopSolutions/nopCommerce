using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;

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
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="customerRoleRepository">Customer role repository</param>
        /// <param name="customerAttributeRepository">Customer attribute repository</param>
        /// <param name="eventPublisher"></param>
        public CustomerService(ICacheManager cacheManager,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._customerAttributeRepository = customerAttributeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Customers
        
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="email">Email; null to load all customers</param>
        /// <param name="username">Username; null to load all customers</param>
        /// <param name="firstName">First name; null to load all customers</param>
        /// <param name="lastName">Last name; null to load all customers</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all customers</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all customers</param>
        /// <param name="loadOnlyWithShoppingCart">Value indicating whther to load customers only with shopping cart</param>
        /// <param name="sct">Value indicating what shopping cart type to filter; userd when 'loadOnlyWithShoppingCart' param is 'true'</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        public virtual IPagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, int[] customerRoleIds, string email, string username,
            string firstName, string lastName, int dayOfBirth, int monthOfBirth,
            bool loadOnlyWithShoppingCart, ShoppingCartType? sct, int pageIndex, int pageSize)
        {
            var query = _customerRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);
            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Intersect(customerRoleIds).Count() > 0);
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!String.IsNullOrWhiteSpace(firstName))
                query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.FirstName && ca.Value.Contains(firstName)).Count() > 0);
            if (!String.IsNullOrWhiteSpace(lastName))
                query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.LastName && ca.Value.Contains(lastName)).Count() > 0);
            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.DateOfBirth && ca.Value.EndsWith(dateOfBirthStr)).Count() > 0);
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00
                query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.DateOfBirth && ca.Value.Substring(ca.Value.Length - dateOfBirthStr.Length, dateOfBirthStr.Length) == dateOfBirthStr).Count() > 0);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.DateOfBirth && ca.Value.EndsWith(dateOfBirthStr)).Count() > 0);
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00
                query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.DateOfBirth && ca.Value.Substring(ca.Value.Length - dateOfBirthStr.Length, dateOfBirthStr.Length) == dateOfBirthStr).Count() > 0);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query.Where(c => c.CustomerAttributes.Where(ca => ca.Key == SystemCustomerAttributeNames.DateOfBirth && ca.Value.Contains(dateOfBirthStr)).Count() > 0);
            }


            if (loadOnlyWithShoppingCart)
            {
                int? sctId = null;
                if (sct.HasValue)
                    sctId = (int)sct.Value;

                query = sct.HasValue ?
                    query.Where(c => c.ShoppingCartItems.Where(x => x.ShoppingCartTypeId == sctId).Count() > 0) :
                    query.Where(c => c.ShoppingCartItems.Count() > 0);
            }
            
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var customers = new PagedList<Customer>(query, pageIndex, pageSize);
            return customers;
        }

        /// <summary>
        /// Gets online customers
        /// </summary>
        /// <param name="lastActivityFromUtc">Customer last activity date (from)</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        public virtual IPagedList<Customer> GetOnlineCustomers(DateTime lastActivityFromUtc,
            int[] customerRoleIds, int pageIndex, int pageSize)
        {
            var query = _customerRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Intersect(customerRoleIds).Count() > 0);
            
            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);
            return customers;
        }

        /// <summary>
        /// Gets all customers by customer role id
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer collection</returns>
        public virtual IList<Customer> GetCustomersByCustomerRoleId(int customerRoleId, bool showHidden = false)
        {
            var query = from c in _customerRepository.Table
                        from cr in c.CustomerRoles
                        where (showHidden || c.Active) &&
                            !c.Deleted &&
                            cr.Id == customerRoleId
                        orderby c.CreatedOnUtc descending
                        select c;
            
            var customers = query.ToList();
            return customers;
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void DeleteCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.IsSystemAccount)
                throw new NopException(string.Format("System customer account ({0}) could not be deleted", customer.SystemName));

            customer.Deleted = true;
            UpdateCustomer(customer);
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        public virtual Customer GetCustomerById(int customerId)
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
        public virtual Customer GetCustomerByGuid(Guid customerGuid)
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
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Customer</returns>
        public virtual Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Get customer by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Customer</returns>
        public virtual Customer GetCustomerBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Get customer by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Customer</returns>
        public virtual Customer GetCustomerByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Get customers by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Customers</returns>
        public virtual IList<Customer> GetCustomersByLanguageId(int languageId)
        {
            var query = _customerRepository.Table;
            if (languageId > 0)
                query = query.Where(c => c.LanguageId.HasValue && c.LanguageId.Value == languageId);
            else
                query = query.Where(c => !c.LanguageId.HasValue);
            query = query.OrderBy(c => c.Id);
            var customers = query.ToList();
            return customers;
        }

        /// <summary>
        /// Get customers by currency identifier
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Customers</returns>
        public virtual IList<Customer> GetCustomersByCurrencyId(int currencyId)
        {
            var query = _customerRepository.Table;
            if (currencyId > 0)
                query = query.Where(c => c.CurrencyId.HasValue && c.CurrencyId.Value == currencyId);
            else
                query = query.Where(c => !c.CurrencyId.HasValue);
            query = query.OrderBy(c => c.Id);
            var customers = query.ToList();
            return customers;
        }

        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>Customer</returns>
        public virtual Customer InsertGuestCustomer()
        {
            var customer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");
            customer.CustomerRoles.Add(guestRole);

            _customerRepository.Insert(customer);

            return customer;
        }
        
        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void InsertCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }
        
        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Update(customer);

            //event notification
            _eventPublisher.EntityUpdated(customer);
        }

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        public virtual void ResetCheckoutData(Customer customer, bool clearCouponCodes = false)
        {
            if (customer == null)
                throw new ArgumentNullException();

            //clear reward points flag
            customer.UseRewardPointsDuringCheckout = false;

            //clear selected shipping and payment methods
            SaveCustomerAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.LastShippingOption, null);
            customer.SelectedPaymentMethodSystemName = "";

            //clear entered coupon codes
            if (clearCouponCodes)
            {
                customer.DiscountCouponCode = "";
                customer.GiftCardCouponCodes = "";
                customer.CheckoutAttributes = "";
            }
            UpdateCustomer(customer);
        }
        
        /// <summary>
        /// Delete guest customer records
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete customers only without shopping cart</param>
        /// <returns>Number of deleted customers</returns>
        public virtual int DeleteGuestCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, bool onlyWithoutShoppingCart)
        {
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");

            var query = _customerRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Contains(guestRole.Id));
            if (onlyWithoutShoppingCart)
                query = query.Where(c => c.ShoppingCartItems.Count() == 0);
            //no orders
            query = query.Where(c => c.Orders.Count() == 0);
            //no customer content
            query = query.Where(c => c.CustomerContent.Count() == 0);
            //ensure that customers doesn't have forum posts or topics
            query = query.Where(c => c.ForumTopics.Count() == 0);
            query = query.Where(c => c.ForumPosts.Count() == 0);
            //don't delete system accounts
            query = query.Where(c => !c.IsSystemAccount);
            var customers = query.ToList();


            int numberOfDeletedCustomers = 0;
            foreach (var c in customers)
            {
                try
                {
                    //delete from database
                    _customerRepository.Delete(c);
                    numberOfDeletedCustomers++;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }
            return numberOfDeletedCustomers;
        }

        #endregion
        
        #region Customer roles

        /// <summary>
        /// Delete a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public virtual void DeleteCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            if (customerRole.IsSystemRole)
                throw new NopException("System role could not be deleted");

            _customerRoleRepository.Delete(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(customerRole);
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        public virtual CustomerRole GetCustomerRoleById(int customerRoleId)
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
        public virtual CustomerRole GetCustomerRoleBySystemName(string systemName)
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
        public virtual IList<CustomerRole> GetAllCustomerRoles(bool showHidden = false)
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
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public virtual void InsertCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Insert(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(customerRole);
        }

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public virtual void UpdateCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Update(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(customerRole);
        }

        #endregion

        #region Customer attributes

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Delete(customerAttribute);

            //event notification
            _eventPublisher.EntityDeleted(customerAttribute);
        }

        /// <summary>
        /// Gets a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>A customer attribute</returns>
        public virtual CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
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
        public virtual void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Insert(customerAttribute);

            //event notification
            _eventPublisher.EntityInserted(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Update(customerAttribute);

            //event notification
            _eventPublisher.EntityUpdated(customerAttribute);
        }

        /// <summary>
        /// Save customer attribute
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customer">Customer</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Customer attribute</returns>
        public virtual CustomerAttribute SaveCustomerAttribute<T>(Customer customer,
            string key, T value)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (!CommonHelper.GetNopCustomTypeConverter(typeof(T)).CanConvertTo(typeof(string)))
                throw new NopException("Not supported customer attribute type");

            string valueStr = CommonHelper.GetNopCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            //use the code below in order to support all serializable types (for example, ShippingOption)
            //or use custom TypeConverters like it's implemented for ISettings
            //using (var tr = new StringReader(customerAttribute.Value))
            //{
            //    var xmlS = new XmlSerializer(typeof(T));
            //    valueStr = (T)xmlS.Deserialize(tr);
            //}
            
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