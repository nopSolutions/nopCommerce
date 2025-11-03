using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;

using Nop.Plugin.Api.Converters;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Maps;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Services.Topics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Api.Infrastructure
{

    public class DependencyRegister : INopStartup
    {
        public int Order => 3000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICustomerApiService, CustomerApiService>();
            services.AddScoped<ICategoryApiService, CategoryApiService>();
            services.AddScoped<IProductApiService, ProductApiService>();
            services.AddScoped<IProductCategoryMappingsApiService, ProductCategoryMappingsApiService>();
            services.AddScoped<IProductManufacturerMappingsApiService, ProductManufacturerMappingsApiService>();
            services.AddScoped<IOrderApiService, OrderApiService>();
            services.AddScoped<IShoppingCartItemApiService, ShoppingCartItemApiService>();
            services.AddScoped<IOrderItemApiService, OrderItemApiService>();
            services.AddScoped<IProductAttributesApiService, ProductAttributesApiService>();
            services.AddScoped<IProductPictureService, ProductPictureService>();
            services.AddScoped<IProductAttributeConverter, ProductAttributeConverter>();
            services.AddScoped<ISpecificationAttributeApiService, SpecificationAttributesApiService>();
            services.AddScoped<INewsLetterSubscriptionApiService, NewsLetterSubscriptionApiService>();
            services.AddScoped<IManufacturerApiService, ManufacturerApiService>();
            services.AddScoped<IWarehouseApiService, WarehouseApiService>();
            services.AddScoped<IProductWarehouseInventoriesApiService, ProductWarehouseInventoriesApiService>();

            services.AddScoped<IMappingHelper, MappingHelper>();
            services.AddScoped<ICustomerRolesHelper, CustomerRolesHelper>();
            services.AddScoped<IJsonHelper, JsonHelper>();
            services.AddScoped<IDTOHelper, DTOHelper>();

            services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();

            services.AddScoped<IFieldsValidator, FieldsValidator>();

            services.AddScoped<IObjectConverter, ObjectConverter>();
            services.AddScoped<IApiTypeConverter, ApiTypeConverter>();

            services.AddScoped<IFactory<Category>, CategoryFactory>();
            services.AddScoped<IFactory<Product>, ProductFactory>();
            services.AddScoped<IFactory<Customer>, CustomerFactory>();
            services.AddScoped<IFactory<Address>, AddressFactory>();
            services.AddScoped<IFactory<Order>, OrderFactory>();
            services.AddScoped<IFactory<ShoppingCartItem>, ShoppingCartItemFactory>();
            services.AddScoped<IFactory<Manufacturer>, ManufacturerFactory>();
            services.AddScoped<IFactory<Topic>, TopicFactory>();
            services.AddScoped<IFactory<Warehouse>, WarehouseFactory>();

            services.AddScoped<IJsonPropertyMapper, JsonPropertyMapper>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Dictionary<string, object>>(); // TODO: refactor to typed class. make Scoped?

            services.AddScoped<ITopicService, TopicService>();

            services.AddScoped<IAddressApiService, AddressApiService>();

            // replace IAuthenticationService CookieAutheticationService (used in NopCommerce web) with BearerTokenOrCookieAuthenticationService that will combine Bearer token  (used in Nop api plugin) and Cookies authentication
            services.Replace(ServiceDescriptor.Scoped<Nop.Services.Authentication.IAuthenticationService, BearerTokenOrCookieAuthenticationService>());

            // replace IStoreContext WebStoreContext with similar implementation that tries to determine host using api client provided header "NopApiClientHost"
            services.Replace(ServiceDescriptor.Scoped<Nop.Core.IStoreContext, WebApiStoreContext>());

            services.AddScoped(typeof(ParametersModelBinder<>));
            services.AddScoped(typeof(JsonModelBinder<>));
        }
    }
}
