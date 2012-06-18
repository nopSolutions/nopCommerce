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
            this.Property(ear => ear.Email);
            this.Property(ear => ear.ExternalIdentifier);
            this.Property(ear => ear.ExternalDisplayIdentifier);
            this.Property(ear => ear.OAuthToken);
            this.Property(ear => ear.OAuthAccessToken);
            this.Property(ear => ear.ProviderSystemName);

            this.HasRequired(ear => ear.Customer)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.CustomerId);

        }
    }
}