using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Web.Models.Catalog
{
    public partial class CategoryMenuModel : BaseNopModel
    {
        public CategoryMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
        }
        public IList<CategorySimpleModel> Categories { get; set; }
    }
}