using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication;

/// <summary>
/// Authentication service interface
/// </summary>
public partial interface IAuthenticationService
{
    /// <summary>
    /// Sign in
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SignInAsync(Customer customer, bool isPersistent);

    /// <summary>
    /// Sign out
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SignOutAsync();

    /// <summary>
    /// Get authenticated customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    Task<Customer> GetAuthenticatedCustomerAsync();
}