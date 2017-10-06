using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    public partial class LogMap : NopEntityTypeConfiguration<Log>
    {
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            base.Configure(builder);
            builder.ToTable("Log");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.ShortMessage).IsRequired();
            builder.Property(l => l.IpAddress).HasMaxLength(200);

            builder.Ignore(l => l.LogLevel);

            builder.HasOne(l => l.Customer)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}