using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service
    /// </summary>
    public partial class ReturnRequestService : IReturnRequestService
    {
        #region Fields

        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;

        #endregion

        #region Ctor

        public ReturnRequestService(IRepository<ReturnRequest> returnRequestRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository)
        {
            _returnRequestRepository = returnRequestRepository;
            _returnRequestActionRepository = returnRequestActionRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual async Task DeleteReturnRequest(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.Delete(returnRequest);
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        public virtual async Task<ReturnRequest> GetReturnRequestById(int returnRequestId)
        {
            return await _returnRequestRepository.GetById(returnRequestId);
        }

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="customNumber">Custom number; null or empty to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Return requests</returns>
        public virtual async Task<IPagedList<ReturnRequest>> SearchReturnRequests(int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", ReturnRequestStatus? rs = null, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _returnRequestRepository.Table;
            if (storeId > 0)
                query = query.Where(rr => storeId == rr.StoreId);
            if (customerId > 0)
                query = query.Where(rr => customerId == rr.CustomerId);
            if (rs.HasValue)
            {
                var returnStatusId = (int)rs.Value;
                query = query.Where(rr => rr.ReturnRequestStatusId == returnStatusId);
            }

            if (orderItemId > 0)
                query = query.Where(rr => rr.OrderItemId == orderItemId);

            if (!string.IsNullOrEmpty(customNumber))
                query = query.Where(rr => rr.CustomNumber == customNumber);

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = await query.ToPagedList(pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        /// <summary>
        /// Delete a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual async Task DeleteReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.Delete(returnRequestAction);
        }

        /// <summary>
        /// Gets all return request actions
        /// </summary>
        /// <returns>Return request actions</returns>
        public virtual async Task<IList<ReturnRequestAction>> GetAllReturnRequestActions()
        {
            return await _returnRequestActionRepository.GetAll(query =>
            {
                return from rra in query
                    orderby rra.DisplayOrder, rra.Id
                    select rra;
            }, cache => default);
        }

        /// <summary>
        /// Gets a return request action
        /// </summary>
        /// <param name="returnRequestActionId">Return request action identifier</param>
        /// <returns>Return request action</returns>
        public virtual async Task<ReturnRequestAction> GetReturnRequestActionById(int returnRequestActionId)
        {
            return await _returnRequestActionRepository.GetById(returnRequestActionId, cache => default);
        }

        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual async Task InsertReturnRequest(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.Insert(returnRequest);
        }

        /// <summary>
        /// Inserts a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual async Task InsertReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.Insert(returnRequestAction);
        }

        /// <summary>
        /// Updates the return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual async Task UpdateReturnRequest(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.Update(returnRequest);
        }

        /// <summary>
        /// Updates the return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual async Task UpdateReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.Update(returnRequestAction);
        }

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual async Task DeleteReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.Delete(returnRequestReason);
        }

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>Return request reasons</returns>
        public virtual async Task<IList<ReturnRequestReason>> GetAllReturnRequestReasons()
        {
            return await _returnRequestReasonRepository.GetAll(query =>
            {
                return from rra in query
                    orderby rra.DisplayOrder, rra.Id
                    select rra;
            }, cache => default);
        }

        /// <summary>
        /// Gets a return request reason
        /// </summary>
        /// <param name="returnRequestReasonId">Return request reason identifier</param>
        /// <returns>Return request reason</returns>
        public virtual async Task<ReturnRequestReason> GetReturnRequestReasonById(int returnRequestReasonId)
        {
            return await _returnRequestReasonRepository.GetById(returnRequestReasonId, cache => default);
        }

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual async Task InsertReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.Insert(returnRequestReason);
        }

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual async Task UpdateReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.Update(returnRequestReason);
        }

        #endregion
    }
}