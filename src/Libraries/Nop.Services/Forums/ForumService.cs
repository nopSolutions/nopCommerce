using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Customers;

namespace Nop.Services.Forums
{
    /// <summary>
    /// Forum service
    /// </summary>
    public partial class ForumService : IForumService
    {
        #region Constants
        private const string FORUMGROUP_ALL_KEY = "Nop.forumgroup.all";
        private const string FORUMGROUP_BY_ID_KEY = "Nop.forumgroup.id-{0}";
        private const string FORUM_ALLBYFORUMGROUPID_KEY = "Nop.forum.allbyforumgroupid-{0}";
        private const string FORUM_BY_ID_KEY = "Nop.forum.id-{0}";
        private const string FORUMGROUP_PATTERN_KEY = "Nop.forumgroup.";
        private const string FORUM_PATTERN_KEY = "Nop.forum.";
        #endregion

        #region Fields
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
        private readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
        private readonly ForumSettings _forumSettings;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="forumGroupRepository">ForumGroup repository</param>
        /// <param name="forumRepository">Forum repository</param>
        /// <param name="forumTopicRepository">ForumTopic repository</param>
        /// <param name="forumPostRepository">ForumPost repository</param>
        /// <param name="forumPrivateMessageRepository">PrivateMessage repository</param>
        /// <param name="forumSubscriptionRepository">ForumSubscription repository</param>
        /// <param name="customerRepository">Customer repository</param>
        public ForumService(ICacheManager cacheManager,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            ForumSettings forumSettings,
            IRepository<Customer> customerRepository,
            ISettingService settingService,
            ICustomerService customerService,
            IWorkContext workcContext
            )
        {
            this._cacheManager = cacheManager;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumPrivateMessageRepository = forumPrivateMessageRepository;
            this._forumSubscriptionRepository = forumSubscriptionRepository;
            this._forumSettings = forumSettings;
            this._customerRepository = customerRepository;
            this._settingService = settingService;
            this._customerService = customerService;
            this._workContext = workcContext;
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Update forum stats
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        public void UpdateForumStats(int forumId)
        {
            if (forumId == 0)
                return;
            var forum = GetForumById(forumId);
            if (forum == null)
                return;

            //number of topics
            var queryNumTopics = from ft in _forumTopicRepository.Table
                                 where ft.ForumId == forumId
                                 select ft.Id;
            int numTopics = queryNumTopics.Count();

            //number of posts
            var queryNumPosts = from ft in _forumTopicRepository.Table
                                join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                where ft.ForumId == forumId
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastTopicId = 0;
            int lastPostId = 0;
            int lastPostUserId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from ft in _forumTopicRepository.Table
                                  join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                  where ft.ForumId == forumId
                                  orderby fp.CreatedOn descending, ft.CreatedOn descending
                                  select new
                                  {
                                      LastTopicId = ft.Id,
                                      LastPostId = fp.Id,
                                      LastPostUserId = fp.UserId,
                                      LastPostTime = fp.CreatedOn
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastTopicId = lastValues.LastTopicId;
                lastPostId = lastValues.LastPostId;
                lastPostUserId = lastValues.LastPostUserId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update forum
            forum.NumTopics = numTopics;
            forum.NumPosts = numPosts;
            forum.LastTopicId = lastTopicId;
            forum.LastPostId = lastPostId;
            forum.LastPostUserId = lastPostUserId;
            forum.LastPostTime = lastPostTime;
            UpdateForum(forum);
        }

        /// <summary>
        /// Update forum topic stats
        /// </summary>
        /// <param name="Id">The forum topic identifier</param>
        public void UpdateForumTopicStats(int Id)
        {
            if (Id == 0)
                return;
            var forumTopic = GetTopicById(Id);
            if (forumTopic == null)
                return;

            //number of posts
            var queryNumPosts = from fp in _forumPostRepository.Table
                                where fp.TopicId == Id
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastPostId = 0;
            int lastPostUserId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from fp in _forumPostRepository.Table
                                  where fp.TopicId == Id
                                  orderby fp.CreatedOn descending
                                  select new
                                  {
                                      LastPostId = fp.Id,
                                      LastPostUserId = fp.UserId,
                                      LastPostTime = fp.CreatedOn
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastPostId = lastValues.LastPostId;
                lastPostUserId = lastValues.LastPostUserId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update topic
            forumTopic.NumPosts = numPosts;
            forumTopic.LastPostId = lastPostId;
            forumTopic.LastPostUserId = lastPostUserId;
            forumTopic.LastPostTime = lastPostTime;
            UpdateTopic(forumTopic);
        }

        /// <summary>
        /// Update user stats
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        public void UpdateUserStats(int customerId)
        {
            if (customerId == 0)
                return;
            var customer = _customerService.GetCustomerById(customerId);

            if (customer == null)
                return;

            var query = from fp in _forumPostRepository.Table
                        where fp.UserId == customerId
                        select fp.Id;
            int numPosts = query.Count();

            //TODO need TotalForumPosts field
            //customer.TotalForumPosts = numPosts;
            _customerService.UpdateCustomer(customer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        public void DeleteForumGroup(int forumGroupId)
        {
            var forumGroup = GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                return;

            _forumGroupRepository.Delete(forumGroup);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forum group</returns>
        public ForumGroup GetForumGroupById(int forumGroupId)
        {
            if (forumGroupId == 0)
                return null;

            string key = string.Format(FORUMGROUP_BY_ID_KEY, forumGroupId);
            return _cacheManager.Get(key, () =>
            {
                var forumGroup = _forumGroupRepository.GetById(forumGroupId);
                return forumGroup;
            });
        }

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>Forum groups</returns>
        public IList<ForumGroup> GetAllForumGroups()
        {
            var query = from fg in _forumGroupRepository.Table
                        orderby fg.DisplayOrder
                        select fg;
            return query.ToList();
        }

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        public void InsertForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
                throw new ArgumentNullException("forumGroup");

            _forumGroupRepository.Insert(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        public void UpdateForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
                throw new ArgumentNullException("forumGroup");

            _forumGroupRepository.Update(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        public void DeleteForum(int forumId)
        {
            var forum = GetForumById(forumId);
            if (forum == null)
            {
                return;
            }

            //delete forum subscriptions (topics)
            foreach (var topic in forum.ForumTopics)
            {
                var queryFs = from ft in _forumSubscriptionRepository.Table
                              where ft.TopicId == topic.Id
                              select ft;
                foreach (var fs in queryFs.ToList())
                {
                    _forumSubscriptionRepository.Delete(fs);
                }
            }

            //delete forum subscriptions (forum)
            var queryFs2 = from fs in _forumSubscriptionRepository.Table
                           where fs.ForumId == forumId
                           select fs;
            foreach (var fs2 in queryFs2.ToList())
            {
                _forumSubscriptionRepository.Delete(fs2);
            }

            //delete forum
            _forumRepository.Delete(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>Forum</returns>
        public Forum GetForumById(int forumId)
        {
            if (forumId == 0)
                return null;

            string key = string.Format(FORUM_BY_ID_KEY, forumId);
            return _cacheManager.Get(key, () =>
            {
                var forum = _forumRepository.GetById(forumId);
                return forum;
            });
        }

        /// <summary>
        /// Gets forums by group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forums</returns>
        public IList<Forum> GetAllForumsByGroupId(int forumGroupId)
        {
            var query = from f in _forumRepository.Table
                        orderby f.DisplayOrder
                        where f.ForumGroupId == forumGroupId
                        select f;
            var forums = query.ToList();
            return forums;
        }

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        public void InsertForum(Forum forum)
        {
            if (forum == null)
                throw new ArgumentNullException("forum");

            _forumRepository.Insert(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        public void UpdateForum(Forum forum)
        {
            if (forum == null)
                throw new ArgumentNullException("forum");

            _forumRepository.Update(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="Id">The topic identifier</param>
        public void DeleteTopic(int Id)
        {
            var forumTopic = GetTopicById(Id);
            if (forumTopic == null)
                return;

            int userId = forumTopic.UserId;
            int forumId = forumTopic.ForumId;

            //delete topic
            _forumTopicRepository.Delete(forumTopic);

            //delete forum subscriptions
            var queryFs = from ft in _forumSubscriptionRepository.Table
                          where ft.TopicId == Id
                          select ft;
            var forumSubscriptions = queryFs.ToList();
            foreach (var fs in forumSubscriptions)
            {
                _forumSubscriptionRepository.Delete(fs);
            }

            //update stats
            UpdateForumStats(forumId);
            UpdateUserStats(userId);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="Id">The topic identifier</param>
        /// <returns>Topic</returns>
        public ForumTopic GetTopicById(int Id)
        {
            return GetTopicById(Id, false);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="Id">The topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase topic views</param>
        /// <returns>Topic</returns>
        public ForumTopic GetTopicById(int Id, bool increaseViews)
        {
            if (Id == 0)
                return null;

            var query = from ft in _forumTopicRepository.Table
                        where ft.Id == Id
                        select ft;
            var forumTopic = query.SingleOrDefault();
            if (forumTopic == null)
                return null;

            if (increaseViews)
            {
                forumTopic.Views = ++forumTopic.Views;
                UpdateTopic(forumTopic);
            }

            return forumTopic;
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="forumId">The forum group identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Topics</returns>
        public IList<ForumTopic> GetAllTopics(int forumId,
            int userId, string keywords, ForumSearchTypeEnum searchType,
            int limitDays, int pageIndex, int pageSize)
        {
            DateTime? limitDate = null;
            if (limitDays > 0)
            {
                limitDate = DateTime.UtcNow.AddDays(-limitDays);
            }
            var query1 = from ft in _forumTopicRepository.Table
                         join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (userId == 0 || ft.UserId == userId) &&
                         (
                             // following line causes SqlCeException on SQLCE4 (comparing parameter to IS NULL in query) -works on SQL Server
                             // String.IsNullOrEmpty(keywords) ||
                         ((searchType == ForumSearchTypeEnum.All || searchType == ForumSearchTypeEnum.TopicTitlesOnly) && ft.Subject.Contains(keywords)) ||
                         ((searchType == ForumSearchTypeEnum.All || searchType == ForumSearchTypeEnum.PostTextOnly) && fp.Text.Contains(keywords))) &&
                         (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                         select ft.Id;

            var query2 = from ft in _forumTopicRepository.Table
                         where query1.Contains(ft.Id)
                         orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                         select ft;

            return query2.ToList();
        }

        /// <summary>
        /// Gets active topics
        /// </summary>
        /// <param name="forumId">The forum group identifier</param>
        /// <param name="topicCount">Topic count</param>
        /// <returns>Topics</returns>
        public IList<ForumTopic> GetActiveTopics(int forumId, int topicCount)
        {
            var query1 = from ft in _forumTopicRepository.Table
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (ft.LastPostTime.HasValue)
                         select ft.Id;

            var query2 = from ft in _forumTopicRepository.Table
                         where query1.Contains(ft.Id)
                         orderby ft.LastPostTime descending
                         select ft;

            var forumTopics = query2.Take(topicCount).ToList();
            return forumTopics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to users</param>
        public void InsertTopic(ForumTopic forumTopic, bool sendNotifications)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            _forumTopicRepository.Insert(forumTopic);

            //update stats
            UpdateForumStats(forumTopic.ForumId);

            //cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);


            //send notifications
            if (sendNotifications)
            {
                var forum = forumTopic.Forum;
                var subscriptions = GetAllSubscriptions(0, forum.Id, 0, 0, int.MaxValue);

                foreach (var subscription in subscriptions)
                {
                    if (subscription.UserId == forumTopic.UserId)
                        continue;

                    // TODO send message about new topic to forum subscribers
                    //IoC.Resolve<IMessageService>().SendNewForumTopicMessage(subscription.User,
                    //    forumTopic, forum, NopContext.Current.WorkingLanguage.LanguageId);
                }
            }
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        public void UpdateTopic(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            _forumTopicRepository.Update(forumTopic);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Moves the topic
        /// </summary>
        /// <param name="Id">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>Moved topic</returns>
        public ForumTopic MoveTopic(int Id, int newForumId)
        {
            var forumTopic = GetTopicById(Id);
            if (forumTopic == null)
                return forumTopic;

            if (this.IsUserAllowedToMoveTopic(_workContext.CurrentCustomer, forumTopic))
            {
                int previousForumId = forumTopic.ForumId;
                var newForum = GetForumById(newForumId);

                if (newForum != null)
                {
                    if (previousForumId != newForumId)
                    {
                        forumTopic.ForumId = newForum.Id;
                        forumTopic.UpdatedOn = DateTime.UtcNow;
                        UpdateTopic(forumTopic);

                        //update forum stats
                        UpdateForumStats(previousForumId);
                        UpdateForumStats(newForumId);
                    }
                }
            }
            return forumTopic;
        }

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="forumPostId">The post identifier</param>
        public void DeletePost(int forumPostId)
        {
            var forumPost = GetPostById(forumPostId);
            if (forumPost == null)
                return;

            int Id = forumPost.TopicId;
            int userId = forumPost.UserId;
            var forumTopic = this.GetTopicById(Id);
            int forumId = forumTopic.ForumId;

            //delete topic if it was the first post
            bool deleteTopic = false;
            if (forumTopic != null)
            {
                ForumPost firstPost = forumTopic.FirstPost;
                if (firstPost != null && firstPost.Id == forumPostId)
                {
                    deleteTopic = true;
                }
            }

            //delete forum post
            _forumPostRepository.Delete(forumPost);

            //delete topic
            if (deleteTopic)
            {
                DeleteTopic(Id);
            }

            //update stats
            if (!deleteTopic)
                UpdateForumTopicStats(Id);
            UpdateForumStats(forumId);
            UpdateUserStats(userId);

            //clear cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

        }

        /// <summary>
        /// Gets a post
        /// </summary>
        /// <param name="forumPostId">The post identifier</param>
        /// <returns>Post</returns>
        public ForumPost GetPostById(int forumPostId)
        {
            if (forumPostId == 0)
                return null;

            var query = from fp in _forumPostRepository.Table
                        where fp.Id == forumPostId
                        select fp;
            var forumPost = query.SingleOrDefault();

            return forumPost;
        }

        /// <summary>
        /// Gets all posts
        /// </summary>
        /// <param name="Id">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        public PagedList<ForumPost> GetAllPosts(int Id,
            int userId, string keywords, int pageIndex, int pageSize)
        {
            return GetAllPosts(Id, userId, keywords, true,
                pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all posts
        /// </summary>
        /// <param name="Id">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        public PagedList<ForumPost> GetAllPosts(int Id, int userId,
            string keywords, bool ascSort, int pageIndex, int pageSize)
        {
            var query = _forumPostRepository.Table;
            if (Id > 0)
                query = query.Where(fp => Id == fp.TopicId);
            if (userId > 0)
                query = query.Where(fp => userId == fp.UserId);
            if (!String.IsNullOrEmpty(keywords))
                query = query.Where(fp => fp.Text.Contains(keywords));
            if (ascSort)
                query = query.OrderBy(fp => fp.CreatedOn).ThenBy(fp => fp.Id);
            else
                query = query.OrderByDescending(fp => fp.CreatedOn).ThenBy(fp => fp.Id);

            var forumPosts = new PagedList<ForumPost>(query, pageIndex, pageSize);

            return forumPosts;
        }

        /// <summary>
        /// Inserts a post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to users</param>
        public void InsertPost(ForumPost forumPost, bool sendNotifications)
        {
            if (forumPost == null)
                throw new ArgumentNullException("forumPost");

            _forumPostRepository.Insert(forumPost);

            //update stats
            int userId = forumPost.UserId;
            var forumTopic = this.GetTopicById(forumPost.TopicId);
            int forumId = forumTopic.ForumId;
            UpdateForumTopicStats(forumPost.TopicId);
            UpdateForumStats(forumId);
            UpdateUserStats(userId);

            //clear cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //notifications
            if (sendNotifications)
            {
                var forum = forumTopic.Forum;
                var subscriptions = GetAllSubscriptions(0, 0,
                    forumTopic.Id, 0, int.MaxValue);

                foreach (ForumSubscription subscription in subscriptions)
                {
                    if (subscription.UserId == forumPost.UserId)
                        continue;

                    // TODO send message about new post to topic subscribers
                    //IoC.Resolve<IMessageService>().SendNewForumPostMessage(subscription.User,
                    //    forumPost, forumTopic, forum,
                    //    NopContext.Current.WorkingLanguage.LanguageId);
                }
            }
        }

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        public void UpdatePost(ForumPost forumPost)
        {
            //validation
            if (forumPost == null)
                throw new ArgumentNullException("forumPost");

            _forumPostRepository.Update(forumPost);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="forumPrivateMessageId">The private message identifier</param>
        public void DeletePrivateMessage(int forumPrivateMessageId)
        {
            var privateMessage = GetPrivateMessageById(forumPrivateMessageId);
            if (privateMessage == null)
                return;
            _forumPrivateMessageRepository.Delete(privateMessage);
        }

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="forumPrivateMessageId">The private message identifier</param>
        /// <returns>Private message</returns>
        public PrivateMessage GetPrivateMessageById(int forumPrivateMessageId)
        {
            if (forumPrivateMessageId == 0)
                return null;

            var query = from pm in _forumPrivateMessageRepository.Table
                        where pm.Id == forumPrivateMessageId
                        select pm;
            var privateMessage = query.SingleOrDefault();

            return privateMessage;
        }

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="fromUserId">The user identifier who sent the message</param>
        /// <param name="toUserId">The user identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Private messages</returns>
        public PagedList<PrivateMessage> GetAllPrivateMessages(int fromUserId,
            int toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex, int pageSize)
        {
            var query = from pm in _forumPrivateMessageRepository.Table
                        where
                        (fromUserId == 0 || fromUserId == pm.FromUserId) &&
                        (toUserId == 0 || toUserId == pm.ToUserId) &&
                        (!isRead.HasValue || isRead.Value == pm.IsRead) &&
                        (!isDeletedByAuthor.HasValue || isDeletedByAuthor.Value == pm.IsDeletedByAuthor) &&
                        (!isDeletedByRecipient.HasValue || isDeletedByRecipient.Value == pm.IsDeletedByRecipient) &&
                        (String.IsNullOrEmpty(keywords) || pm.Subject.Contains(keywords)) &&
                        (String.IsNullOrEmpty(keywords) || pm.Text.Contains(keywords))
                        orderby pm.CreatedOn descending
                        select pm;
            var privateMessages = new PagedList<PrivateMessage>(query, pageIndex, pageSize);

            return privateMessages;
        }

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        public void InsertPrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
                throw new ArgumentNullException("privateMessage");

            _forumPrivateMessageRepository.Insert(privateMessage);

            var customerTo = _customerService.GetCustomerById(privateMessage.ToUserId);
            if (customerTo == null)
                throw new NopException("Recipient could not be loaded");

            // TODO notify customer about new PM
            //UI notification
            //customerTo.NotifiedAboutNewPrivateMessages = false;
            ////Email notification
            //if (this.NotifyAboutPrivateMessages)
            //{
            //    IoC.Resolve<IMessageService>().SendPrivateMessageNotification(privateMessage, NopContext.Current.WorkingLanguage.LanguageId);
            //}
        }

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        public void UpdatePrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
                throw new ArgumentNullException("privateMessage");

            if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
            {
                _forumPrivateMessageRepository.Delete(privateMessage);
            }
            else
            {
                _forumPrivateMessageRepository.Update(privateMessage);
            }
        }

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        public void DeleteSubscription(int forumSubscriptionId)
        {
            var forumSubscription = GetSubscriptionById(forumSubscriptionId);
            if (forumSubscription == null)
                return;

            _forumSubscriptionRepository.Delete(forumSubscription);
        }

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>Forum subscription</returns>
        public ForumSubscription GetSubscriptionById(int forumSubscriptionId)
        {
            if (forumSubscriptionId == 0)
                return null;

            var query = from fs in _forumSubscriptionRepository.Table
                        where fs.Id == forumSubscriptionId
                        select fs;
            var forumSubscription = query.SingleOrDefault();

            return forumSubscription;
        }

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum subscriptions</returns>
        public PagedList<ForumSubscription> GetAllSubscriptions(int userId, int forumId,
            int topicId, int pageIndex, int pageSize)
        {
            var fsQuery = from fs in _forumSubscriptionRepository.Table
                          join c in _customerRepository.Table on fs.UserId equals c.Id
                          where
                          (userId == 0 || fs.UserId == userId) &&
                          (forumId == 0 || fs.ForumId == forumId) &&
                          (topicId == 0 || fs.TopicId == topicId) &&
                          (c.Active && !c.Deleted)
                          select fs.SubscriptionGuid;

            var query = from fs in _forumSubscriptionRepository.Table
                        where fsQuery.Contains(fs.SubscriptionGuid)
                        orderby fs.CreatedOn descending, fs.SubscriptionGuid descending
                        select fs;

            var forumSubscriptions = new PagedList<ForumSubscription>(query, pageIndex, pageSize);
            return forumSubscriptions;
        }

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        public void InsertSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
                throw new ArgumentNullException("forumSubscription");

            _forumSubscriptionRepository.Insert(forumSubscription);
        }

        /// <summary>
        /// Updates the forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        public void UpdateSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
                throw new ArgumentNullException("forumSubscription");

            _forumSubscriptionRepository.Update(forumSubscription);
        }

        /// <summary>
        /// Check whether user is allowed to create new topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="forum">Forum</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToCreateTopic(Customer customer, Forum forum)
        {
            if (forum == null)
                return false;

            if (customer == null)
                return false;

            // TODO get AllowGuestsToCreateTopics from forum settings
            //if (customer.IsGuest() && !AllowGuestsToCreateTopics)
            //    return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            return true;
        }

        /// <summary>
        /// Check whether user is allowed to edit topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToEditTopic(Customer customer, ForumTopic topic)
        {
            if (topic == null)
                return false;

            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            //TODO get AllowCustomersToEditPosts from forum settings
            //if (this.AllowCustomersToEditPosts)
            //{
            //    bool ownTopic = customer.Id == topic.UserId;
            //    return ownTopic;
            //}

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to move topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToMoveTopic(Customer customer, ForumTopic topic)
        {
            if (topic == null)
                return false;

            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to delete topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToDeleteTopic(Customer customer, ForumTopic topic)
        {
            if (topic == null)
                return false;

            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            // TODO get AllowCustomersToDeletePosts from forum settings
            //if (this.AllowCustomersToDeletePosts)
            //{
            //    bool ownTopic = customer.Id == topic.UserId;
            //    return ownTopic;
            //}

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to create new post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToCreatePost(Customer customer, ForumTopic topic)
        {
            if (topic == null)
                return false;

            if (customer == null)
                return false;

            // TODO get AllowGuestsToCreatePosts from forum settings
            //if (customer.IsGuest() && !AllowGuestsToCreatePosts)
            //    return false;

            // TODO remove, unnecessary
            //if (customer.IsForumModerator)
            //    return true;

            return true;
        }

        /// <summary>
        /// Check whether user is allowed to edit post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToEditPost(Customer customer, ForumPost post)
        {
            if (post == null)
                return false;

            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            //TODO 
            //if (this.AllowCustomersToEditPosts)
            //{
            //    bool ownPost = customer.CustomerId == post.UserId;
            //    return ownPost;
            //}

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to delete post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToDeletePost(Customer customer, ForumPost post)
        {
            if (post == null)
                return false;

            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            // TODO get AllowCustomersToDeletePosts from forum settings
            //if (this.AllowCustomersToDeletePosts)
            //{
            //    bool ownPost = customer.Id == post.UserId;
            //    return ownPost;
            //}

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to set topic priority
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToSetTopicPriority(Customer customer)
        {
            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            // TODO
            //if (customer.IsForumModerator)
            //    return true;

            return false;
        }

        /// <summary>
        /// Check whether user is allowed to watch topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        public bool IsUserAllowedToSubscribe(Customer customer)
        {
            if (customer == null)
                return false;

            if (customer.IsGuest())
                return false;

            return true;
        }

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="Id">Topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>Page index</returns>
        public int CalculateTopicPageIndex(int Id, int pageSize, int postId)
        {
            int pageIndex = 0;
            var forumPosts = GetAllPosts(Id, 0,
                string.Empty, true, 0, int.MaxValue);

            for (int i = 0; i < forumPosts.TotalCount; i++)
            {
                if (forumPosts[i].Id == postId)
                {
                    if (pageSize > 0)
                    {
                        pageIndex = i / pageSize;
                    }
                }
            }

            return pageIndex;
        }
        #endregion
    }
}
