using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SearchTermMap : NopEntityTypeConfiguration<SearchTerm>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SearchTermMap()
        {
            this.ToTable("SearchTerm");
            this.HasKey(st => st.Id);
        }
    }
}
