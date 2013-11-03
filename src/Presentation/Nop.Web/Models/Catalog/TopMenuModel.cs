using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Web.Models.Catalog
{
    public partial class TopMenuModel : BaseNopModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }

        public bool BlogEnabled { get; set; }
        public bool RecentlyAddedProductsEnabled { get; set; }
        public bool ForumEnabled { get; set; }
    }
}