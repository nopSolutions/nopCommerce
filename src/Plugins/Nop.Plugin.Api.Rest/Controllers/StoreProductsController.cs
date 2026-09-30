using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// The catalog as a browsing client sees it
/// </summary>
/// <remarks>
/// A read only projection for storefront and mobile clients, kept apart from
/// <c>/api/rest/products</c> so that the admin view of a product and the shop view of it can evolve
/// separately. Follows the same credential rule as every other read: anonymous when the read setting is
/// off, behind the key when it is on.
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

    #endregion

    #region Ctor

    public StoreProductsController(IProductService productService,
        IProductAttributeService productAttributeService,
        IManufacturerService manufacturerService,
        IProductTagService productTagService)
    {
        _productService = productService;
        _productAttributeService = productAttributeService;
        _manufacturerService = manufacturerService;
        _productTagService = productTagService;
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

        var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        var manufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
        var tags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        return Ok(product.ToStoreDto(combinations, manufacturers, tags));
    }

    #endregion
}
