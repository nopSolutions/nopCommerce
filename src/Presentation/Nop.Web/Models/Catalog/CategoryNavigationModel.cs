using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class CategoryNavigationModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int NumberOfParentCategories { get; set; }

        public bool DisplayNumberOfProducts { get; set; }
        public int NumberOfProducts { get; set; }

        public bool IsActive { get; set; }
    }
}