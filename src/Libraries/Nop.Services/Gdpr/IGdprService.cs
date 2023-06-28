using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;

namespace Nop.Services.Gdpr
{
    /// <summary>
    /// Represents the GDPR service interface
    /// </summary>
    public partial interface IGdprService
    {
        #region GDPR consent

        /// <summary>
        /// Get a GDPR consent
        /// </summary>
        /// <param name="gdprConsentId">The GDPR consent identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent
        /// </returns>
        Task<GdprConsent> GetConsentByIdAsync(int gdprConsentId);

        /// <summary>
        /// Get all GDPR consents
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent
        /// </returns>
        Task<IList<GdprConsent>> GetAllConsentsAsync();

        /// <summary>
        /// Insert a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Update the GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Delete a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Gets the latest selected value (a consent is accepted or not by a customer)
        /// </summary>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; null if previous a customer hasn't been asked
        /// </returns>
        Task<bool?> IsConsentAcceptedAsync(int consentId, int customerId);

        #endregion

        #region GDPR log

        /// <summary>
        /// Get all GDPR log records
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="customerInfo">Customer info (Exact match)</param>
        /// <param name="requestType">GDPR request type</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR log records
        /// </returns>
        Task<IPagedList<GdprLog>> GetAllLogAsync(int customerId = 0, int consentId = 0,
            string customerInfo = "", GdprRequestType? requestType = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="requestType">Request type</param>
        /// <param name="requestDetails">Request details</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertLogAsync(Customer customer, int consentId, GdprRequestType requestType, string requestDetails);

        #endregion

        #region Customer

        /// <summary>
        /// Permanent delete of customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PermanentDeleteCustomerAsync(Customer customer);

        #endregion
    }
}
