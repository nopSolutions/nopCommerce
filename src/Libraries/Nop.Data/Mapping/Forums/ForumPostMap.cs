using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum post mapping configuration
    /// </summary>
    public partial class ForumPostMap : NopEntityTypeConfiguration<ForumPost>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ForumPost> builder)
        {
            builder.ToTable(NopMappingDefaults.ForumsPostTable);
            builder.HasKey(post => post.Id);

            builder.Property(post => post.Text).IsRequired();
            builder.Property(post => post.IPAddress).HasMaxLength(100);

            builder.HasOne(post => post.ForumTopic)
                .WithMany()
                .HasForeignKey(post => post.TopicId)
                .IsRequired();

            builder.HasOne(post => post.Customer)
               .WithMany()
               .HasForeignKey(post => post.CustomerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }

        #endregion
    }
}