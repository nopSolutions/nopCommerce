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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using System.Data.Objects;

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Customer service
    /// </summary>
    public partial class CustomerService : ICustomerService
    {
        #region Constants

        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        private const string CUSTOMERROLES_BY_ID_KEY = "Nop.customerrole.id-{0}";
        private const string CUSTOMERROLES_BY_DISCOUNTID_KEY = "Nop.customerrole.bydiscountid-{0}-{1}";
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public CustomerService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Inserts a customer session
        /// </summary>
        /// <param name="customerSession">Customer session</param>
        protected void InsertCustomerSession(CustomerSession customerSession)
        {
            if (customerSession == null)
                throw new ArgumentNullException("customerSession");

            

            _context.CustomerSessions.AddObject(customerSession);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the customer session
        /// </summary>
        /// <param name="customerSession">Customer session</param>
        protected void UpdateCustomerSession(CustomerSession customerSession)
        {
            if (customerSession == null)
                throw new ArgumentNullException("customerSession");

            
            if (!_context.IsAttached(customerSession))
                _context.CustomerSessions.Attach(customerSession);

            _context.SaveChanges();
        }

        /// <summary>
        /// Creates a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Password hash</returns>
        private string CreatePasswordHash(string password, string salt)
        {
            //MD5, SHA1
            string passwordFormat = IoC.Resolve<ISettingManager>().GetSettingValue("Security.PasswordFormat");
            if (String.IsNullOrEmpty(passwordFormat))
                passwordFormat = "SHA1";

            return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, passwordFormat);
        }

        /// <summary>
        /// Creates a salt
        /// </summary>
        /// <param name="size">A salt size</param>
        /// <returns>A salt</returns>
        private string CreateSalt(int size)
        {
            var provider = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];
            provider.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        #endregion

        #region Methods

        #region Addresses

        /// <summary>
        /// Deletes an address by address identifier 
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        public void DeleteAddress(int addressId)
        {
            var address = GetAddressById(addressId);
            if (address == null)
                return;

            var customer = address.Customer;
            if (customer != null)
            {
                if (customer.BillingAddressId == address.AddressId)
                {
                    //set default billing address
                    customer.BillingAddressId = 0;
                    UpdateCustomer(customer);
                }
                if (customer.ShippingAddressId == address.AddressId)
                {
                    //set default shipping address
                    customer.ShippingAddressId = 0;
                    UpdateCustomer(customer);
                }
            }

            
            if (!_context.IsAttached(address))
                _context.Addresses.Attach(address);
            _context.DeleteObject(address);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        public Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            
            var query = from a in _context.Addresses
                        where a.AddressId == addressId
                        select a;
            var address = query.SingleOrDefault();

            return address;
        }

        /// <summary>
        /// Gets a collection of addresses by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="getBillingAddresses">Gets or sets a value indicating whether the addresses are billing or shipping</param>
        /// <returns>A collection of addresses</returns>
        public List<Address> GetAddressesByCustomerId(int customerId, bool getBillingAddresses)
        {
            
            var query = from a in _context.Addresses
                        orderby a.CreatedOn
                        where a.CustomerId == customerId && a.IsBillingAddress == getBillingAddresses
                        select a;
            var addresses = query.ToList();

            return addresses;
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        public void InsertAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            
            address.FirstName = CommonHelper.EnsureNotNull(address.FirstName);
            address.LastName = CommonHelper.EnsureNotNull(address.LastName);
            address.PhoneNumber = CommonHelper.EnsureNotNull(address.PhoneNumber);
            address.Email = CommonHelper.EnsureNotNull(address.Email);
            address.FaxNumber = CommonHelper.EnsureNotNull(address.FaxNumber);
            address.Company = CommonHelper.EnsureNotNull(address.Company);
            address.Address1 = CommonHelper.EnsureNotNull(address.Address1);
            address.Address2 = CommonHelper.EnsureNotNull(address.Address2);
            address.City = CommonHelper.EnsureNotNull(address.City);
            address.ZipPostalCode = CommonHelper.EnsureNotNull(address.ZipPostalCode);

            address.FirstName = address.FirstName.Trim();
            address.LastName = address.LastName.Trim();
            address.PhoneNumber = address.PhoneNumber.Trim();
            address.Email = address.Email.Trim();
            address.FaxNumber = address.FaxNumber.Trim();
            address.Company = address.Company.Trim();
            address.Address1 = address.Address1.Trim();
            address.Address2 = address.Address2.Trim();
            address.City = address.City.Trim();
            address.ZipPostalCode = address.ZipPostalCode.Trim();

            address.FirstName = CommonHelper.EnsureMaximumLength(address.FirstName, 100);
            address.LastName = CommonHelper.EnsureMaximumLength(address.LastName, 100);
            address.PhoneNumber = CommonHelper.EnsureMaximumLength(address.PhoneNumber, 50);
            address.Email = CommonHelper.EnsureMaximumLength(address.Email, 255);
            address.FaxNumber = CommonHelper.EnsureMaximumLength(address.FaxNumber, 50);
            address.Company = CommonHelper.EnsureMaximumLength(address.Company, 100);
            address.Address1 = CommonHelper.EnsureMaximumLength(address.Address1, 100);
            address.Address2 = CommonHelper.EnsureMaximumLength(address.Address2, 100);
            address.City = CommonHelper.EnsureMaximumLength(address.City, 100);
            address.ZipPostalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 30);

            

            _context.Addresses.AddObject(address);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        public void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            address.FirstName = CommonHelper.EnsureNotNull(address.FirstName);
            address.LastName = CommonHelper.EnsureNotNull(address.LastName);
            address.PhoneNumber = CommonHelper.EnsureNotNull(address.PhoneNumber);
            address.Email = CommonHelper.EnsureNotNull(address.Email);
            address.FaxNumber = CommonHelper.EnsureNotNull(address.FaxNumber);
            address.Company = CommonHelper.EnsureNotNull(address.Company);
            address.Address1 = CommonHelper.EnsureNotNull(address.Address1);
            address.Address2 = CommonHelper.EnsureNotNull(address.Address2);
            address.City = CommonHelper.EnsureNotNull(address.City);
            address.ZipPostalCode = CommonHelper.EnsureNotNull(address.ZipPostalCode);

            address.FirstName = address.FirstName.Trim();
            address.LastName = address.LastName.Trim();
            address.PhoneNumber = address.PhoneNumber.Trim();
            address.Email = address.Email.Trim();
            address.FaxNumber = address.FaxNumber.Trim();
            address.Company = address.Company.Trim();
            address.Address1 = address.Address1.Trim();
            address.Address2 = address.Address2.Trim();
            address.City = address.City.Trim();
            address.ZipPostalCode = address.ZipPostalCode.Trim();

            address.FirstName = CommonHelper.EnsureMaximumLength(address.FirstName, 100);
            address.LastName = CommonHelper.EnsureMaximumLength(address.LastName, 100);
            address.PhoneNumber = CommonHelper.EnsureMaximumLength(address.PhoneNumber, 50);
            address.Email = CommonHelper.EnsureMaximumLength(address.Email, 255);
            address.FaxNumber = CommonHelper.EnsureMaximumLength(address.FaxNumber, 50);
            address.Company = CommonHelper.EnsureMaximumLength(address.Company, 100);
            address.Address1 = CommonHelper.EnsureMaximumLength(address.Address1, 100);
            address.Address2 = CommonHelper.EnsureMaximumLength(address.Address2, 100);
            address.City = CommonHelper.EnsureMaximumLength(address.City, 100);
            address.ZipPostalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 30);

            
            if (!_context.IsAttached(address))
                _context.Addresses.Attach(address);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a value indicating whether address can be used as billing address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public bool CanUseAddressAsBillingAddress(Address address)
        {
            if (address == null)
                return false;

            if (address.FirstName == null)
                return false;
            if (String.IsNullOrEmpty(address.FirstName.Trim()))
                return false;

            if (address.LastName == null)
                return false;
            if (String.IsNullOrEmpty(address.LastName.Trim()))
                return false;

            if (address.PhoneNumber == null)
                return false;
            if (String.IsNullOrEmpty(address.PhoneNumber.Trim()))
                return false;

            if (address.Email == null)
                return false;
            if (String.IsNullOrEmpty(address.Email.Trim()))
                return false;

            if (address.Address1 == null)
                return false;
            if (String.IsNullOrEmpty(address.Address1.Trim()))
                return false;

            if (address.City == null)
                return false;
            if (String.IsNullOrEmpty(address.City.Trim()))
                return false;

            if (address.ZipPostalCode == null)
                return false;
            if (String.IsNullOrEmpty(address.ZipPostalCode.Trim()))
                return false;

            if (address.Country == null)
                return false;

            if (!address.Country.AllowsBilling)
                return false;

            if (address.Country.StateProvinces.Count > 0 &&
                address.StateProvince == null)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether address can be used as shipping address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public bool CanUseAddressAsShippingAddress(Address address)
        {
            if (address == null)
                return false;

            if (address.FirstName == null)
                return false;
            if (String.IsNullOrEmpty(address.FirstName.Trim()))
                return false;

            if (address.LastName == null)
                return false;
            if (String.IsNullOrEmpty(address.LastName.Trim()))
                return false;

            if (address.PhoneNumber == null)
                return false;
            if (String.IsNullOrEmpty(address.PhoneNumber.Trim()))
                return false;

            if (address.Email == null)
                return false;
            if (String.IsNullOrEmpty(address.Email.Trim()))
                return false;

            if (address.Address1 == null)
                return false;
            if (String.IsNullOrEmpty(address.Address1.Trim()))
                return false;

            if (address.City == null)
                return false;
            if (String.IsNullOrEmpty(address.City.Trim()))
                return false;

            if (address.ZipPostalCode == null)
                return false;
            if (String.IsNullOrEmpty(address.ZipPostalCode.Trim()))
                return false;

            if (address.Country == null)
                return false;

            if (!address.Country.AllowsShipping)
                return false;

            if (address.Country.StateProvinces.Count > 0 &&
                address.StateProvince == null)
                return false;

            return true;
        }

        #endregion 
        
        #region Customers

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        public void ResetCheckoutData(int customerId, bool clearCouponCodes)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {                
                //clear reward points flag
                customer.UseRewardPointsDuringCheckout = false;
                
                //clear selected shipping and payment methods
                customer.LastShippingOption = null;
                customer.LastPaymentMethodId = 0;
                UpdateCustomer(customer);

                //clear entered coupon codes
                if (clearCouponCodes)
                {
                    customer = ApplyDiscountCouponCode(customer.CustomerId, string.Empty);
                    customer = ApplyGiftCardCouponCode(customer.CustomerId, string.Empty);
                    customer = ApplyCheckoutAttributes(customer.CustomerId, string.Empty);
                }
            }
        }

        /// <summary>
        /// Sets a customer email
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newEmail">New email</param>
        /// <returns>Customer</returns>
        public Customer SetEmail(int customerId, string newEmail)
        {
            if (newEmail == null)
                newEmail = string.Empty;
            newEmail = newEmail.Trim();

            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                if (!CommonHelper.IsValidEmail(newEmail))
                {
                    throw new NopException("New email is not valid");
                }
                
                if (customer.IsGuest)
                {
                    throw new NopException("You cannot change email for guest customer");
                }

                var cust2 = GetCustomerByEmail(newEmail);
                if (cust2 != null && customer.CustomerId != cust2.CustomerId)
                {
                    throw new NopException("The e-mail address is already in use.");
                }

                if (newEmail.Length > 100)
                {
                    throw new NopException("E-mail address is too long.");
                }

                customer.Email = newEmail;
                UpdateCustomer(customer);
            }
            return customer;
        }
        
        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>Customer</returns>
        public Customer ChangeCustomerUsername(int customerId, string newUsername)
        {
            if (!this.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            if (!this.AllowCustomersToChangeUsernames)
                throw new NopException("Changing usernames is not allowed");

            if (newUsername == null)
                newUsername = string.Empty;
            newUsername = newUsername.Trim();

            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    throw new NopException("You cannot change username for guest customer");
                }

                var cust2 = GetCustomerByUsername(newUsername);
                if (cust2 != null && customer.CustomerId != cust2.CustomerId)
                {
                    throw new NopException("This username is already in use.");
                }

                if (newUsername.Length > 100)
                {
                    throw new NopException("Username is too long.");
                }

                customer.Username = newUsername;
                UpdateCustomer(customer);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer sugnature
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="signature">Signature</param>
        /// <returns>Customer</returns>
        public Customer SetCustomerSignature(int customerId, string signature)
        {
            if (signature == null)
                signature = string.Empty;
            signature = signature.Trim();

            int maxLength = 300;
            if (signature.Length > maxLength)
                signature = signature.Substring(0, maxLength);

            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.Signature = signature;
                UpdateCustomer(customer);
            }
            return customer;
        }
        
        /// <summary>
        /// Create anonymous user for current user
        /// </summary>
        /// <returns>Guest user</returns>
        public void CreateAnonymousUser()
        {
            //create anonymous record
            string email = "anonymous@anonymous.com";
            string password = string.Empty;
            MembershipCreateStatus status = MembershipCreateStatus.UserRejected;
            var guestCustomer = AddCustomer(email, email, password, false, true, true, out status);
            if (guestCustomer != null && status == MembershipCreateStatus.Success)
            {
                NopContext.Current.User = guestCustomer;

                if (NopContext.Current.Session == null)
                {
                    NopContext.Current.Session = NopContext.Current.GetSession(true);
                }

                DateTime LastAccessed = DateTime.UtcNow;

                var customerSession = NopContext.Current.Session;
                customerSession.CustomerId = guestCustomer.CustomerId;
                customerSession.LastAccessed = LastAccessed;
                UpdateCustomerSession(customerSession);
                NopContext.Current.Session = customerSession;
            }
        }

        /// <summary>
        /// Applies a discount coupon code to a current customer
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        public void ApplyDiscountCouponCode(string couponCode)
        {
            if (NopContext.Current.User == null)
            {
                //create anonymous record
                CreateAnonymousUser();
            }
            NopContext.Current.User = ApplyDiscountCouponCode(NopContext.Current.User.CustomerId, couponCode);
        }

        /// <summary>
        /// Applies a discount coupon code
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>Customer</returns>
        public Customer ApplyDiscountCouponCode(int customerId, string couponCode)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.LastAppliedCouponCode = couponCode;
                UpdateCustomer(customer);
            }
            return customer;
        }

        /// <summary>
        /// Applies a gift card coupon code to a current customer
        /// </summary>
        /// <param name="couponCodesXml">Coupon code (XML)</param>
        public void ApplyGiftCardCouponCode(string couponCodesXml)
        {
            if (NopContext.Current.User == null)
            {
                //create anonymous record
                CreateAnonymousUser();
            }
            NopContext.Current.User = ApplyGiftCardCouponCode(NopContext.Current.User.CustomerId, couponCodesXml);
        }

        /// <summary>
        /// Applies a gift card coupon code
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="couponCodesXml">Coupon code (XML)</param>
        /// <returns>Customer</returns>
        public Customer ApplyGiftCardCouponCode(int customerId, string couponCodesXml)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.GiftCardCouponCodes = couponCodesXml;
                UpdateCustomer(customer);
            }
            return customer;
        }

        /// <summary>
        /// Applies selected checkout attibutes to a current customer
        /// </summary>
        /// <param name="attributesXml">Checkout attibutes (XML)</param>
        public void ApplyCheckoutAttributes(string attributesXml)
        {
            if (NopContext.Current.User == null)
            {
                //create anonymous record
                CreateAnonymousUser();
            }
            NopContext.Current.User = ApplyCheckoutAttributes(NopContext.Current.User.CustomerId, attributesXml);
        }

        /// <summary>
        /// Applies selected checkout attibutes to a current customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="attributesXml">Checkout attibutes (XML)</param>
        /// <returns>Customer</returns>
        public Customer ApplyCheckoutAttributes(int customerId, string attributesXml)
        {
            if (attributesXml == null)
                attributesXml = string.Empty;
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.CheckoutAttributes = attributesXml;
                UpdateCustomer(customer);
            }
            return customer;
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>Customer collection</returns>
        public PagedList<Customer> GetAllCustomers()
        {
            return GetAllCustomers(null, null, null, string.Empty, false,
                int.MaxValue, 0);
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="dontLoadGuestCustomers">A value indicating whether to don't load guest customers</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Customer collection</returns>
        public PagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int pageSize, int pageIndex)
        {
            return GetAllCustomers(registrationFrom, registrationTo,
                email, username, dontLoadGuestCustomers, 0, 0,
                pageSize, pageIndex);
        }
        
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="dontLoadGuestCustomers">A value indicating whether to don't load guest customers</param>
        /// <param name="dateOfBirthMonth">Filter by date of birth (month); 0 to load all customers;</param>
        /// <param name="dateOfBirthDay">Filter by date of birth (day); 0 to load all customers;</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Customer collection</returns>
        public PagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int dateOfBirthMonth, int dateOfBirthDay, 
            int pageSize, int pageIndex)
        {
            if (email == null)
                email = string.Empty;

            if (username == null)
                username = string.Empty;

            var query = from c in _context.Customers
                        where
                        (!registrationFrom.HasValue || registrationFrom.Value <= c.RegistrationDate) &&
                        (!registrationTo.HasValue || registrationTo.Value >= c.RegistrationDate) &&
                        (String.IsNullOrEmpty(email) || c.Email.Contains(email)) &&
                        (String.IsNullOrEmpty(username) || c.Username.Contains(username)) &&
                        (!dontLoadGuestCustomers || !c.IsGuest) &&
                        (dateOfBirthMonth == 0 || (c.DateOfBirth.HasValue && c.DateOfBirth.Value.Month == dateOfBirthMonth)) &&
                        (dateOfBirthDay == 0 || (c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dateOfBirthDay)) &&
                        (!c.Deleted)
                        orderby c.RegistrationDate descending
                        select c;
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);

            return customers;
        }

        /// <summary>
        /// Gets all customers by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Customer collection</returns>
        public List<Customer> GetAffiliatedCustomers(int affiliateId)
        {
            if (affiliateId == 0)
                return new List<Customer>();

            
            var query = from c in _context.Customers
                        orderby c.RegistrationDate descending
                        where c.AffiliateId == affiliateId && !c.Deleted
                        select c;
            var customers = query.ToList();
            return customers;
        }

        /// <summary>
        /// Gets all customers by customer role id
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer collection</returns>
        public List<Customer> GetCustomersByCustomerRoleId(int customerRoleId)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = from c in _context.Customers
                        from cr in c.NpCustomerRoles
                        where (showHidden || c.Active) &&
                            !c.Deleted &&
                            cr.CustomerRoleId == customerRoleId
                        orderby c.RegistrationDate descending
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
        /// Marks customer as deleted
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        public void MarkCustomerAsDeleted(int customerId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.Deleted = true;
                UpdateCustomer(customer);
            }
        }

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="email">Customer Email</param>
        /// <returns>A customer</returns>
        public Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            
            var query = from c in _context.Customers
                        orderby c.CustomerId
                        where c.Email == email
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="username">Customer username</param>
        /// <returns>A customer</returns>
        public Customer GetCustomerByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            
            var query = from c in _context.Customers
                        orderby c.CustomerId
                        where c.Username == username
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
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

            
            var query = from c in _context.Customers
                        where c.CustomerId == customerId
                        select c;
            var customer = query.SingleOrDefault();
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

            
            var query = from c in _context.Customers
                        where c.CustomerGuid == customerGuid
                        orderby c.CustomerId
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Adds a customer
        /// </summary>
        /// <param name="email">The email</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="isAdmin">A value indicating whether the customer is administrator</param>
        /// <param name="isGuest">A value indicating whether the customer is guest</param>
        /// <param name="active">A value indicating whether the customer is active</param>
        /// <param name="status">Status</param>
        /// <returns>A customer</returns>
        public Customer AddCustomer(string email, string username, string password,
            bool isAdmin, bool isGuest, bool active, out MembershipCreateStatus status)
        {
            int affiliateId = 0;
            HttpCookie affiliateCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.AffiliateId");
            if (affiliateCookie != null)
            {
                Affiliate affiliate = IoC.Resolve<IAffiliateService>().GetAffiliateById(Convert.ToInt32(affiliateCookie.Value));
                if (affiliate != null && affiliate.Active)
                    affiliateId = affiliate.AffiliateId;
            }

            var customer = AddCustomer(Guid.NewGuid(), email, username, password, affiliateId,
                0, 0, 0, string.Empty, string.Empty, string.Empty,
                NopContext.Current.WorkingLanguage.LanguageId,
                NopContext.Current.WorkingCurrency.CurrencyId,
                NopContext.Current.TaxDisplayType, false, isAdmin, isGuest,
                false, 0, string.Empty, string.Empty, active,
                false, DateTime.UtcNow, string.Empty, 0, null, out status);

            if (status == MembershipCreateStatus.Success)
            {
                if (affiliateCookie != null)
                {
                    affiliateCookie.Expires = DateTime.Now.AddMonths(-1);
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Response.Cookies.Set(affiliateCookie);
                    }
                }
            }

            return customer;
        }

        /// <summary>
        /// Adds a customer
        /// </summary>
        /// <param name="email">The email</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="isAdmin">A value indicating whether the customer is administrator</param>
        /// <param name="isGuest">A value indicating whether the customer is guest</param>
        /// <param name="active">A value indicating whether the customer is active</param>
        /// <param name="status">Status</param>
        /// <returns>A customer</returns>
        public Customer AddCustomer(string email, string username, string password,
            int affiliateId, bool isAdmin, bool isGuest, bool active,
            out MembershipCreateStatus status)
        {
            return AddCustomer(Guid.NewGuid(), email, username, password,
                affiliateId, 0, 0, 0, string.Empty, string.Empty, string.Empty,
                NopContext.Current.WorkingLanguage.LanguageId,
                NopContext.Current.WorkingCurrency.CurrencyId,
                NopContext.Current.TaxDisplayType, false,
                isAdmin, isGuest, false, 0, string.Empty, string.Empty, active,
                false, DateTime.UtcNow, string.Empty, 0, null, out status);
        }

        /// <summary>
        /// Adds a customer
        /// </summary>
        /// <param name="customerGuid">The customer identifier</param>
        /// <param name="email">The email</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="billingAddressId">The billing address identifier</param>
        /// <param name="shippingAddressId">The shipping address identifier</param>
        /// <param name="lastPaymentMethodId">The last payment method identifier</param>
        /// <param name="lastAppliedCouponCode">The last applied coupon code</param>
        /// <param name="giftCardCouponCodes">The applied gift card coupon code</param>
        /// <param name="checkoutAttributes">The selected checkout attributes</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="currencyId">The currency identifier</param>
        /// <param name="taxDisplayType">The tax display type</param>
        /// <param name="isTaxExempt">A value indicating whether the customer is tax exempt</param>
        /// <param name="isAdmin">A value indicating whether the customer is administrator</param>
        /// <param name="isGuest">A value indicating whether the customer is guest</param>
        /// <param name="isForumModerator">A value indicating whether the customer is forum moderator</param>
        /// <param name="totalForumPosts">A forum post count</param>
        /// <param name="signature">Signature</param>
        /// <param name="adminComment">Admin comment</param>
        /// <param name="active">A value indicating whether the customer is active</param>
        /// <param name="deleted">A value indicating whether the customer has been deleted</param>
        /// <param name="registrationDate">The date and time of customer registration</param>
        /// <param name="timeZoneId">The time zone identifier</param>
        /// <param name="avatarId">The avatar identifier</param>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="status">Status</param>
        /// <returns>A customer</returns>
        public Customer AddCustomer(Guid customerGuid, string email, string username,
            string password, int affiliateId, int billingAddressId,
            int shippingAddressId, int lastPaymentMethodId,
            string lastAppliedCouponCode, string giftCardCouponCodes,
            string checkoutAttributes, int languageId, int currencyId,
            TaxDisplayTypeEnum taxDisplayType, bool isTaxExempt, bool isAdmin, bool isGuest,
            bool isForumModerator, int totalForumPosts, string signature, string adminComment,
            bool active, bool deleted, DateTime registrationDate,
            string timeZoneId, int avatarId, DateTime? dateOfBirth, out MembershipCreateStatus status)
        {
            Customer customer = null;

            if (username == null)
                username = string.Empty;
            username = username.Trim();

            if (email == null)
                email = string.Empty;
            email = email.Trim();

            if (signature == null)
                signature = string.Empty;
            signature = signature.Trim();

            string saltKey = string.Empty;
            string passwordHash = string.Empty;

            status = MembershipCreateStatus.UserRejected;
            if (!isGuest)
            {
                if (!NopContext.Current.IsAdmin)
                {
                    if (this.CustomerRegistrationType == CustomerRegistrationTypeEnum.Disabled)
                    {
                        status = MembershipCreateStatus.ProviderError;
                        return customer;
                    }
                }

                if (this.UsernamesEnabled)
                {
                    if (GetCustomerByUsername(username) != null)
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                        return customer;
                    }

                    if (username.Length > 100)
                    {
                        status = MembershipCreateStatus.InvalidUserName;
                        return customer;
                    }
                }

                if (GetCustomerByEmail(email) != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return customer;
                }

                if (!CommonHelper.IsValidEmail(email))
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return customer;
                }

                if (email.Length > 100)
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return customer;
                }

                if (!NopContext.Current.IsAdmin)
                {
                    if (this.CustomerRegistrationType == CustomerRegistrationTypeEnum.EmailValidation ||
                        this.CustomerRegistrationType == CustomerRegistrationTypeEnum.AdminApproval)
                    {
                        active = false;
                    }
                }
                saltKey = CreateSalt(5);
                passwordHash = CreatePasswordHash(password, saltKey);
            }

            customer = AddCustomerForced(customerGuid, email, username,
                passwordHash, saltKey, affiliateId, billingAddressId,
                shippingAddressId, lastPaymentMethodId,
                lastAppliedCouponCode, giftCardCouponCodes,
                checkoutAttributes, languageId, currencyId, taxDisplayType,
                isTaxExempt, isAdmin, isGuest, isForumModerator,
                totalForumPosts, signature, adminComment, active,
                deleted, registrationDate, timeZoneId, avatarId, dateOfBirth);

            if (!isGuest)
            {
                DateTime lastAccessed = DateTime.UtcNow;
                SaveCustomerSession(Guid.NewGuid(), customer.CustomerId, lastAccessed, false);
            }

            status = MembershipCreateStatus.Success;

            if (!isGuest)
            {
                if (active)
                {
                    IoC.Resolve<IMessageService>().SendCustomerWelcomeMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
                }
                else
                {
                    if (this.CustomerRegistrationType == CustomerRegistrationTypeEnum.EmailValidation)
                    {
                        Guid accountActivationToken = Guid.NewGuid();
                        customer.AccountActivationToken = accountActivationToken.ToString();

                        IoC.Resolve<IMessageService>().SendCustomerEmailValidationMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
                    }
                }
            }
            return customer;
        }

        /// <summary>
        /// Adds a customer without any validations, welcome messages
        /// </summary>
        /// <param name="customerGuid">The customer identifier</param>
        /// <param name="email">The email</param>
        /// <param name="username">The username</param>
        /// <param name="passwordHash">The password hash</param>
        /// <param name="saltKey">The salt key</param>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="billingAddressId">The billing address identifier</param>
        /// <param name="shippingAddressId">The shipping address identifier</param>
        /// <param name="lastPaymentMethodId">The last payment method identifier</param>
        /// <param name="lastAppliedCouponCode">The last applied coupon code</param>
        /// <param name="giftCardCouponCodes">The applied gift card coupon code</param>
        /// <param name="checkoutAttributes">The selected checkout attributes</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="currencyId">The currency identifier</param>
        /// <param name="taxDisplayType">The tax display type</param>
        /// <param name="isTaxExempt">A value indicating whether the customer is tax exempt</param>
        /// <param name="isAdmin">A value indicating whether the customer is administrator</param>
        /// <param name="isGuest">A value indicating whether the customer is guest</param>
        /// <param name="isForumModerator">A value indicating whether the customer is forum moderator</param>
        /// <param name="totalForumPosts">A forum post count</param>
        /// <param name="signature">Signature</param>
        /// <param name="adminComment">Admin comment</param>
        /// <param name="active">A value indicating whether the customer is active</param>
        /// <param name="deleted">A value indicating whether the customer has been deleted</param>
        /// <param name="registrationDate">The date and time of customer registration</param>
        /// <param name="timeZoneId">The time zone identifier</param>
        /// <param name="avatarId">The avatar identifier</param>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <returns>A customer</returns>
        public Customer AddCustomerForced(Guid customerGuid, string email,
            string username, string passwordHash, string saltKey,
            int affiliateId, int billingAddressId,
            int shippingAddressId, int lastPaymentMethodId,
            string lastAppliedCouponCode, string giftCardCouponCodes,
            string checkoutAttributes, int languageId,
            int currencyId, TaxDisplayTypeEnum taxDisplayType, bool isTaxExempt,
            bool isAdmin, bool isGuest, bool isForumModerator,
            int totalForumPosts, string signature, string adminComment, 
            bool active, bool deleted, DateTime registrationDate, string timeZoneId,
            int avatarId, DateTime? dateOfBirth)
        {
            email = CommonHelper.EnsureNotNull(email);
            email = email.Trim();
            email = CommonHelper.EnsureMaximumLength(email, 255);
            username = CommonHelper.EnsureNotNull(username);
            username = username.Trim();
            username = CommonHelper.EnsureMaximumLength(username, 100);
            passwordHash = CommonHelper.EnsureNotNull(passwordHash);
            passwordHash = CommonHelper.EnsureMaximumLength(passwordHash, 255);
            saltKey = CommonHelper.EnsureNotNull(saltKey);
            saltKey = CommonHelper.EnsureMaximumLength(saltKey, 255);
            lastAppliedCouponCode = CommonHelper.EnsureNotNull(lastAppliedCouponCode);
            lastAppliedCouponCode = CommonHelper.EnsureMaximumLength(lastAppliedCouponCode, 100);
            signature = CommonHelper.EnsureNotNull(signature);
            signature = CommonHelper.EnsureMaximumLength(signature, 300);
            adminComment = CommonHelper.EnsureNotNull(adminComment);
            adminComment = CommonHelper.EnsureMaximumLength(adminComment, 4000);
            timeZoneId = CommonHelper.EnsureNotNull(timeZoneId);
            timeZoneId = CommonHelper.EnsureMaximumLength(timeZoneId, 200);

            

            var customer = _context.Customers.CreateObject();
            customer.CustomerGuid = customerGuid;
            customer.Email = email;
            customer.Username = username;
            customer.PasswordHash = passwordHash;
            customer.SaltKey = saltKey;
            customer.AffiliateId = affiliateId;
            customer.BillingAddressId = billingAddressId;
            customer.ShippingAddressId = shippingAddressId;
            customer.LastPaymentMethodId = lastPaymentMethodId;
            customer.LastAppliedCouponCode = lastAppliedCouponCode;
            customer.GiftCardCouponCodes = giftCardCouponCodes;
            customer.CheckoutAttributes = checkoutAttributes;
            customer.LanguageId = languageId;
            customer.CurrencyId = currencyId;
            customer.TaxDisplayTypeId = (int)taxDisplayType;
            customer.IsTaxExempt = isTaxExempt;
            customer.IsAdmin = isAdmin;
            customer.IsGuest = isGuest;
            customer.IsForumModerator = isForumModerator;
            customer.TotalForumPosts = totalForumPosts;
            customer.Signature = signature;
            customer.AdminComment = adminComment;
            customer.Active = active;
            customer.Deleted = deleted;
            customer.RegistrationDate = registrationDate;
            customer.TimeZoneId = timeZoneId;
            customer.AvatarId = avatarId;
            customer.DateOfBirth = dateOfBirth;

            _context.Customers.AddObject(customer);
            _context.SaveChanges();

            //reward points
            if (!isGuest &&
                IoC.Resolve<IOrderService>().RewardPointsEnabled &&
                IoC.Resolve<IOrderService>().RewardPointsForRegistration > 0)
            {
                var rph = IoC.Resolve<IOrderService>().InsertRewardPointsHistory(customer.CustomerId, 0,
                    IoC.Resolve<IOrderService>().RewardPointsForRegistration, decimal.Zero, decimal.Zero,
                    string.Empty, IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("RewardPoints.Message.EarnedForRegistration"),
                    DateTime.UtcNow);
            }

            //raise event             
            EventContext.Current.OnCustomerCreated(null, 
                new CustomerEventArgs() { Customer = customer});
            
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

            customer.Email = CommonHelper.EnsureNotNull(customer.Email);
            customer.Email = customer.Email.Trim();
            customer.Email = CommonHelper.EnsureMaximumLength(customer.Email, 255);
            customer.Username = CommonHelper.EnsureNotNull(customer.Username);
            customer.Username = customer.Username.Trim();
            customer.Username = CommonHelper.EnsureMaximumLength(customer.Username, 100);
            customer.PasswordHash = CommonHelper.EnsureNotNull(customer.PasswordHash);
            customer.PasswordHash = CommonHelper.EnsureMaximumLength(customer.PasswordHash, 255);
            customer.SaltKey = CommonHelper.EnsureNotNull(customer.SaltKey);
            customer.SaltKey = CommonHelper.EnsureMaximumLength(customer.SaltKey, 255);
            customer.LastAppliedCouponCode = CommonHelper.EnsureNotNull(customer.LastAppliedCouponCode);
            customer.LastAppliedCouponCode = CommonHelper.EnsureMaximumLength(customer.LastAppliedCouponCode, 255);
            customer.Signature = CommonHelper.EnsureNotNull(customer.Signature);
            customer.Signature = customer.Signature.Trim();
            customer.Signature = CommonHelper.EnsureMaximumLength(customer.Signature, 300);
            customer.AdminComment = CommonHelper.EnsureNotNull(customer.AdminComment);
            customer.AdminComment = CommonHelper.EnsureMaximumLength(customer.AdminComment, 4000);
            customer.TimeZoneId = CommonHelper.EnsureNotNull(customer.TimeZoneId);
            customer.TimeZoneId = CommonHelper.EnsureMaximumLength(customer.TimeZoneId, 200);
            
            var subscriptionOld = customer.NewsLetterSubscription;

            
            if (!_context.IsAttached(customer))
                _context.Customers.Attach(customer);
            _context.SaveChanges();

            if (subscriptionOld != null && !customer.Email.ToLower().Equals(subscriptionOld.Email.ToLower()))
            {
                subscriptionOld.Email = customer.Email;
                IoC.Resolve<IMessageService>().UpdateNewsLetterSubscription(subscriptionOld);
            }

            //raise event             
            EventContext.Current.OnCustomerUpdated(null,
                new CustomerEventArgs() { Customer = customer });
        }

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <param name="oldPassword">Old password</param>
        /// <param name="password">Password</param>
        public void ModifyPassword(string email, string oldPassword, string password)
        {
            var customer = GetCustomerByEmail(email);
            if (customer != null)
            {
                string oldPasswordHash = CreatePasswordHash(oldPassword, customer.SaltKey);
                if (!customer.PasswordHash.Equals(oldPasswordHash))
                    throw new NopException("Current Password doesn't match.");

                ModifyPassword(customer.CustomerId, password);
            }
        }

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <param name="newPassword">New password</param>
        public void ModifyPassword(string email, string newPassword)
        {
            var customer = GetCustomerByEmail(email);
            if (customer != null)
            {
                ModifyPassword(customer.CustomerId, newPassword);
            }
        }

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newPassword">New password</param>
        public void ModifyPassword(int customerId, string newPassword)
        {
            if (String.IsNullOrWhiteSpace(newPassword))
                throw new NopException(IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("Customer.PasswordIsRequired"));
            newPassword = newPassword.Trim();

            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                string newPasswordSalt = CreateSalt(5);
                string newPasswordHash = CreatePasswordHash(newPassword, newPasswordSalt);

                customer.PasswordHash = newPasswordHash;
                customer.SaltKey = newPasswordSalt;
                UpdateCustomer(customer);
            }
        }

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        public void Activate(Guid customerGuid)
        {
            var customer = GetCustomerByGuid(customerGuid);
            if (customer != null)
            {
                Activate(customer.CustomerId);
            }
        }

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        public void Activate(int customerId)
        {
            Activate(customerId, false);
        }

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="sendCustomerWelcomeMessage">A value indivating whether to send customer welcome message</param>
        public void Activate(int customerId, bool sendCustomerWelcomeMessage)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.Active = true;
                UpdateCustomer(customer);

                if (sendCustomerWelcomeMessage)
                {
                    IoC.Resolve<IMessageService>().SendCustomerWelcomeMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
                }
            }
        }

        /// <summary>
        /// Deactivates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        public void Deactivate(Guid customerGuid)
        {
            var customer = GetCustomerByGuid(customerGuid);
            if (customer != null)
            {
                Deactivate(customer.CustomerId);
            }
        }

        /// <summary>
        /// Deactivates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        public void Deactivate(int customerId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer.Active = false;
                UpdateCustomer(customer);
            }
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        /// <param name="email">A customer email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public bool Login(string email, string password)
        {
            if (email == null)
                email = string.Empty;
            email = email.Trim();

            var customer = GetCustomerByEmail(email);

            if (customer == null)
                return false;

            if (!customer.Active)
                return false;

            if (customer.Deleted)
                return false;

            if (customer.IsGuest)
                return false;

            string passwordHash = CreatePasswordHash(password, customer.SaltKey);
            bool result = customer.PasswordHash.Equals(passwordHash);
            if (result)
            {
                var registeredCustomerSession = GetCustomerSessionByCustomerId(customer.CustomerId);
                if (registeredCustomerSession != null)
                {
                    registeredCustomerSession.IsExpired = false;
                    var anonCustomerSession = NopContext.Current.Session;
                    var cart1 = IoC.Resolve<IShoppingCartService>().GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                    var cart2 = IoC.Resolve<IShoppingCartService>().GetCurrentShoppingCart(ShoppingCartTypeEnum.Wishlist);
                    NopContext.Current.Session = registeredCustomerSession;

                    if ((anonCustomerSession != null) && (anonCustomerSession.CustomerSessionGuid != registeredCustomerSession.CustomerSessionGuid))
                    {
                        if (anonCustomerSession.Customer != null)
                        {
                            customer = ApplyDiscountCouponCode(customer.CustomerId, anonCustomerSession.Customer.LastAppliedCouponCode);
                            customer = ApplyGiftCardCouponCode(customer.CustomerId, anonCustomerSession.Customer.GiftCardCouponCodes);
                        }

                        foreach (ShoppingCartItem item in cart1)
                        {
                            IoC.Resolve<IShoppingCartService>().AddToCart(
                                item.ShoppingCartType,
                                item.ProductVariantId,
                                item.AttributesXml,
                                item.CustomerEnteredPrice,
                                item.Quantity);
                            IoC.Resolve<IShoppingCartService>().DeleteShoppingCartItem(item.ShoppingCartItemId, true);
                        }
                        foreach (ShoppingCartItem item in cart2)
                        {
                            IoC.Resolve<IShoppingCartService>().AddToCart(
                                item.ShoppingCartType,
                                item.ProductVariantId,
                                item.AttributesXml,
                                item.CustomerEnteredPrice,
                                item.Quantity);
                            IoC.Resolve<IShoppingCartService>().DeleteShoppingCartItem(item.ShoppingCartItemId, true);
                        }
                    }
                }
                if (NopContext.Current.Session == null)
                    NopContext.Current.Session = NopContext.Current.GetSession(true);
                NopContext.Current.Session.IsExpired = false;
                NopContext.Current.Session.LastAccessed = DateTime.UtcNow;
                NopContext.Current.Session.CustomerId = customer.CustomerId;
                NopContext.Current.Session = SaveCustomerSession(NopContext.Current.Session.CustomerSessionGuid, NopContext.Current.Session.CustomerId, NopContext.Current.Session.LastAccessed, NopContext.Current.Session.IsExpired);
            }
            return result;
        }

        /// <summary>
        /// Logout customer
        /// </summary>
        public void Logout()
        {
            if (NopContext.Current != null)
            {
                NopContext.Current.ResetSession();
            }
            if (NopContext.Current != null &&
                NopContext.Current.IsCurrentCustomerImpersonated &&
                NopContext.Current.OriginalUser!=null)
            {
                NopContext.Current.OriginalUser.ImpersonatedCustomerGuid = Guid.Empty;
            }
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Abandon();
            }
            FormsAuthentication.SignOut();
        }

        #endregion

        #region Reports

        /// <summary>
        /// Get best customers
        /// </summary>
        /// <param name="startTime">Order start time; null to load all</param>
        /// <param name="endTime">Order end time; null to load all</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
        /// <returns>Report</returns>
        public List<CustomerBestReportLine> GetBestCustomersReport(DateTime? startTime,
            DateTime? endTime, OrderStatusEnum? os, PaymentStatusEnum? ps,
            ShippingStatusEnum? ss, int orderBy)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            
            var report = _context.Sp_CustomerBestReport(startTime, endTime,
                orderStatusId, paymentStatusId, shippingStatusId, orderBy).ToList();

            return report;
        }

        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Int</returns>
        public int GetRegisteredCustomersReport(int days)
        {
            DateTime date = DateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            
            var query = from c in _context.Customers
                        where c.Active &&
                        !c.Deleted &&
                        !c.IsGuest &&
                        c.RegistrationDate >= date &&
                        c.RegistrationDate <= DateTime.UtcNow
                        select c;
            int count = query.Count();
            
            return count;
        }

        /// <summary>
        /// Get customer report by language
        /// </summary>
        /// <returns>Report</returns>
        public List<CustomerReportByLanguageLine> GetCustomerReportByLanguage()
        {
            var query = from c in _context.Customers
                        where !c.Deleted
                        group c by c.LanguageId into grp
                        orderby grp.Count() descending
                        select new
                        {
                            LanguageId = grp.Key,
                            CustomerCount = grp.Count()
                        };
            var tmp1 = query.ToList();
            List<CustomerReportByLanguageLine> report = new List<CustomerReportByLanguageLine>();
            foreach (var r1 in tmp1)
            {
                report.Add(new CustomerReportByLanguageLine()
                {
                    LanguageId = r1.LanguageId,
                    CustomerCount = r1.CustomerCount,
                });
            }
            return report;
        }

        /// <summary>
        /// Get customer report by attribute key
        /// </summary>
        /// <param name="customerAttributeKey">Customer attribute key</param>
        /// <returns>Report</returns>
        public List<CustomerReportByAttributeKeyLine> GetCustomerReportByAttributeKey(string customerAttributeKey)
        {
            if (String.IsNullOrEmpty(customerAttributeKey))
                throw new ArgumentNullException("customerAttributeKey");

            
            var report = _context.Sp_CustomerReportByAttributeKey(customerAttributeKey).ToList();

            return report;
        }
        
        #endregion

        #region Customer attributes

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        public void DeleteCustomerAttribute(int customerAttributeId)
        {
            var customerAttribute = GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                return;

            
            if (!_context.IsAttached(customerAttribute))
                _context.CustomerAttributes.Attach(customerAttribute);
            _context.DeleteObject(customerAttribute);
            _context.SaveChanges();
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

            
            var query = from ca in _context.CustomerAttributes
                        where ca.CustomerAttributeId == customerAttributeId
                        select ca;
            var customerAttribute = query.SingleOrDefault();

            return customerAttribute;
        }

        /// <summary>
        /// Gets a collection of customer attributes by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer attributes</returns>
        public List<CustomerAttribute> GetCustomerAttributesByCustomerId(int customerId)
        {
            
            var query = from ca in _context.CustomerAttributes
                        where ca.CustomerId == customerId
                        select ca;
            var customerAttributes = query.ToList();
            return customerAttributes;
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            if (customerAttribute.CustomerId == 0)
                throw new NopException("Cannot insert attribute for non existing customer");

            if (customerAttribute.Value == null)
                customerAttribute.Value = string.Empty;

            customerAttribute.Key = CommonHelper.EnsureNotNull(customerAttribute.Key);
            customerAttribute.Key = CommonHelper.EnsureMaximumLength(customerAttribute.Key, 100);
            customerAttribute.Value = CommonHelper.EnsureNotNull(customerAttribute.Value);
            customerAttribute.Value = CommonHelper.EnsureMaximumLength(customerAttribute.Value, 1000);

            
            
            _context.CustomerAttributes.AddObject(customerAttribute);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            if (customerAttribute.CustomerId == 0)
                throw new NopException("Cannot insert attribute for non existing customer");

            customerAttribute.Key = CommonHelper.EnsureNotNull(customerAttribute.Key);
            customerAttribute.Key = CommonHelper.EnsureMaximumLength(customerAttribute.Key, 100);
            customerAttribute.Value = CommonHelper.EnsureNotNull(customerAttribute.Value);
            customerAttribute.Value = CommonHelper.EnsureMaximumLength(customerAttribute.Value, 1000);

            
            if (!_context.IsAttached(customerAttribute))
                _context.CustomerAttributes.Attach(customerAttribute);

            _context.SaveChanges();
        }

        #endregion

        #region Customer roles

        /// <summary>
        /// Marks customer role as deleted
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        public void MarkCustomerRoleAsDeleted(int customerRoleId)
        {
            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole != null)
            {
                customerRole.Deleted = true;
                UpdateCustomerRole(customerRole);
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }
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
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CustomerRole)obj2;
            }

            
            var query = from cr in _context.CustomerRoles
                        where cr.CustomerRoleId == customerRoleId
                        select cr;
            var customerRole = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, customerRole);
            }
            return customerRole;
        }

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <returns>Customer role collection</returns>
        public List<CustomerRole> GetAllCustomerRoles()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(CUSTOMERROLES_ALL_KEY, showHidden);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRole>)obj2;
            }

            
            var query = from cr in _context.CustomerRoles
                        orderby cr.Name
                        where (showHidden || cr.Active) && !cr.Deleted
                        select cr;
            var customerRoles = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, customerRoles);
            }
            return customerRoles;
        }

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer role collection</returns>
        public List<CustomerRole> GetCustomerRolesByCustomerId(int customerId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetCustomerRolesByCustomerId(customerId, showHidden);
        }

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer role collection</returns>
        public List<CustomerRole> GetCustomerRolesByCustomerId(int customerId, bool showHidden)
        {
            if (customerId == 0)
                return new List<CustomerRole>();

            var query = from cr in _context.CustomerRoles
                        from c in cr.NpCustomers
                        where (showHidden || cr.Active) &&
                            !cr.Deleted &&
                            c.CustomerId == customerId
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

            customerRole.Name = CommonHelper.EnsureNotNull(customerRole.Name);
            customerRole.Name = CommonHelper.EnsureMaximumLength(customerRole.Name, 255);

            
            
            _context.CustomerRoles.AddObject(customerRole);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public void UpdateCustomerRole(CustomerRole customerRole)    
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            customerRole.Name = CommonHelper.EnsureNotNull(customerRole.Name);
            customerRole.Name = CommonHelper.EnsureMaximumLength(customerRole.Name, 255);
            
            
            if (!_context.IsAttached(customerRole))
                _context.CustomerRoles.Attach(customerRole);
            
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Adds a customer to role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        public void AddCustomerToRole(int customerId, int customerRoleId)
        {
            var customer = GetCustomerById(customerId);
            if (customer == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            
            if (!_context.IsAttached(customer))
                _context.Customers.Attach(customer);
            if (!_context.IsAttached(customerRole))
                _context.CustomerRoles.Attach(customerRole);
            
            //ensure that navigation property is loaded
            if (customer.NpCustomerRoles == null)
                _context.LoadProperty(customer, c => c.NpCustomerRoles);

            customer.NpCustomerRoles.Add(customerRole);

            _context.SaveChanges();
        }

        /// <summary>
        /// Removes a customer from role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        public void RemoveCustomerFromRole(int customerId, int customerRoleId)
        {
            var customer = GetCustomerById(customerId);
            if (customer == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            
            if (!_context.IsAttached(customer))
                _context.Customers.Attach(customer);
            if (!_context.IsAttached(customerRole))
                _context.CustomerRoles.Attach(customerRole);

            //ensure that navigation property is loaded
            if (customer.NpCustomerRoles == null)
                _context.LoadProperty(customer, c => c.NpCustomerRoles);

            customer.NpCustomerRoles.Remove(customerRole);
            _context.SaveChanges();
        }

        #endregion

        #region Customer discount

        /// <summary>
        /// Adds a discount to a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void AddDiscountToCustomerRole(int customerRoleId, int discountId)
        {
            var discount = IoC.Resolve<IDiscountService>().GetDiscountById(discountId);
            if (discount == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);
            if (!_context.IsAttached(customerRole))
                _context.CustomerRoles.Attach(customerRole);

            //ensure that navigation property is loaded
            if (discount.NpCustomerRoles == null)
                _context.LoadProperty(discount, d => d.NpCustomerRoles);

            discount.NpCustomerRoles.Add(customerRole);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes a discount from a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public void RemoveDiscountFromCustomerRole(int customerRoleId, int discountId)
        {
            var discount = IoC.Resolve<IDiscountService>().GetDiscountById(discountId);
            if (discount == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            
            if (!_context.IsAttached(discount))
                _context.Discounts.Attach(discount);
            if (!_context.IsAttached(customerRole))
                _context.CustomerRoles.Attach(customerRole);

            //ensure that navigation property is loaded
            if (discount.NpCustomerRoles == null)
                _context.LoadProperty(discount, d => d.NpCustomerRoles);

            discount.NpCustomerRoles.Remove(customerRole);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a customer roles assigned to discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Customer roles</returns>
        public List<CustomerRole> GetCustomerRolesByDiscountId(int discountId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(CUSTOMERROLES_BY_DISCOUNTID_KEY, discountId, showHidden);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRole>)obj2;
            }

            
            var query = from cr in _context.CustomerRoles
                        from d in cr.NpDiscounts
                        where (showHidden || cr.Active) &&
                            !cr.Deleted &&
                            d.DiscountId == discountId
                        orderby cr.Name
                        select cr;
            var customerRoles = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, customerRoles);
            }
            return customerRoles;
        }
        
        #endregion

        #region Customer sessions

        /// <summary>
        /// Gets a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <returns>Customer session</returns>
        public CustomerSession GetCustomerSessionByGuid(Guid customerSessionGuid)
        {
            if (customerSessionGuid == Guid.Empty)
                return null;

            
            var query = from cs in _context.CustomerSessions
                        where cs.CustomerSessionGuid == customerSessionGuid
                        orderby cs.LastAccessed descending
                        select cs;
            var customerSession = query.FirstOrDefault();
            return customerSession;
        }

        /// <summary>
        /// Gets a customer session by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer session</returns>
        public CustomerSession GetCustomerSessionByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            
            var query = from cs in _context.CustomerSessions
                        where cs.CustomerId == customerId
                        orderby cs.LastAccessed descending
                        select cs;
            var customerSession = query.FirstOrDefault();
            return customerSession;
        }

        /// <summary>
        /// Deletes a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        public void DeleteCustomerSession(Guid customerSessionGuid)
        {
            var customerSession = GetCustomerSessionByGuid(customerSessionGuid);
            if (customerSession == null)
                return;
                        
            if (!_context.IsAttached(customerSession))
                _context.CustomerSessions.Attach(customerSession);
            _context.DeleteObject(customerSession);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all customer sessions
        /// </summary>
        /// <returns>Customer session collection</returns>
        public List<CustomerSession> GetAllCustomerSessions()
        {
            
            var query = from cs in _context.CustomerSessions
                        orderby cs.LastAccessed descending
                        select cs;
            var customerSessions = query.ToList();
            return customerSessions;
        }

        /// <summary>
        /// Gets all customer sessions with non empty shopping cart
        /// </summary>
        /// <returns>Customer session collection</returns>
        public List<CustomerSession> GetAllCustomerSessionsWithNonEmptyShoppingCart()
        {
            var sciQuery = from sci in _context.ShoppingCartItems
                           where sci.ShoppingCartTypeId == (int)ShoppingCartTypeEnum.ShoppingCart
                           select sci.CustomerSessionGuid;
            var query = from cs in _context.CustomerSessions
                        where sciQuery.Contains(cs.CustomerSessionGuid)
                        orderby cs.LastAccessed descending
                        select cs;
            var customerSessions = query.ToList();
            return customerSessions;
        }

        /// <summary>
        /// Deletes all expired customer sessions
        /// </summary>
        /// <param name="olderThan">Older than date and time</param>
        public void DeleteExpiredCustomerSessions(DateTime olderThan)
        {
            var sciQuery = from sci in _context.ShoppingCartItems
                           select sci.CustomerSessionGuid;

            var scQuery = from sc in _context.CustomerSessions
                          where !sciQuery.Contains(sc.CustomerSessionGuid) &&
                          sc.LastAccessed < olderThan
                          select sc;

            var customerSessions = scQuery.ToList();
            foreach (var customerSession in customerSessions)
            {
                _context.DeleteObject(customerSession);
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Saves a customer session to the data storage if it exists or creates new one
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessed">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        public CustomerSession SaveCustomerSession(Guid customerSessionGuid,
            int customerId, DateTime lastAccessed, bool isExpired)
        {
            var customerSession = GetCustomerSessionByGuid(customerSessionGuid);
            if (customerSession == null)
            {
                customerSession = new CustomerSession()
                {
                    CustomerSessionGuid = customerSessionGuid,
                    CustomerId = customerId,
                    LastAccessed = lastAccessed,
                    IsExpired = isExpired
                };
                InsertCustomerSession(customerSession);
            }
            else
            {
                customerSession.CustomerSessionGuid = customerSessionGuid;
                customerSession.CustomerId = customerId;
                customerSession.LastAccessed = lastAccessed;
                customerSession.IsExpired = isExpired;
                UpdateCustomerSession(customerSession);
            }
            return customerSession;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.CustomerManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        public bool AnonymousCheckoutAllowed
        {
            get
            {
                bool allowed = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Checkout.AnonymousCheckoutAllowed", false);
                return allowed;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Checkout.AnonymousCheckoutAllowed", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public bool UsernamesEnabled
        {
            get
            {
                bool usernamesEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.UsernamesEnabled");
                return usernamesEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.UsernamesEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change their usernames
        /// </summary>
        public bool AllowCustomersToChangeUsernames
        {
            get
            {
                bool result = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.AllowCustomersToChangeUsernames");
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.AllowCustomersToChangeUsernames", value.ToString());
            }
        }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public CustomerNameFormatEnum CustomerNameFormatting
        {
            get
            {
                int customerNameFormatting = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Customer.CustomerNameFormatting");
                return (CustomerNameFormatEnum)customerNameFormatting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.CustomerNameFormatting", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload avatars.
        /// </summary>
        public bool AllowCustomersToUploadAvatars
        {
            get
            {
                bool allowCustomersToUploadAvatars = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.CustomersAllowedToUploadAvatars");
                return allowCustomersToUploadAvatars;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.CustomersAllowedToUploadAvatars", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display default user avatar.
        /// </summary>
        public bool DefaultAvatarEnabled
        {
            get
            {
                bool defaultAvatarEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.DefaultAvatarEnabled");
                return defaultAvatarEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.DefaultAvatarEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown
        /// </summary>
        public bool ShowCustomersLocation
        {
            get
            {
                bool showCustomersLocation = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.ShowCustomersLocation");
                return showCustomersLocation;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.ShowCustomersLocation", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date
        /// </summary>
        public bool ShowCustomersJoinDate
        {
            get
            {
                bool showCustomersJoinDate = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.ShowCustomersJoinDate");
                return showCustomersJoinDate;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.ShowCustomersJoinDate", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to view profiles of other customers
        /// </summary>
        public bool AllowViewingProfiles
        {
            get
            {
                bool allowViewingProfiles = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Customer.AllowViewingProfiles");
                return allowViewingProfiles;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Customer.AllowViewingProfiles", value.ToString());
            }
        }

        /// <summary>
        /// Tax display type
        /// </summary>
        public CustomerRegistrationTypeEnum CustomerRegistrationType
        {
            get
            {
                int customerRegistrationType = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Common.CustomerRegistrationType", (int)CustomerRegistrationTypeEnum.Standard);
                return (CustomerRegistrationTypeEnum)customerRegistrationType;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.CustomerRegistrationType", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow navigation only for registered users.
        /// </summary>
        public bool AllowNavigationOnlyRegisteredCustomers
        {
            get
            {
                bool allowOnlyRegisteredCustomers = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowNavigationOnlyRegisteredCustomers");
                return allowOnlyRegisteredCustomers;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.AllowNavigationOnlyRegisteredCustomers", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether product reviews must be approved by administrator.
        /// </summary>
        public bool ProductReviewsMustBeApproved
        {
            get
            {
                bool productReviewsMustBeApproved = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.ProductReviewsMustBeApproved");
                return productReviewsMustBeApproved;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.ProductReviewsMustBeApproved", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewProduct
        {
            get
            {
                bool allowAnonymousUsersToReviewProduct = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowAnonymousUsersToReviewProduct");
                return allowAnonymousUsersToReviewProduct;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.AllowAnonymousUsersToReviewProduct", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowAnonymousUsersToEmailAFriend", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.AllowAnonymousUsersToEmailAFriend", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to set product ratings.
        /// </summary>
        public bool AllowAnonymousUsersToSetProductRatings
        {
            get
            {
                bool allowAnonymousUsersToSetProductRatings = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowAnonymousUsersToSetProductRatings");
                return allowAnonymousUsersToSetProductRatings;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.AllowAnonymousUsersToSetProductRatings", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'New customer' notification message should be sent to a store owner
        /// </summary>
        public bool NotifyNewCustomerRegistration
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.NotifyNewCustomerRegistration", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.NotifyNewCustomerRegistration", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Gender' is enabled
        /// </summary>
        public bool FormFieldGenderEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.GenderEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.GenderEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is enabled
        /// </summary>
        public bool FormFieldDateOfBirthEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.DateOfBirthEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.DateOfBirthEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is enabled
        /// </summary>
        public bool FormFieldCompanyEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.CompanyEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.CompanyEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is required
        /// </summary>
        public bool FormFieldCompanyRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.CompanyRequired", false);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.CompanyRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is enabled
        /// </summary>
        public bool FormFieldStreetAddressEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.StreetAddressEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.StreetAddressEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is required
        /// </summary>
        public bool FormFieldStreetAddressRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.StreetAddressRequired", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.StreetAddressRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is enabled
        /// </summary>
        public bool FormFieldStreetAddress2Enabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.StreetAddress2Enabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.StreetAddress2Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is required
        /// </summary>
        public bool FormFieldStreetAddress2Required
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.StreetAddress2Required", false);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.StreetAddress2Required", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is enabled
        /// </summary>
        public bool FormFieldPostCodeEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.PostCodeEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.PostCodeEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is required
        /// </summary>
        public bool FormFieldPostCodeRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.PostCodeRequired", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.PostCodeRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is enabled
        /// </summary>
        public bool FormFieldCityEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.CityEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.CityEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is required
        /// </summary>
        public bool FormFieldCityRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.CityRequired", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.CityRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Country' is enabled
        /// </summary>
        public bool FormFieldCountryEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.CountryEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.CountryEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'State' is enabled
        /// </summary>
        public bool FormFieldStateEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.StateEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.StateEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is enabled
        /// </summary>
        public bool FormFieldPhoneEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.PhoneEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.PhoneEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is required
        /// </summary>
        public bool FormFieldPhoneRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.PhoneRequired", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.PhoneRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is enabled
        /// </summary>
        public bool FormFieldFaxEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.FaxEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.FaxEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is required
        /// </summary>
        public bool FormFieldFaxRequired
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.FaxRequired", false);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.FaxRequired", value.ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' is enabled
        /// </summary>
        public bool FormFieldNewsletterEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.NewsletterEnabled", true);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.NewsletterEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Time Zone' is enabled
        /// </summary>
        public bool FormFieldTimeZoneEnabled
        {
            get
            {
                bool setting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("FormField.TimeZoneEnabled", false);
                return setting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("FormField.TimeZoneEnabled", value.ToString());
            }
        }

        #endregion
    }
}