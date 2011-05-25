using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Customer activity service
    /// </summary>
    public class CustomerActivityService : ICustomerActivityService
    {
        #region Constants
        private const string ACTIVITYTYPE_ALL_KEY = "Nop.activitytype.all";
        private const string ACTIVITYTYPE_BY_ID_KEY = "Nop.activitytype.id-{0}";
        private const string ACTIVITYTYPE_PATTERN_KEY = "Nop.activitytype.";
        #endregion

        #region Fields

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IWorkContext _workContext;
        #endregion
        
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="activityLogRepository"></param>
        /// <param name="activityLogTypeRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="workContext"></param>
        public CustomerActivityService(ICacheManager cacheManager,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Customer> customerRepository,
            IWorkContext workContext)
        {
            this._cacheManager = cacheManager;
            this._activityLogRepository = activityLogRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._customerRepository = customerRepository;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void InsertActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Insert(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void UpdateActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Update(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
                
        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        public virtual void DeleteActivityType(int activityLogTypeId)
        {
            ActivityLogType activityLogType = _activityLogTypeRepository.GetById(activityLogTypeId);
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Delete(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
        
        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type collection</returns>
        public virtual List<ActivityLogType> GetAllActivityTypes()
        {
            string key = ACTIVITYTYPE_PATTERN_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from alt in _activityLogTypeRepository.Table
                            orderby alt.Name
                            select alt;
                var activityLogTypes = query.ToList();
                return activityLogTypes;
            });
        }
        
        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        public virtual ActivityLogType GetActivityTypeById(int activityLogTypeId)
        {
            if (activityLogTypeId == 0)
                return null;

            string key = string.Format(ACTIVITYTYPE_BY_ID_KEY, activityLogTypeId);
            return _cacheManager.Get(key, () =>
            {
                return _activityLogTypeRepository.GetById(activityLogTypeId);
            });
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(string systemKeyword, string comment)
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
        public virtual ActivityLog InsertActivity(string systemKeyword, 
            string comment, params object[] commentParams)
        {
            if (_workContext.CurrentCustomer== null ||
                _workContext.CurrentCustomer.IsGuest())
                return null;

            var activityTypes = GetAllActivityTypes();
            var activityType = activityTypes.Find(at => at.SystemKeyword == systemKeyword);
            if (activityType == null || !activityType.Enabled)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            

            var activity = new ActivityLog();
            activity.ActivityLogTypeId = activityType.ActivityLogTypeId;
            activity.Customer = _workContext.CurrentCustomer;
            activity.Comment = comment;
            activity.CreatedOn = DateTime.UtcNow;

            _activityLogRepository.Insert(activity);

            return activity;
        }
        
        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log type identifier</param>
        public virtual void DeleteActivity(int activityLogId)
        {
            ActivityLog activityLogType = _activityLogRepository.GetById(activityLogId);
            if (activityLogType == null)
                throw new ArgumentNullException("activityLog");

            _activityLogRepository.Delete(activityLogType);
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all customers</param>
        /// <param name="createdOnTo">Log item creation to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Activity log collection</returns>
        public virtual PagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom,
            DateTime? createdOnTo, string email, string username, int activityLogTypeId,
            int pageIndex, int pageSize)
        {
            var query = from al in _activityLogRepository.Table
                        from a in al.Customer.Addresses
                        where (!createdOnFrom.HasValue || createdOnFrom.Value <= al.CreatedOn) &&
                        (!createdOnTo.HasValue || createdOnTo.Value >= al.CreatedOn) &&
                        (activityLogTypeId == 0 || activityLogTypeId == al.ActivityLogTypeId) &&
                        (String.IsNullOrEmpty(email) || a.Email == email) &&
                        !al.Customer.IsGuest(true) &&
                        !al.Customer.Deleted
                        orderby al.CreatedOn descending
                        select al;
            var activityLog = new PagedList<ActivityLog>(query, pageIndex, pageSize);
            return activityLog;
        }
        
        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog GetActivityById(int activityLogId)
        {
            if (activityLogId == 0)
                return null;

            
            var query = from al in _activityLogRepository.Table
                        where al.ActivityLogId == activityLogId
                        select al;
            var activityLog = query.SingleOrDefault();
            return activityLog;
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        public virtual void ClearAllActivities()
        {
            
            var activityLog = _activityLogRepository.Table.ToList();
            foreach (var activityLogItem in activityLog)
                _activityLogRepository.Delete(activityLogItem);
        }
        #endregion

    }
}
