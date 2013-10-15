using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.FacebookShop.Models
{
    //just a simplified copy of \Nop.Web\Models\Catalog\SearchModel.cs
    public partial class SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            PagingFilteringContext = new SearchPagingFilteringModel();
            Products = new List<ProductOverviewModel>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string Q { get; set; }


        public SearchPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }
    }
}