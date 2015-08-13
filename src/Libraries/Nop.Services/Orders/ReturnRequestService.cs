using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service
    /// </summary>
    public partial class ReturnRequestService : IReturnRequestService
    {
        #region Fields

        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="returnRequestRepository">Return request repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ReturnRequestService(IRepository<ReturnRequest> returnRequestRepository,
            IEventPublisher eventPublisher)
        {
            this._returnRequestRepository = returnRequestRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual void DeleteReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            _returnRequestRepository.Delete(returnRequest);

            //event notification
            _eventPublisher.EntityDeleted(returnRequest);
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        public virtual ReturnRequest GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            return _returnRequestRepository.GetById(returnRequestId);
        }

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; null to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Return requests</returns>
        public virtual IPagedList<ReturnRequest> SearchReturnRequests(int storeId = 0, int customerId = 0,
            int orderItemId = 0, ReturnRequestStatus? rs = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
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

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr=>rr.Id);

            var returnRequests = new PagedList<ReturnRequest>(query, pageIndex, pageSize);
            return returnRequests;
        }

        #endregion
    }
}
