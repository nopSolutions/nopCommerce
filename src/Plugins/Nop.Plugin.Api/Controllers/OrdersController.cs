using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
	public class OrdersController : BaseApiController
	{
		private readonly IDTOHelper _dtoHelper;
		private readonly IFactory<Order> _factory;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly IOrderApiService _orderApiService;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly IOrderService _orderService;
		private readonly IProductAttributeConverter _productAttributeConverter;
		private readonly IPaymentService _paymentService;
		private readonly IPdfService _pdfService;
		private readonly IPermissionService _permissionService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IProductService _productService;
		private readonly IShippingService _shippingService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IStoreContext _storeContext;

		// We resolve the order settings this way because of the tests.
		// The auto mocking does not support concreate types as dependencies. It supports only interfaces.
		private OrderSettings _orderSettings;

		public OrdersController(
			IOrderApiService orderApiService,
			IJsonFieldsSerializer jsonFieldsSerializer,
			IAclService aclService,
			ICustomerService customerService,
			IStoreMappingService storeMappingService,
			IStoreService storeService,
			IDiscountService discountService,
			ICustomerActivityService customerActivityService,
			ILocalizationService localizationService,
			IProductService productService,
			IFactory<Order> factory,
			IOrderProcessingService orderProcessingService,
			IOrderService orderService,
			IShoppingCartService shoppingCartService,
			IGenericAttributeService genericAttributeService,
			IStoreContext storeContext,
			IShippingService shippingService,
			IPictureService pictureService,
			IDTOHelper dtoHelper,
			IProductAttributeConverter productAttributeConverter,
			IPaymentService paymentService,
			IPdfService pdfService,
			IPermissionService permissionService,
			IAuthenticationService authenticationService)
			: base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
				   storeService, discountService, customerActivityService, localizationService, pictureService)
		{
			_orderApiService = orderApiService;
			_factory = factory;
			_orderProcessingService = orderProcessingService;
			_orderService = orderService;
			_shoppingCartService = shoppingCartService;
			_genericAttributeService = genericAttributeService;
			_storeContext = storeContext;
			_shippingService = shippingService;
			_dtoHelper = dtoHelper;
			_productService = productService;
			_productAttributeConverter = productAttributeConverter;
			_paymentService = paymentService;
			_pdfService = pdfService;
			_permissionService = permissionService;
			_authenticationService = authenticationService;
		}

		private OrderSettings OrderSettings => _orderSettings ?? (_orderSettings = EngineContext.Current.Resolve<OrderSettings>());

		/// <summary>
		///     Receive a list of all Orders
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="400">Bad Request</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/orders", Name = "GetOrders")]
		[ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetOrders([FromQuery] OrdersParametersModel parameters)
		{
			if (parameters.Page < Constants.Configurations.DefaultPageValue)
			{
				return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
			}

			if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
			{
				return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");
			}

			if (!await CheckPermissions(parameters.CustomerId))
			{
				return AccessDenied();
			}

			var storeId = _storeContext.GetCurrentStore().Id;

			var orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
													parameters.CreatedAtMax,
													parameters.Limit, parameters.Page, parameters.SinceId,
													parameters.Status.HasValue ? (Core.Domain.Orders.OrderStatus)parameters.Status : null,
													parameters.PaymentStatus.HasValue ? (Core.Domain.Payments.PaymentStatus)parameters.PaymentStatus : null,
													parameters.ShippingStatus.HasValue ? (Core.Domain.Shipping.ShippingStatus)parameters.ShippingStatus : null,
													parameters.CustomerId, storeId);

			IList<OrderDto> ordersAsDtos = await orders.SelectAwait(async x => await _dtoHelper.PrepareOrderDTOAsync(x)).ToListAsync();

			var ordersRootObject = new OrdersRootObject
			{
				Orders = ordersAsDtos
			};

			var json = JsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

			return new RawJsonActionResult(json);
		}

		/// <summary>
		///     Receive a count of all Orders
		/// </summary>
		/// <response code="200">OK</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/orders/count", Name = "GetOrdersCount")]
		[ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetOrdersCount([FromQuery] OrdersCountParametersModel parameters)
		{
			if (!await CheckPermissions(parameters.CustomerId))
			{
				return AccessDenied();
			}

			// TODO: make async

			var storeId = _storeContext.GetCurrentStore().Id;

			var ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
															  parameters.Status.HasValue ? (Core.Domain.Orders.OrderStatus)parameters.Status : null,
															  parameters.PaymentStatus.HasValue ? (Core.Domain.Payments.PaymentStatus)parameters.PaymentStatus : null,
															  parameters.ShippingStatus.HasValue ? (Core.Domain.Shipping.ShippingStatus)parameters.ShippingStatus : null,
															  parameters.CustomerId, storeId);

			var ordersCountRootObject = new OrdersCountRootObject
			{
				Count = ordersCount
			};

			return Ok(ordersCountRootObject);
		}

		/// <summary>
		///     Retrieve order by spcified id
		/// </summary>
		/// ///
		/// <param name="id">Id of the order</param>
		/// <param name="fields">Fields from the order you want your json to contain</param>
		/// <response code="200">OK</response>
		/// <response code="404">Not Found</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/orders/{id}", Name = "GetOrderById")]
		[ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetOrderById([FromRoute] int id, [FromQuery] string fields = "")
		{
			if (id <= 0)
			{
				return Error(HttpStatusCode.BadRequest, "id", "invalid id");
			}

			var order = _orderApiService.GetOrderById(id);

			if (order == null)
			{
				return Error(HttpStatusCode.NotFound, "order", "not found");
			}

			if (!await CheckPermissions(order.CustomerId))
			{
				return AccessDenied();
			}

			var ordersRootObject = new OrdersRootObject();

			var orderDto = await _dtoHelper.PrepareOrderDTOAsync(order);
			ordersRootObject.Orders.Add(orderDto);

			var json = JsonFieldsSerializer.Serialize(ordersRootObject, fields);

			return new RawJsonActionResult(json);
		}

		/// <summary>
		///     Retrieve all orders for customer
		/// </summary>
		/// <param name="customerId">Id of the customer whoes orders you want to get</param>
		/// <response code="200">OK</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/orders/customer/{customerId}", Name = "GetOrdersByCustomerId")]
		[ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetOrdersByCustomerId([FromRoute] int customerId)
		{
			if (!await CheckPermissions(customerId))
			{
				return AccessDenied();
			}

			IList<OrderDto> ordersForCustomer = await _orderApiService.GetOrdersByCustomerId(customerId).SelectAwait(async x => await _dtoHelper.PrepareOrderDTOAsync(x)).ToListAsync();

			var ordersRootObject = new OrdersRootObject
			{
				Orders = ordersForCustomer
			};

			return Ok(ordersRootObject);
		}

		[HttpPost]
		[Route("/api/orders", Name = "CreateOrder")]
		[ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(ErrorsRootObject), 422)]
		public async Task<IActionResult> CreateOrder(
			[FromBody]
			OrderDto dto)
		{
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = new Dictionary<string, object>();
            
            foreach (var jsonProperty in jsonProperties)
            {
                objectPropertyNameValuePairs.Add(jsonProperty.Name, jsonProperty.Value);
            }
            
            var orderDelta = new Delta<OrderDto>(objectPropertyNameValuePairs);
            orderDelta.Dto = dto;
			if (orderDelta.Dto.CustomerId == null)
			{
				return Error(HttpStatusCode.BadRequest, "customerId", "invalid customer id");
			}

			if (orderDelta.Dto.BillingAddress == null)
			{
				return Error(HttpStatusCode.BadRequest, "billingAddress", "missing billing address");
			}
			if (orderDelta.Dto.BillingAddress.Id == 0)
			{
				return Error(HttpStatusCode.BadRequest, "billingAddress", "non-existing billing address");
			}
			if (orderDelta.Dto.ShippingAddress != null && orderDelta.Dto.ShippingAddress.Id == 0) // shipping address CAN be null, but if it is not, it must exist in db
			{
				return Error(HttpStatusCode.BadRequest, "shippingAddress", "non-existing shipping address");
			}

			if (!await CheckPermissions(orderDelta.Dto.CustomerId))
			{
				// TODO: check _orderSettings.AnonymousCheckoutAllowed if IsGuest
				return AccessDenied();
			}

			// We doesn't have to check for value because this is done by the order validator.
			var customer = await CustomerService.GetCustomerByIdAsync(orderDelta.Dto.CustomerId.Value);

			if (customer == null)
			{
				return Error(HttpStatusCode.NotFound, "customer", "not found");
			}

			var shippingRequired = false;

			if (orderDelta.Dto.OrderItems != null)
			{
				var shouldReturnError = await AddOrderItemsToCartAsync(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id);
				if (shouldReturnError)
				{
					return Error(HttpStatusCode.BadRequest);
				}

				shippingRequired = await IsShippingAddressRequiredAsync(orderDelta.Dto.OrderItems);
			}

			if (shippingRequired)
			{
				var isValid = true;

				isValid &= await SetShippingOptionAsync(orderDelta.Dto.ShippingRateComputationMethodSystemName,
											 orderDelta.Dto.ShippingMethod,
											 orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id,
											 customer,
											 BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItems.ToList(),
																					 customer.Id,
																					 orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id));

				if (!isValid)
				{
					return Error(HttpStatusCode.BadRequest);
				}
			}

			var newOrder = await _factory.InitializeAsync();
			orderDelta.Merge(newOrder);

			customer.BillingAddressId = newOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
			customer.ShippingAddressId = newOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress?.Id;

			await CustomerService.UpdateCustomerAsync(customer); // update billing and shipping addresses

			// If the customer has something in the cart it will be added too. Should we clear the cart first? 
			newOrder.CustomerId = customer.Id;

			// The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
			if (!orderDelta.Dto.StoreId.HasValue)
			{
				newOrder.StoreId = _storeContext.GetCurrentStore().Id;
			}

			var placeOrderResult = await PlaceOrderAsync(newOrder, customer);

			if (!placeOrderResult.Success)
			{
				foreach (var error in placeOrderResult.Errors)
				{
					ModelState.AddModelError("order placement", error);
				}

				return Error(HttpStatusCode.BadRequest);
			}

			await CustomerActivityService.InsertActivityAsync("AddNewOrder", await LocalizationService.GetResourceAsync("ActivityLog.AddNewOrder"), newOrder);

			var ordersRootObject = new OrdersRootObject();

			var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(placeOrderResult.PlacedOrder);

			ordersRootObject.Orders.Add(placedOrderDto);

			var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpPut]
		[Route("/api/orders/{id}", Name = "UpdateOrder")]
		[ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(ErrorsRootObject), 422)]
		public async Task<IActionResult> UpdateOrder(
			[FromBody]
			OrderDto dto)
		{
            
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = new Dictionary<string, object>();
            
            foreach (var jsonProperty in jsonProperties)
            {
                objectPropertyNameValuePairs.Add(jsonProperty.Name, jsonProperty.Value);
            }
            
            var orderDelta = new Delta<OrderDto>(objectPropertyNameValuePairs);
            orderDelta.Dto = dto;
            
			// Here we display the errors if the validation has failed at some point.
			if (!ModelState.IsValid)
			{
				return Error();
			}

			var currentOrder = _orderApiService.GetOrderById(orderDelta.Dto.Id);

			if (currentOrder == null)
			{
				return Error(HttpStatusCode.NotFound, "order", "not found");
			}

			if (!await CheckPermissions(currentOrder.CustomerId))
			{
				return AccessDenied();
			}

			var customer = await CustomerService.GetCustomerByIdAsync(currentOrder.CustomerId);

			var shippingRequired = await (await _orderService.GetOrderItemsAsync(currentOrder.Id)).AnyAwaitAsync(async item => !(await _productService.GetProductByIdAsync(item.Id)).IsFreeShipping);

			if (shippingRequired)
			{
				var isValid = true;

				if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
					!string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
				{
					var storeId = orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id;

					isValid &= await SetShippingOptionAsync(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
												 orderDelta.Dto.ShippingMethod,
												 storeId,
												 customer, BuildShoppingCartItemsFromOrderItems(await _orderService.GetOrderItemsAsync(currentOrder.Id), customer.Id, storeId));
				}

				if (isValid)
				{
					currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
				}
				else
				{
					return Error(HttpStatusCode.BadRequest);
				}
			}

			orderDelta.Merge(currentOrder);

			customer.BillingAddressId = currentOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
			customer.ShippingAddressId = currentOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress.Id;

			await CustomerService.UpdateCustomerAsync(customer); // update billing and shipping addresses

			await _orderService.UpdateOrderAsync(currentOrder);

			await CustomerActivityService.InsertActivityAsync("UpdateOrder", await LocalizationService.GetResourceAsync("ActivityLog.UpdateOrder"), currentOrder);

			var ordersRootObject = new OrdersRootObject();

			var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(currentOrder);
			placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

			ordersRootObject.Orders.Add(placedOrderDto);

			var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

			return new RawJsonActionResult(json);
		}

		[HttpDelete]
		[Route("/api/orders/{id}", Name = "DeleteOrder")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(ErrorsRootObject), 422)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> DeleteOrder([FromRoute] int id)
		{
			if (id <= 0)
			{
				return Error(HttpStatusCode.BadRequest, "id", "invalid id");
			}

			var orderToDelete = _orderApiService.GetOrderById(id);

			if (orderToDelete == null)
			{
				return Error(HttpStatusCode.NotFound, "order", "not found");
			}

			if (!await CheckPermissions(orderToDelete.CustomerId))
			{
				return AccessDenied();
			}

			await _orderProcessingService.DeleteOrderAsync(orderToDelete);

			//activity log
			await CustomerActivityService.InsertActivityAsync("DeleteOrder", await LocalizationService.GetResourceAsync("ActivityLog.DeleteOrder"), orderToDelete);

			return new RawJsonActionResult("{}");
		}

		[HttpGet]
		[Route("/api/orders/{orderId}/pdf-invoice", Name = "GetPdfInvoice")]
		[ProducesResponseType(typeof(DTOs.BinaryFileDto), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetPdfInvoice([FromRoute] int orderId)
		{
			var order = await _orderService.GetOrderByIdAsync(orderId);
			if (order == null || order.Deleted)
			{
				return NotFound();
			}

			if (!await CheckPermissions(order.CustomerId))
			{
				return AccessDenied();
			}

			var orders = new List<Order> { order };
			byte[] bytes;
			await using (var stream = new MemoryStream())
			{
				await _pdfService.PrintOrdersToPdfAsync(stream, orders);
				bytes = stream.ToArray();
			}

			var document = new DTOs.BinaryFileDto
			{
				FileName = $"order_{order.OrderGuid:N}.pdf",
				Content = bytes,
				MimeType = MimeTypes.ApplicationPdf,
				CreatedAt = DateTimeOffset.Now,
			};

			return Ok(document);
		}

		#region Private methods

		private async Task<bool> CheckPermissions(int? customerId)
		{
			var currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();
			if (currentCustomer is null) // authenticated, but does not exist in db
				return false;
			if (customerId.HasValue && currentCustomer.Id == customerId)
			{
				// if I want to handle my own orders, check only public store permission
                    return await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART, currentCustomer);
                }
                // if I want to handle other customer's orders, check admin permission
                return await _permissionService.AuthorizeAsync(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE, currentCustomer);
            }

		private async Task<bool> SetShippingOptionAsync(
			string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Customer customer, List<ShoppingCartItem> shoppingCartItems)
		{
			var isValid = true;

			if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
			{
				isValid = false;

				ModelState.AddModelError("shipping_rate_computation_method_system_name",
										 "Please provide shipping_rate_computation_method_system_name");
			}
			else if (string.IsNullOrEmpty(shippingOptionName))
			{
				isValid = false;

				ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
			}
			else
			{
				var shippingOptionResponse = await _shippingService.GetShippingOptionsAsync(shoppingCartItems, await CustomerService.GetCustomerShippingAddressAsync(customer), customer,
																				 shippingRateComputationMethodSystemName, storeId);

				if (shippingOptionResponse.Success)
				{
					var shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

					var shippingOption = shippingOptions
						.Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));

					await _genericAttributeService.SaveAttributeAsync(customer,
														   NopCustomerDefaults.SelectedShippingOptionAttribute,
														   shippingOption, storeId);
				}
				else
				{
					isValid = false;

					foreach (var errorMessage in shippingOptionResponse.Errors)
					{
						ModelState.AddModelError("shipping_option", errorMessage);
					}
				}
			}

			return isValid;
		}

		private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(IList<OrderItem> orderItems, int customerId, int storeId)
		{
			var shoppingCartItems = new List<ShoppingCartItem>();

			foreach (var orderItem in orderItems)
			{
				shoppingCartItems.Add(new ShoppingCartItem
				{
					ProductId = orderItem.ProductId,
					CustomerId = customerId,
					Quantity = orderItem.Quantity,
					RentalStartDateUtc = orderItem.RentalStartDateUtc,
					RentalEndDateUtc = orderItem.RentalEndDateUtc,
					StoreId = storeId,
					ShoppingCartType = ShoppingCartType.ShoppingCart
				});
			}

			return shoppingCartItems;
		}

		private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
		{
			var shoppingCartItems = new List<ShoppingCartItem>();

			foreach (var orderItem in orderItemDtos)
			{
				if (orderItem.ProductId != null)
				{
					shoppingCartItems.Add(new ShoppingCartItem
					{
						ProductId = orderItem.ProductId.Value, // required field
						CustomerId = customerId,
						Quantity = orderItem.Quantity ?? 1,
						RentalStartDateUtc = orderItem.RentalStartDateUtc,
						RentalEndDateUtc = orderItem.RentalEndDateUtc,
						StoreId = storeId,
						ShoppingCartType = ShoppingCartType.ShoppingCart
					});
				}
			}

			return shoppingCartItems;
		}

		private async Task<PlaceOrderResult> PlaceOrderAsync(Order newOrder, Customer customer)
		{
			var processPaymentRequest = new ProcessPaymentRequest
			{
				StoreId = newOrder.StoreId,
				CustomerId = customer.Id,
				PaymentMethodSystemName = newOrder.PaymentMethodSystemName,
				// TODO: fill credit card info,
				OrderGuid = Guid.NewGuid(),
				OrderGuidGeneratedOnUtc = DateTime.UtcNow,
			};

			//_paymentService.GenerateOrderGuid(processPaymentRequest);

			var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);

			if (placeOrderResult.Success)
			{
				var postProcessPaymentRequest = new PostProcessPaymentRequest
				{
					Order = placeOrderResult.PlacedOrder
				};

				await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);
			}

			return placeOrderResult;
		}

		private async Task<bool> IsShippingAddressRequiredAsync(ICollection<OrderItemDto> orderItems)
		{
			var shippingAddressRequired = false;

			foreach (var orderItem in orderItems)
			{
				if (orderItem.ProductId != null)
				{
					var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

					shippingAddressRequired |= product.IsShipEnabled;
				}
			}

			return shippingAddressRequired;
		}

		private async Task<bool> AddOrderItemsToCartAsync(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
		{
			var shouldReturnError = false;

			foreach (var orderItem in orderItems)
			{
				if (orderItem.ProductId != null)
				{
					var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

					if (!product.IsRental)
					{
						orderItem.RentalStartDateUtc = null;
						orderItem.RentalEndDateUtc = null;
					}

					var attributesXml = await _productAttributeConverter.ConvertToXmlAsync(orderItem.Attributes.ToList(), product.Id);

					var errors = await _shoppingCartService.AddToCartAsync(customer, product,
																ShoppingCartType.ShoppingCart, storeId, attributesXml,
																0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
																orderItem.Quantity ?? 1);

					if (errors.Count > 0)
					{
						foreach (var error in errors)
						{
							ModelState.AddModelError("order", error);
						}

						shouldReturnError = true;
					}
				}
			}

			return shouldReturnError;
		}

		#endregion
	}
}
