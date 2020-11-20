using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _customerService = customerService;
            _topicRepository = topicRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Filter hidden entries according to constraints if any
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="storeId">A store identifier</param>
        /// <param name="customerRoleIds">Identifiers of customer's roles</param>
        /// <returns>Filtered query</returns>
        protected virtual IQueryable<TEntity> FilterHiddenEntries<TEntity>(IQueryable<TEntity> query, int storeId, int[] customerRoleIds)
            where TEntity : Topic
        {
            //filter unpublished entries
            query = query.Where(entry => entry.Published);

            //apply store mapping constraints
            if (!_catalogSettings.IgnoreStoreLimitations && _storeMappingService.IsEntityMappingExists<TEntity>(storeId))
                query = query.Where(_storeMappingService.ApplyStoreMapping<TEntity>(storeId));

            //apply ACL constraints
            if (!_catalogSettings.IgnoreAcl && _aclService.IsEntityAclMappingExist<TEntity>(customerRoleIds))
                query = query.Where(_aclService.ApplyAcl<TEntity>(customerRoleIds));

            return query;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void DeleteTopic(Topic topic)
        {
            _topicRepository.Delete(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicById(int topicId)
        {
            return _topicRepository.GetById(topicId, cache => default);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicBySystemName(string systemName, int storeId = 0, bool showHidden = false)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var customerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopTopicDefaults.TopicBySystemNameCacheKey, systemName, storeId, customerRolesIds);

            var topic = _staticCacheManager.Get(cacheKey, () =>
            {
                var query = _topicRepository.Table;

                if (!showHidden)
                    query = FilterHiddenEntries(query, storeId, customerRolesIds);

                return query.Where(t => t.SystemName == systemName)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault();
            });

            return topic;
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            //ACL (access control list)
            var customerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);

            return _topicRepository.GetAll(query =>
            {
                if (!showHidden)
                    query = FilterHiddenEntries(query, storeId, customerRolesIds);

                if (onlyIncludedInTopMenu)
                    query = query.Where(t => t.IncludeInTopMenu);

                return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
            }, cache =>
            {
                return ignoreAcl
                    ? cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllCacheKey, storeId, showHidden, onlyIncludedInTopMenu)
                    : cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, onlyIncludedInTopMenu, customerRolesIds);
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search into body or title</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, string keywords, bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = GetAllTopics(storeId, ignoreAcl: ignoreAcl, showHidden: showHidden, onlyIncludedInTopMenu: onlyIncludedInTopMenu);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                return topics
                        .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                                        (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
                        .ToList();
            }

            return topics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void InsertTopic(Topic topic)
        {
            _topicRepository.Insert(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void UpdateTopic(Topic topic)
        {
            _topicRepository.Update(topic);
        }

        #endregion
    }
}