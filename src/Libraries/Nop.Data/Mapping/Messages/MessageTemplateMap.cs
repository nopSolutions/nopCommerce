using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MessageTemplateMap : NopEntityTypeConfiguration<MessageTemplate>
    {
        public override void Configure(EntityTypeBuilder<MessageTemplate> builder)
        {
            base.Configure(builder);
            builder.ToTable("MessageTemplate");
            builder.HasKey(mt => mt.Id);

            builder.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            builder.Property(mt => mt.BccEmailAddresses).HasMaxLength(200);
            builder.Property(mt => mt.Subject).HasMaxLength(1000);
            builder.Property(mt => mt.EmailAccountId).IsRequired();

            builder.Ignore(mt => mt.DelayPeriod);
        }
    }
}