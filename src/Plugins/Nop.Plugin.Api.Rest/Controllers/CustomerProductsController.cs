using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest.Controllers
{
    /// <summary>
    /// Exposes the products a customer keeps on their wishlist. A "customer product" is a wishlist line,
    /// stored as a ShoppingCartItem of ShoppingCartType.Wishlist; CustomWishlistId points at the named
    /// wishlist holding the line, or is null for the default (unnamed) wishlist.
    /// </summary>
    [ApiController]
    [Route("api/rest/customerproducts")]
    public class CustomerProductsController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomWishlistService _customWishlistService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public CustomerProductsController(ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            ICustomWishlistService customWishlistService,
            IProductService productService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            ShoppingCartSettings shoppingCartSettings)
        {
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _customWishlistService = customWishlistService;
            _productService = productService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _shoppingCartSettings = shoppingCartSettings;
        }

        // GET api/rest/customerproducts/{customerId}?page=1&pageSize=20
        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetAll(int customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            //the whole wishlist of a customer is a single bounded query, so paging is applied in memory
            var items = (await GetWishlistAsync(customer))
                .OrderBy(item => item.CreatedOnUtc)
                .ThenBy(item => item.Id)
                .ToList();

            var paged = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(await MapAsync(paged));
        }

        // GET api/rest/customerproducts/{customerId}/{id}
        [HttpGet("{customerId:int}/{id:int}")]
        public async Task<IActionResult> GetById(int customerId, int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var item = await GetOwnedItemAsync(customer, id);
            if (item == null) return NotFound();

            return Ok((await MapAsync([item])).FirstOrDefault());
        }

        // POST api/rest/customerproducts/{customerId}
        [HttpPost("{customerId:int}")]
        public async Task<IActionResult> Post(int customerId, [FromBody] AddCustomerProductRequest request)
        {
            if (request == null) return BadRequest(new { error = "A request body is required." });

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            if (request.Quantity <= 0)
                return BadRequest(new { error = await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive") });

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null) return NotFound();

            if (request.WishlistId.HasValue)
            {
                var error = await ValidateWishlistAsync(customer, request.WishlistId.Value);
                if (error != null) return BadRequest(new { error });
            }

            var storeId = request.StoreId ?? (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
            var attributesXml = request.AttributesXml ?? string.Empty;

            //returns the business rule warnings (availability, attributes, gift cards, rentals, limits)
            var warnings = await _shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.Wishlist,
                storeId, attributesXml, request.CustomerEnteredPrice ?? decimal.Zero,
                request.RentalStartDateUtc, request.RentalEndDateUtc, request.Quantity, true, request.WishlistId);
            if (warnings?.Any() == true)
                return BadRequest(new { error = string.Join(" ", warnings), warnings });

            var item = await ResolveAsync(customer, 0, product.Id, request.WishlistId, attributesXml);
            if (item == null) return NotFound();

            return CreatedAtAction(nameof(GetById), new { customerId, id = item.Id }, item);
        }

        // PATCH api/rest/customerproducts/{customerId}/{id}
        [HttpPatch("{customerId:int}/{id:int}")]
        public async Task<IActionResult> Patch(int customerId, int id, [FromBody] UpdateCustomerProductRequest request)
        {
            if (request == null) return BadRequest(new { error = "A request body is required." });

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var item = await GetOwnedItemAsync(customer, id);
            if (item == null) return NotFound();

            var attributesXml = item.AttributesXml;
            var productId = item.ProductId;

            if (request.Quantity.HasValue)
            {
                if (request.Quantity.Value <= 0)
                    return BadRequest(new { error = await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive") });

                //a quantity of zero deletes the line through this service, so zero is rejected in favour of the delete endpoint
                var warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer, item.Id,
                    attributesXml, item.CustomerEnteredPrice, item.RentalStartDateUtc, item.RentalEndDateUtc,
                    request.Quantity.Value, false);
                if (warnings?.Any() == true)
                    return BadRequest(new { error = string.Join(" ", warnings), warnings });
            }

            if (request.WishlistId.HasValue && request.WishlistId.Value != (item.CustomWishlistId ?? 0))
            {
                if (request.WishlistId.Value > 0)
                {
                    var error = await ValidateWishlistAsync(customer, request.WishlistId.Value);
                    if (error != null) return BadRequest(new { error });
                }

                //merges the line into an equal one when the target wishlist already holds the same product
                await _shoppingCartService.MoveItemToCustomWishlistAsync(item.Id,
                    request.WishlistId.Value > 0 ? request.WishlistId.Value : null);
            }

            var updated = await ResolveAsync(customer, id, productId, request.WishlistId ?? item.CustomWishlistId, attributesXml);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE api/rest/customerproducts/{customerId}/{id}
        [HttpDelete("{customerId:int}/{id:int}")]
        public async Task<IActionResult> Delete(int customerId, int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var item = await GetOwnedItemAsync(customer, id);
            if (item == null) return NotFound();

            //wishlist lines must not touch the customer's checkout data
            await _shoppingCartService.DeleteShoppingCartItemAsync(item, false);

            return NoContent();
        }

        // GET api/rest/customerproducts/{customerId}/wishlists
        [HttpGet("{customerId:int}/wishlists")]
        public async Task<IActionResult> GetWishlists(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var wishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customerId);
            var items = await GetWishlistAsync(customer);

            var dto = wishlists
                .Select(wishlist => wishlist.ToDto(items.Count(item => item.CustomWishlistId == wishlist.Id)))
                .ToList();

            return Ok(dto);
        }

        // POST api/rest/customerproducts/{customerId}/wishlists
        [HttpPost("{customerId:int}/wishlists")]
        public async Task<IActionResult> PostWishlist(int customerId, [FromBody] AddCustomerWishlistRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = await _localizationService.GetResourceAsync("Wishlist.NameRequired") });

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var error = await ValidateCustomWishlistsEnabledAsync(customer);
            if (error != null) return BadRequest(new { error });

            var name = request.Name.Trim();
            var wishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customerId);

            if (wishlists.Any(w => string.Equals(w.Name, name, StringComparison.InvariantCultureIgnoreCase)))
                return BadRequest(new { error = await _localizationService.GetResourceAsync("Wishlist.DuplicateName") });

            if (wishlists.Count >= _shoppingCartSettings.MaximumNumberOfCustomWishlist)
                return BadRequest(new
                {
                    error = string.Format(await _localizationService.GetResourceAsync("Wishlist.MaximumNumberReached"),
                        _shoppingCartSettings.MaximumNumberOfCustomWishlist)
                });

            var wishlist = new CustomWishlist
            {
                Name = name,
                CustomerId = customer.Id,
                CreatedOnUtc = DateTime.UtcNow
            };
            await _customWishlistService.AddCustomWishlistAsync(wishlist);

            return CreatedAtAction(nameof(GetWishlists), new { customerId }, wishlist.ToDto());
        }

        // DELETE api/rest/customerproducts/{customerId}/wishlists/{wishlistId}
        [HttpDelete("{customerId:int}/wishlists/{wishlistId:int}")]
        public async Task<IActionResult> DeleteWishlist(int customerId, int wishlistId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();

            var wishlist = await _customWishlistService.GetCustomWishlistByIdAsync(wishlistId);
            if (wishlist == null || wishlist.CustomerId != customerId) return NotFound();

            //the wishlist and the lines it holds are removed together
            var items = (await GetWishlistAsync(customer)).Where(item => item.CustomWishlistId == wishlistId).ToList();
            foreach (var item in items)
                await _shoppingCartService.DeleteShoppingCartItemAsync(item, false);

            await _customWishlistService.RemoveCustomWishlistAsync(wishlistId);

            return NoContent();
        }

        #region Utilities

        /// <summary>
        /// Loads every wishlist line of a customer, across all stores and all named wishlists.
        /// </summary>
        protected virtual async Task<IList<ShoppingCartItem>> GetWishlistAsync(Customer customer)
        {
            //storeId 0 loads lines from every store, customWishlistId 0 loads lines from every named wishlist
            //(leaving customWishlistId null would restrict the result to the default wishlist)
            return await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist,
                storeId: 0, customWishlistId: 0);
        }

        /// <summary>
        /// Gets a wishlist line by identifier, but only when the customer owns it.
        /// </summary>
        protected virtual async Task<ShoppingCartItem> GetOwnedItemAsync(Customer customer, int id)
        {
            var items = await GetWishlistAsync(customer);

            return items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Resolves the wishlist line produced by a write. Adding or moving can merge a line into an
        /// existing one, so the resulting identifier is not always the one that was written to.
        /// </summary>
        protected virtual async Task<CustomerProductDto> ResolveAsync(Customer customer, int id, int productId,
            int? wishlistId, string attributesXml)
        {
            var items = await GetWishlistAsync(customer);

            var item = items.FirstOrDefault(item => item.Id == id)
                ?? items.Where(item => item.ProductId == productId
                        && item.CustomWishlistId == wishlistId
                        && string.Equals(item.AttributesXml ?? string.Empty, attributesXml ?? string.Empty, StringComparison.Ordinal))
                    .OrderByDescending(item => item.UpdatedOnUtc)
                    .ThenByDescending(item => item.Id)
                    .FirstOrDefault();
            if (item == null) return null;

            return (await MapAsync([item])).FirstOrDefault();
        }

        /// <summary>
        /// Maps wishlist lines to DTOs, fetching the products of the whole set in a single call.
        /// </summary>
        protected virtual async Task<IList<CustomerProductDto>> MapAsync(IList<ShoppingCartItem> items)
        {
            if (items == null || items.Count == 0) return new List<CustomerProductDto>();

            var products = await _productService.GetProductsByIdsAsync(items.Select(item => item.ProductId).Distinct().ToArray());
            var productsById = products.ToDictionary(product => product.Id);

            return items.Select(item => item.ToDto(productsById.GetValueOrDefault(item.ProductId))).ToList();
        }

        /// <summary>
        /// Validates that a customer may add products to, or move products into, a named wishlist.
        /// </summary>
        protected virtual async Task<string> ValidateWishlistAsync(Customer customer, int wishlistId)
        {
            if (wishlistId <= 0)
                return await _localizationService.GetResourceAsync("Wishlist.NotFound");

            var error = await ValidateCustomWishlistsEnabledAsync(customer);
            if (error != null) return error;

            var wishlist = await _customWishlistService.GetCustomWishlistByIdAsync(wishlistId);
            if (wishlist == null || wishlist.CustomerId != customer.Id)
                return await _localizationService.GetResourceAsync("Wishlist.NotFound");

            return null;
        }

        /// <summary>
        /// Validates that the store allows named wishlists for this customer.
        /// </summary>
        protected virtual async Task<string> ValidateCustomWishlistsEnabledAsync(Customer customer)
        {
            if (await _customerService.IsGuestAsync(customer))
                return await _localizationService.GetResourceAsync("Wishlist.MultipleWishlistNotForGuest");

            if (!_shoppingCartSettings.AllowMultipleWishlist)
                return await _localizationService.GetResourceAsync("Wishlist.NotAllowMultipleWishlist");

            return null;
        }

        #endregion
    }
}
