using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.UI.Paging;
//using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcPromos.Models
{
    public record PromoListingModel : BasePageableModel
    {
        public string Name { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }
        //public CatalogProductsCommand PagingFilteringContext { get; set; }
        public string BannerImageUrl { get; set; }
        public string PromoFormPopup { get; set; }
        public int? OrderBy { get; set; }
        public bool AllowProductSorting { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public PromoListingModel()
        {
            AvailableSortOptions = new List<SelectListItem>();
        }
        // public class AbcPromoModel {
        //     public int AbcPromoId { get; set; }
        //     public int ProductId { get; set; }
        //     public int StoreId { get; set; }
        // }
        // public class HawthornePromoModel {
        //     public int HawthornePromoId { get; set; }
        //     public int ProductId { get; set; }
        //     public int StoreId { get; set; }
        // }
    }
}
