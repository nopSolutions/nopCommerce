using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
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
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;

        #endregion

        #region Ctor

        public ReturnRequestService(IRepository<ReturnRequest> returnRequestRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository)
        {
            _returnRequestRepository = returnRequestRepository;
            _returnRequestActionRepository = returnRequestActionRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteReturnRequestAsync(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.DeleteAsync(returnRequest);
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request
        /// </returns>
        public virtual async Task<ReturnRequest> GetReturnRequestByIdAsync(int returnRequestId)
        {
            return await _returnRequestRepository.GetByIdAsync(returnRequestId);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return requests
        /// </returns>
        public virtual async Task<IPagedList<ReturnRequest>> SearchReturnRequestsAsync(int storeId = 0, int customerId = 0,
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

            var returnRequests = await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        /// <summary>
        /// Gets the return request availability
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>The <see cref="Task"/> containing the <see cref="ReturnRequestAvailability"/></returns>
        public virtual async Task<ReturnRequestAvailability> GetReturnRequestAvailabilityAsync(int orderId)
        {
            var result = new ReturnRequestAvailability();

            if (orderId > 0)
            {
                var cancelledStatusId = (int)ReturnRequestStatus.Cancelled;
                var requestedOrderItemsForReturn =
                    from rr in _returnRequestRepository.Table
                    where rr.ReturnRequestStatusId != cancelledStatusId
                    group rr by new
                    {
                        rr.OrderItemId,
                        rr.Quantity
                    } into g
                    select new
                    {
                        OrderItemId = g.Key.OrderItemId,
                        RequestedQuantityForReturn = g.Sum(rr => rr.Quantity)
                    };

                var query =
                    from oi in _orderItemRepository.Table
                    join roi in requestedOrderItemsForReturn
                        on oi.Id equals roi.OrderItemId into alreadyRequestedForReturn
                    from aroi in alreadyRequestedForReturn.DefaultIfEmpty()
                    join p in _productRepository.Table
                        on oi.ProductId equals p.Id
                    where !p.NotReturnable && oi.OrderId == orderId
                    select new ReturnableOrderItem
                    {
                        AvailableQuantityForReturn = aroi != null
                            ? Math.Max(oi.Quantity - aroi.RequestedQuantityForReturn, 0)
                            : oi.Quantity,
                        OrderItem = oi
                    };

                result.ReturnableOrderItems = await query.ToListAsync();
            }

            return result;
        }

        /// <summary>
        /// Delete a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.DeleteAsync(returnRequestAction);
        }

        /// <summary>
        /// Gets all return request actions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request actions
        /// </returns>
        public virtual async Task<IList<ReturnRequestAction>> GetAllReturnRequestActionsAsync()
        {
            return await _returnRequestActionRepository.GetAllAsync(query =>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request action
        /// </returns>
        public virtual async Task<ReturnRequestAction> GetReturnRequestActionByIdAsync(int returnRequestActionId)
        {
            return await _returnRequestActionRepository.GetByIdAsync(returnRequestActionId, cache => default);
        }

        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertReturnRequestAsync(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.InsertAsync(returnRequest);
        }

        /// <summary>
        /// Inserts a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.InsertAsync(returnRequestAction);
        }

        /// <summary>
        /// Updates the return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateReturnRequestAsync(ReturnRequest returnRequest)
        {
            await _returnRequestRepository.UpdateAsync(returnRequest);
        }

        /// <summary>
        /// Updates the return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
        {
            await _returnRequestActionRepository.UpdateAsync(returnRequestAction);
        }

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.DeleteAsync(returnRequestReason);
        }

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reasons
        /// </returns>
        public virtual async Task<IList<ReturnRequestReason>> GetAllReturnRequestReasonsAsync()
        {
            return await _returnRequestReasonRepository.GetAllAsync(query =>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reason
        /// </returns>
        public virtual async Task<ReturnRequestReason> GetReturnRequestReasonByIdAsync(int returnRequestReasonId)
        {
            return await _returnRequestReasonRepository.GetByIdAsync(returnRequestReasonId, cache => default);
        }

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.InsertAsync(returnRequestReason);
        }

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.UpdateAsync(returnRequestReason);
        }

        #endregion
    }
}