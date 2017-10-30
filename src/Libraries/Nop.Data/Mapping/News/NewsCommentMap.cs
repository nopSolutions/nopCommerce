using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class NewsCommentMap : NopEntityTypeConfiguration<NewsComment>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NewsCommentMap()
        {
            this.ToTable("NewsComment");
            this.HasKey(comment => comment.Id);

            this.HasRequired(comment => comment.NewsItem)
                .WithMany(news => news.NewsComments)
                .HasForeignKey(comment => comment.NewsItemId);

            this.HasRequired(comment => comment.Customer)
                .WithMany()
                .HasForeignKey(comment => comment.CustomerId);

            this.HasRequired(comment => comment.Store)
                .WithMany()
                .HasForeignKey(comment => comment.StoreId);
        }
    }
}