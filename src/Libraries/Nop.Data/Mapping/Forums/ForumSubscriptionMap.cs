using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumSubscriptionMap : NopEntityTypeConfiguration<ForumSubscription>
    {
        public override void Configure(EntityTypeBuilder<ForumSubscription> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_Subscription");
            builder.HasKey(fs => fs.Id);

            builder.HasOne(fs => fs.Customer)
                .WithMany()
                .HasForeignKey(fs => fs.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
