using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CampaignMap : NopEntityTypeConfiguration<Campaign>
    {
        /// <summary>
        /// Ctor
        /// </summary>
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