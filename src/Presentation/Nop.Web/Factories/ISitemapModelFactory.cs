using Nop.Web.Models.Sitemap;

namespace Nop.Web.Factories;

/// <summary>
/// Represents a sitemap model factory
/// </summary>
public partial interface ISitemapModelFactory
{
    /// <summary>
    /// Prepare the sitemap model
    /// </summary>
    /// <param name="pageModel">Sitemap page model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model
    /// </returns>
    Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel);

    /// <summary>
    /// Prepare sitemap model.
    /// This will build an XML sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="id">Sitemap identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model with sitemap.xml as string
    /// </returns>
    Task<SitemapXmlModel> PrepareSitemapXmlModelAsync(int id = 0);
}