using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
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

        protected IAclService AclService { get; }
        protected ICustomerService CustomerService { get; }
        protected IRepository<Topic> TopicRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public TopicService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            AclService = aclService;
            CustomerService = customerService;
            TopicRepository = topicRepository;
            StaticCacheManager = staticCacheManager;
            StoreMappingService = storeMappingService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicAsync(Topic topic)
        {
            await TopicRepository.DeleteAsync(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic
        /// </returns>
        public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await TopicRepository.GetByIdAsync(topicId, cache => default);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic
        /// </returns>
        public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, int storeId = 0, bool showHidden = false)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);

            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopTopicDefaults.TopicBySystemNameCacheKey, systemName, storeId, customerRoleIds);

            var topic = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = TopicRepository.Table;

                if (!showHidden)
                    query = query.Where(t => t.Published);

                //apply store mapping constraints
                query = await StoreMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                if (!showHidden)
                    query = await AclService.ApplyAcl(query, customerRoleIds);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);

            return await TopicRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                    query = query.Where(t => t.Published);

                //apply store mapping constraints
                query = await StoreMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                if (!showHidden && !ignoreAcl)
                    query = await AclService.ApplyAcl(query, customerRoleIds);

                if (onlyIncludedInTopMenu)
                    query = query.Where(t => t.IncludeInTopMenu);

                return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
            }, cache =>
            {
                return ignoreAcl
                    ? cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllCacheKey, storeId, showHidden, onlyIncludedInTopMenu)
                    : cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, onlyIncludedInTopMenu, customerRoleIds);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId, string keywords,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = await GetAllTopicsAsync(storeId,
                ignoreAcl: ignoreAcl,
                showHidden: showHidden,
                onlyIncludedInTopMenu: onlyIncludedInTopMenu);

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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicAsync(Topic topic)
        {
            await TopicRepository.InsertAsync(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicAsync(Topic topic)
        {
            await TopicRepository.UpdateAsync(topic);
        }

        #endregion
    }
}