using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// Exposes the authenticated customer's own data
/// </summary>
/// <remarks>
/// Every route here takes the customer from the credential presented, never from a query string, a
/// route value or a body field. That is the whole point of the customer token: a caller cannot reach
/// another customer's addresses, orders or wishlist, because there is no parameter to change.
/// <c>ApiKeyRateLimitMiddleware</c> already refuses these routes with a 403 unless the request carried
/// a customer token, so the identifier is known to be present by the time an action runs.
/// </remarks>
[ApiController]
[Route("api/rest/customer/me")]
public class CustomerMeController : ControllerBase
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ICustomWishlistService _customWishlistService;
    private readonly IProductService _productService;

    #endregion

    #region Ctor

    public CustomerMeController(ICustomerService customerService,
        IOrderService orderService,
        IShoppingCartService shoppingCartService,
        ICustomWishlistService customWishlistService,
        IProductService productService)
    {
        _customerService = customerService;
        _orderService = orderService;
        _shoppingCartService = shoppingCartService;
        _customWishlistService = customWishlistService;
        _productService = productService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns the profile of the customer the token was issued to
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<MeDto>> GetProfile()
    {
        var customer = await GetCurrentCustomerAsync();
        if (customer == null) return NotFound();

        return Ok(customer.ToMeDto());
    }

    /// <summary>
    /// Returns the addresses of the customer the token was issued to
    /// </summary>
    [HttpGet("addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses()
    {
        var customer = await GetCurrentCustomerAsync();
        if (customer == null) return NotFound();

        var addresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

        return Ok(addresses.Select(a => a.ToDto()).ToList());
    }

    /// <summary>
    /// Returns the orders the customer the token was issued to has placed
    /// </summary>
    /// <param name="pageIndex">Zero based page index</param>
    /// <param name="pageSize">Page size, capped at 200</param>
    /// <param name="orderStatusId">Only orders in this status; 0 to skip the filter</param>
    /// <param name="paymentStatusId">Only orders in this payment status; 0 to skip the filter</param>
    /// <param name="shippingStatusId">Only orders in this shipping status; 0 to skip the filter</param>
    [HttpGet("orders")]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
        [FromQuery] int? orderStatusId = null,
        [FromQuery] int? paymentStatusId = null,
        [FromQuery] int? shippingStatusId = null)
    {
        var customer = await GetCurrentCustomerAsync();
        if (customer == null) return NotFound();

        var index = PagingHelper.NormalizePageIndex(pageIndex);
        var size = PagingHelper.NormalizePageSize(pageSize);

        //the customer identifier comes from the credential, so the query is already scoped and a caller
        //cannot widen it
        var orders = await _orderService.SearchOrdersAsync(
            customerId: customer.Id,
            osIds: orderStatusId is > 0 ? new List<int> { orderStatusId.Value } : null,
            psIds: paymentStatusId is > 0 ? new List<int> { paymentStatusId.Value } : null,
            ssIds: shippingStatusId is > 0 ? new List<int> { shippingStatusId.Value } : null,
            billingLastName: string.Empty,
            pageIndex: index,
            pageSize: size);

        return Ok(orders.ToPagedResult().Map(o => o.ToDto()));
    }

    /// <summary>
    /// Returns the named wishlists the customer the token was issued to keeps
    /// </summary>
    /// <remarks>
    /// The default unnamed wishlist has no record in nopCommerce and so is not listed here. Its lines
    /// are reachable through <c>GET /api/rest/customer/me/wishlist</c>, where they carry a null
    /// <c>WishlistId</c>.
    /// </remarks>
    [HttpGet("wishlists")]
    public async Task<ActionResult<List<CustomerWishlistDto>>> GetWishlists()
    {
        var customer = await GetCurrentCustomerAsync();
        if (customer == null) return NotFound();

        var wishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customer.Id);
        var items = await GetWishlistLinesAsync(customer);

        var result = new List<CustomerWishlistDto>();
        foreach (var wishlist in wishlists)
        {
            var count = items.Count(item => item.CustomWishlistId == wishlist.Id);
            result.Add(wishlist.ToDto(count));
        }

        return Ok(result);
    }

    /// <summary>
    /// Returns the wishlist lines of the customer the token was issued to
    /// </summary>
    /// <param name="pageIndex">Zero based page index</param>
    /// <param name="pageSize">Page size, capped at 200</param>
    /// <param name="wishlistId">Only lines on this named wishlist; 0 or null to return every line</param>
    [HttpGet("wishlist")]
    public async Task<ActionResult<PagedResult<WishlistItemDto>>> GetWishlist(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
        [FromQuery] int? wishlistId = null)
    {
        var customer = await GetCurrentCustomerAsync();
        if (customer == null) return NotFound();

        var index = PagingHelper.NormalizePageIndex(pageIndex);
        var size = PagingHelper.NormalizePageSize(pageSize);

        var items = await GetWishlistLinesAsync(customer);

        var filtered = items
            .OrderBy(item => item.CreatedOnUtc)
            .ThenBy(item => item.Id)
            .AsEnumerable();

        //a null wishlistId means every line, which is what a caller asking without one expects
        if (wishlistId.HasValue)
            filtered = filtered.Where(item => item.CustomWishlistId == wishlistId.Value);

        var page = PagingHelper.ToInMemoryPagedResult(filtered.ToList(), index, size);

        //the products of the whole page are fetched in one call rather than one per line
        var products = await _productService.GetProductsByIdsAsync(page.Items.Select(i => i.ProductId).Distinct().ToArray());
        var productsById = products.ToDictionary(p => p.Id);

        return Ok(page.Map(item =>
        {
            productsById.TryGetValue(item.ProductId, out var product);
            return item.ToDto(product);
        }));
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Reads the customer identifier from the credential and loads the customer
    /// </summary>
    /// <returns>The customer, or null when the claim is absent or names a customer that no longer exists</returns>
    protected virtual async Task<Customer> GetCurrentCustomerAsync()
    {
        var customerId = CurrentCustomerId();

        if (customerId <= 0)
            return null;

        return await _customerService.GetCustomerByIdAsync(customerId);
    }

    /// <summary>
    /// Reads the customer identifier out of the authenticated principal
    /// </summary>
    /// <returns>The customer identifier, or 0 when the request did not carry a customer token</returns>
    protected virtual int CurrentCustomerId()
    {
        var claim = User?.FindFirst(ApiRestDefaults.CustomerIdClaim)?.Value;

        return int.TryParse(claim, out var customerId) ? customerId : 0;
    }

    /// <summary>
    /// Loads every wishlist line of a customer, across all stores and all named wishlists
    /// </summary>
    /// <param name="customer">The owning customer</param>
    /// <returns>The wishlist lines</returns>
    protected virtual async Task<IList<ShoppingCartItem>> GetWishlistLinesAsync(Customer customer)
    {
        //storeId 0 loads lines from every store, customWishlistId 0 loads lines from every named wishlist
        //and from the default unnamed one, which is represented by a null CustomWishlistId
        return await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist,
            storeId: 0, customWishlistId: 0);
    }

    #endregion
}
