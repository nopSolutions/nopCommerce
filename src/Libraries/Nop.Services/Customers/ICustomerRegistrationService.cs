using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer registration interface
    /// </summary>
    public partial interface ICustomerRegistrationService
    {
        /// <summary>
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<CustomerLoginResults> ValidateCustomerAsync(string usernameOrEmail, string password);

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<CustomerRegistrationResult> RegisterCustomerAsync(CustomerRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="customer">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <param name="isPersist">Is remember me</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        Task<IActionResult> SignInCustomerAsync(Customer customer, string returnUrl, bool isPersist = false);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetEmailAsync(Customer customer, string newEmail, bool requireValidation);

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetUsernameAsync(Customer customer, string newUsername);
    }
}