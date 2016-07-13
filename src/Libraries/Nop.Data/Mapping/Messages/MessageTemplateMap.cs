using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class MessageTemplateMap : NopEntityTypeConfiguration<MessageTemplate>
    {
        public MessageTemplateMap()
        {
            this.ToTable("MessageTemplate");
            this.HasKey(mt => mt.Id);

            this.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            this.Property(mt => mt.BccEmailAddresses).HasMaxLength(200);
            this.Property(mt => mt.Subject).HasMaxLength(1000);
            this.Property(mt => mt.EmailAccountId).IsRequired();

            this.Ignore(mt => mt.DelayPeriod);
        }
    }
}