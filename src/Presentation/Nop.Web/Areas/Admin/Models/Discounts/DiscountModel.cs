using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Discounts;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts
{
    [Validator(typeof(DiscountValidator))]
    public partial class DiscountModel : BaseNopEntityModel
    {
        public DiscountModel()
        {
            AvailableDiscountRequirementRules = new List<SelectListItem>();
            AvailableRequirementGroups = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountType")]
        public int DiscountTypeId { get; set; }
        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountType")]
        public string DiscountTypeName { get; set; }

        //used for the list page
        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.TimesUsed")]
        public int TimesUsed { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.UsePercentage")]
        public bool UsePercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountAmount")]
        public decimal DiscountAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.MaximumDiscountAmount")]
        [UIHint("DecimalNullable")]
        public decimal? MaximumDiscountAmount { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.RequiresCouponCode")]
        public bool RequiresCouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.CouponCode")]
        public string CouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.IsCumulative")]
        public bool IsCumulative { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountLimitation")]
        public int DiscountLimitationId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.LimitationTimes")]
        public int LimitationTimes { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.MaximumDiscountedQuantity")]
        [UIHint("Int32Nullable")]
        public int? MaximumDiscountedQuantity { get; set; }
        
        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.AppliedToSubCategories")]
        public bool AppliedToSubCategories { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Requirements.DiscountRequirementType")]
        public string AddDiscountRequirement { get; set; }

        public IList<SelectListItem> AvailableDiscountRequirementRules { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Requirements.GroupName")]
        public string GroupName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Requirements.RequirementGroup")]
        public int RequirementGroupId { get; set; }

        public IList<SelectListItem> AvailableRequirementGroups { get; set; }

        #region Nested classes

        public partial class DiscountRequirementMetaInfo : BaseNopModel
        {
            public DiscountRequirementMetaInfo()
            {
                ChildRequirements = new List<DiscountRequirementMetaInfo>();
            }

            public int DiscountRequirementId { get; set; }
            public string RuleName { get; set; }
            public string ConfigurationUrl { get; set; }
            public int InteractionTypeId { get; set; }
            public int? ParentId { get; set; }
            public SelectList AvailableInteractionTypes { get; set; }
            public bool IsGroup { get; set; }
            public bool IsLastInGroup { get; set; }
            public IList<DiscountRequirementMetaInfo> ChildRequirements { get; set; }
        }

        public partial class DiscountUsageHistoryModel : BaseNopEntityModel
        {
            public int DiscountId { get; set; }
            
            public int OrderId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Discounts.History.CustomOrderNumber")]
            public string CustomOrderNumber { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Discounts.History.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Discounts.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class AppliedToCategoryModel : BaseNopModel
        {
            public int CategoryId { get; set; }

            public string CategoryName { get; set; }
        }

        public partial class AddCategoryToDiscountModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
            public string SearchCategoryName { get; set; }

            public int DiscountId { get; set; }

            public int[] SelectedCategoryIds { get; set; }
        }

        public partial class AppliedToManufacturerModel : BaseNopModel
        {
            public int ManufacturerId { get; set; }

            public string ManufacturerName { get; set; }
        }

        public partial class AddManufacturerToDiscountModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
            public string SearchManufacturerName { get; set; }

            public int DiscountId { get; set; }

            public int[] SelectedManufacturerIds { get; set; }
        }

        public partial class AppliedToProductModel : BaseNopModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }
        }

        public partial class AddProductToDiscountModel : BaseNopModel
        {
            public AddProductToDiscountModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int DiscountId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #endregion
    }
}