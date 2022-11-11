using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Customer activity service interface
    /// </summary>
    public partial interface ICustomerActivityService
    {
        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateActivityTypeAsync(ActivityLogType activityLogType);

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log type items
        /// </returns>
        Task<IList<ActivityLogType>> GetAllActivityTypesAsync();

        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log type item
        /// </returns>
        Task<ActivityLogType> GetActivityTypeByIdAsync(int activityLogTypeId);

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
        Task<ActivityLog> InsertActivityAsync(string systemKeyword, string comment, BaseEntity entity = null);

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
        Task<ActivityLog> InsertActivityAsync(Customer customer, string systemKeyword, string comment, BaseEntity entity = null);

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteActivityAsync(ActivityLog activityLog);

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
        Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
            int? customerId = null, int? activityLogTypeId = null, string ipAddress = null, string entityName = null, int? entityId = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log item
        /// </returns>
        Task<ActivityLog> GetActivityByIdAsync(int activityLogId);

        /// <summary>
        /// Clears activity log
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ClearAllActivitiesAsync();
    }
}
