using Nop.Data;
using Nop.Plugin.Payments.CyberSource.Domain;

namespace Nop.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents CyberSource customer token service
    /// </summary>
    public class CustomerTokenService
    {
        #region Fields

        protected readonly IRepository<CyberSourceCustomerToken> _customerTokenRepository;

        #endregion

        #region Ctor

        public CustomerTokenService(IRepository<CyberSourceCustomerToken> customerTokenRepository)
        {
            _customerTokenRepository = customerTokenRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all CyberSource customer token records
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="instrumentIdentifier">Instrument identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the CyberSource customer token record
        /// </returns>
        public async Task<IList<CyberSourceCustomerToken>> GetAllTokensAsync(int customerId = 0, string instrumentIdentifier = null)
        {
            return await _customerTokenRepository.GetAllAsync(query =>
            {
                if (customerId > 0)
                    query = query.Where(token => token.CustomerId == customerId);

                if (!string.IsNullOrEmpty(instrumentIdentifier))
                    query = query.Where(token => token.InstrumentIdentifier == instrumentIdentifier);

                return query;
            }, null);
        }

        /// <summary>
        /// Get a CyberSource customer token record by identifier
        /// </summary>
        /// <param name="id">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the CyberSource customer token record
        /// </returns>
        public async Task<CyberSourceCustomerToken> GetByIdAsync(int id)
        {
            return await _customerTokenRepository.GetByIdAsync(id, null);
        }

        /// <summary>
        /// Insert the CyberSource customer token record
        /// </summary>
        /// <param name="cyberSourceCustomerToken">CyberSource customer token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertAsync(CyberSourceCustomerToken cyberSourceCustomerToken)
        {
            await _customerTokenRepository.InsertAsync(cyberSourceCustomerToken, false);
        }

        /// <summary>
        /// Update the CyberSource customer token record
        /// </summary>
        /// <param name="cyberSourceCustomerToken">CyberSource customer token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateAsync(CyberSourceCustomerToken cyberSourceCustomerToken)
        {
            await _customerTokenRepository.UpdateAsync(cyberSourceCustomerToken, false);
        }

        /// <summary>
        /// Delete the CyberSource customer token record
        /// </summary>
        /// <param name="cyberSourceCustomerToken">CyberSource customer token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteAsync(CyberSourceCustomerToken cyberSourceCustomerToken)
        {
            await _customerTokenRepository.DeleteAsync(cyberSourceCustomerToken, false);
        }

        #endregion
    }
}