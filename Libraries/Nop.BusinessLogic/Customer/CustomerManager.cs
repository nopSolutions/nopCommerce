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

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Customer manager
    /// </summary>
    public partial class CustomerManager
    {
        #region Constants
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        private const string CUSTOMERROLES_BY_ID_KEY = "Nop.customerrole.id-{0}";
        private const string CUSTOMERROLES_BY_DISCOUNTID_KEY = "Nop.customerrole.bydiscountid-{0}-{1}";
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";
        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes an address by address identifier 
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        public static void DeleteAddress(int addressId)
        {
            var address = GetAddressById(addressId);
            if (address == null)
                return;

            var customer = address.Customer;
            if (customer != null)
            {
                if (customer.BillingAddressId == address.AddressId)
                    customer = SetDefaultBillingAddress(customer.CustomerId, 0);

                if (customer.ShippingAddressId == address.AddressId)
                    customer = SetDefaultShippingAddress(customer.CustomerId, 0);
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(address))
                context.Addresses.Attach(address);
            context.DeleteObject(address);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        public static Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.Addresses
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
        public static List<Address> GetAddressesByCustomerId(int customerId, bool getBillingAddresses)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.Addresses
                        orderby a.CreatedOn
                        where a.CustomerId == customerId && a.IsBillingAddress == getBillingAddresses
                        select a;
            var addresses = query.ToList();

            return addresses;
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="isBillingAddress">A value indicating whether the address is billing or shipping</param>
        /// <param name="firstName">The first name</param>
        /// <param name="lastName">The last name</param>
        /// <param name="phoneNumber">The phone number</param>
        /// <param name="email">The email</param>
        /// <param name="faxNumber">The fax number</param>
        /// <param name="company">The company</param>
        /// <param name="address1">The address 1</param>
        /// <param name="address2">The address 2</param>
        /// <param name="city">The city</param>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <param name="zipPostalCode">The zip/postal code</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>An address</returns>
        public static Address InsertAddress(int customerId, bool isBillingAddress,
            string firstName, string lastName, string phoneNumber,
            string email, string faxNumber, string company, string address1,
            string address2, string city, int stateProvinceId, string zipPostalCode,
            int countryId, DateTime createdOn, DateTime updatedOn)
        {
            if (firstName == null)
                firstName = string.Empty;
            if (lastName == null)
                lastName = string.Empty;
            if (phoneNumber == null)
                phoneNumber = string.Empty;
            if (email == null)
                email = string.Empty;
            if (faxNumber == null)
                faxNumber = string.Empty;
            if (company == null)
                company = string.Empty;
            if (address1 == null)
                address1 = string.Empty;
            if (address2 == null)
                address2 = string.Empty;
            if (city == null)
                city = string.Empty;
            if (zipPostalCode == null)
                zipPostalCode = string.Empty;
            firstName = firstName.Trim();
            lastName = lastName.Trim();
            phoneNumber = phoneNumber.Trim();
            email = email.Trim();
            faxNumber = faxNumber.Trim();
            company = company.Trim();
            address1 = address1.Trim();
            address2 = address2.Trim();
            city = city.Trim();
            zipPostalCode = zipPostalCode.Trim();

            firstName = CommonHelper.EnsureMaximumLength(firstName, 100);
            lastName = CommonHelper.EnsureMaximumLength(lastName, 100);
            phoneNumber = CommonHelper.EnsureMaximumLength(phoneNumber, 50);
            email = CommonHelper.EnsureMaximumLength(email, 255);
            faxNumber = CommonHelper.EnsureMaximumLength(faxNumber, 50);
            company = CommonHelper.EnsureMaximumLength(company, 100);
            address1 = CommonHelper.EnsureMaximumLength(address1, 100);
            address2 = CommonHelper.EnsureMaximumLength(address2, 100);
            city = CommonHelper.EnsureMaximumLength(city, 100);
            zipPostalCode = CommonHelper.EnsureMaximumLength(zipPostalCode, 10);

            var context = ObjectContextHelper.CurrentObjectContext;
            var address = context.Addresses.CreateObject();
            address.CustomerId = customerId;
            address.IsBillingAddress = isBillingAddress;
            address.FirstName = firstName;
            address.LastName = lastName;
            address.PhoneNumber = phoneNumber;
            address.Email = email;
            address.FaxNumber = faxNumber;
            address.Company = company;
            address.Address1 = address1;
            address.Address2 = address2;
            address.City = city;
            address.StateProvinceId = stateProvinceId;
            address.ZipPostalCode = zipPostalCode;
            address.CountryId = countryId;
            address.CreatedOn = createdOn;
            address.UpdatedOn = updatedOn;

            context.Addresses.AddObject(address);
            context.SaveChanges();
            return address;
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="addressId">The address identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="isBillingAddress">A value indicating whether the address is billing or shipping</param>
        /// <param name="firstName">The first name</param>
        /// <param name="lastName">The last name</param>
        /// <param name="phoneNumber">The phone number</param>
        /// <param name="email">The email</param>
        /// <param name="faxNumber">The fax number</param>
        /// <param name="company">The company</param>
        /// <param name="address1">The address 1</param>
        /// <param name="address2">The address 2</param>
        /// <param name="city">The city</param>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <param name="zipPostalCode">The zip/postal code</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>An address</returns>
        public static Address UpdateAddress(int addressId, int customerId,
            bool isBillingAddress, string firstName, string lastName, string phoneNumber,
            string email, string faxNumber, string company, string address1,
            string address2, string city, int stateProvinceId, string zipPostalCode,
            int countryId, DateTime createdOn, DateTime updatedOn)
        {
            if (firstName == null)
                firstName = string.Empty;
            if (lastName == null)
                lastName = string.Empty;
            if (phoneNumber == null)
                phoneNumber = string.Empty;
            if (email == null)
                email = string.Empty;
            if (faxNumber == null)
                faxNumber = string.Empty;
            if (company == null)
                company = string.Empty;
            if (address1 == null)
                address1 = string.Empty;
            if (address2 == null)
                address2 = string.Empty;
            if (city == null)
                city = string.Empty;
            if (zipPostalCode == null)
                zipPostalCode = string.Empty;
            firstName = firstName.Trim();
            lastName = lastName.Trim();
            phoneNumber = phoneNumber.Trim();
            email = email.Trim();
            faxNumber = faxNumber.Trim();
            company = company.Trim();
            address1 = address1.Trim();
            address2 = address2.Trim();
            city = city.Trim();
            zipPostalCode = zipPostalCode.Trim();

            firstName = CommonHelper.EnsureMaximumLength(firstName, 100);
            lastName = CommonHelper.EnsureMaximumLength(lastName, 100);
            phoneNumber = CommonHelper.EnsureMaximumLength(phoneNumber, 50);
            email = CommonHelper.EnsureMaximumLength(email, 255);
            faxNumber = CommonHelper.EnsureMaximumLength(faxNumber, 50);
            company = CommonHelper.EnsureMaximumLength(company, 100);
            address1 = CommonHelper.EnsureMaximumLength(address1, 100);
            address2 = CommonHelper.EnsureMaximumLength(address2, 100);
            city = CommonHelper.EnsureMaximumLength(city, 100);
            zipPostalCode = CommonHelper.EnsureMaximumLength(zipPostalCode, 10);

            var address = GetAddressById(addressId);
            if (address == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(address))
                context.Addresses.Attach(address);

            address.CustomerId = customerId;
            address.IsBillingAddress = isBillingAddress;
            address.FirstName = firstName;
            address.LastName = lastName;
            address.PhoneNumber = phoneNumber;
            address.Email = email;
            address.FaxNumber = faxNumber;
            address.Company = company;
            address.Address1 = address1;
            address.Address2 = address2;
            address.City = city;
            address.StateProvinceId = stateProvinceId;
            address.ZipPostalCode = zipPostalCode;
            address.CountryId = countryId;
            address.CreatedOn = createdOn;
            address.UpdatedOn = updatedOn;
            context.SaveChanges();
            return address;
        }

        /// <summary>
        /// Gets a value indicating whether address can be used as billing address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public static bool CanUseAddressAsBillingAddress(Address address)
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
        public static bool CanUseAddressAsShippingAddress(Address address)
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

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        public static void ResetCheckoutData(int customerId, bool clearCouponCodes)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                //customer = SetDefaultShippingAddress(customer.CustomerId, 0);
                //customer = SetDefaultBillingAddress(customer.CustomerId, 0);
                customer.LastShippingOption = null;
                customer = SetLastPaymentMethodId(customer.CustomerId, 0);
                customer.UseRewardPointsDuringCheckout = false;
                if (clearCouponCodes)
                {
                    customer = ApplyDiscountCouponCode(customer.CustomerId, string.Empty);
                    customer = ApplyGiftCardCouponCode(customer.CustomerId, string.Empty);
                    customer = ApplyCheckoutAttributes(customer.CustomerId, string.Empty);
                }
            }
        }

        /// <summary>
        /// Sets a default billing address
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="billingAddressId">Billing address identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetDefaultBillingAddress(int customerId, int billingAddressId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId,
                    customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey,
                    customer.AffiliateId, billingAddressId,
                    customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator,
                    customer.TotalForumPosts, customer.Signature,
                    customer.AdminComment, customer.Active,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a default shipping address
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="shippingAddressId">Shipping address identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetDefaultShippingAddress(int customerId, int shippingAddressId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId,
                    customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash,
                    customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId,
                    shippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator,
                    customer.TotalForumPosts, customer.Signature,
                    customer.AdminComment, customer.Active,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer payment method
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetLastPaymentMethodId(int customerId, int paymentMethodId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId,
                    customer.CustomerGuid, customer.Email,
                     customer.Username, customer.PasswordHash,
                     customer.SaltKey, customer.AffiliateId, customer.BillingAddressId,
                     customer.ShippingAddressId, paymentMethodId,
                     customer.LastAppliedCouponCode,
                     customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                     customer.LanguageId, customer.CurrencyId,
                     customer.TaxDisplayType, customer.IsTaxExempt, customer.IsAdmin,
                     customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                     customer.Signature, customer.AdminComment,
                     customer.Active, customer.Deleted,
                     customer.RegistrationDate, customer.TimeZoneId,
                     customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer time zone
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="timeZoneId">Time zone identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetTimeZoneId(int customerId, string timeZoneId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                     customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId, customer.BillingAddressId,
                     customer.ShippingAddressId, customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                     customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                     customer.LanguageId, customer.CurrencyId, customer.TaxDisplayType,
                     customer.IsTaxExempt, customer.IsAdmin,
                     customer.IsGuest, customer.IsForumModerator, 
                     customer.TotalForumPosts,  customer.Signature, customer.AdminComment, 
                     customer.Active, customer.Deleted, customer.RegistrationDate, 
                     timeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer email
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newEmail">New email</param>
        /// <returns>Customer</returns>
        public static Customer SetEmail(int customerId, string newEmail)
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

                var cust2 = GetCustomerByEmail(newEmail);
                if (cust2 != null && customer.CustomerId != cust2.CustomerId)
                {
                    throw new NopException("The e-mail address is already in use.");
                }

                if (newEmail.Length > 40)
                {
                    throw new NopException("E-mail address is too long.");
                }

                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, newEmail,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active, customer.Deleted,
                    customer.RegistrationDate, customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer sugnature
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="signature">Signature</param>
        /// <returns>Customer</returns>
        public static Customer SetCustomerSignature(int customerId, string signature)
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
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                    signature, customer.AdminComment, customer.Active, customer.Deleted,
                    customer.RegistrationDate, customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer's affiliate
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetAffiliate(int customerId, int affiliateId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, affiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active, customer.Deleted,
                    customer.RegistrationDate, customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Sets a customer's affiliate
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <returns>Customer</returns>
        public static Customer SetCustomerDateOfBirth(int customerId, DateTime? dateOfBirth)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active, customer.Deleted,
                    customer.RegistrationDate, customer.TimeZoneId, customer.AvatarId, dateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Removes customer avatar
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="avatarId">Customer avatar identifier</param>
        /// <returns>Customer</returns>
        public static Customer SetCustomerAvatarId(int customerId, int avatarId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                     customer.Username, customer.PasswordHash, customer.SaltKey,
                     customer.AffiliateId, customer.BillingAddressId,
                     customer.ShippingAddressId, customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                     customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                     customer.LanguageId, customer.CurrencyId,
                     customer.TaxDisplayType, customer.IsTaxExempt, customer.IsAdmin,
                     customer.IsGuest, customer.IsForumModerator, customer.TotalForumPosts,
                     customer.Signature, customer.AdminComment, customer.Active, customer.Deleted,
                     customer.RegistrationDate, customer.TimeZoneId, avatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Create anonymous user for current user
        /// </summary>
        /// <returns>Guest user</returns>
        public static void CreateAnonymousUser()
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

                NopContext.Current.Session = UpdateCustomerSession(NopContext.Current.Session.CustomerSessionGuid,
                    guestCustomer.CustomerId, LastAccessed, NopContext.Current.Session.IsExpired);
            }
        }

        /// <summary>
        /// Applies a discount coupon code to a current customer
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        public static void ApplyDiscountCouponCode(string couponCode)
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
        public static Customer ApplyDiscountCouponCode(int customerId, string couponCode)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                    customer.Email, customer.Username, customer.PasswordHash,
                    customer.SaltKey, customer.AffiliateId, customer.BillingAddressId,
                    customer.ShippingAddressId, customer.LastPaymentMethodId,
                    couponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType, customer.IsTaxExempt,
                    customer.IsAdmin, customer.IsGuest, customer.IsForumModerator,
                    customer.TotalForumPosts, customer.Signature, customer.AdminComment,
                    customer.Active, customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Applies a gift card coupon code to a current customer
        /// </summary>
        /// <param name="couponCodesXml">Coupon code (XML)</param>
        public static void ApplyGiftCardCouponCode(string couponCodesXml)
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
        public static Customer ApplyGiftCardCouponCode(int customerId, string couponCodesXml)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, couponCodesXml,
                    customer.CheckoutAttributes, customer.LanguageId, customer.CurrencyId,
                    customer.TaxDisplayType, customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                    customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active,
                    customer.Deleted, customer.RegistrationDate, 
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Applies selected checkout attibutes to a current customer
        /// </summary>
        /// <param name="attributesXml">Checkout attibutes (XML)</param>
        public static void ApplyCheckoutAttributes(string attributesXml)
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
        public static Customer ApplyCheckoutAttributes(int customerId, string attributesXml)
        {
            if (attributesXml == null)
                attributesXml = string.Empty;
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    attributesXml, customer.LanguageId, customer.CurrencyId,
                    customer.TaxDisplayType, customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                    customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            return customer;
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>Customer collection</returns>
        public static List<Customer> GetAllCustomers()
        {
            int totalRecords = 0;
            return GetAllCustomers(null, null, null, string.Empty, false,
                int.MaxValue, 0, out totalRecords);
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
        /// <param name="totalRecords">Total records</param>
        /// <returns>Customer collection</returns>
        public static List<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int pageSize, int pageIndex, out int totalRecords)
        {
            return GetAllCustomers(registrationFrom, registrationTo,
                email, username, dontLoadGuestCustomers, 0, 0,
                pageSize, pageIndex, out totalRecords);
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
        /// <param name="totalRecords">Total records</param>
        /// <returns>Customer collection</returns>
        public static List<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int dateOfBirthMonth, int dateOfBirthDay, 
            int pageSize, int pageIndex, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;

            if (email == null)
                email = string.Empty;

            if (username == null)
                username = string.Empty;

            var context = ObjectContextHelper.CurrentObjectContext;
            var customers = context.Sp_CustomerLoadAll(registrationFrom,
                registrationTo, email, username, dontLoadGuestCustomers,
                dateOfBirthMonth, dateOfBirthDay, 
                pageSize, pageIndex, out totalRecords).ToList();

            return customers;
        }

        /// <summary>
        /// Gets all customers by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Customer collection</returns>
        public static List<Customer> GetAffiliatedCustomers(int affiliateId)
        {
            if (affiliateId == 0)
                return new List<Customer>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static List<Customer> GetCustomersByCustomerRoleId(int customerRoleId)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
                        from cr in c.NpCustomerRoles
                        where (showHidden || c.Active) &&
                            !c.Deleted &&
                            cr.CustomerRoleId == customerRoleId
                        orderby c.RegistrationDate descending
                        select c;

            //var query = from c in context.Customers
            //            where (showHidden || c.Active) && !c.Deleted
            //            && c.NpCustomerRoles.Any(cr => cr.CustomerRoleId == customerRoleId)
            //            orderby c.RegistrationDate descending
            //            select c;


            //var query = context.CustomerRoles.Where(cr => cr.CustomerRoleId == customerRoleId)
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
        public static void MarkCustomerAsDeleted(int customerId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey,
                    customer.AffiliateId, customer.BillingAddressId,
                    customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator,
                    customer.TotalForumPosts, customer.Signature,
                    customer.AdminComment, customer.Active,
                    true, customer.RegistrationDate, customer.TimeZoneId, 
                    customer.AvatarId, customer.DateOfBirth);
            }
        }

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="email">Customer Email</param>
        /// <returns>A customer</returns>
        public static Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static Customer GetCustomerByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static Customer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static Customer GetCustomerByGuid(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static Customer AddCustomer(string email, string username, string password,
            bool isAdmin, bool isGuest, bool active, out MembershipCreateStatus status)
        {
            int affiliateId = 0;
            HttpCookie affiliateCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.AffiliateId");
            if (affiliateCookie != null)
            {
                Affiliate affiliate = AffiliateManager.GetAffiliateById(Convert.ToInt32(affiliateCookie.Value));
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
        public static Customer AddCustomer(string email, string username, string password,
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
        public static Customer AddCustomer(Guid customerGuid, string email, string username,
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
                    if (CustomerManager.CustomerRegistrationType == CustomerRegistrationTypeEnum.Disabled)
                    {
                        status = MembershipCreateStatus.ProviderError;
                        return customer;
                    }
                }
                if (CustomerManager.UsernamesEnabled)
                {
                    if (GetCustomerByUsername(username) != null)
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                        return customer;
                    }

                    if (username.Length > 40)
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

                if (email.Length > 40)
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return customer;
                }

                if (!NopContext.Current.IsAdmin)
                {
                    if (CustomerManager.CustomerRegistrationType == CustomerRegistrationTypeEnum.EmailValidation ||
                        CustomerManager.CustomerRegistrationType == CustomerRegistrationTypeEnum.AdminApproval)
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
                    MessageManager.SendCustomerWelcomeMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
                }
                else
                {
                    if (CustomerManager.CustomerRegistrationType == CustomerRegistrationTypeEnum.EmailValidation)
                    {
                        Guid accountActivationToken = Guid.NewGuid();
                        customer.AccountActivationToken = accountActivationToken.ToString();

                        MessageManager.SendCustomerEmailValidationMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
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
        public static Customer AddCustomerForced(Guid customerGuid, string email,
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
            if (username == null)
                username = string.Empty;
            username = username.Trim();

            if (email == null)
                email = string.Empty;
            email = email.Trim();
            
            email = CommonHelper.EnsureMaximumLength(email, 255);
            username = CommonHelper.EnsureMaximumLength(username, 100);
            passwordHash = CommonHelper.EnsureMaximumLength(passwordHash, 255);
            saltKey = CommonHelper.EnsureMaximumLength(saltKey, 255);
            signature = CommonHelper.EnsureMaximumLength(signature, 300);
            adminComment = CommonHelper.EnsureMaximumLength(adminComment, 4000);
            timeZoneId = CommonHelper.EnsureMaximumLength(timeZoneId, 200);

            var context = ObjectContextHelper.CurrentObjectContext;

            var customer = context.Customers.CreateObject();
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

            context.Customers.AddObject(customer);
            context.SaveChanges();

            //reward points
            if (!isGuest &&
                OrderManager.RewardPointsEnabled &&
                OrderManager.RewardPointsForRegistration > 0)
            {
                var rph = OrderManager.InsertRewardPointsHistory(customer.CustomerId, 0,
                    OrderManager.RewardPointsForRegistration, decimal.Zero, decimal.Zero,
                    string.Empty, LocalizationManager.GetLocaleResourceString("RewardPoints.Message.EarnedForRegistration"),
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
        /// <param name="customerId">The customer identifier</param>
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
        public static Customer UpdateCustomer(int customerId,
            Guid customerGuid, string email,
            string username, string passwordHash, string saltKey,
            int affiliateId, int billingAddressId,
            int shippingAddressId, int lastPaymentMethodId,
            string lastAppliedCouponCode, string giftCardCouponCodes,
            string checkoutAttributes, int languageId,
            int currencyId, TaxDisplayTypeEnum taxDisplayType,
            bool isTaxExempt, bool isAdmin, bool isGuest, bool isForumModerator,
            int totalForumPosts, string signature, string adminComment, 
            bool active, bool deleted, DateTime registrationDate, 
            string timeZoneId, int avatarId, DateTime? dateOfBirth)
        {
            if (username == null)
                username = string.Empty;
            username = username.Trim();

            if (email == null)
                email = string.Empty;
            email = email.Trim();

            if (signature == null)
                signature = string.Empty;
            signature = signature.Trim();

            email = CommonHelper.EnsureMaximumLength(email, 255);
            username = CommonHelper.EnsureMaximumLength(username, 100);
            passwordHash = CommonHelper.EnsureMaximumLength(passwordHash, 255);
            saltKey = CommonHelper.EnsureMaximumLength(saltKey, 255);
            signature = CommonHelper.EnsureMaximumLength(signature, 300);
            adminComment = CommonHelper.EnsureMaximumLength(adminComment, 4000);
            timeZoneId = CommonHelper.EnsureMaximumLength(timeZoneId, 200);

            var customer = GetCustomerById(customerId);
            if (customer == null)
                return null;

            var subscriptionOld = customer.NewsLetterSubscription;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customer))
                context.Customers.Attach(customer);
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
            context.SaveChanges();

            if (subscriptionOld != null && !email.ToLower().Equals(subscriptionOld.Email.ToLower()))
            {
                MessageManager.UpdateNewsLetterSubscription(subscriptionOld.NewsLetterSubscriptionId,
                    email, subscriptionOld.Active);
            }

            //raise event             
            EventContext.Current.OnCustomerUpdated(null,
                new CustomerEventArgs() { Customer = customer });
            
            return customer;
        }

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <param name="oldPassword">Old password</param>
        /// <param name="password">Password</param>
        public static void ModifyPassword(string email, string oldPassword, string password)
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
        public static void ModifyPassword(string email, string newPassword)
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
        public static void ModifyPassword(int customerId, string newPassword)
        {
            if (String.IsNullOrEmpty(newPassword))
                throw new NopException(LocalizationManager.GetLocaleResourceString("Customer.PasswordIsRequired"));
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                string newPasswordSalt = CreateSalt(5);
                string newPasswordHash = CreatePasswordHash(newPassword, newPasswordSalt);
                UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, newPasswordHash, newPasswordSalt,
                    customer.AffiliateId, customer.BillingAddressId,
                    customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                    customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, customer.Active,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
        }

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        public static void Activate(Guid customerGuid)
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
        public static void Activate(int customerId)
        {
            Activate(customerId, false);
        }

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="sendCustomerWelcomeMessage">A value indivating whether to send customer welcome message</param>
        public static void Activate(int customerId, bool sendCustomerWelcomeMessage)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                    customer.Email, customer.Username,
                    customer.PasswordHash, customer.SaltKey, customer.AffiliateId, customer.BillingAddressId,
                    customer.ShippingAddressId, customer.LastPaymentMethodId,
                    customer.LastAppliedCouponCode, customer.GiftCardCouponCodes,
                    customer.CheckoutAttributes, customer.LanguageId,
                    customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                    customer.IsForumModerator, customer.TotalForumPosts,
                    customer.Signature, customer.AdminComment, true,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);

                if (sendCustomerWelcomeMessage)
                {
                    MessageManager.SendCustomerWelcomeMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);
                }
            }
        }

        /// <summary>
        /// Deactivates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        public static void Deactivate(Guid customerGuid)
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
        public static void Deactivate(int customerId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
            {
                customer = UpdateCustomer(customer.CustomerId, customer.CustomerGuid, customer.Email,
                    customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId,
                    customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                    customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                    customer.LanguageId, customer.CurrencyId, customer.TaxDisplayType,
                    customer.IsTaxExempt, customer.IsAdmin,
                    customer.IsGuest, customer.IsForumModerator,
                    customer.TotalForumPosts, customer.Signature,
                    customer.AdminComment, false, customer.Deleted,
                    customer.RegistrationDate, customer.TimeZoneId, 
                    customer.AvatarId, customer.DateOfBirth);
            }
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        /// <param name="email">A customer email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public static bool Login(string email, string password)
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
                    var cart1 = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                    var cart2 = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.Wishlist);
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
                            ShoppingCartManager.AddToCart(
                                item.ShoppingCartType,
                                item.ProductVariantId,
                                item.AttributesXml,
                                item.CustomerEnteredPrice,
                                item.Quantity);
                            ShoppingCartManager.DeleteShoppingCartItem(item.ShoppingCartItemId, true);
                        }
                        foreach (ShoppingCartItem item in cart2)
                        {
                            ShoppingCartManager.AddToCart(
                                item.ShoppingCartType,
                                item.ProductVariantId,
                                item.AttributesXml,
                                item.CustomerEnteredPrice,
                                item.Quantity);
                            ShoppingCartManager.DeleteShoppingCartItem(item.ShoppingCartItemId, true);
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
        public static void Logout()
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

        /// <summary>
        /// Creates a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Password hash</returns>
        private static string CreatePasswordHash(string password, string salt)
        {
            //MD5, SHA1
            string passwordFormat = SettingManager.GetSettingValue("Security.PasswordFormat");
            if (String.IsNullOrEmpty(passwordFormat))
                passwordFormat = "SHA1";

            return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, passwordFormat);
        }

        /// <summary>
        /// Creates a salt
        /// </summary>
        /// <param name="size">A salt size</param>
        /// <returns>A salt</returns>
        private static string CreateSalt(int size)
        {
            var provider = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];
            provider.GetBytes(data);
            return Convert.ToBase64String(data);
        }

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
        public static List<CustomerBestReportLine> GetBestCustomersReport(DateTime? startTime,
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_CustomerBestReport(startTime, endTime,
                orderStatusId, paymentStatusId, shippingStatusId, orderBy).ToList();

            return report;
        }

        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Int</returns>
        public static int GetRegisteredCustomersReport(int days)
        {
            DateTime date = DateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Customers
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
        public static List<CustomerReportByLanguageLine> GetCustomerReportByLanguage()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_CustomerReportByLanguage().ToList();

            return report;
        }

        /// <summary>
        /// Get customer report by attribute key
        /// </summary>
        /// <param name="customerAttributeKey">Customer attribute key</param>
        /// <returns>Report</returns>
        public static List<CustomerReportByAttributeKeyLine> GetCustomerReportByAttributeKey(string customerAttributeKey)
        {
            if (String.IsNullOrEmpty(customerAttributeKey))
                throw new ArgumentNullException("customerAttributeKey");

            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_CustomerReportByAttributeKey(customerAttributeKey).ToList();

            return report;
        }

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        public static void DeleteCustomerAttribute(int customerAttributeId)
        {
            var customerAttribute = GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerAttribute))
                context.CustomerAttributes.Attach(customerAttribute);
            context.DeleteObject(customerAttribute);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>A customer attribute</returns>
        public static CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CustomerAttributes
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
        public static List<CustomerAttribute> GetCustomerAttributesByCustomerId(int customerId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CustomerAttributes
                        where ca.CustomerId == customerId
                        select ca;
            var customerAttributes = query.ToList();
            return customerAttributes;
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="key">An attribute key</param>
        /// <param name="value">An attribute value</param>
        /// <returns>A customer attribute</returns>
        public static CustomerAttribute InsertCustomerAttribute(int customerId,
            string key, string value)
        {
            if (customerId == 0)
                throw new NopException("Cannot insert attribute for non existing customer");

            if (value == null)
                value = string.Empty;

            key = CommonHelper.EnsureMaximumLength(key, 100);
            value = CommonHelper.EnsureMaximumLength(value, 1000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var customerAttribute = context.CustomerAttributes.CreateObject();
            customerAttribute.CustomerId = customerId;
            customerAttribute.Key = key;
            customerAttribute.Value = value;

            context.CustomerAttributes.AddObject(customerAttribute);
            context.SaveChanges();

            return customerAttribute;
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="key">An attribute key</param>
        /// <param name="value">An attribute value</param>
        /// <returns>A customer attribute</returns>
        public static CustomerAttribute UpdateCustomerAttribute(int customerAttributeId,
            int customerId, string key, string value)
        {
            if (customerId == 0)
                throw new NopException("Cannot update attribute for non existing customer");

            if (value == null)
                value = string.Empty;

            key = CommonHelper.EnsureMaximumLength(key, 100);
            value = CommonHelper.EnsureMaximumLength(value, 1000);

            var customerAttribute = GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerAttribute))
                context.CustomerAttributes.Attach(customerAttribute);

            customerAttribute.CustomerId = customerId;
            customerAttribute.Key = key;
            customerAttribute.Value = value;
            context.SaveChanges();

            return customerAttribute;
        }

        /// <summary>
        /// Marks customer role as deleted
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        public static void MarkCustomerRoleAsDeleted(int customerRoleId)
        {
            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole != null)
            {
                customerRole = UpdateCustomerRole(customerRole.CustomerRoleId,
                    customerRole.Name, customerRole.FreeShipping,
                    customerRole.TaxExempt, customerRole.Active, true);
            }

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        public static CustomerRole GetCustomerRoleById(int customerRoleId)
        {
            if (customerRoleId == 0)
                return null;

            string key = string.Format(CUSTOMERROLES_BY_ID_KEY, customerRoleId);
            object obj2 = NopRequestCache.Get(key);
            if (CustomerManager.CacheEnabled && (obj2 != null))
            {
                return (CustomerRole)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cr in context.CustomerRoles
                        where cr.CustomerRoleId == customerRoleId
                        select cr;
            var customerRole = query.SingleOrDefault();

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.Add(key, customerRole);
            }
            return customerRole;
        }

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <returns>Customer role collection</returns>
        public static List<CustomerRole> GetAllCustomerRoles()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(CUSTOMERROLES_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (CustomerManager.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRole>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cr in context.CustomerRoles
                        orderby cr.Name
                        where (showHidden || cr.Active) && !cr.Deleted
                        select cr;
            var customerRoles = query.ToList();

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.Add(key, customerRoles);
            }
            return customerRoles;
        }

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer role collection</returns>
        public static List<CustomerRole> GetCustomerRolesByCustomerId(int customerId)
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
        public static List<CustomerRole> GetCustomerRolesByCustomerId(int customerId, bool showHidden)
        {
            if (customerId == 0)
                return new List<CustomerRole>();

            var context = ObjectContextHelper.CurrentObjectContext;

            var query = from cr in context.CustomerRoles
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
        /// <param name="name">The customer role name</param>
        /// <param name="freeShipping">A value indicating whether the customer role is marked as free shiping</param>
        /// <param name="taxExempt">A value indicating whether the customer role is marked as tax exempt</param>
        /// <param name="active">A value indicating whether the customer role is active</param>
        /// <param name="deleted">A value indicating whether the customer role has been deleted</param>
        /// <returns>Customer role</returns>
        public static CustomerRole InsertCustomerRole(string name,
            bool freeShipping, bool taxExempt, bool active, bool deleted)
        {
            name = CommonHelper.EnsureMaximumLength(name, 255);

            var context = ObjectContextHelper.CurrentObjectContext;

            var customerRole = context.CustomerRoles.CreateObject();
            customerRole.Name = name;
            customerRole.FreeShipping = freeShipping;
            customerRole.TaxExempt = taxExempt;
            customerRole.Active = active;
            customerRole.Deleted = deleted;

            context.CustomerRoles.AddObject(customerRole);
            context.SaveChanges();

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }

            return customerRole;
        }

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRoleId">The customer role identifier</param>
        /// <param name="name">The customer role name</param>
        /// <param name="freeShipping">A value indicating whether the customer role is marked as free shiping</param>
        /// <param name="taxExempt">A value indicating whether the customer role is marked as tax exempt</param>
        /// <param name="active">A value indicating whether the customer role is active</param>
        /// <param name="deleted">A value indicating whether the customer role has been deleted</param>
        /// <returns>Customer role</returns>
        public static CustomerRole UpdateCustomerRole(int customerRoleId, string name,
            bool freeShipping, bool taxExempt, bool active, bool deleted)
        {
            name = CommonHelper.EnsureMaximumLength(name, 255);

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerRole))
                context.CustomerRoles.Attach(customerRole);
            
            customerRole.Name = name;
            customerRole.FreeShipping = freeShipping;
            customerRole.TaxExempt = taxExempt;
            customerRole.Active = active;
            customerRole.Deleted = deleted;
            context.SaveChanges();

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
            }

            return customerRole;
        }

        /// <summary>
        /// Adds a customer to role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        public static void AddCustomerToRole(int customerId, int customerRoleId)
        {
            var customer = GetCustomerById(customerId);
            if (customer == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customer))
                context.Customers.Attach(customer);
            if (!context.IsAttached(customerRole))
                context.CustomerRoles.Attach(customerRole);

            customer.NpCustomerRoles.Add(customerRole);
            context.SaveChanges();
        }

        /// <summary>
        /// Removes a customer from role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        public static void RemoveCustomerFromRole(int customerId, int customerRoleId)
        {
            var customer = GetCustomerById(customerId);
            if (customer == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customer))
                context.Customers.Attach(customer);
            if (!context.IsAttached(customerRole))
                context.CustomerRoles.Attach(customerRole);

            customer.NpCustomerRoles.Remove(customerRole);
            context.SaveChanges();
        }

        /// <summary>
        /// Adds a discount to a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void AddDiscountToCustomerRole(int customerRoleId, int discountId)
        {
            var discount = DiscountManager.GetDiscountById(discountId);
            if (discount == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);
            if (!context.IsAttached(customerRole))
                context.CustomerRoles.Attach(customerRole);

            discount.NpCustomerRoles.Add(customerRole);
            context.SaveChanges();
        }

        /// <summary>
        /// Removes a discount from a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        public static void RemoveDiscountFromCustomerRole(int customerRoleId, int discountId)
        {
            var discount = DiscountManager.GetDiscountById(discountId);
            if (discount == null)
                return;

            var customerRole = GetCustomerRoleById(customerRoleId);
            if (customerRole == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(discount))
                context.Discounts.Attach(discount);
            if (!context.IsAttached(customerRole))
                context.CustomerRoles.Attach(customerRole);

            discount.NpCustomerRoles.Remove(customerRole);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a customer roles assigned to discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Customer roles</returns>
        public static List<CustomerRole> GetCustomerRolesByDiscountId(int discountId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(CUSTOMERROLES_BY_DISCOUNTID_KEY, discountId, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (CustomerManager.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRole>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cr in context.CustomerRoles
                        from d in cr.NpDiscounts
                        where (showHidden || cr.Active) &&
                            !cr.Deleted &&
                            d.DiscountId == discountId
                        orderby cr.Name
                        select cr;
            var customerRoles = query.ToList();

            if (CustomerManager.CacheEnabled)
            {
                NopRequestCache.Add(key, customerRoles);
            }
            return customerRoles;
        }

        /// <summary>
        /// Gets a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <returns>Customer session</returns>
        public static CustomerSession GetCustomerSessionByGuid(Guid customerSessionGuid)
        {
            if (customerSessionGuid == Guid.Empty)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cs in context.CustomerSessions
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
        public static CustomerSession GetCustomerSessionByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cs in context.CustomerSessions
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
        public static void DeleteCustomerSession(Guid customerSessionGuid)
        {
            var customerSession = GetCustomerSessionByGuid(customerSessionGuid);
            if (customerSession == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerSession))
                context.CustomerSessions.Attach(customerSession);
            context.DeleteObject(customerSession);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all customer sessions
        /// </summary>
        /// <returns>Customer session collection</returns>
        public static List<CustomerSession> GetAllCustomerSessions()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cs in context.CustomerSessions
                        orderby cs.LastAccessed descending
                        select cs;
            var customerSessions = query.ToList();
            return customerSessions;
        }

        /// <summary>
        /// Gets all customer sessions with non empty shopping cart
        /// </summary>
        /// <returns>Customer session collection</returns>
        public static List<CustomerSession> GetAllCustomerSessionsWithNonEmptyShoppingCart()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            return context.Sp_CustomerSessionLoadNonEmpty();
        }

        /// <summary>
        /// Deletes all expired customer sessions
        /// </summary>
        /// <param name="olderThan">Older than date and time</param>
        public static void DeleteExpiredCustomerSessions(DateTime olderThan)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_CustomerSessionDeleteExpired(olderThan);
        }

        /// <summary>
        /// Saves a customer session to the data storage if it exists or creates new one
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessed">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        public static CustomerSession SaveCustomerSession(Guid customerSessionGuid,
            int customerId, DateTime lastAccessed, bool isExpired)
        {
            if (GetCustomerSessionByGuid(customerSessionGuid) == null)
                return InsertCustomerSession(customerSessionGuid, customerId, lastAccessed, isExpired);
            else
                return UpdateCustomerSession(customerSessionGuid, customerId, lastAccessed, isExpired);
        }

        /// <summary>
        /// Inserts a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessed">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        protected static CustomerSession InsertCustomerSession(Guid customerSessionGuid,
            int customerId, DateTime lastAccessed, bool isExpired)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var customerSession = context.CustomerSessions.CreateObject();
            customerSession.CustomerSessionGuid = customerSessionGuid;
            customerSession.CustomerId = customerId;
            customerSession.LastAccessed = lastAccessed;
            customerSession.IsExpired = isExpired;

            context.CustomerSessions.AddObject(customerSession);
            context.SaveChanges();
            return customerSession;
        }

        /// <summary>
        /// Updates the customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessed">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        protected static CustomerSession UpdateCustomerSession(Guid customerSessionGuid,
            int customerId, DateTime lastAccessed, bool isExpired)
        {
            var customerSession = GetCustomerSessionByGuid(customerSessionGuid);
            if (customerSession == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerSession))
                context.CustomerSessions.Attach(customerSession);

            customerSession.CustomerSessionGuid = customerSessionGuid;
            customerSession.CustomerId = customerId;
            customerSession.LastAccessed = lastAccessed;
            customerSession.IsExpired = isExpired;
            context.SaveChanges();

            return customerSession;
        }

        /// <summary>
        /// Formats customer name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Name</returns>
        public static string FormatUserName(Customer customer)
        {
            return FormatUserName(customer, false);
        }


        /// <summary>
        /// Formats customer name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <returns>Name</returns>
        public static string FormatUserName(Customer customer, bool stripTooLong)
        {
            if (customer == null)
                return string.Empty;

            if (customer.IsGuest)
            {
                return LocalizationManager.GetLocaleResourceString("Customer.Guest");
            }

            string result = string.Empty;
            switch (CustomerManager.CustomerNameFormatting)
            {
                case CustomerNameFormatEnum.ShowEmails:
                    result = customer.Email;
                    break;
                case CustomerNameFormatEnum.ShowFullNames:
                    result = customer.FullName;
                    break;
                case CustomerNameFormatEnum.ShowUsernames:
                    result = customer.Username;
                    break;
                default:
                    break;
            }

            if (stripTooLong)
            {
                int maxLength = SettingManager.GetSettingValueInteger("Customer.FormatNameMaxLength", 0);
                if (maxLength > 0 && result.Length > maxLength)
                {
                    result = result.Substring(0, maxLength);
                }
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.CustomerManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        public static bool AnonymousCheckoutAllowed
        {
            get
            {
                bool allowed = SettingManager.GetSettingValueBoolean("Checkout.AnonymousCheckoutAllowed", false);
                return allowed;
            }
            set
            {
                SettingManager.SetParam("Checkout.AnonymousCheckoutAllowed", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public static bool UsernamesEnabled
        {
            get
            {
                bool usernamesEnabled = SettingManager.GetSettingValueBoolean("Customer.UsernamesEnabled");
                return usernamesEnabled;
            }
            set
            {
                SettingManager.SetParam("Customer.UsernamesEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public static CustomerNameFormatEnum CustomerNameFormatting
        {
            get
            {
                int customerNameFormatting = SettingManager.GetSettingValueInteger("Customer.CustomerNameFormatting");
                return (CustomerNameFormatEnum)customerNameFormatting;
            }
            set
            {
                SettingManager.SetParam("Customer.CustomerNameFormatting", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload avatars.
        /// </summary>
        public static bool AllowCustomersToUploadAvatars
        {
            get
            {
                bool allowCustomersToUploadAvatars = SettingManager.GetSettingValueBoolean("Customer.CustomersAllowedToUploadAvatars");
                return allowCustomersToUploadAvatars;
            }
            set
            {
                SettingManager.SetParam("Customer.CustomersAllowedToUploadAvatars", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display default user avatar.
        /// </summary>
        public static bool DefaultAvatarEnabled
        {
            get
            {
                bool defaultAvatarEnabled = SettingManager.GetSettingValueBoolean("Customer.DefaultAvatarEnabled");
                return defaultAvatarEnabled;
            }
            set
            {
                SettingManager.SetParam("Customer.DefaultAvatarEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown
        /// </summary>
        public static bool ShowCustomersLocation
        {
            get
            {
                bool showCustomersLocation = SettingManager.GetSettingValueBoolean("Customer.ShowCustomersLocation");
                return showCustomersLocation;
            }
            set
            {
                SettingManager.SetParam("Customer.ShowCustomersLocation", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date
        /// </summary>
        public static bool ShowCustomersJoinDate
        {
            get
            {
                bool showCustomersJoinDate = SettingManager.GetSettingValueBoolean("Customer.ShowCustomersJoinDate");
                return showCustomersJoinDate;
            }
            set
            {
                SettingManager.SetParam("Customer.ShowCustomersJoinDate", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to view profiles of other customers
        /// </summary>
        public static bool AllowViewingProfiles
        {
            get
            {
                bool allowViewingProfiles = SettingManager.GetSettingValueBoolean("Customer.AllowViewingProfiles");
                return allowViewingProfiles;
            }
            set
            {
                SettingManager.SetParam("Customer.AllowViewingProfiles", value.ToString());
            }
        }

        /// <summary>
        /// Tax display type
        /// </summary>
        public static CustomerRegistrationTypeEnum CustomerRegistrationType
        {
            get
            {
                int customerRegistrationType = SettingManager.GetSettingValueInteger("Common.CustomerRegistrationType", (int)CustomerRegistrationTypeEnum.Standard);
                return (CustomerRegistrationTypeEnum)customerRegistrationType;
            }
            set
            {
                SettingManager.SetParam("Common.CustomerRegistrationType", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow navigation only for registered users.
        /// </summary>
        public static bool AllowNavigationOnlyRegisteredCustomers
        {
            get
            {
                bool allowOnlyRegisteredCustomers = SettingManager.GetSettingValueBoolean("Common.AllowNavigationOnlyRegisteredCustomers");
                return allowOnlyRegisteredCustomers;
            }
            set
            {
                SettingManager.SetParam("Common.AllowNavigationOnlyRegisteredCustomers", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether product reviews must be approved by administrator.
        /// </summary>
        public static bool ProductReviewsMustBeApproved
        {
            get
            {
                bool productReviewsMustBeApproved = SettingManager.GetSettingValueBoolean("Common.ProductReviewsMustBeApproved");
                return productReviewsMustBeApproved;
            }
            set
            {
                SettingManager.SetParam("Common.ProductReviewsMustBeApproved", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        public static bool AllowAnonymousUsersToReviewProduct
        {
            get
            {
                bool allowAnonymousUsersToReviewProduct = SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToReviewProduct");
                return allowAnonymousUsersToReviewProduct;
            }
            set
            {
                SettingManager.SetParam("Common.AllowAnonymousUsersToReviewProduct", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public static bool AllowAnonymousUsersToEmailAFriend
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToEmailAFriend", false);
            }
            set
            {
                SettingManager.SetParam("Common.AllowAnonymousUsersToEmailAFriend", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to set product ratings.
        /// </summary>
        public static bool AllowAnonymousUsersToSetProductRatings
        {
            get
            {
                bool allowAnonymousUsersToSetProductRatings = SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToSetProductRatings");
                return allowAnonymousUsersToSetProductRatings;
            }
            set
            {
                SettingManager.SetParam("Common.AllowAnonymousUsersToSetProductRatings", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Gender' is enabled
        /// </summary>
        public static bool FormFieldGenderEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.GenderEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.GenderEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is enabled
        /// </summary>
        public static bool FormFieldDateOfBirthEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.DateOfBirthEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.DateOfBirthEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is enabled
        /// </summary>
        public static bool FormFieldCompanyEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.CompanyEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.CompanyEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is required
        /// </summary>
        public static bool FormFieldCompanyRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.CompanyRequired", false);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.CompanyRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is enabled
        /// </summary>
        public static bool FormFieldStreetAddressEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.StreetAddressEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.StreetAddressEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is required
        /// </summary>
        public static bool FormFieldStreetAddressRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.StreetAddressRequired", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.StreetAddressRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is enabled
        /// </summary>
        public static bool FormFieldStreetAddress2Enabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.StreetAddress2Enabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.StreetAddress2Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is required
        /// </summary>
        public static bool FormFieldStreetAddress2Required
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.StreetAddress2Required", false);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.StreetAddress2Required", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is enabled
        /// </summary>
        public static bool FormFieldPostCodeEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.PostCodeEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.PostCodeEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is required
        /// </summary>
        public static bool FormFieldPostCodeRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.PostCodeRequired", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.PostCodeRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is enabled
        /// </summary>
        public static bool FormFieldCityEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.CityEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.CityEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is required
        /// </summary>
        public static bool FormFieldCityRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.CityRequired", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.CityRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Country' is enabled
        /// </summary>
        public static bool FormFieldCountryEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.CountryEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.CountryEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'State' is enabled
        /// </summary>
        public static bool FormFieldStateEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.StateEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.StateEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is enabled
        /// </summary>
        public static bool FormFieldPhoneEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.PhoneEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.PhoneEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is required
        /// </summary>
        public static bool FormFieldPhoneRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.PhoneRequired", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.PhoneRequired", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is enabled
        /// </summary>
        public static bool FormFieldFaxEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.FaxEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.FaxEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is required
        /// </summary>
        public static bool FormFieldFaxRequired
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.FaxRequired", false);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.FaxRequired", value.ToString());
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' is enabled
        /// </summary>
        public static bool FormFieldNewsletterEnabled
        {
            get
            {
                bool setting = SettingManager.GetSettingValueBoolean("FormField.NewsletterEnabled", true);
                return setting;
            }
            set
            {
                SettingManager.SetParam("FormField.NewsletterEnabled", value.ToString());
            }
        }
        #endregion
    }
}