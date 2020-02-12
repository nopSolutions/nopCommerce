using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents an email account mapping configuration
    /// </summary>
    public partial class EmailAccountMap : NopEntityTypeConfiguration<EmailAccount>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<EmailAccount> builder)
        {
            builder.HasTableName(nameof(EmailAccount));

            builder.Property(emailAccount => emailAccount.DisplayName).HasLength(255);
            builder.Property(emailAccount => emailAccount.Email).HasLength(255).IsNullable(false);
            builder.Property(emailAccount => emailAccount.Host).HasLength(255).IsNullable(false);
            builder.Property(emailAccount => emailAccount.Username).HasLength(255).IsNullable(false);
            builder.Property(emailAccount => emailAccount.Password).HasLength(255).IsNullable(false);
            builder.Property(emailaccount => emailaccount.Port);
            builder.Property(emailaccount => emailaccount.EnableSsl);

            builder.Ignore(emailAccount => emailAccount.FriendlyName);
        }

        #endregion
    }
}