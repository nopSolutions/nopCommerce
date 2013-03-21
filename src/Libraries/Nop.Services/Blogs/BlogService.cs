using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Blog service
    /// </summary>
    public partial class BlogService : IBlogService
    {
        #region Fields

        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public BlogService(IRepository<BlogPost> blogPostRepository,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ICacheManager cacheManager, 
            IEventPublisher eventPublisher)
        {
            this._blogPostRepository = blogPostRepository;
            this._blogCommentRepository = blogCommentRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void DeleteBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            _blogPostRepository.Delete(blogPost);

            //event notification
            _eventPublisher.EntityDeleted(blogPost);
        }

        /// <summary>
        /// Gets a blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        public virtual BlogPost GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0)
                return null;

            return _blogPostRepository.GetById(blogPostId);
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
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPosts(int storeId, int languageId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _blogPostRepository.Table;
            if (dateFrom.HasValue)
                query = query.Where(b => dateFrom.Value <= b.CreatedOnUtc);
            if (dateTo.HasValue)
                query = query.Where(b => dateTo.Value >= b.CreatedOnUtc);
            if (languageId > 0)
                query = query.Where(b => languageId == b.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(b => !b.StartDateUtc.HasValue || b.StartDateUtc <= utcNow);
                query = query.Where(b => !b.EndDateUtc.HasValue || b.EndDateUtc >= utcNow);
            }

            if (storeId > 0)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table on bp.Id equals sm.EntityId into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || (sm.EntityName == "BlogPost" && storeId == sm.StoreId)
                        select bp;

                //only distinct blog posts (group by ID)
                query = from bp in query
                        group bp by bp.Id
                        into bpGroup
                        orderby bpGroup.Key
                        select bpGroup.FirstOrDefault();
            }

            query = query.OrderByDescending(b => b.CreatedOnUtc);
            
            var blogPosts = new PagedList<BlogPost>(query, pageIndex, pageSize);
            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPostsByTag(int storeId, int languageId, string tag,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            tag = tag.Trim();

            //we laod all records and only then filter them by tag
            var blogPostsAll = GetAllBlogPosts(storeId, languageId, null, null, 0, int.MaxValue, showHidden);
            var taggedBlogPosts = new List<BlogPost>();
            foreach (var blogPost in blogPostsAll)
            {
                var tags = blogPost.ParseTags();
                if (!String.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
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
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        public virtual IList<BlogPostTag> GetAllBlogPostTags(int storeId, int languageId, bool showHidden = false)
        {
            var blogPostTags = new List<BlogPostTag>();

            var blogPosts = GetAllBlogPosts(storeId, languageId, null, null, 0, int.MaxValue, showHidden);
            foreach (var blogPost in blogPosts)
            {
                var tags = blogPost.ParseTags();
                foreach (string tag in tags)
                {
                    var foundBlogPostTag = blogPostTags.Find(bpt => bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (foundBlogPostTag == null)
                    {
                        foundBlogPostTag = new BlogPostTag()
                        {
                            Name = tag,
                            BlogPostCount = 1
                        };
                        blogPostTags.Add(foundBlogPostTag);
                    }
                    else
                        foundBlogPostTag.BlogPostCount++;
                }
            }

            return blogPostTags;
        }

        /// <summary>
        /// Inserts an blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void InsertBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            _blogPostRepository.Insert(blogPost);

            //event notification
            _eventPublisher.EntityInserted(blogPost);
        }

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void UpdateBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            _blogPostRepository.Update(blogPost);

            //event notification
            _eventPublisher.EntityUpdated(blogPost);
        }
        
        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <returns>Comments</returns>
        public virtual IList<BlogComment> GetAllComments(int customerId)
        {
            var query = from c in _blogCommentRepository.Table
                        orderby c.CreatedOnUtc
                        where (customerId == 0 || c.CustomerId == customerId)
                        select c;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets a blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>Blog comment</returns>
        public virtual BlogComment GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0)
                return null;

            return _blogCommentRepository.GetById(blogCommentId);
        }

        /// <summary>
        /// Deletes a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual void DeleteBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException("blogComment");

            _blogCommentRepository.Delete(blogComment);
        }

        #endregion
    }
}
