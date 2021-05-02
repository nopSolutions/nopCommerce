using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductOverviewApiModel : BaseNopEntityModel
    {
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
        public bool GluttenFree { get; set; }
        public bool Halal { get; set; }
        public bool AllergyFriendly { get; set; }
        public bool WellPacked { get; set; }
        public bool SustainablePackaging { get; set; }
        public bool FastAndReliable { get; set; }
        public bool ExcellentValue { get; set; }
        public bool FollowOrderNotes { get; set; }
        public bool Minimum { get; set; }
        public bool Average { get; set; }
        public bool Expensive { get; set; }
        public bool RibbonEnable { get; set; }
        public string RibbonText { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public int RatingSum { get; set; }
        public int TotalReviews { get; set; }
    }
}