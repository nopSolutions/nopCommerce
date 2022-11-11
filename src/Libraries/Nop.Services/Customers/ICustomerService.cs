using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface ICustomerService
    {
        #region Customers

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
        /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="email">Email; null to load all customers</param>
        /// <param name="username">Username; null to load all customers</param>
        /// <param name="firstName">First name; null to load all customers</param>
        /// <param name="lastName">Last name; null to load all customers</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all customers</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all customers</param>
        /// <param name="company">Company; null to load all customers</param>
        /// <param name="phone">Phone; null to load all customers</param>
        /// <param name="zipPostalCode">Phone; null to load all customers</param>
        /// <param name="ipAddress">IP address; null to load all customers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        Task<IPagedList<Customer>> GetAllCustomersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] customerRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets online customers
        /// </summary>
        /// <param name="lastActivityFromUtc">Customer last activity date (from)</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        Task<IPagedList<Customer>> GetOnlineCustomersAsync(DateTime lastActivityFromUtc,
            int[] customerRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets customers with shopping carts
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="productId">Product identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <param name="countryId">Billing country identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        Task<IPagedList<Customer>> GetCustomersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? productId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets customer for shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Customer> GetShoppingCartCustomerAsync(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerAsync(Customer customer);

        /// <summary>
        /// Gets built-in system record used for background tasks
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer object
        /// </returns>
        Task<Customer> GetOrCreateBackgroundTaskUserAsync();

        /// <summary>
        /// Gets built-in system guest record used for requests from search engines
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer object
        /// </returns>
        Task<Customer> GetOrCreateSearchEngineUserAsync();

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer
        /// </returns>
        Task<Customer> GetCustomerByIdAsync(int customerId);

        /// <summary>
        /// Get customers by identifiers
        /// </summary>
        /// <param name="customerIds">Customer identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds);

        /// <summary>
        /// Get customers by guids
        /// </summary>
        /// <param name="customerGuids">Customer guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        Task<IList<Customer>> GetCustomersByGuidsAsync(Guid[] customerGuids);

        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer
        /// </returns>
        Task<Customer> GetCustomerByGuidAsync(Guid customerGuid);

        /// <summary>
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        Task<Customer> GetCustomerByEmailAsync(string email);

        /// <summary>
        /// Get customer by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        Task<Customer> GetCustomerBySystemNameAsync(string systemName);

        /// <summary>
        /// Get customer by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        Task<Customer> GetCustomerByUsernameAsync(string username);

        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        Task<Customer> InsertGuestCustomerAsync();

        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerAsync(Customer customer);

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ResetCheckoutDataAsync(Customer customer, int storeId,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true);

        /// <summary>
        /// Delete guest customer records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete customers only without shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted customers
        /// </returns>
        Task<int> DeleteGuestCustomersAsync(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart);

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<TaxDisplayType?> GetCustomerDefaultTaxDisplayTypeAsync(Customer customer);

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer full name
        /// </returns>
        Task<string> GetCustomerFullNameAsync(Customer customer);

        /// <summary>
        /// Formats the customer name
        /// </summary>
        /// <param name="customer">Source</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <param name="maxLength">Maximum customer name length</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted text
        /// </returns>
        Task<string> FormatUsernameAsync(Customer customer, bool stripTooLong = false, int maxLength = 0);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedDiscountCouponCodesAsync(Customer customer);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyDiscountCouponCodeAsync(Customer customer, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveDiscountCouponCodeAsync(Customer customer, string couponCode);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedGiftCardCouponCodesAsync(Customer customer);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyGiftCardCouponCodeAsync(Customer customer, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveGiftCardCouponCodeAsync(Customer customer, string couponCode);

        /// <summary>
        /// Returns a list of guids of not existing customers
        /// </summary>
        /// <param name="guids">The guids of the customers to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of guids not existing customers
        /// </returns>
        Task<Guid[]> GetNotExistingCustomersAsync(Guid[] guids);

        #endregion

        #region Customer roles

        /// <summary>
        /// Add a customer-customer role mapping
        /// </summary>
        /// <param name="roleMapping">Customer-customer role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddCustomerRoleMappingAsync(CustomerCustomerRoleMapping roleMapping);

        /// <summary>
        /// Remove a customer-customer role mapping
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="role">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveCustomerRoleMappingAsync(Customer customer, CustomerRole role);

        /// <summary>
        /// Delete a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerRoleAsync(CustomerRole customerRole);

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role
        /// </returns>
        Task<CustomerRole> GetCustomerRoleByIdAsync(int customerRoleId);

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="systemName">Customer role system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role
        /// </returns>
        Task<CustomerRole> GetCustomerRoleBySystemNameAsync(string systemName);

        /// <summary>
        /// Get customer role identifiers
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role identifiers
        /// </returns>
        Task<int[]> GetCustomerRoleIdsAsync(Customer customer, bool showHidden = false);

        /// <summary>
        /// Gets list of customer roles
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<CustomerRole>> GetCustomerRolesAsync(Customer customer, bool showHidden = false);

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer roles
        /// </returns>
        Task<IList<CustomerRole>> GetAllCustomerRolesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerRoleAsync(CustomerRole customerRole);

        /// <summary>
        /// Gets a value indicating whether customer is in a certain customer role
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="customerRoleSystemName">Customer role system name</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsInCustomerRoleAsync(Customer customer, string customerRoleSystemName, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Gets a value indicating whether customer is administrator
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsAdminAsync(Customer customer, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Gets a value indicating whether customer is a forum moderator
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsForumModeratorAsync(Customer customer, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Gets a value indicating whether customer is registered
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsRegisteredAsync(Customer customer, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Gets a value indicating whether customer is guest
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsGuestAsync(Customer customer, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Gets a value indicating whether customer is vendor
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsVendorAsync(Customer customer, bool onlyActiveCustomerRoles = true);

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerRoleAsync(CustomerRole customerRole);

        #endregion

        #region Customer passwords

        /// <summary>
        /// Gets customer passwords
        /// </summary>
        /// <param name="customerId">Customer identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of customer passwords
        /// </returns>
        Task<IList<CustomerPassword>> GetCustomerPasswordsAsync(int? customerId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null);

        /// <summary>
        /// Get current customer password
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer password
        /// </returns>
        Task<CustomerPassword> GetCurrentPasswordAsync(int customerId);

        /// <summary>
        /// Insert a customer password
        /// </summary>
        /// <param name="customerPassword">Customer password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerPasswordAsync(CustomerPassword customerPassword);

        /// <summary>
        /// Update a customer password
        /// </summary>
        /// <param name="customerPassword">Customer password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerPasswordAsync(CustomerPassword customerPassword);

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="token">Token to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryTokenValidAsync(Customer customer, string token);

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryLinkExpiredAsync(Customer customer);

        /// <summary>
        /// Check whether customer password is expired 
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if password is expired; otherwise false
        /// </returns>
        Task<bool> IsPasswordExpiredAsync(Customer customer);

        #endregion

        #region Customer address mapping

        /// <summary>
        /// Gets a list of addresses mapped to customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        Task<IList<Address>> GetAddressesByCustomerIdAsync(int customerId);

        /// <summary>
        /// Gets a address mapped to customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetCustomerAddressAsync(int customerId, int addressId);

        /// <summary>
        /// Gets a customer billing address
        /// </summary>
        /// <param name="customer">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetCustomerBillingAddressAsync(Customer customer);

        /// <summary>
        /// Gets a customer shipping address
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetCustomerShippingAddressAsync(Customer customer);

        /// <summary>
        /// Remove a customer-address mapping record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveCustomerAddressAsync(Customer customer, Address address);

        /// <summary>
        /// Inserts a customer-address mapping record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerAddressAsync(Customer customer, Address address);

        #endregion
    }
}