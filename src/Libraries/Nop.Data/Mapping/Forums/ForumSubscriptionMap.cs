using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumSubscriptionMap : EntityTypeConfiguration<ForumSubscription>
    {
        public ForumSubscriptionMap()
        {
            this.ToTable("Forums_Subscription");
            this.HasKey(fs => fs.Id);
            this.Property(fs => fs.ForumId).IsOptional();
            this.Property(fs => fs.TopicId).IsOptional();

            this.HasRequired(fs => fs.Customer)
                .WithMany()
                .HasForeignKey(fs => fs.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}
