using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the plugins helpers class
/// </summary>
public class OmnisendHelper(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper)
{
    #region Fields

    private string _primaryStoreCurrencyCode;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the primary store currency code
    /// </summary>
    public async Task<string> GetPrimaryStoreCurrencyCodeAsync()
    {
        if (!string.IsNullOrEmpty(_primaryStoreCurrencyCode))
            return _primaryStoreCurrencyCode;

        var currency = await currencyService.GetCurrencyByIdAsync(currencySettings.PrimaryStoreCurrencyId);

        _primaryStoreCurrencyCode = currency.CurrencyCode;

        return _primaryStoreCurrencyCode;
    }

    /// <summary>
    /// Gets the product or product attribute combination SKU and ID
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Product attributes on XML format</param>
    public async Task<(string sku, int variantId)> GetSkuAndVariantIdAsync(Product product, string attributesXml)
    {
        var sku = product.Sku;
        var variantId = product.Id;

        var combination =
            await productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);

        if (combination == null)
            return (sku, variantId);

        sku = combination.Sku;
        variantId = combination.Id;

        return (sku, variantId);
    }

    /// <summary>
    /// Gets the product URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<string> GetProductUrlAsync(Product product)
    {
        var values = new { SeName = await urlRecordService.GetSeNameAsync(product) };

        return await nopUrlHelper.RouteGenericUrlAsync<Product>(values, webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Gets the product picture URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<ProductDto.Image> GetProductPictureUrlAsync(Product product)
    {
        var picture = (await pictureService
            .GetPicturesByProductIdAsync(product.Id, 1)).DefaultIfEmpty(null).FirstOrDefault();

        var (url, _) = await pictureService.GetPictureUrlAsync(picture);

        var storeLocation = webHelper.GetStoreLocation();

        if (!url.StartsWith(storeLocation))
            url = storeLocation + url;

        return new ProductDto.Image { ImageId = (picture?.Id ?? 0).ToString(), Url = url };
    }

    #endregion
}