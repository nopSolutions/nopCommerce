using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Events;
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
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _aclRepository = aclRepository;
            _storeMappingRepository = storeMappingRepository;
            _topicRepository = topicRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _topicRepository.Delete(topic);

            //event notification
            _eventPublisher.EntityDeleted(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            return _topicRepository.ToCachedGetById(topicId);
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

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopTopicDefaults.TopicBySystemNameCacheKey
                , systemName, storeId, _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));

            var topic = _staticCacheManager.Get(cacheKey, () =>
            {
                var query = _topicRepository.Table;
                query = query.Where(t => t.SystemName == systemName);
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.OrderBy(t => t.Id);
                var topics = query.ToList();
                if (storeId > 0)
                {
                    //filter by store
                    topics = topics.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
                }

                if (!showHidden)
                {
                    //ACL (access control list)
                    topics = topics.Where(x => _aclService.Authorize(x)).ToList();
                }

                return topics.FirstOrDefault();
            });

            return topic;
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, bool ignorAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var key = ignorAcl
                ? _cacheKeyService.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllCacheKey, storeId, showHidden,
                    onlyIncludedInTopMenu)
                : _cacheKeyService.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllWithACLCacheKey,
                    storeId, showHidden, onlyIncludedInTopMenu,
                    _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));

            var query = _topicRepository.Table;
            query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);

            if (!showHidden)
                query = query.Where(t => t.Published);

            if (onlyIncludedInTopMenu)
                query = query.Where(t => t.IncludeInTopMenu);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) ||
                (!ignorAcl && !_catalogSettings.IgnoreAcl))
            {
                if (!ignorAcl && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new
                                {
                                    c1 = c.Id,
                                    c2 = nameof(Topic)
                                }
                                equals new
                                {
                                    c1 = acl.EntityId,
                                    c2 = acl.EntityName
                                }
                                into cAcl
                            from acl in cAcl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (!_catalogSettings.IgnoreStoreLimitations && storeId > 0)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new
                                {
                                    c1 = c.Id,
                                    c2 = nameof(Topic)
                                }
                                equals new
                                {
                                    c1 = sm.EntityId,
                                    c2 = sm.EntityName
                                }
                                into cSm
                            from sm in cSm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
            }

            return query.ToCachedList(key);
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search into body or title</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, string keywords, bool ignorAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = GetAllTopics(storeId, ignorAcl: ignorAcl, showHidden: showHidden, onlyIncludedInTopMenu: onlyIncludedInTopMenu);

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
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _topicRepository.Insert(topic);

            //event notification
            _eventPublisher.EntityInserted(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _topicRepository.Update(topic);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}