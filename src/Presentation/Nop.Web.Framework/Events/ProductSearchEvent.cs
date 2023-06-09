namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Product search event
    /// </summary>
    public partial class ProductSearchEvent
    {
        /// <summary>
        /// Search term
        /// </summary>
        public string SearchTerm { get; set; }
        /// <summary>
        /// Search in descriptions
        /// </summary>
        public bool SearchInDescriptions { get; set; }
        /// <summary>
        /// Category identifiers
        /// </summary>
        public IList<int> CategoryIds { get; set; }
        /// <summary>
        /// Manufacturer identifier
        /// </summary>
        public int ManufacturerId { get; set; }
        /// <summary>
        /// Language identifier
        /// </summary>
        public int WorkingLanguageId { get; set; }
        /// <summary>
        /// Vendor identifier
        /// </summary>
        public int VendorId { get; set; }
    }
}
