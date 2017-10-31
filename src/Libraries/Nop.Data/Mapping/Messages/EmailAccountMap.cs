using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class EmailAccountMap : NopEntityTypeConfiguration<EmailAccount>
    {
        public override void Configure(EntityTypeBuilder<EmailAccount> builder)
        {
            base.Configure(builder);
            builder.ToTable("EmailAccount");
            builder.HasKey(ea => ea.Id);

            builder.Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            builder.Property(ea => ea.DisplayName).HasMaxLength(255);
            builder.Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            builder.Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            builder.Property(ea => ea.Password).IsRequired().HasMaxLength(255);

            builder.Ignore(ea => ea.FriendlyName);
        }
    }
}