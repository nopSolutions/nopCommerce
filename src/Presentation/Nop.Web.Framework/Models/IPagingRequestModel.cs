
namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents a paging request model
    /// </summary>
    public partial interface IPagingRequestModel
    {
        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets draw
        /// </summary>
        string Draw { get; set; }

        /// <summary>
        /// Gets or sets skiping number of rows count
        /// </summary>
        int Start { get; set; }

        /// <summary>
        /// Gets or sets paging length 
        /// </summary>
        int Length { get; set; }
    }
}