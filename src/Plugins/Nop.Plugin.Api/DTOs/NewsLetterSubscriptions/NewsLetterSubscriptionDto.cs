using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.NewsLetterSubscriptions
{
    [JsonObject(Title = "news_letter_subscription")]
    public class NewsLetterSubscriptionDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets whether the subscription is active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets whether the subscription is active
        /// </summary>
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        /// <summary>
        ///     Gets or sets created on utc date
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }
    }
}
