using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Seo
{
    public partial class AclRecordMap : EntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            this.ToTable("AclRecord");
            this.HasKey(lp => lp.Id);

            this.Property(lp => lp.EntityName).IsRequired().HasMaxLength(400);
        }
    }
}