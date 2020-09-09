using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents the tax transaction log service implementation
    /// </summary>
    public class TaxTransactionLogService
    {
        #region Fields

        private readonly IRepository<TaxTransactionLog> _taxTransactionLogRepository;

        #endregion

        #region Ctor

        public TaxTransactionLogService(IRepository<TaxTransactionLog> taxTransactionLogRepository)
        {
            _taxTransactionLogRepository = taxTransactionLogRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get tax transaction log
        /// </summary>
        /// <param name="customerId">Customer identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Log item creation from; pass null to load all records</param>
        /// <param name="createdToUtc">Log item creation to; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged list of tax transaction log items</returns>
        public virtual IPagedList<TaxTransactionLog> GetTaxTransactionLog(int? customerId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //get all logs
            var query = _taxTransactionLogRepository.Table;

            //filter by customer
            if (customerId.HasValue)
                query = query.Where(logItem => logItem.CustomerId == customerId);

            //filter by dates
            if (createdFromUtc.HasValue)
                query = query.Where(logItem => logItem.CreatedDateUtc >= createdFromUtc.Value);
            if (createdToUtc.HasValue)
                query = query.Where(logItem => logItem.CreatedDateUtc <= createdToUtc.Value);

            //order log records
            query = query.OrderByDescending(logItem => logItem.CreatedDateUtc).ThenByDescending(logItem => logItem.Id);

            //return paged log
            return new PagedList<TaxTransactionLog>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Get a log item by the identifier
        /// </summary>
        /// <param name="logItemId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual async Task<TaxTransactionLog> GetTaxTransactionLogById(int logItemId)
        {
            if (logItemId == 0)
                return null;

            return await _taxTransactionLogRepository.GetById(logItemId);
        }

        /// <summary>
        /// Insert the log item
        /// </summary>
        /// <param name="logItem">Log item</param>
        public virtual async Task InsertTaxTransactionLog(TaxTransactionLog logItem)
        {
            if (logItem == null)
                throw new ArgumentNullException(nameof(logItem));

            await _taxTransactionLogRepository.Insert(logItem);
        }

        /// <summary>
        /// Update the log item
        /// </summary>
        /// <param name="logItem">Log item</param>
        public virtual async Task UpdateTaxTransactionLog(TaxTransactionLog logItem)
        {
            if (logItem == null)
                throw new ArgumentNullException(nameof(logItem));

            await _taxTransactionLogRepository.Update(logItem);
        }

        /// <summary>
        /// Delete the log item
        /// </summary>
        /// <param name="logItem">Log item</param>
        public virtual async Task DeleteTaxTransactionLog(TaxTransactionLog logItem)
        {
            if (logItem == null)
                throw new ArgumentNullException(nameof(logItem));

            await _taxTransactionLogRepository.Delete(logItem);
        }

        /// <summary>
        /// Delete log items
        /// </summary>
        /// <param name="ids">Log items identifiers</param>
        public virtual async Task DeleteTaxTransactionLog(int[] ids)
        {
            await _taxTransactionLogRepository.Delete(logItem => ids.Contains(logItem.Id));
        }

        #endregion
    }
}