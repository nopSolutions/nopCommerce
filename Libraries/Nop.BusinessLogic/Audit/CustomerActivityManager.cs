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
    public class CustomerActivityManager : ICustomerActivityManager
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
        /// <param name="activityLogType">Activity log type item</param>
        public void InsertActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            activityLogType.SystemKeyword = CommonHelper.EnsureNotNull(activityLogType.SystemKeyword);
            activityLogType.SystemKeyword = CommonHelper.EnsureMaximumLength(activityLogType.SystemKeyword, 50);
            activityLogType.Name = CommonHelper.EnsureNotNull(activityLogType.Name);            
            activityLogType.Name = CommonHelper.EnsureMaximumLength(activityLogType.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.ActivityLogTypes.AddObject(activityLogType);
            context.SaveChanges();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public void UpdateActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            activityLogType.SystemKeyword = CommonHelper.EnsureNotNull(activityLogType.SystemKeyword);
            activityLogType.SystemKeyword = CommonHelper.EnsureMaximumLength(activityLogType.SystemKeyword, 50);
            activityLogType.Name = CommonHelper.EnsureNotNull(activityLogType.Name);
            activityLogType.Name = CommonHelper.EnsureMaximumLength(activityLogType.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(activityLogType))
                context.ActivityLogTypes.Attach(activityLogType);

            context.SaveChanges();

            if (NopRequestCache.IsEnabled)
                NopRequestCache.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
                
        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        public void DeleteActivityType(int activityLogTypeId)
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
        public List<ActivityLogType> GetAllActivityTypes()
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
        public ActivityLogType GetActivityTypeById(int activityLogTypeId)
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
        public ActivityLog InsertActivity(string systemKeyword, string comment)
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
        public ActivityLog InsertActivity(string systemKeyword, 
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
            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var activity = context.ActivityLog.CreateObject();
            activity.ActivityLogTypeId = activityType.ActivityLogTypeId;
            activity.CustomerId = customerId;
            activity.Comment = comment;
            activity.CreatedOn = DateTime.UtcNow;

            context.ActivityLog.AddObject(activity);
            context.SaveChanges();

            return activity;
        }
        
        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log type identifier</param>
        public void DeleteActivity(int activityLogId)
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
        public List<ActivityLog> GetAllActivities(DateTime? createdOnFrom,
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
        public ActivityLog GetActivityById(int activityLogId)
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
        public void ClearAllActivities()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ActivityLogClearAll();
        }
        #endregion
    }
}
