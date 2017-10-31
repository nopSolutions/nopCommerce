using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class NewsCommentMap : NopEntityTypeConfiguration<NewsComment>
    {
        public override void Configure(EntityTypeBuilder<NewsComment> builder)
        {
            base.Configure(builder);
            builder.ToTable("NewsComment");
            builder.HasKey(comment => comment.Id);

            builder.HasOne(comment => comment.NewsItem)
                .WithMany(news => news.NewsComments)
                .IsRequired(true)
                .HasForeignKey(comment => comment.NewsItemId);

            builder.HasOne(comment => comment.Customer)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(comment => comment.CustomerId);

            builder.HasOne(comment => comment.Store)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(comment => comment.StoreId);
        }
    }
}