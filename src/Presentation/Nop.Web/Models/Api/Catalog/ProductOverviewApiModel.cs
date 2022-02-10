using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Api.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Api.Catalog
{
    public partial class ProductOverviewApiModel
    {
        public ProductOverviewApiModel()
        {
            ProductSpecificationModel = new ProductSpecificationApiModel();
        }
        public bool RibbonEnable { get; set; }
        public string RibbonText { get; set; }
        public string CategoryName { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public decimal PriceValue { get; set; }
        public int RatingSum { get; set; }
        public int TotalReviews { get; set; }
        public int PopularityCount { get; set; }
        public string VendorLogoPictureUrl { get; set; }
        public List<KeyValuePair<ProductAttributeValue, ProductAttributeMapping>> ProductAttributeValues { get; set; }
        public ProductSpecificationApiModel ProductSpecificationModel { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class ProductOverviewApiModelDTO
    {
        public List<ProductOverviewApiModel> ProductOverviewApiModels { get; set; }
    }
}