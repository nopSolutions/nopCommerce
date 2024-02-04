#nullable enable

using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.Topics
{
    [JsonObject(Title ="topic")]
    public class TopicDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("system_name")]
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in sitemap
        /// </summary>
        [JsonProperty("include_in_sitemap")]
        public bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in top menu
        /// </summary>
        [JsonProperty("include_in_top_menu")]
        public bool? IncludeInTopMenu { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        [JsonProperty("include_in_footer_column1")]
        public bool? IncludeInFooterColumn1 { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        [JsonProperty("include_in_footer_column2")]
        public bool? IncludeInFooterColumn2 { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        [JsonProperty("include_in_footer_column3")]
        public bool? IncludeInFooterColumn3 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is accessible when a store is closed
        /// </summary>
        [JsonProperty("accessible_when_store_closed")]
        public bool? AccessibleWhenStoreClosed { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is password protected
        /// </summary>
        [JsonProperty("is_password_protected")]
        public bool? IsPasswordProtected { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        [JsonProperty("password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        [JsonProperty("published")]
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value of used topic template identifier
        /// </summary>
        [JsonProperty("topic_template_id")]
        public int? TopicTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        [JsonProperty("meta_keywords")]
        public string? MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        [JsonProperty("meta_description")]
        public string? MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        [JsonProperty("meta_title")]
        public string? MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        [JsonProperty("subject_to_acl")]
        public bool? SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        [JsonProperty("limited_to_stores")]
        public bool? LimitedToStores { get; set; }
    }
}
