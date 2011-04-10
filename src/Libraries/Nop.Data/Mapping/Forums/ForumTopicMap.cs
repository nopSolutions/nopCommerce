using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Content.Forums
{
    public partial class ForumTopicMap : EntityTypeConfiguration<ForumTopic>
    {
        public ForumTopicMap()
        {
            this.ToTable("Forums_Topic");
            this.HasKey(ft => ft.Id);
            this.Property(ft => ft.Subject).IsRequired().HasMaxLength(450);
            this.Ignore(ft => ft.ForumTopicType);

            this.HasRequired(ft => ft.Forum)
                .WithMany(f => f.ForumTopics)
                .HasForeignKey(ft => ft.ForumId);

            this.HasRequired(ft => ft.Customer)
               .WithMany()
               .HasForeignKey(ft => ft.UserId)
               .WillCascadeOnDelete(false);
        }
    }
}
