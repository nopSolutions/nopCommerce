using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SearchTermMap : NopEntityTypeConfiguration<SearchTerm>
    {
        public override void Configure(EntityTypeBuilder<SearchTerm> builder)
        {
            base.Configure(builder);
            builder.ToTable("SearchTerm");
            builder.HasKey(st => st.Id);
        }
    }
}
