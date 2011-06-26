using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    public class TopicMap : EntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(sm => sm.Id);
            this.Property(sm => sm.SystemName);
            this.Property(sm => sm.Title).IsMaxLength();
            this.Property(sm => sm.Body).IsMaxLength();
            this.Property(sm => sm.MetaKeywords).IsMaxLength();
            this.Property(sm => sm.MetaDescription).IsMaxLength();
            this.Property(sm => sm.MetaTitle).IsMaxLength();
        }
    }
}
