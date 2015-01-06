using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    public partial class TopicTemplateMap : NopEntityTypeConfiguration<TopicTemplate>
    {
        public TopicTemplateMap()
        {
            this.ToTable("TopicTemplate");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired().HasMaxLength(400);
            this.Property(t => t.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}