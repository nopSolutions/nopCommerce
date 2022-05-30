using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Customer activity service
    /// </summary>
    public class CustomerActivityService : ICustomerActivityService
    {
        #region Fields

        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CustomerActivityService(IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateActivityTypeAsync(ActivityLogType activityLogType)
        {
            await _activityLogTypeRepository.UpdateAsync(activityLogType);
        }

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log type items
        /// </returns>
        public virtual async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
        {
            var activityLogTypes = await _activityLogTypeRepository.GetAllAsync(query=>
            {
                return from alt in query
                    orderby alt.Name
                    select alt;
            }, cache => default);

            return activityLogTypes;
        }

        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log type item
        /// </returns>
        public virtual async Task<ActivityLogType> GetActivityTypeByIdAsync(int activityLogTypeId)
        {
            return await _activityLogTypeRepository.GetByIdAsync(activityLogTypeId, cache => default);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">System keyword</param>
        /// <param name="comment">Comment</param>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log item
        /// </returns>
        public virtual async Task<ActivityLog> InsertActivityAsync(string systemKeyword, string comment, BaseEntity entity = null)
        {
            return await InsertActivityAsync(await _workContext.GetCurrentCustomerAsync(), systemKeyword, comment, entity);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="systemKeyword">System keyword</param>
        /// <param name="comment">Comment</param>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log item
        /// </returns>
        public virtual async Task<ActivityLog> InsertActivityAsync(Customer customer, string systemKeyword, string comment, BaseEntity entity = null)
        {
            if (customer == null)
                return null;

            //try to get activity log type by passed system keyword
            var activityLogType = (await GetAllActivityTypesAsync()).FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword));
            if (!activityLogType?.Enabled ?? true)
                return null;

            //insert log item
            var logItem = new ActivityLog
            {
                ActivityLogTypeId = activityLogType.Id,
                EntityId = entity?.Id,
                EntityName = entity?.GetType().Name,
                CustomerId = customer.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
                CreatedOnUtc = DateTime.UtcNow,
                IpAddress = _webHelper.GetCurrentIpAddress()
            };
            await _activityLogRepository.InsertAsync(logItem);

            return logItem;
        }

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteActivityAsync(ActivityLog activityLog)
        {
            await _activityLogRepository.DeleteAsync(activityLog);
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; pass null to load all records</param>
        /// <param name="createdOnTo">Log item creation to; pass null to load all records</param>
        /// <param name="customerId">Customer identifier; pass null to load all records</param>
        /// <param name="activityLogTypeId">Activity log type identifier; pass null to load all records</param>
        /// <param name="ipAddress">IP address; pass null or empty to load all records</param>
        /// <param name="entityName">Entity name; pass null to load all records</param>
        /// <param name="entityId">Entity identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log items
        /// </returns>
        public virtual async Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
            int? customerId = null, int? activityLogTypeId = null, string ipAddress = null, string entityName = null, int? entityId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _activityLogRepository.GetAllPagedAsync(query =>
            {
                //filter by IP
                if (!string.IsNullOrEmpty(ipAddress))
                    query = query.Where(logItem => logItem.IpAddress.Contains(ipAddress));

                //filter by creation date
                if (createdOnFrom.HasValue)
                    query = query.Where(logItem => createdOnFrom.Value <= logItem.CreatedOnUtc);
                if (createdOnTo.HasValue)
                    query = query.Where(logItem => createdOnTo.Value >= logItem.CreatedOnUtc);

                //filter by log type
                if (activityLogTypeId.HasValue && activityLogTypeId.Value > 0)
                    query = query.Where(logItem => activityLogTypeId == logItem.ActivityLogTypeId);

                //filter by customer
                if (customerId.HasValue && customerId.Value > 0)
                    query = query.Where(logItem => customerId.Value == logItem.CustomerId);

                //filter by entity
                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(logItem => logItem.EntityName.Equals(entityName));
                if (entityId.HasValue && entityId.Value > 0)
                    query = query.Where(logItem => entityId.Value == logItem.EntityId);

                query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenBy(logItem => logItem.Id);

                return query;
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log item
        /// </returns>
        public virtual async Task<ActivityLog> GetActivityByIdAsync(int activityLogId)
        {
            return await _activityLogRepository.GetByIdAsync(activityLogId);
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearAllActivitiesAsync()
        {
            await _activityLogRepository.TruncateAsync();
        }

        #endregion
    }
}