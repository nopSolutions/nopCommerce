using LinqToDB.Mapping;
using Nop.Core.Domain.Gdpr;

namespace Nop.Data.Mapping.Gdpr
{
    /// <summary>
    /// Represents a GDPR log mapping configuration
    /// </summary>
    public partial class GdprLogMap : NopEntityTypeConfiguration<GdprLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GdprLog> builder)
        {
            builder.HasTableName(nameof(GdprLog));

            builder.Property(gdprlog => gdprlog.CustomerId);
            builder.Property(gdprlog => gdprlog.ConsentId);
            builder.Property(gdprlog => gdprlog.CustomerInfo);
            builder.Property(gdprlog => gdprlog.RequestTypeId);
            builder.Property(gdprlog => gdprlog.RequestDetails);
            builder.Property(gdprlog => gdprlog.CreatedOnUtc);

            builder.Ignore(gdpr => gdpr.RequestType);
        }

        #endregion
    }
}