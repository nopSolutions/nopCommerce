using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer password mapping configuration
    /// </summary>
    public partial class CustomerPasswordMap : NopEntityTypeConfiguration<CustomerPassword>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerPassword> builder)
        {
            builder.HasTableName(nameof(CustomerPassword));

            builder.Property(password => password.CustomerId);
            builder.Property(password => password.Password);
            builder.Property(password => password.PasswordFormatId);
            builder.Property(password => password.PasswordSalt);
            builder.Property(password => password.CreatedOnUtc);

            builder.Ignore(password => password.PasswordFormat);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(password => password.Customer)
            //    .WithMany()
            //    .HasForeignKey(password => password.CustomerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}