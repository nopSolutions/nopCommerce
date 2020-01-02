using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
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
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Api.Controllers
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using DTOs.Errors;
    using JSON.Serializers;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShippingService _shippingService;
        private readonly IDTOHelper _dtoHelper;        
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IStoreContext _storeContext;
        private readonly IFactory<Order> _factory;

        // We resolve the order settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private OrderSettings _orderSettings;

        private OrderSettings OrderSettings => _orderSettings ?? (_orderSettings = EngineContext.Current.Resolve<OrderSettings>());

        public OrdersController(IOrderApiService orderApiService,
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
            IProductAttributeConverter productAttributeConverter)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                 storeService, discountService, customerActivityService, localizationService,pictureService)
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
        }

        /// <summary>
        /// Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");
            }

            var storeId = _storeContext.CurrentStore.Id;

            var orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = orders.Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/count")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersCount(OrdersCountParametersModel parameters)
        {
            var storeId = _storeContext.CurrentStore.Id;

            var ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                                                              parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId, storeId);

            var ordersCountRootObject = new OrdersCountRootObject()
            {
                Count = ordersCount
            };

            return Ok(ordersCountRootObject);
        }

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderById(int id, string fields = "")
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

            var ordersRootObject = new OrdersRootObject();

            var orderDto = _dtoHelper.PrepareOrderDTO(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve all orders for customer
        /// </summary>
        /// <param name="customerId">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersByCustomerId(int customerId)
        {
            IList<OrderDto> ordersForCustomer = _orderApiService.GetOrdersByCustomerId(customerId).Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }

        [HttpPost]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (orderDelta.Dto.CustomerId == null)
            {
                return Error();
            }

            // We doesn't have to check for value because this is done by the order validator.
            var customer = CustomerService.GetCustomerById(orderDelta.Dto.CustomerId.Value);
            
            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            var shippingRequired = false;

            if (orderDelta.Dto.OrderItems != null)
            {
                var shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id);
                if (shouldReturnError)
                {
                    return Error(HttpStatusCode.BadRequest);
                }

                shippingRequired = IsShippingAddressRequired(orderDelta.Dto.OrderItems);
            }

            if (shippingRequired)
            {
                var isValid = true;

                isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                                            orderDelta.Dto.ShippingMethod,
                                            orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id,
                                            customer, 
                                            BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItems.ToList(), 
                                                                                    customer.Id, 
                                                                                    orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id));

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            var newOrder = _factory.Initialize();
            orderDelta.Merge(newOrder);

            customer.BillingAddress = newOrder.BillingAddress;
            customer.ShippingAddress = newOrder.ShippingAddress;

            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.Customer = customer;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue)
            {
                newOrder.StoreId = _storeContext.CurrentStore.Id;
            }
            
            var placeOrderResult = PlaceOrder(newOrder, customer);

            if (!placeOrderResult.Success)
            {
                foreach (var error in placeOrderResult.Errors)
                {
                    ModelState.AddModelError("order placement", error);
                }

                return Error(HttpStatusCode.BadRequest);
            }

            CustomerActivityService.InsertActivity("AddNewOrder",
                 LocalizationService.GetResource("ActivityLog.AddNewOrder"), newOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = _dtoHelper.PrepareOrderDTO(placeOrderResult.PlacedOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteOrder(int id)
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

            _orderProcessingService.DeleteOrder(orderToDelete);

            //activity log
            CustomerActivityService.InsertActivity("DeleteOrder", LocalizationService.GetResource("ActivityLog.DeleteOrder"), orderToDelete);

            return new RawJsonActionResult("{}");
        }

        [HttpPut]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
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

            var customer = currentOrder.Customer;

            var shippingRequired = currentOrder.OrderItems.Any(item => !item.Product.IsFreeShipping);

            if (shippingRequired)
            {
                var isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    var storeId = orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id;

                    isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                        orderDelta.Dto.ShippingMethod, 
                        storeId,
                        customer, BuildShoppingCartItemsFromOrderItems(currentOrder.OrderItems.ToList(), customer.Id, storeId));
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
            
            customer.BillingAddress = currentOrder.BillingAddress;
            customer.ShippingAddress = currentOrder.ShippingAddress;

            _orderService.UpdateOrder(currentOrder);

            CustomerActivityService.InsertActivity("UpdateOrder",
                 LocalizationService.GetResource("ActivityLog.UpdateOrder"), currentOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = _dtoHelper.PrepareOrderDTO(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private bool SetShippingOption(string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Customer customer, List<ShoppingCartItem> shoppingCartItems)
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
                var shippingOptionResponse = _shippingService.GetShippingOptions(shoppingCartItems, customer.ShippingAddress, customer,
                        shippingRateComputationMethodSystemName, storeId);

                if (shippingOptionResponse.Success)
                {
                    var shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    var shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));
                    
                    _genericAttributeService.SaveAttribute(customer,
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

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItems)
            {
                shoppingCartItems.Add(new ShoppingCartItem()
                {
                    ProductId = orderItem.ProductId,
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    Product = orderItem.Product,
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
                    shoppingCartItems.Add(new ShoppingCartItem()
                    {
                        ProductId = orderItem.ProductId.Value, // required field
                        CustomerId = customerId,
                        Quantity = orderItem.Quantity ?? 1,
                        RentalStartDateUtc = orderItem.RentalStartDateUtc,
                        RentalEndDateUtc = orderItem.RentalEndDateUtc,
                        StoreId = storeId,
                        Product = _productService.GetProductById(orderItem.ProductId.Value),
                        ShoppingCartType = ShoppingCartType.ShoppingCart
                    });
                }
            }

            return shoppingCartItems;
        }

        private PlaceOrderResult PlaceOrder(Order newOrder, Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest
            {
                StoreId = newOrder.StoreId,
                CustomerId = customer.Id,
                PaymentMethodSystemName = newOrder.PaymentMethodSystemName
            };


            var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            return placeOrderResult;
        }

        private bool IsShippingAddressRequired(ICollection<OrderItemDto> orderItems)
        {
            var shippingAddressRequired = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = _productService.GetProductById(orderItem.ProductId.Value);

                    shippingAddressRequired |= product.IsShipEnabled;
                }
            }

            return shippingAddressRequired;
        }

        private bool AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
        {
            var shouldReturnError = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = _productService.GetProductById(orderItem.ProductId.Value);

                    if (!product.IsRental)
                    {
                        orderItem.RentalStartDateUtc = null;
                        orderItem.RentalEndDateUtc = null;
                    }

                    var attributesXml = _productAttributeConverter.ConvertToXml(orderItem.Attributes.ToList(), product.Id);                

                    var errors = _shoppingCartService.AddToCart(customer, product,
                        ShoppingCartType.ShoppingCart, storeId,attributesXml,
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
     }
}