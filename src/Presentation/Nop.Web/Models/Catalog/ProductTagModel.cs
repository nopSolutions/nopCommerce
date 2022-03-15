using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductTagModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        //product tag seo update by Lancelot
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public int ProductCount { get; set; }
    }
}