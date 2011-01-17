using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Tax;

namespace Nop.Data.Mapping.Tax
{
    public class TaxProviderMap : EntityTypeConfiguration<TaxProvider>
    {
        public TaxProviderMap()
        {
            this.ToTable("TaxProvider");
            this.HasKey(p => p.Id);
        }
    }
}
