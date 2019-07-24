using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum group mapping configuration
    /// </summary>
    public partial class ForumGroupMap : NopEntityTypeConfiguration<ForumGroup>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ForumGroup> builder)
        {
            builder.ToTable(NopMappingDefaults.ForumsGroupTable);
            builder.HasKey(forumGroup => forumGroup.Id);

            builder.Property(forumGroup => forumGroup.Name).HasMaxLength(200).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}