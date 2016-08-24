using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductReviewListModel : BaseNopModel
    {
        public ProductReviewListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchText")]
        [AllowHtml]
        public string SearchText { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchProduct")]
        public int SearchProductId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }
}