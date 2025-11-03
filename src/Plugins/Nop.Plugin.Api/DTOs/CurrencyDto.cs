using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO
{
	[JsonObject(Title = "currency")]
	public class CurrencyDto : BaseDto
	{
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("display_locale")]
        public string DisplayLocale { get; set; }

        [JsonProperty("custom_formatting")]
        public string CustomFormatting { get; set; }

        [JsonProperty("limited_to_stores")]
        public bool LimitedToStores { get; set; }

        [JsonProperty("published")]
        public bool Published { get; set; }
        
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        [JsonProperty("created_on_utc")]
        public DateTime CreatedOnUtc { get; set; }

        [JsonProperty("updated_on_utc")]
        public DateTime UpdatedOnUtc { get; set; }

        [JsonProperty("rounding_type")]
        public RoundingType RoundingType { get; set; }

        /// <summary>
        ///     Gets or sets the store ids in which the language is enabled
        /// </summary>
        [JsonProperty("store_ids")]
        public List<int> StoreIds { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoundingType
    {
        /// <summary>
        /// Default rounding (Match.Round(num, 2))
        /// </summary>
        Rounding001 = 0,

        /// <summary>
        /// <![CDATA[Prices are rounded up to the nearest multiple of 5 cents for sales ending in: 3¢ & 4¢ round to 5¢; and, 8¢ & 9¢ round to 10¢]]>
        /// </summary>
        Rounding005Up = 10,

        /// <summary>
        /// <![CDATA[Prices are rounded down to the nearest multiple of 5 cents for sales ending in: 1¢ & 2¢ to 0¢; and, 6¢ & 7¢ to 5¢]]>
        /// </summary>
        Rounding005Down = 20,

        /// <summary>
        /// <![CDATA[Round up to the nearest 10 cent value for sales ending in 5¢]]>
        /// </summary>
        Rounding01Up = 30,

        /// <summary>
        /// <![CDATA[Round down to the nearest 10 cent value for sales ending in 5¢]]>
        /// </summary>
        Rounding01Down = 40,

        /// <summary>
        /// <![CDATA[Sales ending in 1–24 cents round down to 0¢
        /// Sales ending in 25–49 cents round up to 50¢
        /// Sales ending in 51–74 cents round down to 50¢
        /// Sales ending in 75–99 cents round up to the next whole dollar]]>
        /// </summary>
        Rounding05 = 50,

        /// <summary>
        /// Sales ending in 1–49 cents round down to 0
        /// Sales ending in 50–99 cents round up to the next whole dollar
        /// For example, Swedish Krona
        /// </summary>
        Rounding1 = 60,

        /// <summary>
        /// Sales ending in 1–99 cents round up to the next whole dollar
        /// </summary>
        Rounding1Up = 70
    }
}
