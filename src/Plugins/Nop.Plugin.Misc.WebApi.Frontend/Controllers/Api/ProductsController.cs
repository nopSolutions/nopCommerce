using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers.Api;

/// <summary>
/// Products API controller
/// </summary>
public class ProductsController : BaseApiController
{
    #region Fields

    private readonly IProductService _productService;
    private readonly IPictureService _pictureService;
    private readonly ICategoryService _categoryService;
    private readonly IManufacturerService _manufacturerService;

    #endregion

    #region Ctor

    public ProductsController(
        IWorkContext workContext,
        IPermissionService permissionService,
        ICustomerService customerService,
        IProductService productService,
        IPictureService pictureService,
        ICategoryService categoryService,
        IManufacturerService manufacturerService)
        : base(workContext, permissionService, customerService)
    {
        _productService = productService;
        _pictureService = pictureService;
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all products
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
    public async Task<IActionResult> GetProducts(int pageIndex = 0, int pageSize = 10)
    {
        if (!await HasPermissionAsync(StandardPermission.Catalog.PRODUCTS_VIEW))
            return Forbid();

        var products = await _productService.SearchProductsAsync(
            pageIndex: pageIndex,
            pageSize: pageSize,
            showHidden: true);

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            ShortDescription = p.ShortDescription,
            FullDescription = p.FullDescription,
            Sku = p.Sku,
            Price = p.Price,
            Published = p.Published,
            CreatedOnUtc = p.CreatedOnUtc
        });

        return Ok(productDtos);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProduct(int id)
    {
        if (!await HasPermissionAsync(StandardPermission.Catalog.PRODUCTS_VIEW))
            return Forbid();

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            FullDescription = product.FullDescription,
            Sku = product.Sku,
            Price = product.Price,
            Published = product.Published,
            CreatedOnUtc = product.CreatedOnUtc
        };

        return Ok(productDto);
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Product DTO
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short description
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Published status
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Created on UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }

    #endregion
}