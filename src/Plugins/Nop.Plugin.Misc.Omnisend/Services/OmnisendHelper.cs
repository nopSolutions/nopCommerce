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
public class OmnisendHelper
{
    #region Fields

    private string _primaryStoreCurrencyCode;

    private readonly CurrencySettings _currencySettings;
    private readonly ICurrencyService _currencyService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public OmnisendHelper(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper)
    {
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _productAttributeParser = productAttributeParser;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the primary store currency code
    /// </summary>
    public async Task<string> GetPrimaryStoreCurrencyCodeAsync()
    {
        if (!string.IsNullOrEmpty(_primaryStoreCurrencyCode))
            return _primaryStoreCurrencyCode;

        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

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
            await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);

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
        var values = new { SeName = await _urlRecordService.GetSeNameAsync(product) };

        return await _nopUrlHelper.RouteGenericUrlAsync<Product>(values, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Gets the product picture URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<ProductDto.Image> GetProductPictureUrlAsync(Product product)
    {
        var picture = (await _pictureService
            .GetPicturesByProductIdAsync(product.Id, 1)).DefaultIfEmpty(null).FirstOrDefault();

        var (url, _) = await _pictureService.GetPictureUrlAsync(picture);

        var storeLocation = _webHelper.GetStoreLocation();

        if (!url.StartsWith(storeLocation))
            url = storeLocation + url;

        return new ProductDto.Image { ImageId = (picture?.Id ?? 0).ToString(), Url = url };
    }

    #endregion
}