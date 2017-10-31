using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CampaignMap : NopEntityTypeConfiguration<Campaign>
    {
        public override void Configure(EntityTypeBuilder<Campaign> builder)
        {
            base.Configure(builder);
            builder.ToTable("Campaign");
            builder.HasKey(ea => ea.Id);

            builder.Property(ea => ea.Name).IsRequired();
            builder.Property(ea => ea.Subject).IsRequired();
            builder.Property(ea => ea.Body).IsRequired();
        }
    }
}