using Nop.Core;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service interface
    /// </summary>
    public partial interface IReturnRequestService
    {
        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        void DeleteReturnRequest(ReturnRequest returnRequest);

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        ReturnRequest GetReturnRequestById(int returnRequestId);
        
        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Return requests</returns>
        IPagedList<ReturnRequest> SearchReturnRequests(int storeId = 0, int customerId = 0,
            int orderItemId = 0, ReturnRequestStatus? rs = null, 
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
