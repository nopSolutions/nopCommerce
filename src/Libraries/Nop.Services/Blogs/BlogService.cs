using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Services.Events;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Blog service
    /// </summary>
    public partial class BlogService : IBlogService
    {
        #region Constants
        private const string BLOGPOST_BY_ID_KEY = "Nop.blogpost.id-{0}";
        private const string BLOGPOST_PATTERN_KEY = "Nop.blogpost.";
        #endregion

        #region Fields

        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public BlogService(IRepository<BlogPost> blogPostRepository, 
            ICacheManager cacheManager, IEventPublisher eventPublisher)
        {
            _blogPostRepository = blogPostRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
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

            _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);

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

            string key = string.Format(BLOGPOST_BY_ID_KEY, blogPostId);
            return _cacheManager.Get(key, () =>
            {
                var pv = _blogPostRepository.GetById(blogPostId);
                return pv;
            });
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPosts(int languageId,
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
            query = query.OrderByDescending(b => b.CreatedOnUtc);
            
            var blogPosts = new PagedList<BlogPost>(query, pageIndex, pageSize);
            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPostsByTag(int languageId, string tag,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            tag = tag.Trim();

            //we laod all records and only then filter them by tag
            var blogPostsAll = GetAllBlogPosts(languageId, null, null, 0, int.MaxValue, showHidden);
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
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        public virtual IList<BlogPostTag> GetAllBlogPostTags(int languageId, bool showHidden = false)
        {
            var blogPostTags = new List<BlogPostTag>();

            var blogPosts = GetAllBlogPosts(languageId, null, null, 0, int.MaxValue, showHidden);
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

            _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);

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

            _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(blogPost);
        }
        
        /// <summary>
        /// Update blog post comment totals
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void UpdateCommentTotals(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            int approvedCommentCount = 0;
            int notApprovedCommentCount = 0;
            var blogComments = blogPost.BlogComments;
            foreach (var bc in blogComments)
            {
                if (bc.IsApproved)
                    approvedCommentCount++;
                else
                    notApprovedCommentCount++;
            }

            blogPost.ApprovedCommentCount = approvedCommentCount;
            blogPost.NotApprovedCommentCount = notApprovedCommentCount;
            UpdateBlogPost(blogPost);
        }

        #endregion
    }
}
