using Nop.Core;
using Nop.Core.Domain.Seo;
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

    /// <summary>
    /// Return localized URLs
    /// </summary>
    /// <param name="routeName">Route name</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized URLs
    /// </returns>
    Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName,
        DateTime? dateTimeUpdatedOn = null, UpdateFrequency updateFreq = UpdateFrequency.Weekly);

    /// <summary>
    /// Return localized sitemap URL models
    /// </summary>
    /// <param name="entity">An entity which supports slug</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update URL</param>
    /// <param name="languageId">Language id; pass when URL for this entity should be used in one language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized sitemap URL models
    /// </returns>
    Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync<TEntity>(TEntity entity,
        DateTime? dateTimeUpdatedOn = null, UpdateFrequency updateFreq = UpdateFrequency.Weekly, int? languageId = null)
        where TEntity : BaseEntity, ISlugSupported;
}