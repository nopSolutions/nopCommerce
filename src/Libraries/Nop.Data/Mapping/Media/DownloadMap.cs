using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class DownloadMap : NopEntityTypeConfiguration<Download>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DownloadMap()
        {
            this.ToTable("Download");
            this.HasKey(p => p.Id);
            this.Property(p => p.DownloadBinary).IsMaxLength();
        }
    }
}