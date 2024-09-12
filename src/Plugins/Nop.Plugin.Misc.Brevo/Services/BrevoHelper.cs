using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents the plugins helpers class
/// </summary>
public class BrevoHelper
{
    #region Fields

    private string _primaryStoreCurrencyCode;

    private readonly ICategoryService _categoryService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public BrevoHelper(        ICategoryService categoryService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper)
    {
        _categoryService = categoryService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _productAttributeParser = productAttributeParser;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

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
    /// Gets the category URL
    /// </summary>
    /// <param name="category">Category</param>
    public async Task<string> GetCategoryUrlAsync(Category category)
    {
        var values = new { SeName = await _urlRecordService.GetSeNameAsync(category) };
        return await _nopUrlHelper.RouteGenericUrlAsync<Category>(values, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Gets the product categories
    /// </summary>
    public async Task<List<string>> getProductCategories(Product product)
    {
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

        return productCategories.Select(pc => pc.CategoryId.ToString()).ToList();
    }

    /// <summary>
    /// Gets the product picture URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<string> GetProductPictureUrlAsync(Product product)
    {
        var picture = (await _pictureService
            .GetPicturesByProductIdAsync(product.Id, 1)).DefaultIfEmpty(null).FirstOrDefault();

        var (url, _) = await _pictureService.GetPictureUrlAsync(picture);

        var storeLocation = _webHelper.GetStoreLocation();

        if (!url.StartsWith(storeLocation))
            url = storeLocation + url;

        return url;
    }

    public Dictionary<string, string> ProductMetaInfo(Product product)
    {
        return new Dictionary<string, string>()
        {
            { nameof(product.MetaDescription), product.MetaDescription },
            { nameof(product.ShortDescription), product.ShortDescription },
            { nameof(product.StockQuantity), product.StockQuantity.ToString() },
            { nameof(product.Price), product.Price.ToString() },
            { nameof(product.VendorId), product.VendorId.ToString() },
        };
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

    #endregion
}
