using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.ProductWarehouseIventories;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
	public class ShoppingCartItemsController : BaseApiController
	{
		private readonly IDTOHelper _dtoHelper;
		private readonly IFactory<ShoppingCartItem> _factory;
		private readonly IProductAttributeConverter _productAttributeConverter;
		private readonly IProductService _productService;
		private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IStoreContext _storeContext;
		private readonly IPermissionService _permissionService;
		private readonly IAuthenticationService _authenticationService;

		public ShoppingCartItemsController(
			IShoppingCartItemApiService shoppingCartItemApiService,
			IJsonFieldsSerializer jsonFieldsSerializer,
			IAclService aclService,
			ICustomerService customerService,
			IStoreMappingService storeMappingService,
			IStoreService storeService,
			IDiscountService discountService,
			ICustomerActivityService customerActivityService,
			ILocalizationService localizationService,
			IShoppingCartService shoppingCartService,
			IProductService productService,
			IFactory<ShoppingCartItem> factory,
			IPictureService pictureService,
			IProductAttributeConverter productAttributeConverter,
			IDTOHelper dtoHelper,
			IStoreContext storeContext,
			IPermissionService permissionService,
			IAuthenticationService authenticationService)
			: base(jsonFieldsSerializer,
				   aclService,
				   customerService,
				   storeMappingService,
				   storeService,
				   discountService,
				   customerActivityService,
				   localizationService,
				   pictureService)
		{
			_shoppingCartItemApiService = shoppingCartItemApiService;
			_shoppingCartService = shoppingCartService;
			_productService = productService;
			_factory = factory;
			_productAttributeConverter = productAttributeConverter;
			_dtoHelper = dtoHelper;
			_storeContext = storeContext;
			_permissionService = permissionService;
			_authenticationService = authenticationService;
		}

		/// <summary>
		///     Receive a list of all shopping cart items of all customers if the customerId is NOT specified. Otherwise, return all shopping cart items of a customer.
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad Request</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/shopping_cart_items", Name = "GetShoppingCartItems")]
		[ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetShoppingCartItems([FromQuery] ShoppingCartItemsParametersModel parameters)
		{
			if (parameters.Limit.HasValue && (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit))
			{
				return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
			}

			if (parameters.Page.HasValue && (parameters.Page < Constants.Configurations.DefaultPageValue))
			{
				return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
			}

			ShoppingCartType? shoppingCartType = null;

			if (parameters.ShoppingCartType.HasValue && Enum.IsDefined(parameters.ShoppingCartType.Value))
			{
				shoppingCartType = (ShoppingCartType)parameters.ShoppingCartType.Value;
			}

			if (!await CheckPermissions(parameters.CustomerId, shoppingCartType))
			{
				return AccessDenied();
			}

			IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(parameters.CustomerId,
																										 parameters.CreatedAtMin,
																										 parameters.CreatedAtMax,
																										 parameters.UpdatedAtMin,
																										 parameters.UpdatedAtMax,
																										 parameters.Limit,
																										 parameters.Page,
																										 shoppingCartType);

			var shoppingCartItemsDtos = await shoppingCartItems
										.SelectAwait(async shoppingCartItem => await _dtoHelper.PrepareShoppingCartItemDTOAsync(shoppingCartItem))
										.ToListAsync();

			var shoppingCartsRootObject = new ShoppingCartItemsRootObject
			{
				ShoppingCartItems = shoppingCartItemsDtos
			};

			var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

			return new RawJsonActionResult(json);
		}

		[HttpGet]
		[Route("/api/shopping_cart_items/me", Name = "GetCurrentShoppingCart")]
		[ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> GetCurrentShoppingCart()
		{
			var customer = await _authenticationService.GetAuthenticatedCustomerAsync();

			if (customer is null)
			{
				return Error(HttpStatusCode.Unauthorized);
			}

			var shoppingCartType = ShoppingCartType.ShoppingCart;

			if (!await CheckPermissions(customer.Id, shoppingCartType))
			{
				return AccessDenied();
			}

			// load current shopping cart and return it as result of request
			var shoppingCartsRootObject = await LoadCurrentShoppingCartItems(shoppingCartType, customer);

			var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpPost]
		[Route("/api/shopping_cart_items", Name = "CreateShoppingCartItem")]
		[ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> CreateShoppingCartItem(
			[FromBody]
			ShoppingCartItemDto dto)
		{
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var shoppingCartItemDelta = new Delta<ShoppingCartItemDto>(objectPropertyNameValuePairs);
            shoppingCartItemDelta.Dto = dto;
            
			var newShoppingCartItem = await _factory.InitializeAsync();
			shoppingCartItemDelta.Merge(newShoppingCartItem);

			// We know that the product id and customer id will be provided because they are required by the validator.
			// TODO: validate
			var product = await _productService.GetProductByIdAsync(newShoppingCartItem.ProductId);

			if (product == null)
			{
				return Error(HttpStatusCode.NotFound, "product", "not found");
			}

			var customer = await CustomerService.GetCustomerByIdAsync(newShoppingCartItem.CustomerId);

			if (customer == null)
			{
				return Error(HttpStatusCode.NotFound, "customer", "not found");
			}

			if (!await CheckPermissions(shoppingCartItemDelta.Dto.CustomerId, (ShoppingCartType)shoppingCartItemDelta.Dto.ShoppingCartType))
			{
				return AccessDenied();
			}

			if (!product.IsRental)
			{
				newShoppingCartItem.RentalStartDateUtc = null;
				newShoppingCartItem.RentalEndDateUtc = null;
			}

			var attributesXml = await _productAttributeConverter.ConvertToXmlAsync(shoppingCartItemDelta.Dto.Attributes, product.Id);

			var currentStoreId = _storeContext.GetCurrentStore().Id;

			var warnings = await _shoppingCartService.AddToCartAsync(customer, product, (ShoppingCartType)shoppingCartItemDelta.Dto.ShoppingCartType, currentStoreId, attributesXml, 0M,
														  newShoppingCartItem.RentalStartDateUtc, newShoppingCartItem.RentalEndDateUtc,
														  shoppingCartItemDelta.Dto.Quantity ?? 1);

			if (warnings.Count > 0)
			{
				foreach (var warning in warnings)
				{
					ModelState.AddModelError("shopping cart item", warning);
				}

				return Error(HttpStatusCode.BadRequest);
			}

			// load current shopping cart and return it as result of request
			var shoppingCartsRootObject = await LoadCurrentShoppingCartItems((ShoppingCartType)shoppingCartItemDelta.Dto.ShoppingCartType, customer);

			var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpPost]
		[Route("/api/shopping_cart_items/batch", Name = "BatchCreateShoppingCartItems")]
		[ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> BatchCreateShoppingCartItems(
			[FromBody]
			ShoppingCartItemsCreateParametersModel dto)
		{
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var parameters = new Delta<ShoppingCartItemsCreateParametersModel>(objectPropertyNameValuePairs);
            parameters.Dto = dto;
            
			var customer = await CustomerService.GetCustomerByIdAsync(parameters.Dto.CustomerId);

			if (customer == null)
			{
				return Error(HttpStatusCode.NotFound, "customer", "not found");
			}

			if (!await CheckPermissions(parameters.Dto.CustomerId, (ShoppingCartType)parameters.Dto.ShoppingCartType))
			{
				return AccessDenied();
			}

			List<string> allWarnings = new();

			// add all items to cart
			foreach (var shoppingCartItemDto in parameters.Dto.Items)
			{
				Product product = null;
				if (shoppingCartItemDto.ProductId.HasValue)
				{
					product = await _productService.GetProductByIdAsync(shoppingCartItemDto.ProductId.Value);
				}

				if (product == null)
				{
					return Error(HttpStatusCode.NotFound, "product", "not found");
				}

				if (!product.IsRental)
				{
					shoppingCartItemDto.RentalStartDateUtc = null;
					shoppingCartItemDto.RentalEndDateUtc = null;
				}

				var attributesXml = await _productAttributeConverter.ConvertToXmlAsync(shoppingCartItemDto.Attributes, product.Id);

				var currentStoreId = _storeContext.GetCurrentStore().Id;

				var warnings = await _shoppingCartService.AddToCartAsync(customer, product, (ShoppingCartType)parameters.Dto.ShoppingCartType, currentStoreId, attributesXml, 0M,
															  shoppingCartItemDto.RentalStartDateUtc, shoppingCartItemDto.RentalEndDateUtc,
															  shoppingCartItemDto.Quantity ?? 1);
				allWarnings.AddRange(warnings);
			}

			// report warnings if any
			if (allWarnings.Count > 0)
			{
				foreach (var warning in allWarnings)
				{
					ModelState.AddModelError("shopping cart item", warning);
				}

				return Error(HttpStatusCode.BadRequest);
			}

			// load current shopping cart and return it as result of request
			var shoppingCartsRootObject = await LoadCurrentShoppingCartItems((ShoppingCartType)parameters.Dto.ShoppingCartType, customer);

			var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpPut]
		[Route("/api/shopping_cart_items/{id}", Name = "UpdateShoppingCartItem")]
		[ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> UpdateShoppingCartItem(
			[FromBody]
			ShoppingCartItemDto dto) // NOTE: id parameter is missing intentionally to fix the generation of swagger json
		{
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var shoppingCartItemDelta = new Delta<ShoppingCartItemDto>(objectPropertyNameValuePairs);
            shoppingCartItemDelta.Dto = dto;

            
			// We kno that the id will be valid integer because the validation for this happens in the validator which is executed by the model binder.
			var shoppingCartItemForUpdate = await _shoppingCartItemApiService.GetShoppingCartItemAsync(shoppingCartItemDelta.Dto.Id);

			if (shoppingCartItemForUpdate == null)
			{
				return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
			}

			if (!await CheckPermissions(shoppingCartItemForUpdate.CustomerId, shoppingCartItemForUpdate.ShoppingCartType))
			{
				return AccessDenied();
			}

			shoppingCartItemDelta.Merge(shoppingCartItemForUpdate);

			if (!(await _productService.GetProductByIdAsync(shoppingCartItemForUpdate.ProductId)).IsRental)
			{
				shoppingCartItemForUpdate.RentalStartDateUtc = null;
				shoppingCartItemForUpdate.RentalEndDateUtc = null;
			}

			if (shoppingCartItemDelta.Dto.Attributes != null)
			{
				shoppingCartItemForUpdate.AttributesXml = await _productAttributeConverter.ConvertToXmlAsync(shoppingCartItemDelta.Dto.Attributes, shoppingCartItemForUpdate.ProductId);
			}

			var customer = await CustomerService.GetCustomerByIdAsync(shoppingCartItemForUpdate.CustomerId);
			// The update time is set in the service.
			var warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer, shoppingCartItemForUpdate.Id,
																	   shoppingCartItemForUpdate.AttributesXml, shoppingCartItemForUpdate.CustomerEnteredPrice,
																	   shoppingCartItemForUpdate.RentalStartDateUtc, shoppingCartItemForUpdate.RentalEndDateUtc,
																	   shoppingCartItemForUpdate.Quantity);

			if (warnings.Count > 0)
			{
				foreach (var warning in warnings)
				{
					ModelState.AddModelError("shopping cart item", warning);
				}

				return Error(HttpStatusCode.BadRequest);
			}
			shoppingCartItemForUpdate = await _shoppingCartItemApiService.GetShoppingCartItemAsync(shoppingCartItemForUpdate.Id);

			// Preparing the result dto of the new product category mapping
			var newShoppingCartItemDto = await _dtoHelper.PrepareShoppingCartItemDTOAsync(shoppingCartItemForUpdate);

			var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

			shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

			var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpDelete]
		[Route("/api/shopping_cart_items/{id}", Name = "DeleteShoppingCartItem")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int id)
		{
			if (id <= 0)
			{
				return Error(HttpStatusCode.BadRequest, "id", "invalid id");
			}

			var shoppingCartItemForDelete = await _shoppingCartItemApiService.GetShoppingCartItemAsync(id);

			if (!await CheckPermissions(shoppingCartItemForDelete.CustomerId, shoppingCartItemForDelete.ShoppingCartType))
			{
				return AccessDenied();
			}

			await _shoppingCartService.DeleteShoppingCartItemAsync(shoppingCartItemForDelete);

			//activity log
			await CustomerActivityService.InsertActivityAsync("DeleteShoppingCartItem", await LocalizationService.GetResourceAsync("ActivityLog.DeleteShoppingCartItem"), shoppingCartItemForDelete);

			return new RawJsonActionResult("{}");
		}

		[HttpDelete]
		[Route("/api/shopping_cart_items", Name = "DeleteShoppingCartItems")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> DeleteShoppingCartItems([FromQuery] ShoppingCartItemsDeleteParametersModel parameters)
		{
			ShoppingCartType? shoppingCartType = null;

			if (parameters.ShoppingCartType.HasValue && Enum.IsDefined(parameters.ShoppingCartType.Value))
			{
				shoppingCartType = (ShoppingCartType)parameters.ShoppingCartType.Value;
			}

			int customerId;
			if (parameters.CustomerId.HasValue)
			{
				customerId = parameters.CustomerId.Value;
			}
			else
			{
				var currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();
				if (currentCustomer is null)
					return Unauthorized();
				customerId = currentCustomer.Id;
			}

			if (!await CheckPermissions(customerId, shoppingCartType))
			{
				return AccessDenied();
			}

			if (parameters.Ids is { Count: > 0 })
			{
				foreach (int id in parameters.Ids)
				{
					var item = await _shoppingCartItemApiService.GetShoppingCartItemAsync(id);
					if (item is not null)
					{
						await deleteShoppingCartItem(item);
					}
				}
			}
			else
			{
				var shoppingCartItemsForDelete = _shoppingCartItemApiService.GetShoppingCartItems(customerId: customerId, shoppingCartType: shoppingCartType);
				foreach (var item in shoppingCartItemsForDelete)
				{
					await deleteShoppingCartItem(item);
				}
			}

			async Task deleteShoppingCartItem(ShoppingCartItem item)
			{
				await _shoppingCartService.DeleteShoppingCartItemAsync(item);
				//activity log
				await CustomerActivityService.InsertActivityAsync("DeleteShoppingCartItem", await LocalizationService.GetResourceAsync("ActivityLog.DeleteShoppingCartItem"), item);
			}

			return Ok();
		}

		#region Private methods

		private async Task<bool> CheckPermissions(int? customerId, ShoppingCartType? shoppingCartType)
		{
			var currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();
			if (currentCustomer is null) // authenticated, but does not exist in db
				return false;
			if (customerId.HasValue && currentCustomer.Id == customerId)
			{
				// if I want to handle my own shopping cart, check only public store permission
				switch (shoppingCartType)
				{
					case ShoppingCartType.ShoppingCart:
						return await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart, currentCustomer);
					case ShoppingCartType.Wishlist:
						return await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist, currentCustomer);
					default:
						return await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart, currentCustomer)
							&& await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist, currentCustomer);
				}
			}
			// if I want to handle other customer's shopping carts, check admin permission
			return await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts, currentCustomer);
		}

		private async Task<ShoppingCartItemsRootObject> LoadCurrentShoppingCartItems(ShoppingCartType shoppingCartType, Core.Domain.Customers.Customer customer)
		{
			var updatedShoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer, shoppingCartType);

			var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

			foreach (var newShoppingCartItem in updatedShoppingCart)
			{
				var newShoppingCartItemDto = await _dtoHelper.PrepareShoppingCartItemDTOAsync(newShoppingCartItem);
				shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);
			}

			return shoppingCartsRootObject;
		}

		#endregion
	}
}
