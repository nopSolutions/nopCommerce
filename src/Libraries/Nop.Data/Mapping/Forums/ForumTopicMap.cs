using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumTopicMap : NopEntityTypeConfiguration<ForumTopic>
    {
        public override void Configure(EntityTypeBuilder<ForumTopic> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_Topic");
            builder.HasKey(ft => ft.Id);
            builder.Property(ft => ft.Subject).IsRequired().HasMaxLength(450);
            builder.Ignore(ft => ft.ForumTopicType);

            builder.HasOne(ft => ft.Forum)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(ft => ft.ForumId);

            builder.HasOne(ft => ft.Customer)
               .WithMany()
               .IsRequired(true)
               .HasForeignKey(ft => ft.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
