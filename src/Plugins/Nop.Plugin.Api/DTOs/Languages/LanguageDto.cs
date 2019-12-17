using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Languages
{
    [JsonObject(Title = "language")]
    public class LanguageDto : BaseDto
    {
        private List<int> _storeIds;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        [JsonProperty("language_culture")]
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the unique SEO code
        /// </summary>
        [JsonProperty("unique_seo_code")]
        public string UniqueSeoCode { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        [JsonProperty("flag_image_file_name")]
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language supports "Right-to-left"
        /// </summary>
        [JsonProperty("rtl")]
        public bool? Rtl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        [JsonProperty("limited_to_stores")]
        public bool? LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the default currency for this language; 0 is set when we use the default currency display order
        /// </summary>
        [JsonProperty("default_currency_id")]
        public int? DefaultCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        [JsonProperty("published")]
        public bool? Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the store ids in which the language is enabled
        /// </summary>
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
    }
}
