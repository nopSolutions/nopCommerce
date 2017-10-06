using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumPostMap : NopEntityTypeConfiguration<ForumPost>
    {
        public override void Configure(EntityTypeBuilder<ForumPost> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_Post");
            builder.HasKey(fp => fp.Id);
            builder.Property(fp => fp.Text).IsRequired();
            builder.Property(fp => fp.IPAddress).HasMaxLength(100);

            builder.HasOne(fp => fp.ForumTopic)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(fp => fp.TopicId);

            builder.HasOne(fp => fp.Customer)
               .WithMany()
               .IsRequired(true)
               .HasForeignKey(fp => fp.CustomerId)
               .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
