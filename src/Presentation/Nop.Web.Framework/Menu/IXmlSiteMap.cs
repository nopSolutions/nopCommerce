namespace Nop.Web.Framework.Menu
{
    /// <summary>
    /// XML sitemap interface
    /// </summary>
    public partial interface IXmlSiteMap
    {
        /// <summary>
        /// Root node
        /// </summary>
        SiteMapNode RootNode { get; set; }

        /// <summary>
        /// Load sitemap
        /// </summary>
        /// <param name="physicalPath">Filepath to load a sitemap</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task LoadFromAsync(string physicalPath);
    }
}