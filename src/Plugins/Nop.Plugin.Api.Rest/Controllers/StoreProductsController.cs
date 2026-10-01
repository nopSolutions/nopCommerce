using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// The catalog as a browsing client sees it
/// </summary>
/// <remarks>
/// A read only projection for storefront and mobile clients, kept apart from
/// <c>/api/rest/products</c> so that the admin view of a product and the shop view of it can evolve
/// separately. Part of the public store side of the API, so a customer token is accepted here and an
/// admin level credential is not. Follows the same read rule as every other operation: anonymous when
/// the read setting is off, behind a customer token when it is on.
/// </remarks>
[ApiController]
[Route("api/rest/store/products")]
public class StoreProductsController : ControllerBase
{
    #region Fields

    private readonly IProductService _productService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IProductTagService _productTagService;
    private readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public StoreProductsController(IProductService productService,
        IProductAttributeService productAttributeService,
        IManufacturerService manufacturerService,
        IProductTagService productTagService,
        IStoreMappingService storeMappingService)
    {
        _productService = productService;
        _productAttributeService = productAttributeService;
        _manufacturerService = manufacturerService;
        _productTagService = productTagService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a published product as a storefront client sees it
    /// </summary>
    /// <param name="id">The product identifier</param>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StoreProductDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        //an unpublished or deleted product is reported as missing rather than returned, because the
        //storefront must not be able to tell the difference
        if (product == null || !product.Published || product.Deleted)
            return NotFound();

        //published is not the same as listed: a product can be published yet mapped to other stores only,
        //so on a multi store install the header decides which catalog the caller is asking about. Omitting
        //it leaves the product unfiltered, which is how this endpoint behaved before the header existed.
        if (!await _storeMappingService.AuthorizeAsync(product, GetRequestedStoreId()))
            return NotFound();

        var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        var manufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
        var tags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        return Ok(product.ToStoreDto(combinations, manufacturers, tags));
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Reads the store the caller says it is browsing, if it said
    /// </summary>
    /// <returns>The store identifier, or 0 when the header is absent or not a number</returns>
    /// <remarks>
    /// The header can only narrow what is returned, never widen it: a store of 0 means "no filtering" and
    /// any value the caller sends is still checked against the product's own store mapping. So an
    /// unparseable or absent value degrades to the previous behaviour rather than failing the request,
    /// which keeps clients that predate the header working.
    /// </remarks>
    protected virtual int GetRequestedStoreId()
    {
        var header = Request.Headers[ApiRestDefaults.StoreIdHeaderName].ToString();

        return int.TryParse(header, out var storeId) && storeId > 0 ? storeId : 0;
    }

    #endregion
}