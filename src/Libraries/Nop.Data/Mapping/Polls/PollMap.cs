using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PollMap : NopEntityTypeConfiguration<Poll>
    {
        public override void Configure(EntityTypeBuilder<Poll> builder)
        {
            base.Configure(builder);
            builder.ToTable("Poll");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired();

            builder.HasOne(p => p.Language)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(p => p.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}