using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    public class TopicMap : EntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
            this.Property(t => t.SystemName);
            this.Property(t => t.Password);
            this.Property(t => t.Title);
            this.Property(t => t.Body);
            this.Property(t => t.MetaKeywords);
            this.Property(t => t.MetaDescription);
            this.Property(t => t.MetaTitle);
        }
    }
}
