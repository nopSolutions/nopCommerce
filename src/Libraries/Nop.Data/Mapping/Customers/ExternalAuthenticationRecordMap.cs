using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class ExternalAuthenticationRecordMap : EntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            this.ToTable("ExternalAuthenticationRecord");

            this.HasKey(ear => ear.Id);
            this.Property(ear => ear.Email).IsMaxLength();
            this.Property(ear => ear.ExternalIdentifier).IsMaxLength();
            this.Property(ear => ear.ExternalDisplayIdentifier).IsMaxLength();
            this.Property(ear => ear.OAuthToken).IsMaxLength();
            this.Property(ear => ear.OAuthAccessToken).IsMaxLength();
            this.Property(ear => ear.ProviderSystemName).IsMaxLength();

            this.HasRequired(ear => ear.Customer)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.CustomerId);

        }
    }
}