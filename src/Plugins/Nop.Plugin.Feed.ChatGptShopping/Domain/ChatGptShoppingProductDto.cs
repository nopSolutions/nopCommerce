using Newtonsoft.Json;

namespace Nop.Plugin.Feed.ChatGptShopping.Domain;

/// <summary>
/// Represents a product to sync
/// </summary>
public class ChatGptShoppingProductDto
{
    #region OpenAI Flags

    /// <summary>
    /// Controls whether the product can be surfaced in ChatGPT search results
    /// </summary>
    [JsonProperty("is_eligible_search")]
    public bool? IsEligibleSearch { get; set; }

    /// <summary>
    /// Allows direct purchase inside ChatGPT. is_eligible_search must be true for is_eligible_checkout to be enabled for the product
    /// </summary>
    [JsonProperty("is_eligible_checkout")]
    public bool? IsEligibleCheckout { get; set; }

    /// <summary>
    /// Controls whether the product can be processed for ChatGPT ads. Use is_eligible_ads only as a legacy alias
    /// </summary>
    [JsonProperty("is_ads_eligible")]
    public bool? IsAdsEligible { get; set; }

    #endregion

    #region Basic Product Data

    /// <summary>
    /// Merchant product ID (unique per variant)
    /// </summary>
    [JsonProperty("item_id")]
    public string ItemId { get; set; }

    /// <summary>
    /// Universal product identifier
    /// </summary>
    [JsonProperty("gtin")]
    public string Gtin { get; set; }

    /// <summary>
    /// Manufacturer part number
    /// </summary>
    [JsonProperty("mpn")]
    public string Mpn { get; set; }

    /// <summary>
    /// Product title
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// Full product description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Product detail page URL
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    #endregion

    #region Item Information

    /// <summary>
    /// Product brand
    /// </summary>
    [JsonProperty("brand")]
    public string Brand { get; set; }

    /// <summary>
    /// Condition of product
    /// </summary>
    /// <example>new</example>
    [JsonProperty("condition")]
    public string Condition { get; set; }

    /// <summary>
    /// Category path
    /// </summary>
    [JsonProperty("product_category")]
    public string ProductCategory { get; set; }

    #endregion

    #region Media

    /// <summary>
    /// Main product image URL
    /// </summary>
    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Extra images
    /// </summary>
    [JsonProperty("additional_image_urls")]
    public string[] AdditionalImageUrls { get; set; }

    /// <summary>
    /// Product video
    /// </summary>
    [JsonProperty("video_url")]
    public string VideoUrl { get; set; }

    #endregion

    #region Price & Promotions

    /// <summary>
    /// Regular price
    /// </summary>
    [JsonProperty("price")]
    public string Price { get; set; }

    #endregion

    #region Availability & Inventory

    /// <summary>
    /// Product availability
    /// </summary>
    [JsonProperty("availability")]
    public string Availability { get; set; }

    /// <summary>
    /// Availability date if pre-order
    /// </summary>
    [JsonProperty("availability_date")]
    public DateTime? AvailabilityDate { get; set; }

    #endregion

    #region Merchant Info

    /// <summary>
    /// Seller name
    /// </summary>
    [JsonProperty("seller_name")]
    public string SellerName { get; set; }

    /// <summary>
    /// Seller page
    /// </summary>
    [JsonProperty("seller_url")]
    public string SellerUrl { get; set; }

    #endregion

    #region Reviews and Q&A

    /// <summary>
    /// Number of product reviews
    /// </summary>
    [JsonProperty("review_count")]
    public int? ReviewCount { get; set; }

    /// <summary>
    /// Average review score
    /// </summary>
    [JsonProperty("star_rating")]
    public string StarRating { get; set; }

    #endregion

    #region Geo Tagging

    /// <summary>
    /// Target countries of the item (first entry used)
    /// </summary>
    [JsonProperty("target_countries")]
    public string[] TargetCountries { get; set; }

    /// <summary>
    /// Store country of the item
    /// </summary>
    [JsonProperty("store_country")]
    public string StoreCountry { get; set; }

    #endregion
}
