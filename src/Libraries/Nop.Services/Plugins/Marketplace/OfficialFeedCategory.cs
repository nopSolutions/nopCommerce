namespace Nop.Services.Plugins.Marketplace
{
    /// <summary>
    /// Category for the official marketplace
    /// </summary>
    public class OfficialFeedCategory
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Parent category identifier
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}
