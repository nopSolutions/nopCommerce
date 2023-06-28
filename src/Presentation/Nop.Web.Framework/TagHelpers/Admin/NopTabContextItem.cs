namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// Tab context item
    /// </summary>
    public partial class NopTabContextItem
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// Is default tab
        /// </summary>
        public bool IsDefault { set; get; }
    }
}