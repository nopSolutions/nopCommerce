
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Common;


namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            this.ToTable("Customer");
            this.HasKey(c => c.Id);
            this.Property(c => c.Email).IsRequired().HasMaxLength(255);
            this.Property(c => c.Username).IsRequired().HasMaxLength(255);
            this.Property(c => c.AdminComment).IsMaxLength();
            this.Property(c => c.CheckoutAttributes).IsMaxLength();
            this.Property(c => c.GiftCardCouponCodes).IsMaxLength();

            this.Ignore(c => c.TaxDisplayType);
            this.Ignore(c => c.VatNumberStatus);

            this.HasOptional(c => c.Language)
                .WithMany(l => l.Customers)
                .HasForeignKey(c => c.LanguageId).WillCascadeOnDelete(false);

            this.HasOptional(c => c.Currency)
                .WithMany(cur => cur.Customers)
                .HasForeignKey(c => c.CurrencyId).WillCascadeOnDelete(false);

            this.HasMany<Address>(c => c.Addresses)
                .WithMany()
                .Map(m => m.ToTable("CustomerAddresses"));
            this.HasOptional<Address>(c => c.BillingAddress);
            this.HasOptional<Address>(c => c.ShippingAddress);
        }
    }
}