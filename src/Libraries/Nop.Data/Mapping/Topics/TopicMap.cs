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
        }
    }
}
