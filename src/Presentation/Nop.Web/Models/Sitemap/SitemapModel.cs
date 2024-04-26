using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Sitemap;

public partial record SitemapModel : BaseNopModel
{
    #region Ctor

    public SitemapModel()
    {
        Items = new List<SitemapItemModel>();
        PageModel = new SitemapPageModel();
    }

    #endregion

    #region Properties

    public List<SitemapItemModel> Items { get; set; }

    public SitemapPageModel PageModel { get; set; }

    #endregion

    #region Nested classes

    public partial record SitemapItemModel
    {
        public string GroupTitle { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    #endregion
}