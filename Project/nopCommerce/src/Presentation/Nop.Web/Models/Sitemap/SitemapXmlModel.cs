using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Sitemap;

public partial record SitemapXmlModel : BaseNopModel
{
    public string SitemapXmlPath { get; set; }
}