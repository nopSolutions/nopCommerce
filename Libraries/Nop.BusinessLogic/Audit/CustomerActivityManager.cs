//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Customer activity manager
    /// </summary>
    public class CustomerActivityManager
    {
        #region Constants
        private const string ACTIVITYTYPE_ALL_KEY = "Nop.activitytype.all";
        private const string ACTIVITYTYPE_BY_ID_KEY = "Nop.activitytype.id-{0}";
        private const string ACTIVITYTYPE_PATTERN_KEY = "Nop.activitytype.";
        #endregion
        
        #region Methods

        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="name">The display name</param>
        /// <param name="enabled">Value indicating whether the activity log type is enabled</param>
        /// <returns>Activity log type item</returns>
        public static ActivityLogType InsertActivityType(string systemKeyword,
            string name, bool enabled)
        {
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 50);
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var activityLogType = context.ActivityLogTypes.CreateObject();
            activityLogType.SystemKeyword = systemKeyword;
            activityLogType.Name = name;
            activityLogType.Enabled = enabled;

            context.ActivityLogTypes.AddObject(activityLogType);
            context.SaveChanges();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);

            return activityLogType;
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="name">The display name</param>
        /// <param name="enabled">Value indicating whether the activity log type is enabled</param>
        /// <returns>Activity log type item</returns>
        public static ActivityLogType UpdateActivityType(int activityLogTypeId,
            string systemKeyword, string name, bool enabled)
        {
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 50);
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var activityLogType = GetActivityTypeById(activityLogTypeId);
            if (activityLogType == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(activityLogType))
                context.ActivityLogTypes.Attach(activityLogType);

            activityLogType.SystemKeyword = systemKeyword;
            activityLogType.Name = name;
            activityLogType.Enabled = enabled;

            context.SaveChanges();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);

            return activityLogType;
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="enabled">Value indicating whether the activity log type is enabled</param>
        /// <returns>Activity log type item</returns>
        public static ActivityLogType UpdateActivityType(int activityLogTypeId, bool enabled)
        {
            var activityType = GetActivityTypeById(activityLogTypeId);
            if (activityType == null || activityType.Enabled == enabled)
                return activityType;

            return UpdateActivityType(activityType.ActivityLogTypeId, 
                activityType.SystemKeyword, activityType.Name, enabled);
        }
        
        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        public static void DeleteActivityType(int activityLogTypeId)
        {
            var activityLogType = GetActivityTypeById(activityLogTypeId);
            if (activityLogType == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(activityLogType))
                context.ActivityLogTypes.Attach(activityLogType);
            context.DeleteObject(activityLogType);
            context.SaveChanges();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
        
        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type collection</returns>
        public static List<ActivityLogType> GetAllActivityTypes()
        {
            if (NopRequestCache.IsEnabled)
            {
                object cache = NopRequestCache.Get(ACTIVITYTYPE_ALL_KEY);
                if (cache != null)
                    return (List<ActivityLogType>)cache;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from at in context.ActivityLogTypes
                        orderby at.Name
                        select at;
            var collection = query.ToList();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.Add(ACTIVITYTYPE_ALL_KEY, collection);
            
            return collection;
        }
        
        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        public static ActivityLogType GetActivityTypeById(int activityLogTypeId)
        {
            if (activityLogTypeId == 0)
                return null;

            string key = string.Format(ACTIVITYTYPE_BY_ID_KEY, activityLogTypeId);
            if (NopRequestCache.IsEnabled)
            {
                object cache = NopRequestCache.Get(key);
                if (cache != null)
                    return (ActivityLogType)cache;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from at in context.ActivityLogTypes
                        where at.ActivityLogTypeId == activityLogTypeId
                        select at;
            var activityLogType = query.SingleOrDefault();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.Add(key, activityLogType);
            
            return activityLogType;
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <returns>Activity log item</returns>
        public static ActivityLog InsertActivity(string systemKeyword, string comment)
        {
            return InsertActivity(systemKeyword, comment, new object[0]);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public static ActivityLog InsertActivity(string systemKeyword, 
            string comment, params object[] commentParams)
        {
            if (NopContext.Current == null || 
                NopContext.Current.User == null ||
                NopContext.Current.User.IsGuest)
                return null;

            var activityTypes = GetAllActivityTypes();
            var activityType = activityTypes.FindBySystemKeyword(systemKeyword);
            if (activityType == null || !activityType.Enabled)
                return null;

            int customerId = NopContext.Current.User.CustomerId;
            DateTime createdOn = DateTime.UtcNow;
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var activity = context.ActivityLog.CreateObject();
            activity.ActivityLogTypeId = activityType.ActivityLogTypeId;
            activity.CustomerId = customerId;
            activity.Comment = comment;
            activity.CreatedOn = createdOn;

            context.ActivityLog.AddObject(activity);
            context.SaveChanges();

            return activity;
        }
        
        /// <summary>
        /// Updates an activity log 
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Activity log item</returns>
        public static ActivityLog UpdateActivity(int activityLogId, int activityLogTypeId,
            int customerId, string comment, DateTime createdOn)
        {
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var activity = GetActivityById(activityLogId);
            if (activity == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(activity))
                context.ActivityLog.Attach(activity);

            activity.ActivityLogTypeId = activityLogTypeId;
            activity.CustomerId = customerId;
            activity.Comment = comment;
            activity.CreatedOn = createdOn;
            context.SaveChanges();
            return activity;
        }
        
        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log type identifier</param>
        public static void DeleteActivity(int activityLogId)
        {
            var activity = GetActivityById(activityLogId);
            if (activity == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(activity))
                context.ActivityLog.Attach(activity);
            context.DeleteObject(activity);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all customers</param>
        /// <param name="createdOnTo">Log item creation to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Activity log collection</returns>
        public static List<ActivityLog> GetAllActivities(DateTime? createdOnFrom,
            DateTime? createdOnTo, string email, string username, int activityLogTypeId,
            int pageSize, int pageIndex, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;
            
            var context = ObjectContextHelper.CurrentObjectContext;
            var activityLog = context.Sp_ActivityLogLoadAll(createdOnFrom,
                createdOnTo, email, username, activityLogTypeId,
                pageSize, pageIndex, out totalRecords).ToList();

            return activityLog;
        }
        
        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        public static ActivityLog GetActivityById(int activityLogId)
        {
            if (activityLogId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from al in context.ActivityLog
                        where al.ActivityLogId == activityLogId
                        select al;
            var activityLog = query.SingleOrDefault();
            return activityLog;
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        public static void ClearAllActivities()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ActivityLogClearAll();
        }
        #endregion
    }
}
