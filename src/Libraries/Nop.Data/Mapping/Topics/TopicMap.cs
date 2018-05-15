using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TopicMap : NopEntityTypeConfiguration<Topic>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
        }
    }
}
