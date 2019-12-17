using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.Validators;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{
    [JsonObject(Title = "manufacturer")]
    public class ManufacturerDto : BaseDto
    {
        private ImageDto _imageDto;
        private List<LocalizedNameDto> _localizedNames;
        private List<int> _storeIds;
        private List<int> _discountIds;
        private List<int> _roleIds;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the localized names
        /// </summary>
        [JsonProperty("localized_names")]
        public List<LocalizedNameDto> LocalizedNames
        {
            get
            {
                return _localizedNames;
            }
            set
            {
                _localizedNames = value;
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value of used manufacturer template identifier
        /// </summary>
        [JsonProperty("manufacturer_template_id")]
        public int ManufacturerTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        [JsonProperty("meta_keywords")]
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the parent picture identifier
        /// </summary>
        [JsonProperty("picture_id")]
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        [JsonProperty("allow_customers_to_select_page_size")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        [JsonProperty("page_size_options")]
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the available price ranges
        /// </summary>
        [JsonProperty("price_ranges")]
        public string PriceRanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        [JsonProperty("subject_to_acl")]
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        [JsonProperty("limited_to_stores")]
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        [JsonProperty("published")]
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [JsonProperty("updated_on_utc")]
        public DateTime UpdatedOnUtc { get; set; }

        [JsonProperty("role_ids")]
        public List<int> RoleIds
        {
            get
            {
                return _roleIds;
            }
            set
            {
                _roleIds = value;
            }
        }

        [JsonProperty("discount_ids")]
        public List<int> DiscountIds
        {
            get
            {
                return _discountIds;
            }
            set
            {
                _discountIds = value;
            }
        }

        [JsonProperty("store_ids")]
        public List<int> StoreIds
        {
            get
            {
                return _storeIds;
            }
            set
            {
                _storeIds = value;
            }
        }

        [JsonProperty("image")]
        public ImageDto Image
        {
            get
            {
                return _imageDto;
            }
            set
            {
                _imageDto = value;
            }
        }

        [JsonProperty("se_name")]
        public string SeName { get; set; }
    }
}