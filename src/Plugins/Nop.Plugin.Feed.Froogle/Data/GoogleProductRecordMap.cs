using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.Feed.Froogle.Domain;

namespace Nop.Plugin.Feed.Froogle.Data
{
    public partial class GoogleProductRecordMap : EntityTypeConfiguration<GoogleProductRecord>
    {
        public GoogleProductRecordMap()
        {
            this.ToTable("GoogleProduct");
            this.HasKey(x => x.Id);
        }
    }
}