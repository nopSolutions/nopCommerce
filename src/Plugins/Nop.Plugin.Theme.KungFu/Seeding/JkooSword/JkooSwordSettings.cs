using System.Text.RegularExpressions;

namespace Nop.Plugin.Theme.KungFu.Seeding.JkooSword;

public static class JkooSwordSettings 
{
    public static string StoreUrl { get; set;  } = "https://www.jkoosword.com";

    public static string RobotsTxtUrl => $"{StoreUrl}/robots.txt";
    
    public static string SiteMapUrl => $"{StoreUrl}/sitemap.xml";
    
    public static Regex ChineseSwordsLinkRegex = new Regex("chinese-sword(s)-?", RegexOptions.IgnoreCase);
    
    public static Regex JapaneseSwordsLinkRegex  = new Regex("japanese-sword(s)-?", RegexOptions.IgnoreCase);
    
    public static Regex ChineseDaoBroadSwordLinkRegex  = new Regex("chinese-dao-broad-sword-?", RegexOptions.IgnoreCase);
    
    public static Regex ChineseJianSwordLinkRegex  = new Regex("chinese-jian-sword-?", RegexOptions.IgnoreCase);
    
    public static Regex KatanaShinkenLinkRegex  = new Regex("katana-shinken-?", RegexOptions.IgnoreCase);
    
    public static Regex WakizashiShortSwordLinkRegex  = new Regex("wakizashi-short-sword-?", RegexOptions.IgnoreCase);
    
    public static Regex TantoDaggerLinkRegex  = new Regex("tanto-dagger-?", RegexOptions.IgnoreCase);

    public static Regex NinjatoLinkRegex  = new Regex("ninjato-?", RegexOptions.IgnoreCase);
    
    public static Regex IaitoTrainingSwordLinkRegex  = new Regex("iaito-training-sword-?", RegexOptions.IgnoreCase);
    
    public static Regex TachiGreatSwordsLinkRegex  = new Regex("tachi-great-swords-?", RegexOptions.IgnoreCase);
    
    public static Regex DaishoLinkRegex  = new Regex("daisho-?", RegexOptions.IgnoreCase);
    
    public static Regex ImageTextLocatorRegex  = new Regex("image-set\\(url\\(&quot;(.*?)&quot;\\)");
    
    public static Regex ImageLinkRegex  = new Regex("https:\\/\\/(.*?).cloudfront.net\\/images\\/.*?\\/.+?\\.jpg");
    
    
    // <div class="product-details__product-price ec-price-item" itemprop="price" content="199"><meta itemprop="priceCurrency" content="USD"><span class="details-product-price__value ec-price-item notranslate">$199.00</span><!----></div>
    // itemprop="price" content="199"
    // itemprop="priceCurrency" content="USD"
    public static Regex ProductItemPriceFinder  = new Regex("itemprop=\"price\" content=\"(.*?)\"", RegexOptions.IgnoreCase);
    public static Regex ProductPriceNumber = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
    public static Regex ProductItemCurrencyFinder  = new Regex("itemprop=\"priceCurrency\" content=\"(.*?)\"", RegexOptions.IgnoreCase);
    public static Regex ProductItemCurrencyCode  = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
    public static Regex ProductDescriptionDiv  = new Regex("<div id=\\\"productDescription\\\" class=\\\"product-details__product-description\\\" itemprop=\\\"description\\\">(.*?)<\\/div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // <h1 class="product-details__product-title ec-header-h3" itemprop="name">JKOO-Classic Dotanuki katana(同田贯)</h1>
    public static Regex ProductNameHeading  = new Regex("class=\"product-details__product-title ec-header-h3\" itemprop=\"name\\\">(.*?)<\\/h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    
    // <div class="product-details__product-sku ec-text-muted" itemprop="sku">SKU  TZ6550812</div>
    public static Regex ProductSkuSpan  = new Regex("class=\\\"product-details__product-sku ec-text-muted\\\" itemprop=\\\"sku\\\">SKU  (.*?)<\\/div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    
}

public static class JkooSwordProductExtensions
{
    public static bool IsParentCategory(this string input)
    {
        if (JkooSwordSettings.ChineseSwordsLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.JapaneseSwordsLinkRegex.IsMatch(input))
            return true;
        return false;
        
    }
    
    
    
    
    public static bool isSubCategory(this string input)
    {
        if (JkooSwordSettings.ChineseDaoBroadSwordLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.ChineseJianSwordLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.KatanaShinkenLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.WakizashiShortSwordLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.TantoDaggerLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.NinjatoLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.IaitoTrainingSwordLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.TachiGreatSwordsLinkRegex.IsMatch(input))
            return true;
        if (JkooSwordSettings.DaishoLinkRegex.IsMatch(input))
            return true;
        return false;
    }
    
    public static bool IsProductPage(this string input)
    {
        // this only shows on product pages
        // public static Regex ImageTextLocatorRegex  = new Regex("image-set\\(url\\(&quot;(.*?)&quot;\\)");
        return JkooSwordSettings.ImageTextLocatorRegex.IsMatch(input);
    }
    
    public static bool HasPrice(this string input)
    {
        return JkooSwordSettings.ProductItemPriceFinder.IsMatch(input) &&
               JkooSwordSettings.ProductItemCurrencyFinder.IsMatch(input);
    }

    public static decimal ProductPrice(this string input)
    {
        var match = JkooSwordSettings.ProductItemPriceFinder.Match(input);
        if (!match.Success) return 0m;
        var priceMatch = JkooSwordSettings.ProductPriceNumber.Match(match.Value);
        if (!priceMatch.Success) return 0m;
        var priceString = priceMatch.Groups[1].Value;
        if (decimal.TryParse(priceString, out var price))
        {
            return price;
        }
        return 0m;
        
    }

    public static string Currency(this string input)
    {
        var match = JkooSwordSettings.ProductItemCurrencyFinder.Match(input);
        if (!match.Success) return string.Empty;
        var currencyMatch = JkooSwordSettings.ProductItemCurrencyCode.Match(match.Value);
        if (!currencyMatch.Success) return string.Empty;
        var currencyString = currencyMatch.Groups[1].Value;
        return currencyString;
    }
    
    public static bool HasDescription(this string input)
    {
        return JkooSwordSettings.ProductDescriptionDiv.IsMatch(input);
    }
    
    public static string ProductDescription(this string input)
    {
        var match = JkooSwordSettings.ProductDescriptionDiv.Match(input);
        if (!match.Success) return string.Empty;
        var description = match.Groups[1].Value;
        return description;
    }
    
    public static bool HasName(this string input)
    {
        return JkooSwordSettings.ProductNameHeading.IsMatch(input);
    }
    public static string ProductName(this string input)
    {
        var match = JkooSwordSettings.ProductNameHeading.Match(input);
        if (!match.Success) return string.Empty;
        var name = match.Groups[1].Value;
        return name;
    }
    
    public static bool HasSku(this string input)
    {
        return JkooSwordSettings.ProductSkuSpan.IsMatch(input);
    }
    
    public static string ProductSku(this string input)
    {
        var match = JkooSwordSettings.ProductSkuSpan.Match(input);
        if (!match.Success) return string.Empty;
        var sku = match.Groups[1].Value;
        return sku;
    }
    
    public static string[] ImageLinks(this string input)
    {
        var match = JkooSwordSettings.ImageLinkRegex.Match(input);

        if (!match.Success) return [];
        
        var links = new List<string>();
        foreach (Match m in JkooSwordSettings.ImageLinkRegex.Matches(input))
        {
            links.Add(m.Value);
        }
        return links.ToArray();

    }
}
