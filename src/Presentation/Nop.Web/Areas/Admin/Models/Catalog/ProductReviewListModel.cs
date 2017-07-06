using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class ProductReviewListModel : BaseNopModel
    {
        public ProductReviewListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableApprovedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchText")]
        public string SearchText { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchProduct")]
        public int SearchProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchApproved")]
        public int SearchApprovedId { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableApprovedOptions { get; set; }
    }
}