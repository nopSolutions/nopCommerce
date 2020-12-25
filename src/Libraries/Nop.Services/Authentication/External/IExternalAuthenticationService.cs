using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// External authentication service
    /// </summary>
    public partial interface IExternalAuthenticationService
    {
        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null);

        /// <summary>
        /// Get the external authentication records by identifier
        /// </summary>
        /// <param name="externalAuthenticationRecordId">External authentication record identifier</param>
        /// <returns>Result</returns>
        Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByIdAsync(int externalAuthenticationRecordId);

        /// <summary>
        /// Get all the external authentication records by customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Customer</returns>
        Task<IList<ExternalAuthenticationRecord>> GetCustomerExternalAuthenticationRecordsAsync(Customer customer);

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        Task DeleteExternalAuthenticationRecordAsync(ExternalAuthenticationRecord externalAuthenticationRecord);
    }
}