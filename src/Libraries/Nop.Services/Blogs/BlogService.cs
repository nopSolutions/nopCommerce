using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Blog service
    /// </summary>
    public partial class BlogService : IBlogService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        
        #endregion

        #region Ctor

        public BlogService(CatalogSettings catalogSettings,
            ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _catalogSettings = catalogSettings;
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
            _blogCommentRepository = blogCommentRepository;
            _blogPostRepository = blogPostRepository;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Blog posts

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual async Task DeleteBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            await _blogPostRepository.Delete(blogPost);

            //event notification
            await _eventPublisher.EntityDeleted(blogPost);
        }

        /// <summary>
        /// Gets a blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        public virtual async Task<BlogPost> GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0)
                return null;

            return await _blogPostRepository.ToCachedGetById(blogPostId);
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="title">Filter by blog post title</param>
        /// <returns>Blog posts</returns>
        public virtual async Task<IPagedList<BlogPost>> GetAllBlogPosts(int storeId = 0, int languageId = 0,
            DateTime? dateFrom = null, DateTime? dateTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null)
        {
            var query = _blogPostRepository.Table;
            if (dateFrom.HasValue)
                query = query.Where(b => dateFrom.Value <= (b.StartDateUtc ?? b.CreatedOnUtc));
            if (dateTo.HasValue)
                query = query.Where(b => dateTo.Value >= (b.StartDateUtc ?? b.CreatedOnUtc));
            if (languageId > 0)
                query = query.Where(b => languageId == b.LanguageId);
            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));
            if (!showHidden)
            {
                query = query.Where(b => !b.StartDateUtc.HasValue || b.StartDateUtc <= DateTime.UtcNow);
                query = query.Where(b => !b.EndDateUtc.HasValue || b.EndDateUtc >= DateTime.UtcNow);
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = nameof(BlogPost) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;

                query = query.Distinct();
            }

            query = query.OrderByDescending(b => b.StartDateUtc ?? b.CreatedOnUtc);

            var blogPosts = await query.ToPagedList(pageIndex, pageSize);

            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual async Task<IPagedList<BlogPost>> GetAllBlogPostsByTag(int storeId = 0,
            int languageId = 0, string tag = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            tag = tag.Trim();

            //we load all records and only then filter them by tag
            var blogPostsAll = await GetAllBlogPosts(storeId: storeId, languageId: languageId, showHidden: showHidden);
            var taggedBlogPosts = new List<BlogPost>();
            foreach (var blogPost in blogPostsAll)
            {
                var tags = await ParseTags(blogPost);
                if (!string.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                    taggedBlogPosts.Add(blogPost);
            }

            //server-side paging
            var result = new PagedList<BlogPost>(taggedBlogPosts, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets all blog post tags
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        public virtual async Task<IList<BlogPostTag>> GetAllBlogPostTags(int storeId, int languageId, bool showHidden = false)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopBlogsDefaults.BlogTagsModelCacheKey, languageId, storeId, showHidden);

            var blogPostTags = await _staticCacheManager.Get(cacheKey, async () =>
            {
                var rezBlogPostTags = new List<BlogPostTag>();

                var blogPosts = await GetAllBlogPosts(storeId, languageId, showHidden: showHidden);

                foreach (var blogPost in blogPosts)
                {
                    var tags = await ParseTags(blogPost);
                    foreach (var tag in tags)
                    {
                        var foundBlogPostTag = rezBlogPostTags.Find(bpt =>
                            bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                        if (foundBlogPostTag == null)
                        {
                            foundBlogPostTag = new BlogPostTag
                            {
                                Name = tag,
                                BlogPostCount = 1
                            };
                            rezBlogPostTags.Add(foundBlogPostTag);
                        }
                        else
                            foundBlogPostTag.BlogPostCount++;
                    }
                }

                return rezBlogPostTags;
            });

            return blogPostTags;
        }

        /// <summary>
        /// Inserts a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual async Task InsertBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            await _blogPostRepository.Insert(blogPost);

            //event notification
            await _eventPublisher.EntityInserted(blogPost);
        }

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual async Task UpdateBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            await _blogPostRepository.Update(blogPost);

            //event notification
            await _eventPublisher.EntityUpdated(blogPost);
        }

        /// <summary>
        /// Returns all posts published between the two dates.
        /// </summary>
        /// <param name="blogPosts">Source</param>
        /// <param name="dateFrom">Date from</param>
        /// <param name="dateTo">Date to</param>
        /// <returns>Filtered posts</returns>
        public virtual Task<IList<BlogPost>> GetPostsByDate(IList<BlogPost> blogPosts, DateTime dateFrom, DateTime dateTo)
        {
            if (blogPosts == null)
                throw new ArgumentNullException(nameof(blogPosts));

            var rez = blogPosts
                .Where(p => dateFrom.Date <= (p.StartDateUtc ?? p.CreatedOnUtc) && (p.StartDateUtc ?? p.CreatedOnUtc).Date <= dateTo)
                .ToList();

            return Task.FromResult((IList<BlogPost>)rez);
        }

        /// <summary>
        /// Parse tags
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Tags</returns>
        public virtual Task<IList<string>> ParseTags(BlogPost blogPost) 
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.Tags == null)
                return Task.FromResult((IList<string>)new List<string>());

            var tags = blogPost.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag)).ToList();

            return Task.FromResult((IList<string>)tags);
        }

        /// <summary>
        /// Get a value indicating whether a blog post is available now (availability dates)
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool BlogPostIsAvailable(BlogPost blogPost, DateTime? dateTime = null)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.StartDateUtc.HasValue && blogPost.StartDateUtc.Value >= (dateTime ?? DateTime.UtcNow))
                return false;

            if (blogPost.EndDateUtc.HasValue && blogPost.EndDateUtc.Value <= (dateTime ?? DateTime.UtcNow))
                return false;

            return true;
        }

        #endregion

        #region Blog comments

        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="blogPostId">Blog post ID; 0 or null to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="commentText">Search comment text; null to load all records</param>
        /// <returns>Comments</returns>
        public virtual async Task<IList<BlogComment>> GetAllComments(int customerId = 0, int storeId = 0, int? blogPostId = null,
            bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
        {
            var query = _blogCommentRepository.Table;

            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved);

            if (blogPostId > 0)
                query = query.Where(comment => comment.BlogPostId == blogPostId);

            if (customerId > 0)
                query = query.Where(comment => comment.CustomerId == customerId);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (fromUtc.HasValue)
                query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

            if (toUtc.HasValue)
                query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(c => c.CommentText.Contains(commentText));

            query = query.OrderBy(comment => comment.CreatedOnUtc);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets a blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>Blog comment</returns>
        public virtual async Task<BlogComment> GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0)
                return null;

            return await _blogCommentRepository.ToCachedGetById(blogCommentId);
        }

        /// <summary>
        /// Get blog comments by identifiers
        /// </summary>
        /// <param name="commentIds">Blog comment identifiers</param>
        /// <returns>Blog comments</returns>
        public virtual async Task<IList<BlogComment>> GetBlogCommentsByIds(int[] commentIds)
        {
            if (commentIds == null || commentIds.Length == 0)
                return new List<BlogComment>();

            var query = from bc in _blogCommentRepository.Table
                        where commentIds.Contains(bc.Id)
                        select bc;
            var comments =  await query.ToListAsync();
            //sort by passed identifiers
            var sortedComments = new List<BlogComment>();
            foreach (var id in commentIds)
            {
                var comment = comments.Find(x => x.Id == id);
                if (comment != null)
                    sortedComments.Add(comment);
            }

            return sortedComments;
        }

        /// <summary>
        /// Get the count of blog comments
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
        /// <returns>Number of blog comments</returns>
        public virtual async Task<int> GetBlogCommentsCount(BlogPost blogPost, int storeId = 0, bool? isApproved = null)
        {
            var query = _blogCommentRepository.Table.Where(comment => comment.BlogPostId == blogPost.Id);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (isApproved.HasValue)
                query = query.Where(comment => comment.IsApproved == isApproved.Value);

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopBlogsDefaults.BlogCommentsNumberCacheKey, blogPost, storeId, isApproved);
            
            return await _staticCacheManager.Get(cacheKey, () => query.CountAsync());
        }

        /// <summary>
        /// Deletes a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual async Task DeleteBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            await _blogCommentRepository.Delete(blogComment);

            //event notification
            await _eventPublisher.EntityDeleted(blogComment);
        }

        /// <summary>
        /// Deletes blog comments
        /// </summary>
        /// <param name="blogComments">Blog comments</param>
        public virtual async Task DeleteBlogComments(IList<BlogComment> blogComments)
        {
            if (blogComments == null)
                throw new ArgumentNullException(nameof(blogComments));

            foreach (var blogComment in blogComments) 
                await DeleteBlogComment(blogComment);
        }

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual async Task InsertBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            await _blogCommentRepository.Insert(blogComment);

            //event notification
            await _eventPublisher.EntityInserted(blogComment);
        }

        /// <summary>
        /// Update a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual async Task UpdateBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            await _blogCommentRepository.Update(blogComment);

            //event notification
            await _eventPublisher.EntityUpdated(blogComment);
        }

        #endregion

        #endregion
    }
}