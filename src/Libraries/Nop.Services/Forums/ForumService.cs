using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Messages;
using Nop.Services.Seo;

namespace Nop.Services.Forums
{
    /// <summary>
    /// Forum service
    /// </summary>
    public partial class ForumService : IForumService
    {
        #region Fields

        protected ForumSettings ForumSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected INopHtmlHelper HtmlHelper { get; }
        protected IRepository<Customer> CustomerRepository { get; }
        protected IRepository<Forum> ForumRepository { get; }
        protected IRepository<ForumGroup> ForumGroupRepository{ get; }
        protected IRepository<ForumPost> ForumPostRepository { get; }
        protected IRepository<ForumPostVote> ForumPostVoteRepository { get; }
        protected IRepository<ForumSubscription> ForumSubscriptionRepository { get; }
        protected IRepository<ForumTopic> ForumTopicRepository { get; }
        protected IRepository<PrivateMessage> ForumPrivateMessageRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected SeoSettings SeoSettings { get; }

        #endregion

        #region Ctor

        public ForumService(ForumSettings forumSettings,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            INopHtmlHelper htmlHelper,
            IRepository<Customer> customerRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumPostVote> forumPostVoteRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            SeoSettings seoSettings)
        {
            ForumSettings = forumSettings;
            CustomerService = customerService;
            GenericAttributeService = genericAttributeService;
            HtmlHelper = htmlHelper;
            CustomerRepository = customerRepository;
            ForumRepository = forumRepository;
            ForumGroupRepository = forumGroupRepository;
            ForumPostRepository = forumPostRepository;
            ForumPostVoteRepository = forumPostVoteRepository;
            ForumSubscriptionRepository = forumSubscriptionRepository;
            ForumTopicRepository = forumTopicRepository;
            ForumPrivateMessageRepository = forumPrivateMessageRepository;
            StaticCacheManager = staticCacheManager;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            SeoSettings = seoSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Update forum stats
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UpdateForumStatsAsync(int forumId)
        {
            if (forumId == 0) 
                return;

            var forum = await GetForumByIdAsync(forumId);
            if (forum == null) 
                return;

            //number of topics
            var queryNumTopics = from ft in ForumTopicRepository.Table
                                 where ft.ForumId == forumId
                                 select ft.Id;
            var numTopics = await queryNumTopics.CountAsync();

            //number of posts
            var queryNumPosts = from ft in ForumTopicRepository.Table
                                join fp in ForumPostRepository.Table on ft.Id equals fp.TopicId
                                where ft.ForumId == forumId
                                select fp.Id;
            var numPosts = await queryNumPosts.CountAsync();

            //last values
            var lastTopicId = 0;
            var lastPostId = 0;
            var lastPostCustomerId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from ft in ForumTopicRepository.Table
                                  join fp in ForumPostRepository.Table on ft.Id equals fp.TopicId
                                  where ft.ForumId == forumId
                                  orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
                                  select new
                                  {
                                      LastTopicId = ft.Id,
                                      LastPostId = fp.Id,
                                      LastPostCustomerId = fp.CustomerId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = await queryLastValues.FirstOrDefaultAsync();
            if (lastValues != null)
            {
                lastTopicId = lastValues.LastTopicId;
                lastPostId = lastValues.LastPostId;
                lastPostCustomerId = lastValues.LastPostCustomerId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update forum
            forum.NumTopics = numTopics;
            forum.NumPosts = numPosts;
            forum.LastTopicId = lastTopicId;
            forum.LastPostId = lastPostId;
            forum.LastPostCustomerId = lastPostCustomerId;
            forum.LastPostTime = lastPostTime;
            await UpdateForumAsync(forum);
        }

        /// <summary>
        /// Update forum topic stats
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UpdateForumTopicStatsAsync(int forumTopicId)
        {
            if (forumTopicId == 0) 
                return;

            var forumTopic = await GetTopicByIdAsync(forumTopicId);
            if (forumTopic == null) 
                return;

            //number of posts
            var queryNumPosts = from fp in ForumPostRepository.Table
                                where fp.TopicId == forumTopicId
                                select fp.Id;
            var numPosts = await queryNumPosts.CountAsync();

            //last values
            var lastPostId = 0;
            var lastPostCustomerId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from fp in ForumPostRepository.Table
                                  where fp.TopicId == forumTopicId
                                  orderby fp.CreatedOnUtc descending
                                  select new
                                  {
                                      LastPostId = fp.Id,
                                      LastPostCustomerId = fp.CustomerId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = await queryLastValues.FirstOrDefaultAsync();
            if (lastValues != null)
            {
                lastPostId = lastValues.LastPostId;
                lastPostCustomerId = lastValues.LastPostCustomerId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update topic
            forumTopic.NumPosts = numPosts;
            forumTopic.LastPostId = lastPostId;
            forumTopic.LastPostCustomerId = lastPostCustomerId;
            forumTopic.LastPostTime = lastPostTime;
            
            await UpdateTopicAsync(forumTopic);
        }

        /// <summary>
        /// Update customer stats
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task UpdateCustomerStatsAsync(int customerId)
        {
            if (customerId == 0) 
                return;

            var customer = await CustomerService.GetCustomerByIdAsync(customerId);

            if (customer == null) 
                return;

            var query = from fp in ForumPostRepository.Table
                        where fp.CustomerId == customerId
                        select fp.Id;
            var numPosts = await query.CountAsync();

            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.ForumPostCountAttribute, numPosts);
        }

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Topic
        /// </returns>
        protected virtual async Task<ForumTopic> GetTopicByIdAsync(int forumTopicId, bool increaseViews)
        {
            var forumTopic = await ForumTopicRepository.GetByIdAsync(forumTopicId, cache => default);

            if (forumTopic == null)
                return null;

            if (!increaseViews)
                return forumTopic;

            forumTopic.Views = ++forumTopic.Views;
            await UpdateTopicAsync(forumTopic);

            return forumTopic;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteForumGroupAsync(ForumGroup forumGroup)
        {
            await ForumGroupRepository.DeleteAsync(forumGroup);
        }

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group
        /// </returns>
        public virtual async Task<ForumGroup> GetForumGroupByIdAsync(int forumGroupId)
        {
            return await ForumGroupRepository.GetByIdAsync(forumGroupId, cache => default);
        }

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum groups
        /// </returns>
        public virtual async Task<IList<ForumGroup>> GetAllForumGroupsAsync()
        {
            return await ForumGroupRepository.GetAllAsync(query =>
            {
                return from fg in query
                    orderby fg.DisplayOrder, fg.Id
                    select fg;
            }, cache => default);
        }

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertForumGroupAsync(ForumGroup forumGroup)
        {
            await ForumGroupRepository.InsertAsync(forumGroup);
        }

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateForumGroupAsync(ForumGroup forumGroup)
        {
            await ForumGroupRepository.UpdateAsync(forumGroup);
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteForumAsync(Forum forum)
        {
            if (forum == null) 
                throw new ArgumentNullException(nameof(forum));

            //delete forum subscriptions (topics)
            var queryTopicIds = from ft in ForumTopicRepository.Table
                                where ft.ForumId == forum.Id
                                select ft.Id;
            var queryFs1 = from fs in ForumSubscriptionRepository.Table
                           where queryTopicIds.Contains(fs.TopicId)
                           select fs;

            await ForumSubscriptionRepository.DeleteAsync(queryFs1.ToList());

            //delete forum subscriptions (forum)
            var queryFs2 = from fs in ForumSubscriptionRepository.Table
                           where fs.ForumId == forum.Id
                           select fs;

            await ForumSubscriptionRepository.DeleteAsync(queryFs2.ToList());

            //delete forum
            await ForumRepository.DeleteAsync(forum);
        }

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum
        /// </returns>
        public virtual async Task<Forum> GetForumByIdAsync(int forumId)
        {
            return await ForumRepository.GetByIdAsync(forumId, cache => default);
        }

        /// <summary>
        /// Gets forums by forum group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forums
        /// </returns>
        public virtual async Task<IList<Forum>> GetAllForumsByGroupIdAsync(int forumGroupId)
        {
            var forums = await ForumRepository.GetAllAsync(query =>
            {
                return from f in query
                    orderby f.DisplayOrder, f.Id
                    where f.ForumGroupId == forumGroupId
                    select f;
            }, cache => cache.PrepareKeyForDefaultCache(NopForumDefaults.ForumByForumGroupCacheKey, forumGroupId));

            return forums;
        }

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertForumAsync(Forum forum)
        {
            await ForumRepository.InsertAsync(forum);
        }

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateForumAsync(Forum forum)
        {
            // if the forum group is changed then clear cache for the previous group 
            // (we can't use the event consumer because it will work after saving the changes in DB)
            var forumToUpdate = await ForumRepository.LoadOriginalCopyAsync(forum);
            if (forumToUpdate.ForumGroupId != forum.ForumGroupId)
                await StaticCacheManager.RemoveAsync(NopForumDefaults.ForumByForumGroupCacheKey, forumToUpdate.ForumGroupId);

            await ForumRepository.UpdateAsync(forum);
        }

        /// <summary>
        /// Deletes a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicAsync(ForumTopic forumTopic)
        {
            if (forumTopic == null) throw new ArgumentNullException(nameof(forumTopic));

            var customerId = forumTopic.CustomerId;
            var forumId = forumTopic.ForumId;

            //delete topic
            await ForumTopicRepository.DeleteAsync(forumTopic);

            //delete forum subscriptions
            var queryFs = from ft in ForumSubscriptionRepository.Table
                          where ft.TopicId == forumTopic.Id
                          select ft;
            var forumSubscriptions = queryFs.ToList();

            await ForumSubscriptionRepository.DeleteAsync(forumSubscriptions);

            //update stats
            await UpdateForumStatsAsync(forumId);
            await UpdateCustomerStatsAsync(customerId);
        }

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Topic
        /// </returns>
        public virtual async Task<ForumTopic> GetTopicByIdAsync(int forumTopicId)
        {
            return await GetTopicByIdAsync(forumTopicId, false);
        }
        
        /// <summary>
        /// Gets all forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Topics
        /// </returns>
        public virtual async Task<IPagedList<ForumTopic>> GetAllTopicsAsync(int forumId = 0,
            int customerId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
            int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime? limitDate = null;
            if (limitDays > 0) limitDate = DateTime.UtcNow.AddDays(-limitDays);

            var searchKeywords = !string.IsNullOrEmpty(keywords);
            var searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
            var searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;

            var topics = await ForumTopicRepository.GetAllPagedAsync(query =>
            {
                var query1 = from ft in query
                    join fp in ForumPostRepository.Table on ft.Id equals fp.TopicId
                    where
                        (forumId == 0 || ft.ForumId == forumId) &&
                        (customerId == 0 || ft.CustomerId == customerId) &&
                        (!searchKeywords ||
                         (searchTopicTitles && ft.Subject.Contains(keywords)) ||
                         (searchPostText && fp.Text.Contains(keywords))) &&
                        (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                    select ft.Id;

                var query2 = from ft in query
                    where query1.Contains(ft.Id)
                    orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                    select ft;

                return query2;
            }, pageIndex, pageSize);

            return topics;
        }

        /// <summary>
        /// Gets active forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Topics
        /// </returns>
        public virtual async Task<IPagedList<ForumTopic>> GetActiveTopicsAsync(int forumId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query1 = from ft in ForumTopicRepository.Table
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         ft.LastPostTime.HasValue
                         select ft.Id;

            var query2 = from ft in ForumTopicRepository.Table
                         where query1.Contains(ft.Id)
                         orderby ft.LastPostTime descending
                         select ft;

            var topics = await query2.ToPagedListAsync(pageIndex, pageSize);

            return topics;
        }

        /// <summary>
        /// Inserts a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicAsync(ForumTopic forumTopic, bool sendNotifications)
        {
            await ForumTopicRepository.InsertAsync(forumTopic);

            //update stats
            await UpdateForumStatsAsync(forumTopic.ForumId);
            
            if (!sendNotifications) 
                return;

            //send notifications
            var forum = await GetForumByIdAsync(forumTopic.ForumId);
            var subscriptions = await GetAllSubscriptionsAsync(forumId: forum.Id);
            var languageId = (await WorkContext.GetWorkingLanguageAsync()).Id;

            foreach (var subscription in subscriptions)
            {
                if (subscription.CustomerId == forumTopic.CustomerId) continue;

                var customer = await CustomerService.GetCustomerByIdAsync(subscription.CustomerId);

                if (!string.IsNullOrEmpty(customer?.Email)) 
                    await WorkflowMessageService.SendNewForumTopicMessageAsync(customer, forumTopic, forum, languageId);
            }
        }

        /// <summary>
        /// Updates the forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicAsync(ForumTopic forumTopic)
        {
            await ForumTopicRepository.UpdateAsync(forumTopic);
        }

        /// <summary>
        /// Moves the forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the moved forum topic
        /// </returns>
        public virtual async Task<ForumTopic> MoveTopicAsync(int forumTopicId, int newForumId)
        {
            var forumTopic = await GetTopicByIdAsync(forumTopicId);
            if (forumTopic == null)
                return null;

            if (!await IsCustomerAllowedToMoveTopicAsync(await WorkContext.GetCurrentCustomerAsync(), forumTopic))
                return forumTopic;

            var previousForumId = forumTopic.ForumId;
            var newForum = await GetForumByIdAsync(newForumId);

            if (newForum == null)
                return forumTopic;

            if (previousForumId == newForumId)
                return forumTopic;

            forumTopic.ForumId = newForum.Id;
            forumTopic.UpdatedOnUtc = DateTime.UtcNow;
            await UpdateTopicAsync(forumTopic);

            //update forum stats
            await UpdateForumStatsAsync(previousForumId);
            await UpdateForumStatsAsync(newForumId);
            return forumTopic;
        }

        /// <summary>
        /// Deletes a forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePostAsync(ForumPost forumPost)
        {
            if (forumPost == null) 
                throw new ArgumentNullException(nameof(forumPost));

            var forumTopicId = forumPost.TopicId;
            var customerId = forumPost.CustomerId;
            var forumTopic = await GetTopicByIdAsync(forumTopicId);
            var forumId = forumTopic.ForumId;

            //delete topic if it was the first post
            var deleteTopic = false;
            var firstPost = await GetFirstPostAsync(forumTopic);
            if (firstPost != null && firstPost.Id == forumPost.Id) 
                deleteTopic = true;

            //delete forum post
            await ForumPostRepository.DeleteAsync(forumPost);

            //delete topic
            if (deleteTopic) 
                await DeleteTopicAsync(forumTopic);

            //update stats
            if (!deleteTopic) 
                await UpdateForumTopicStatsAsync(forumTopicId);

            await UpdateForumStatsAsync(forumId);
            await UpdateCustomerStatsAsync(customerId);
        }

        /// <summary>
        /// Gets a forum post
        /// </summary>
        /// <param name="forumPostId">The forum post identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Post
        /// </returns>
        public virtual async Task<ForumPost> GetPostByIdAsync(int forumPostId)
        {
            return await ForumPostRepository.GetByIdAsync(forumPostId, cache => default);
        }

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the posts
        /// </returns>
        public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0,
            int customerId = 0, string keywords = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await GetAllPostsAsync(forumTopicId, customerId, keywords, true,
                pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum Posts
        /// </returns>
        public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0, int customerId = 0,
            string keywords = "", bool ascSort = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var forumPosts = await ForumPostRepository.GetAllPagedAsync(query =>
            {
                if (forumTopicId > 0) 
                    query = query.Where(fp => forumTopicId == fp.TopicId);

                if (customerId > 0)
                    query = query.Where(fp => customerId == fp.CustomerId);

                if (!string.IsNullOrEmpty(keywords))
                    query = query.Where(fp => fp.Text.Contains(keywords));

                query = ascSort
                    ? query.OrderBy(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id)
                    : query.OrderByDescending(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id);

                return query;
            }, pageIndex, pageSize);

            return forumPosts;
        }

        /// <summary>
        /// Inserts a forum post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPostAsync(ForumPost forumPost, bool sendNotifications)
        {
            await ForumPostRepository.InsertAsync(forumPost);

            //update stats
            var customerId = forumPost.CustomerId;
            var forumTopic = await GetTopicByIdAsync(forumPost.TopicId);
            var forumId = forumTopic.ForumId;

            await UpdateForumTopicStatsAsync(forumPost.TopicId);
            await UpdateForumStatsAsync(forumId);
            await UpdateCustomerStatsAsync(customerId);
            
            //notifications
            if (!sendNotifications) 
                return;

            var forum = await GetForumByIdAsync(forumTopic.ForumId);
            var subscriptions = await GetAllSubscriptionsAsync(topicId: forumTopic.Id);

            var languageId = (await WorkContext.GetWorkingLanguageAsync()).Id;

            var friendlyTopicPageIndex = await CalculateTopicPageIndexAsync(forumPost.TopicId,
                                             ForumSettings.PostsPageSize > 0 ? ForumSettings.PostsPageSize : 10,
                                             forumPost.Id) + 1;

            foreach (var subscription in subscriptions)
            {
                if (subscription.CustomerId == forumPost.CustomerId) 
                    continue;

                var customer = await CustomerService.GetCustomerByIdAsync(subscription.CustomerId);

                if (!string.IsNullOrEmpty(customer?.Email)) 
                    await WorkflowMessageService.SendNewForumPostMessageAsync(customer, forumPost, forumTopic, forum, friendlyTopicPageIndex, languageId);
            }
        }

        /// <summary>
        /// Updates the forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePostAsync(ForumPost forumPost)
        {
            await ForumPostRepository.UpdateAsync(forumPost);
        }

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePrivateMessageAsync(PrivateMessage privateMessage)
        {
            await ForumPrivateMessageRepository.DeleteAsync(privateMessage);
        }

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="privateMessageId">The private message identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private message
        /// </returns>
        public virtual async Task<PrivateMessage> GetPrivateMessageByIdAsync(int privateMessageId)
        {
            return await ForumPrivateMessageRepository.GetByIdAsync(privateMessageId, cache => default);
        }

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all messages</param>
        /// <param name="fromCustomerId">The customer identifier who sent the message</param>
        /// <param name="toCustomerId">The customer identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private messages
        /// </returns>
        public virtual async Task<IPagedList<PrivateMessage>> GetAllPrivateMessagesAsync(int storeId, int fromCustomerId,
            int toCustomerId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var privateMessages = await ForumPrivateMessageRepository.GetAllPagedAsync(query =>
            {
                if (storeId > 0)
                    query = query.Where(pm => storeId == pm.StoreId);
                if (fromCustomerId > 0)
                    query = query.Where(pm => fromCustomerId == pm.FromCustomerId);
                if (toCustomerId > 0)
                    query = query.Where(pm => toCustomerId == pm.ToCustomerId);
                if (isRead.HasValue)
                    query = query.Where(pm => isRead.Value == pm.IsRead);
                if (isDeletedByAuthor.HasValue)
                    query = query.Where(pm => isDeletedByAuthor.Value == pm.IsDeletedByAuthor);
                if (isDeletedByRecipient.HasValue)
                    query = query.Where(pm => isDeletedByRecipient.Value == pm.IsDeletedByRecipient);
                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(pm => pm.Subject.Contains(keywords));
                    query = query.Where(pm => pm.Text.Contains(keywords));
                }

                query = query.OrderByDescending(pm => pm.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize);

            return privateMessages;
        }

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPrivateMessageAsync(PrivateMessage privateMessage)
        {
            await ForumPrivateMessageRepository.InsertAsync(privateMessage);

            var customerTo = await CustomerService.GetCustomerByIdAsync(privateMessage.ToCustomerId);
            if (customerTo == null)
                throw new NopException("Recipient could not be loaded");

            //UI notification
            await GenericAttributeService.SaveAttributeAsync(customerTo, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, false, privateMessage.StoreId);

            //Email notification
            if (ForumSettings.NotifyAboutPrivateMessages) 
                await WorkflowMessageService.SendPrivateMessageNotificationAsync(privateMessage, (await WorkContext.GetWorkingLanguageAsync()).Id);
        }

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePrivateMessageAsync(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
                throw new ArgumentNullException(nameof(privateMessage));

            if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
                await ForumPrivateMessageRepository.DeleteAsync(privateMessage);
            else
                await ForumPrivateMessageRepository.UpdateAsync(privateMessage);
        }

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSubscriptionAsync(ForumSubscription forumSubscription)
        {
            await ForumSubscriptionRepository.DeleteAsync(forumSubscription);
        }

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum subscription
        /// </returns>
        public virtual async Task<ForumSubscription> GetSubscriptionByIdAsync(int forumSubscriptionId)
        {
            return await ForumSubscriptionRepository.GetByIdAsync(forumSubscriptionId, cache => default);
        }

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum subscriptions
        /// </returns>
        public virtual async Task<IPagedList<ForumSubscription>> GetAllSubscriptionsAsync(int customerId = 0, int forumId = 0,
            int topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var forumSubscriptions = await ForumSubscriptionRepository.GetAllPagedAsync(query =>
            {
                var fsQuery = from fs in query
                    join c in CustomerRepository.Table on fs.CustomerId equals c.Id
                    where
                        (customerId == 0 || fs.CustomerId == customerId) &&
                        (forumId == 0 || fs.ForumId == forumId) &&
                        (topicId == 0 || fs.TopicId == topicId) &&
                        c.Active &&
                        !c.Deleted
                    select fs.SubscriptionGuid;

                var rez = from fs in query
                    where fsQuery.Contains(fs.SubscriptionGuid)
                    orderby fs.CreatedOnUtc descending, fs.SubscriptionGuid descending
                    select fs;

                return rez;
            }, pageIndex, pageSize);

            return forumSubscriptions;
        }

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSubscriptionAsync(ForumSubscription forumSubscription)
        {
            await ForumSubscriptionRepository.InsertAsync(forumSubscription);
        }
        
        /// <summary>
        /// Check whether customer is allowed to create new topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="forum">Forum</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToCreateTopicAsync(Customer customer, Forum forum)
        {
            if (forum == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer) && !ForumSettings.AllowGuestsToCreateTopics) 
                return false;

            return true;
        }

        /// <summary>
        /// Check whether customer is allowed to edit topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToEditTopicAsync(Customer customer, ForumTopic topic)
        {
            if (topic == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            if (await CustomerService.IsForumModeratorAsync(customer)) 
                return true;

            if (!ForumSettings.AllowCustomersToEditPosts) 
                return false;

            var ownTopic = customer.Id == topic.CustomerId;

            return ownTopic;
        }

        /// <summary>
        /// Check whether customer is allowed to move topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToMoveTopicAsync(Customer customer, ForumTopic topic)
        {
            if (topic == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            return await CustomerService.IsForumModeratorAsync(customer);
        }

        /// <summary>
        /// Check whether customer is allowed to delete topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToDeleteTopicAsync(Customer customer, ForumTopic topic)
        {
            if (topic == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            if (await CustomerService.IsForumModeratorAsync(customer)) 
                return true;

            if (!ForumSettings.AllowCustomersToDeletePosts) 
                return false;

            var ownTopic = customer.Id == topic.CustomerId;

            return ownTopic;
        }

        /// <summary>
        /// Check whether customer is allowed to create new post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToCreatePostAsync(Customer customer, ForumTopic topic)
        {
            if (topic == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer) && !ForumSettings.AllowGuestsToCreatePosts) 
                return false;

            return true;
        }

        /// <summary>
        /// Check whether customer is allowed to edit post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToEditPostAsync(Customer customer, ForumPost post)
        {
            if (post == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            if (await CustomerService.IsForumModeratorAsync(customer)) 
                return true;

            if (!ForumSettings.AllowCustomersToEditPosts) 
                return false;

            var ownPost = customer.Id == post.CustomerId;

            return ownPost;
        }

        /// <summary>
        /// Check whether customer is allowed to delete post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToDeletePostAsync(Customer customer, ForumPost post)
        {
            if (post == null) 
                return false;

            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            if (await CustomerService.IsForumModeratorAsync(customer)) 
                return true;

            if (!ForumSettings.AllowCustomersToDeletePosts)
                return false;

            var ownPost = customer.Id == post.CustomerId;

            return ownPost;
        }

        /// <summary>
        /// Check whether customer is allowed to set topic priority
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToSetTopicPriorityAsync(Customer customer)
        {
            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            return await CustomerService.IsForumModeratorAsync(customer);
        }

        /// <summary>
        /// Check whether customer is allowed to watch topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        public virtual async Task<bool> IsCustomerAllowedToSubscribeAsync(Customer customer)
        {
            if (customer == null) 
                return false;

            if (await CustomerService.IsGuestAsync(customer)) 
                return false;

            return true;
        }

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Forum topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the page index
        /// </returns>
        public virtual async Task<int> CalculateTopicPageIndexAsync(int forumTopicId, int pageSize, int postId)
        {
            var pageIndex = 0;
            var forumPosts = await GetAllPostsAsync(forumTopicId, ascSort: true);

            for (var i = 0; i < forumPosts.TotalCount; i++)
            {
                if (forumPosts[i].Id != postId) 
                    continue;

                if (pageSize > 0) pageIndex = i / pageSize;
            }

            return pageIndex;
        }

        /// <summary>
        /// Get a post vote 
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the post vote
        /// </returns>
        public virtual async Task<ForumPostVote> GetPostVoteAsync(int postId, Customer customer)
        {
            if (customer == null)
                return null;

            return await ForumPostVoteRepository.Table
                .FirstOrDefaultAsync(pv => pv.ForumPostId == postId && pv.CustomerId == customer.Id);
        }

        /// <summary>
        /// Get post vote made since the parameter date
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="createdFromUtc">Date</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the post votes count
        /// </returns>
        public virtual async Task<int> GetNumberOfPostVotesAsync(Customer customer, DateTime createdFromUtc)
        {
            if (customer == null)
                return 0;

            return await ForumPostVoteRepository.Table
                .CountAsync(pv => pv.CustomerId == customer.Id && pv.CreatedOnUtc > createdFromUtc);
        }

        /// <summary>
        /// Insert a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPostVoteAsync(ForumPostVote postVote)
        {
            await ForumPostVoteRepository.InsertAsync(postVote);

            //update post
            var post = await GetPostByIdAsync(postVote.ForumPostId);
            post.VoteCount = postVote.IsUp ? ++post.VoteCount : --post.VoteCount;

            await UpdatePostAsync(post);
        }
        
        /// <summary>
        /// Delete a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePostVoteAsync(ForumPostVote postVote)
        {
            if (postVote == null)
                throw new ArgumentNullException(nameof(postVote));

            await ForumPostVoteRepository.DeleteAsync(postVote);

            // update post
            var post = await GetPostByIdAsync(postVote.ForumPostId);
            post.VoteCount = postVote.IsUp ? --post.VoteCount : ++post.VoteCount;

            await UpdatePostAsync(post);
        }

        /// <summary>
        /// Formats the forum post text
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatPostText(ForumPost forumPost)
        {
            var text = forumPost.Text;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            switch (ForumSettings.ForumEditor)
            {
                case EditorType.SimpleTextBox:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
                    }

                    break;
                case EditorType.BBCodeEditor:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, true, false, false);
                    }

                    break;
                default:
                    break;
            }

            return text;
        }

        /// <summary>
        /// Strips the topic subject
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Formatted subject</returns>
        public virtual string StripTopicSubject(ForumTopic forumTopic)
        {
            var subject = forumTopic.Subject;
            if (string.IsNullOrEmpty(subject)) return subject;

            var strippedTopicMaxLength = ForumSettings.StrippedTopicMaxLength;
            if (strippedTopicMaxLength <= 0)
                return subject;

            if (subject.Length <= strippedTopicMaxLength)
                return subject;

            var index = subject.IndexOf(" ", strippedTopicMaxLength, StringComparison.Ordinal);
            
            if (index <= 0) 
                return subject;

            subject = subject[0..index];
            subject += "...";

            return subject;
        }

        /// <summary>
        /// Formats the forum signature text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatForumSignatureText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        /// <summary>
        /// Formats the private message text
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatPrivateMessageText(PrivateMessage pm)
        {
            var text = pm.Text;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, true, false, false);

            return text;
        }
        
        /// <summary>
        /// Get first post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum post
        /// </returns>
        public virtual async Task<ForumPost> GetFirstPostAsync(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            var forumPosts = await GetAllPostsAsync(forumTopic.Id, 0, string.Empty, 0, 1);
            if (forumPosts.Any())
                return forumPosts[0];

            return null;
        }
        
        /// <summary>
        /// Gets ForumGroup SE (search engine) name
        /// </summary>
        /// <param name="forumGroup">ForumGroup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forumGroup SE (search engine) name
        /// </returns>
        public virtual async Task<string> GetForumGroupSeNameAsync(ForumGroup forumGroup)
        {
            if (forumGroup == null)
                throw new ArgumentNullException(nameof(forumGroup));

            var seName = await UrlRecordService.GetSeNameAsync(forumGroup.Name, SeoSettings.ConvertNonWesternChars, SeoSettings.AllowUnicodeCharsInUrls);
            
            return seName;
        }

        /// <summary>
        /// Gets Forum SE (search engine) name
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum SE (search engine) name
        /// </returns>
        public virtual async Task<string> GetForumSeNameAsync(Forum forum)
        {
            if (forum == null)
                throw new ArgumentNullException(nameof(forum));

            var seName = await UrlRecordService.GetSeNameAsync(forum.Name, SeoSettings.ConvertNonWesternChars, SeoSettings.AllowUnicodeCharsInUrls);
            
            return seName;
        }

        /// <summary>
        /// Gets ForumTopic SE (search engine) name
        /// </summary>
        /// <param name="forumTopic">ForumTopic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forumTopic SE (search engine) name
        /// </returns>
        public virtual async Task<string> GetTopicSeNameAsync(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            var seName = await UrlRecordService.GetSeNameAsync(forumTopic.Subject, SeoSettings.ConvertNonWesternChars, SeoSettings.AllowUnicodeCharsInUrls);

            // Trim SE name to avoid URLs that are too long
            var maxLength = NopSeoDefaults.ForumTopicLength;
            if (seName.Length > maxLength)
                seName = seName[0..maxLength];

            return seName;
        }

        #endregion
    }
}