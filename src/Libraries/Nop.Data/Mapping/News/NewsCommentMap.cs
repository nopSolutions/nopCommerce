using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Represents a news comment mapping configuration
    /// </summary>
    public partial class NewsCommentMap : NopEntityTypeConfiguration<NewsComment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<NewsComment> builder)
        {
            builder.ToTable(nameof(NewsComment));
            builder.HasKey(comment => comment.Id);

            builder.HasOne<NewsItem>().WithMany().HasForeignKey(comment => comment.NewsItemId).IsRequired();

            builder.HasOne<Customer>().WithMany().HasForeignKey(comment => comment.CustomerId).IsRequired();

            builder.HasOne<Store>().WithMany().HasForeignKey(comment => comment.StoreId).IsRequired();

            base.Configure(builder); 
        }

        #endregion
    }
}