using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Categories
{
    public class NewsLetterSubscriptionsRootObject : ISerializableObject
    {
        public NewsLetterSubscriptionsRootObject()
        {
            NewsLetterSubscriptions = new List<NewsLetterSubscriptionDto>();
        }

        [JsonProperty("news_letter_subscriptions")]
        public IList<NewsLetterSubscriptionDto> NewsLetterSubscriptions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "news_letter_subscriptions";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (NewsLetterSubscriptionDto);
        }
    }
}