using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TopicMap : NopEntityTypeConfiguration<Topic>
    {
        public override void Configure(EntityTypeBuilder<Topic> builder)
        {
            base.Configure(builder);
            builder.ToTable("Topic");
            builder.HasKey(t => t.Id);
        }
    }
}
