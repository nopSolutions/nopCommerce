using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductReviewListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }
    }
}