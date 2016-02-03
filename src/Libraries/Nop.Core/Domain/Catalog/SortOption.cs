
namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a sort option
    /// </summary>
    public partial class SortOption : BaseEntity
    {
        /// <summary>
        /// Gets or sets the type of sorting
        /// </summary>
        public int SortOptionTypeID { get; set; }
        
        /// <summary>
        /// Gets or sets the value indicating whether the option is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the type of sorting
        /// </summary>
        public ProductSortingEnum SortOptionType
        {
            get
            {
                return (ProductSortingEnum)this.SortOptionTypeID;
            }
            set
            {
                this.SortOptionTypeID = (int)value;
            }
        }
    }
}
