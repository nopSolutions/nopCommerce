using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Discounts
{
    [Validator(typeof(DiscountValidator))]
    public partial class DiscountModel : BaseNopEntityModel
    {
        public DiscountModel()
        {
            AppliedToCategoryModels = new List<AppliedToCategoryModel>();
            AppliedToProductVariantModels = new List<AppliedToProductVariantModel>();
            AvailableDiscountRequirementRules = new List<SelectListItem>();
            DiscountRequirementMetaInfos = new List<DiscountRequirementMetaInfo>();
        }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountType")]
        public int DiscountTypeId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.UsePercentage")]
        public bool UsePercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountAmount")]
        public decimal DiscountAmount { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.RequiresCouponCode")]
        public bool RequiresCouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.CouponCode")]
        [AllowHtml]
        public string CouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountLimitation")]
        public int DiscountLimitationId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.LimitationTimes")]
        public int LimitationTimes { get; set; }


        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.AppliedToCategories")]
        public IList<AppliedToCategoryModel> AppliedToCategoryModels { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Discounts.Fields.AppliedToProductVariants")]
        public IList<AppliedToProductVariantModel> AppliedToProductVariantModels { get; set; }


        [NopResourceDisplayName("Admin.Promotions.Discounts.Requirements.DiscountRequirementType")]
        public string AddDiscountRequirement { get; set; }

        public IList<SelectListItem> AvailableDiscountRequirementRules { get; set; }

        public IList<DiscountRequirementMetaInfo> DiscountRequirementMetaInfos { get; set; }
        

        #region Nested classes

        public partial class DiscountRequirementMetaInfo : BaseNopModel
        {
            public int DiscountRequirementId { get; set; }
            public string RuleName { get; set; }
            public string ConfigurationUrl { get; set; }
        }

        public partial class DiscountUsageHistoryModel : BaseNopEntityModel
        {
            public int DiscountId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Discounts.History.Order")]
            public int OrderId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Discounts.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class AppliedToCategoryModel : BaseNopModel
        {
            public int CategoryId { get; set; }

            public string Name { get; set; }
        }

        public partial class AppliedToProductVariantModel : BaseNopModel
        {
            public int ProductVariantId { get; set; }

            public string FullProductName { get; set; }
        }
        #endregion
    }
}