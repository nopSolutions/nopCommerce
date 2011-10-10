using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            PagingFilteringContext = new SearchPagingFilteringModel();
            Products = new List<ProductModel>();

            this.AvailableCategories = new List<SelectListItem>();
            this.AvailableManufacturers = new List<SelectListItem>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string Q { get; set; }
        /// <summary>
        /// Category ID
        /// </summary>
        [NopResourceDisplayName("Search.Category")]
        public int Cid { get; set; }
        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [NopResourceDisplayName("Search.Manufacturer")]
        public int Mid { get; set; }
        /// <summary>
        /// Price - From 
        /// </summary>
        [AllowHtml]
        public string Pf { get; set; }
        /// <summary>
        /// Price - To
        /// </summary>
        [AllowHtml]
        public string Pt { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [NopResourceDisplayName("Search.SearchInDescriptions")]
        public bool Sid { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [NopResourceDisplayName("Search.AdvancedSearch")]
        public bool As { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }


        public SearchPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductModel> Products { get; set; }
    }
}