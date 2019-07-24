using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<Download> builder)
        {
            builder.ToTable(nameof(Download));
            builder.HasKey(download => download.Id);

            base.Configure(builder);
        }

        #endregion
    }
}