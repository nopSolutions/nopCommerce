using Nop.Data.Mapping;
using Nop.Plugin.Feed.Froogle.Domain;

namespace Nop.Plugin.Feed.Froogle.Data
{
    public partial class GoogleProductRecordMap : NopEntityTypeConfiguration<GoogleProductRecord>
    {
        public GoogleProductRecordMap()
        {
            this.ToTable("GoogleProduct");
            this.HasKey(x => x.Id);
        }
    }
}