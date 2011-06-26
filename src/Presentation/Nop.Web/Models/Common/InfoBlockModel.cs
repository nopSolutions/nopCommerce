using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class InfoBlockModel : BaseNopModel
    {
        public bool RecentlyAddedProductsEnabled { get; set; }
        public bool RecentlyViewedProductsEnabled { get; set; }
        public bool CompareProductsEnabled { get; set; }
        public bool BlogEnabled { get; set; }
        public bool SitemapEnabled { get; set; }
        public bool ForumEnabled { get; set; }
    }
}