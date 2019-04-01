using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Topics;
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
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<Topic> topicRepository,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _aclRepository = aclRepository;
            _storeMappingRepository = storeMappingRepository;
            _topicRepository = topicRepository;
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

            //cache
            _cacheManager.RemoveByPrefix(NopTopicDefaults.TopicsPrefixCacheKey);

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

            var key = string.Format(NopTopicDefaults.TopicsByIdCacheKey, topicId);
            return _cacheManager.Get(key, () => _topicRepository.GetById(topicId));
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
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, bool ignorAcl = false, bool showHidden = false)
        {
            var key = string.Format(NopTopicDefaults.TopicsAllCacheKey, storeId, ignorAcl, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _topicRepository.Table;
                query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);

                if (!showHidden)
                    query = query.Where(t => t.Published);

                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) ||
                    (!ignorAcl && !_catalogSettings.IgnoreAcl))
                {
                    if (!ignorAcl && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Topic) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into cAcl
                                from acl in cAcl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations && storeId > 0)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Topic) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into cSm
                                from sm in cSm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    query = query.Distinct().OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
                }

                return query.ToList();
            });
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

            //cache
            _cacheManager.RemoveByPrefix(NopTopicDefaults.TopicsPrefixCacheKey);

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

            //cache
            _cacheManager.RemoveByPrefix(NopTopicDefaults.TopicsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}