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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Customer manager interface
    /// </summary>
    public partial interface ICustomerManager
    {
        /// <summary>
        /// Deletes an address by address identifier 
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        void DeleteAddress(int addressId);

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        Address GetAddressById(int addressId);

        /// <summary>
        /// Gets a collection of addresses by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="getBillingAddresses">Gets or sets a value indicating whether the addresses are billing or shipping</param>
        /// <returns>A collection of addresses</returns>
        List<Address> GetAddressesByCustomerId(int customerId, bool getBillingAddresses);

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        void InsertAddress(Address address);

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        void UpdateAddress(Address address);

        /// <summary>
        /// Gets a value indicating whether address can be used as billing address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        bool CanUseAddressAsBillingAddress(Address address);

        /// <summary>
        /// Gets a value indicating whether address can be used as shipping address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        bool CanUseAddressAsShippingAddress(Address address);

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        void ResetCheckoutData(int customerId, bool clearCouponCodes);

        /// <summary>
        /// Sets a customer email
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newEmail">New email</param>
        /// <returns>Customer</returns>
        Customer SetEmail(int customerId, string newEmail);

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>Customer</returns>
        Customer ChangeCustomerUsername(int customerId, string newUsername);

        /// <summary>
        /// Sets a customer sugnature
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="signature">Signature</param>
        /// <returns>Customer</returns>
        Customer SetCustomerSignature(int customerId, string signature);

        /// <summary>
        /// Create anonymous user for current user
        /// </summary>
        /// <returns>Guest user</returns>
        void CreateAnonymousUser();

        /// <summary>
        /// Applies a discount coupon code to a current customer
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        void ApplyDiscountCouponCode(string couponCode);

        /// <summary>
        /// Applies a discount coupon code
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>Customer</returns>
        Customer ApplyDiscountCouponCode(int customerId, string couponCode);

        /// <summary>
        /// Applies a gift card coupon code to a current customer
        /// </summary>
        /// <param name="couponCodesXml">Coupon code (XML)</param>
        void ApplyGiftCardCouponCode(string couponCodesXml);

        /// <summary>
        /// Applies a gift card coupon code
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="couponCodesXml">Coupon code (XML)</param>
        /// <returns>Customer</returns>
        Customer ApplyGiftCardCouponCode(int customerId, string couponCodesXml);

        /// <summary>
        /// Applies selected checkout attibutes to a current customer
        /// </summary>
        /// <param name="attributesXml">Checkout attibutes (XML)</param>
        void ApplyCheckoutAttributes(string attributesXml);

        /// <summary>
        /// Applies selected checkout attibutes to a current customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="attributesXml">Checkout attibutes (XML)</param>
        /// <returns>Customer</returns>
        Customer ApplyCheckoutAttributes(int customerId, string attributesXml);

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>Customer collection</returns>
        List<Customer> GetAllCustomers();

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
        List<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int pageSize, int pageIndex, out int totalRecords);

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
        List<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int dateOfBirthMonth, int dateOfBirthDay,
            int pageSize, int pageIndex, out int totalRecords);

        /// <summary>
        /// Gets all customers by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Customer collection</returns>
        List<Customer> GetAffiliatedCustomers(int affiliateId);

        /// <summary>
        /// Gets all customers by customer role id
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer collection</returns>
        List<Customer> GetCustomersByCustomerRoleId(int customerRoleId);

        /// <summary>
        /// Marks customer as deleted
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        void MarkCustomerAsDeleted(int customerId);

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="email">Customer Email</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByEmail(string email);

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="username">Customer username</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByUsername(string username);

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        Customer GetCustomerById(int customerId);

        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByGuid(Guid customerGuid);

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
        Customer AddCustomer(string email, string username, string password,
            bool isAdmin, bool isGuest, bool active, out MembershipCreateStatus status);

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
        Customer AddCustomer(string email, string username, string password,
            int affiliateId, bool isAdmin, bool isGuest, bool active,
            out MembershipCreateStatus status);

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
        Customer AddCustomer(Guid customerGuid, string email, string username,
            string password, int affiliateId, int billingAddressId,
            int shippingAddressId, int lastPaymentMethodId,
            string lastAppliedCouponCode, string giftCardCouponCodes,
            string checkoutAttributes, int languageId, int currencyId,
            TaxDisplayTypeEnum taxDisplayType, bool isTaxExempt, bool isAdmin, bool isGuest,
            bool isForumModerator, int totalForumPosts, string signature, string adminComment,
            bool active, bool deleted, DateTime registrationDate,
            string timeZoneId, int avatarId, DateTime? dateOfBirth, out MembershipCreateStatus status);

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
        Customer AddCustomerForced(Guid customerGuid, string email,
            string username, string passwordHash, string saltKey,
            int affiliateId, int billingAddressId,
            int shippingAddressId, int lastPaymentMethodId,
            string lastAppliedCouponCode, string giftCardCouponCodes,
            string checkoutAttributes, int languageId,
            int currencyId, TaxDisplayTypeEnum taxDisplayType, bool isTaxExempt,
            bool isAdmin, bool isGuest, bool isForumModerator,
            int totalForumPosts, string signature, string adminComment,
            bool active, bool deleted, DateTime registrationDate, string timeZoneId,
            int avatarId, DateTime? dateOfBirth);

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        void UpdateCustomer(Customer customer);

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <param name="oldPassword">Old password</param>
        /// <param name="password">Password</param>
        void ModifyPassword(string email, string oldPassword, string password);

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <param name="newPassword">New password</param>
        void ModifyPassword(string email, string newPassword);

        /// <summary>
        /// Modifies password
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="newPassword">New password</param>
        void ModifyPassword(int customerId, string newPassword);

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        void Activate(Guid customerGuid);

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        void Activate(int customerId);

        /// <summary>
        /// Activates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="sendCustomerWelcomeMessage">A value indivating whether to send customer welcome message</param>
        void Activate(int customerId, bool sendCustomerWelcomeMessage);

        /// <summary>
        /// Deactivates a customer
        /// </summary>
        /// <param name="customerGuid">Customer identifier</param>
        void Deactivate(Guid customerGuid);

        /// <summary>
        /// Deactivates a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        void Deactivate(int customerId);

        /// <summary>
        /// Login a customer
        /// </summary>
        /// <param name="email">A customer email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        bool Login(string email, string password);

        /// <summary>
        /// Logout customer
        /// </summary>
        void Logout();
        
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
        List<CustomerBestReportLine> GetBestCustomersReport(DateTime? startTime,
            DateTime? endTime, OrderStatusEnum? os, PaymentStatusEnum? ps,
            ShippingStatusEnum? ss, int orderBy);

        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Int</returns>
        int GetRegisteredCustomersReport(int days);

        /// <summary>
        /// Get customer report by language
        /// </summary>
        /// <returns>Report</returns>
        List<CustomerReportByLanguageLine> GetCustomerReportByLanguage();

        /// <summary>
        /// Get customer report by attribute key
        /// </summary>
        /// <param name="customerAttributeKey">Customer attribute key</param>
        /// <returns>Report</returns>
        List<CustomerReportByAttributeKeyLine> GetCustomerReportByAttributeKey(string customerAttributeKey);

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        void DeleteCustomerAttribute(int customerAttributeId);

        /// <summary>
        /// Gets a customer attribute
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>A customer attribute</returns>
        CustomerAttribute GetCustomerAttributeById(int customerAttributeId);

        /// <summary>
        /// Gets a collection of customer attributes by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer attributes</returns>
        List<CustomerAttribute> GetCustomerAttributesByCustomerId(int customerId);

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        void InsertCustomerAttribute(CustomerAttribute customerAttribute);

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        void UpdateCustomerAttribute(CustomerAttribute customerAttribute);

        /// <summary>
        /// Marks customer role as deleted
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        void MarkCustomerRoleAsDeleted(int customerRoleId);

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        CustomerRole GetCustomerRoleById(int customerRoleId);

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <returns>Customer role collection</returns>
        List<CustomerRole> GetAllCustomerRoles();

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer role collection</returns>
        List<CustomerRole> GetCustomerRolesByCustomerId(int customerId);

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer role collection</returns>
        List<CustomerRole> GetCustomerRolesByCustomerId(int customerId, bool showHidden);

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        void InsertCustomerRole(CustomerRole customerRole);

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        void UpdateCustomerRole(CustomerRole customerRole);

        /// <summary>
        /// Adds a customer to role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        void AddCustomerToRole(int customerId, int customerRoleId);

        /// <summary>
        /// Removes a customer from role
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        void RemoveCustomerFromRole(int customerId, int customerRoleId);

        /// <summary>
        /// Adds a discount to a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void AddDiscountToCustomerRole(int customerRoleId, int discountId);

        /// <summary>
        /// Removes a discount from a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void RemoveDiscountFromCustomerRole(int customerRoleId, int discountId);

        /// <summary>
        /// Gets a customer roles assigned to discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Customer roles</returns>
        List<CustomerRole> GetCustomerRolesByDiscountId(int discountId);

        /// <summary>
        /// Gets a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <returns>Customer session</returns>
        CustomerSession GetCustomerSessionByGuid(Guid customerSessionGuid);

        /// <summary>
        /// Gets a customer session by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer session</returns>
        CustomerSession GetCustomerSessionByCustomerId(int customerId);

        /// <summary>
        /// Deletes a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        void DeleteCustomerSession(Guid customerSessionGuid);

        /// <summary>
        /// Gets all customer sessions
        /// </summary>
        /// <returns>Customer session collection</returns>
        List<CustomerSession> GetAllCustomerSessions();

        /// <summary>
        /// Gets all customer sessions with non empty shopping cart
        /// </summary>
        /// <returns>Customer session collection</returns>
        List<CustomerSession> GetAllCustomerSessionsWithNonEmptyShoppingCart();

        /// <summary>
        /// Deletes all expired customer sessions
        /// </summary>
        /// <param name="olderThan">Older than date and time</param>
        void DeleteExpiredCustomerSessions(DateTime olderThan);

        /// <summary>
        /// Saves a customer session to the data storage if it exists or creates new one
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessed">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        CustomerSession SaveCustomerSession(Guid customerSessionGuid,
            int customerId, DateTime lastAccessed, bool isExpired);

        /// <summary>
        /// Formats customer name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Name</returns>
        string FormatUserName(Customer customer);

        /// <summary>
        /// Formats customer name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <returns>Name</returns>
        string FormatUserName(Customer customer, bool stripTooLong);

        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        bool AnonymousCheckoutAllowed {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        bool UsernamesEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change their usernames
        /// </summary>
        bool AllowCustomersToChangeUsernames {get;set;}

        /// <summary>
        /// Customer name formatting
        /// </summary>
        CustomerNameFormatEnum CustomerNameFormatting {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload avatars.
        /// </summary>
        bool AllowCustomersToUploadAvatars {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to display default user avatar.
        /// </summary>
        bool DefaultAvatarEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown
        /// </summary>
        bool ShowCustomersLocation {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date
        /// </summary>
        bool ShowCustomersJoinDate {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to view profiles of other customers
        /// </summary>
        bool AllowViewingProfiles {get;set;}

        /// <summary>
        /// Tax display type
        /// </summary>
        CustomerRegistrationTypeEnum CustomerRegistrationType {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to allow navigation only for registered users.
        /// </summary>
        bool AllowNavigationOnlyRegisteredCustomers {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether product reviews must be approved by administrator.
        /// </summary>
        bool ProductReviewsMustBeApproved {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        bool AllowAnonymousUsersToReviewProduct {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        bool AllowAnonymousUsersToEmailAFriend {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to set product ratings.
        /// </summary>
        bool AllowAnonymousUsersToSetProductRatings {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'New customer' notification message should be sent to a store owner
        /// </summary>
        bool NotifyNewCustomerRegistration {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Gender' is enabled
        /// </summary>
        bool FormFieldGenderEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is enabled
        /// </summary>
        bool FormFieldDateOfBirthEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is enabled
        /// </summary>
        bool FormFieldCompanyEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is required
        /// </summary>
        bool FormFieldCompanyRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is enabled
        /// </summary>
        bool FormFieldStreetAddressEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address' is required
        /// </summary>
        bool FormFieldStreetAddressRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is enabled
        /// </summary>
        bool FormFieldStreetAddress2Enabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Street Address 2' is required
        /// </summary>
        bool FormFieldStreetAddress2Required {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is enabled
        /// </summary>
        bool FormFieldPostCodeEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Post Code' is required
        /// </summary>
        bool FormFieldPostCodeRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is enabled
        /// </summary>
        bool FormFieldCityEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is required
        /// </summary>
        bool FormFieldCityRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Country' is enabled
        /// </summary>
        bool FormFieldCountryEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'State' is enabled
        /// </summary>
        bool FormFieldStateEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is enabled
        /// </summary>
        bool FormFieldPhoneEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone' is required
        /// </summary>
        bool FormFieldPhoneRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is enabled
        /// </summary>
        bool FormFieldFaxEnabled {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax' is required
        /// </summary>
        bool FormFieldFaxRequired {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' is enabled
        /// </summary>
        bool FormFieldNewsletterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Time Zone' is enabled
        /// </summary>
        bool FormFieldTimeZoneEnabled { get; set; }
    }
}