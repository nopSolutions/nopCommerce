using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class CampaignMap : NopEntityTypeConfiguration<Campaign>
    {
        public CampaignMap()
        {
            this.ToTable("Campaign");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Subject).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
        }
    }
}