using System.Collections.Generic;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Official feed manager (official plugins from www.nopCommerce.com site)
    /// </summary>
    public interface IOfficialFeedManager
    {
        /// <summary>
        /// Get categories
        /// </summary>
        /// <returns>Result</returns>
        IList<OfficialFeedCategory> GetCategories();

        /// <summary>
        /// Get versions
        /// </summary>
        /// <returns>Result</returns>
        IList<OfficialFeedVersion> GetVersions();

        /// <summary>
        /// Get all plugins
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="versionId">Version identifier</param>
        /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Plugins</returns>
        IPagedList<OfficialFeedPlugin> GetAllPlugins(int categoryId = 0,
            int versionId = 0, int price = 0,
            string searchTerm = "",
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
