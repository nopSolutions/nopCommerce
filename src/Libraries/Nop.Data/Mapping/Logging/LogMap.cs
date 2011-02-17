

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Logging;


namespace Nop.Data.Mapping.Logging
{
    public partial class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.ToTable("Log");
            this.HasKey(l => l.Id);
            this.Property(l => l.Message).IsRequired().IsMaxLength();
            this.Property(l => l.Exception).IsMaxLength();
            this.Property(l => l.IpAddress).HasMaxLength(200);
            this.Property(l => l.PageUrl);
            this.Property(l => l.ReferrerUrl);
            this.Ignore(l => l.LogLevel);
        }
    }
}