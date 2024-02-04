using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Languages;

namespace Nop.Plugin.Api.DTO.Categories
{
    //[Validator(typeof(CategoryDtoValidator))]
    [JsonObject(Title = "category")]
    public class CategoryDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value of used category template identifier
        /// </summary>
        [JsonProperty("category_template_id")]
        public int? CategoryTemplateId { get; set; }

        /// <summary>
        ///     Gets or sets the meta keywords
        /// </summary>
        [JsonProperty("meta_keywords")]
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     Gets or sets the meta description
        /// </summary>
        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        /// <summary>
        ///     Gets or sets the meta title
        /// </summary>
        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        /// <summary>
        ///     Gets or sets the parent category identifier
        /// </summary>
        [JsonProperty("parent_category_id")]
        public int? ParentCategoryId { get; set; }

        /// <summary>
        ///     Gets or sets the page size
        /// </summary>
        [JsonProperty("page_size")]
        public int? PageSize { get; set; }

        /// <summary>
        ///     Gets or sets the available customer selectable page size options
        /// </summary>
        [JsonProperty("page_size_options")]
        public string PageSizeOptions { get; set; }

        /// <summary>
        ///     Gets or sets the available price ranges
        /// </summary>
        [JsonProperty("price_ranges")]
        public string PriceRanges { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        [JsonProperty("show_on_home_page")]
        public bool? ShowOnHomePage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include this category in the top menu
        /// </summary>
        [JsonProperty("include_in_top_menu")]
        public bool? IncludeInTopMenu { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this category has discounts applied
        ///     <remarks>
        ///         The same as if we run category.AppliedDiscounts.Count > 0
        ///         We use this property for performance optimization:
        ///         if this property is set to false, then we do not need to load Applied Discounts navigation property
        ///     </remarks>
        /// </summary>
        [JsonProperty("has_discounts_applied")]
        public bool? HasDiscountsApplied { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity is published
        /// </summary>
        [JsonProperty("published")]
        public bool? Published { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance update
        /// </summary>
        [JsonProperty("updated_on_utc")]
        public DateTime? UpdatedOnUtc { get; set; }

        [JsonProperty("role_ids")]
        public List<int> RoleIds { get; set; }

        [JsonProperty("discount_ids")]
        public List<int> DiscountIds { get; set; }

        [JsonProperty("store_ids")]
        public List<int> StoreIds { get; set; }

        [JsonProperty("image")]
        public ImageDto Image { get; set; }

        [JsonProperty("se_name")]
        public string SeName { get; set; }
    }
}
