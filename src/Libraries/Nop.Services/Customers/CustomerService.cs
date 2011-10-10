using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Messages;
using Nop.Services.Security;

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
        private readonly IEncryptionService _encryptionService;
        private readonly ICacheManager _cacheManager;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
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
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <param name="eventPublisher"></param>
        public CustomerService(ICacheManager cacheManager,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IEncryptionService encryptionService, INewsLetterSubscriptionService newsLetterSubscriptionService,
            RewardPointsSettings rewardPointsSettings, CustomerSettings customerSettings,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _customerRepository = customerRepository;
            _customerRoleRepository = customerRoleRepository;
            _customerAttributeRepository = customerAttributeRepository;
            _encryptionService = encryptionService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _rewardPointsSettings = rewardPointsSettings;
            _customerSettings = customerSettings;
            _eventPublisher = eventPublisher;
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
        /// <param name="loadOnlyWithShoppingCart">Value indicating whther to load customers only with shopping cart</param>
        /// <param name="sct">Value indicating what shopping cart type to filter; userd when 'loadOnlyWithShoppingCart' param is 'true'</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        public virtual IPagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, int[] customerRoleIds, string email, string username,
            string firstName, string lastName, 
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
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public virtual bool ValidateCustomer(string usernameOrEmail, string password)
        {
            Customer customer = null;
            if (_customerSettings.UsernamesEnabled)
                customer = GetCustomerByUsername(usernameOrEmail);
            else
                customer = GetCustomerByEmail(usernameOrEmail);

            if (customer == null || customer.Deleted || !customer.Active)
                return false;

            //only registered can login
            if (!customer.IsRegistered())
                return false;

            string pwd = string.Empty;
            switch (customer.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, customer.PasswordSalt, _customerSettings.HashedPasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == customer.Password;

            //save last login date
            if (isValid)
            {
                customer.LastLoginDateUtc = DateTime.UtcNow;
                UpdateCustomer(customer);
            }
            //else
            //{
            //    customer.FailedPasswordAttemptCount++;
            //    UpdateCustomer(customer);
            //}

            return isValid;
        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            var result = new CustomerRegistrationResult();
            //validation
            if (request == null)
            {
                result.AddError("The registration request was not valid.");
                return result;
            }
            if (request.Customer == null)
            {
                result.AddError("Can't load current customer");
                return result;
            }
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }
            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError("Email is not provided");
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Invalid email");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("Password is not provided");
                return result;
            }
            if (_customerSettings.UsernamesEnabled)
            {
                if (String.IsNullOrEmpty(request.Username))
                {
                    result.AddError("Username is not provided");
                    return result;
                }
            }

            //validate unique user
            if (GetCustomerByEmail(request.Email) != null)
            {
                result.AddError("The specified email already exists");
                return result;
            }
            if (_customerSettings.UsernamesEnabled)
            {
                if (GetCustomerByUsername(request.Username) != null)
                {
                    result.AddError("The specified username already exists");
                    return result;
                }
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            request.Customer.PasswordFormat = request.PasswordFormat;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        request.Customer.Password = request.Password;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        request.Customer.Password = _encryptionService.EncryptText(request.Password);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        request.Customer.PasswordSalt = saltKey;
                        request.Customer.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                    }
                    break;
                default:
                    break;
            }

            request.Customer.Active = request.IsApproved;
            
            //add to 'Registered' role
            var registeredRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");
            request.Customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                request.Customer.CustomerRoles.Remove(guestRole);

            //Add reward points for customer registration (if enabled)
            if (_rewardPointsSettings.Enabled &&
                _rewardPointsSettings.PointsForRegistration > 0)
                request.Customer.AddRewardPointsHistoryEntry(_rewardPointsSettings.PointsForRegistration, "Registered as customer");

            UpdateCustomer(request.Customer);
            return result;
        }
        
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            var result = new PasswordChangeResult();
            if (request == null)
            {
                result.AddError("The change password request was not valid.");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("The email is not entered");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("The password is not entered");
                return result;
            }

            var customer = GetCustomerByEmail(request.Email);
            if (customer == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd = string.Empty;
                switch (customer.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, customer.PasswordSalt, _customerSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == customer.Password;
                if (!oldPasswordIsValid)
                    result.AddError("Old password doesn't match");

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            customer.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            customer.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            customer.PasswordSalt = saltKey;
                            customer.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _customerSettings.HashedPasswordFormat);
                        }
                        break;
                    default:
                        break;
                }
                customer.PasswordFormat = request.NewPasswordFormat;
                UpdateCustomer(customer);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(Customer customer, string newEmail)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            newEmail = newEmail.Trim();
            string oldEmail = customer.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NopException("New email is not valid");

            if (newEmail.Length > 100)
                throw new NopException("E-mail address is too long.");

            var customer2 = GetCustomerByEmail(newEmail);
            if (customer2 != null && customer.Id != customer2.Id)
                throw new NopException("The e-mail address is already in use.");

            customer.Email = newEmail;
            UpdateCustomer(customer);

            //update newsletter subscription (if required)
            if (!String.IsNullOrEmpty(oldEmail) && !oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                var subscriptionOld = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(oldEmail);
                if (subscriptionOld != null)
                {
                    subscriptionOld.Email = newEmail;
                    _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        public virtual void SetUsername(Customer customer, string newUsername)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (!_customerSettings.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            if (!_customerSettings.AllowUsersToChangeUsernames)
                throw new NopException("Changing usernames is not allowed");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new NopException("Username is too long.");

            var user2 = GetCustomerByUsername(newUsername);
            if (user2 != null && customer.Id != user2.Id)
                throw new NopException("This username is already in use.");

            customer.Username = newUsername;
            UpdateCustomer(customer);
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