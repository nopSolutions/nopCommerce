using LinqToDB.Mapping;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    /// <summary>
    /// Represents a download mapping configuration
    /// </summary>
    public partial class DownloadMap : NopEntityTypeConfiguration<Download>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Download> builder)
        {
            builder.HasTableName(nameof(Download));

            builder.Property(download => download.DownloadGuid);
            builder.Property(download => download.UseDownloadUrl);
            builder.Property(download => download.DownloadUrl);
            builder.Property(download => download.DownloadBinary);
            builder.Property(download => download.ContentType);
            builder.Property(download => download.Filename);
            builder.Property(download => download.Extension);
            builder.Property(download => download.IsNew);
        }

        #endregion
    }
}