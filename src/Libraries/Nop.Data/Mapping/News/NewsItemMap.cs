using LinqToDB.Mapping;
using Nop.Core.Domain.News;
using Nop.Data.Data;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Represents a news mapping configuration
    /// </summary>
    public partial class NewsItemMap : NopEntityTypeConfiguration<NewsItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<NewsItem> builder)
        {
            builder.HasTableName(NopMappingDefaults.NewsItemTable);

            builder.HasColumn(newsItem => newsItem.Title).IsColumnRequired();
            builder.HasColumn(newsItem => newsItem.Short).IsColumnRequired();
            builder.HasColumn(newsItem => newsItem.Full).IsColumnRequired();
            builder.Property(newsItem => newsItem.MetaKeywords).HasLength(400);
            builder.Property(newsItem => newsItem.MetaTitle).HasLength(400);

            builder.Property(newsitem => newsitem.LanguageId);
            builder.Property(newsitem => newsitem.Published);
            builder.Property(newsitem => newsitem.StartDateUtc);
            builder.Property(newsitem => newsitem.EndDateUtc);
            builder.Property(newsitem => newsitem.AllowComments);
            builder.Property(newsitem => newsitem.LimitedToStores);
            builder.Property(newsitem => newsitem.MetaDescription);
            builder.Property(newsitem => newsitem.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(newsItem => newsItem.Language)
            //    .WithMany()
            //    .HasForeignKey(newsItem => newsItem.LanguageId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}