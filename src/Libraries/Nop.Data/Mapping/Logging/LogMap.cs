using LinqToDB.Mapping;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    /// <summary>
    /// Represents a log mapping configuration
    /// </summary>
    public partial class LogMap : NopEntityTypeConfiguration<Log>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Log> builder)
        {
            builder.HasTableName(nameof(Log));

            builder.Property(logItem => logItem.ShortMessage).IsNullable(false);
            builder.Property(logItem => logItem.IpAddress).HasLength(200);
            builder.Property(logItem => logItem.LogLevelId);
            builder.Property(logItem => logItem.FullMessage);
            builder.Property(logItem => logItem.CustomerId);
            builder.Property(logItem => logItem.PageUrl);
            builder.Property(logItem => logItem.ReferrerUrl);
            builder.Property(logItem => logItem.CreatedOnUtc);

            builder.Ignore(logItem => logItem.LogLevel);
        }

        #endregion
    }
}