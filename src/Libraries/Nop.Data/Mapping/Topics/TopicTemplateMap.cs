using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TopicTemplateMap : NopEntityTypeConfiguration<TopicTemplate>
    {
        public override void Configure(EntityTypeBuilder<TopicTemplate> builder)
        {
            base.Configure(builder);
            builder.ToTable("TopicTemplate");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(400);
            builder.Property(t => t.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}