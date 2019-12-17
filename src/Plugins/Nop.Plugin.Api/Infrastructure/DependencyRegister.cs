using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Api.Infrastructure
{
    using Autofac.Core;
    using Microsoft.AspNetCore.Http;
    using Nop.Core.Domain.Catalog;
    using Nop.Core.Domain.Common;
    using Nop.Core.Domain.Customers;
    using Nop.Core.Domain.Orders;
    using Nop.Plugin.Api.Converters;
    using Nop.Plugin.Api.Data;
    using Nop.Plugin.Api.Factories;
    using Nop.Plugin.Api.Helpers;
    using Nop.Plugin.Api.JSON.Serializers;
    using Nop.Plugin.Api.ModelBinders;
    using Nop.Plugin.Api.Validators;
    //using Nop.Plugin.Api.WebHooks;
    using System;
    using System.Collections.Generic;

    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);

            RegisterModelBinders(builder);
        }

        private void RegisterModelBinders(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ParametersModelBinder<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JsonModelBinder<>)).InstancePerLifetimeScope();
        }

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryApiService>().As<ICategoryApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductApiService>().As<IProductApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryMappingsApiService>().As<IProductCategoryMappingsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductManufacturerMappingsApiService>().As<IProductManufacturerMappingsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributesApiService>().As<IProductAttributesApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductPictureService>().As<IProductPictureService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeConverter>().As<IProductAttributeConverter>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificationAttributesApiService>().As<ISpecificationAttributeApiService>().InstancePerLifetimeScope();
            builder.RegisterType<NewsLetterSubscriptionApiService>().As<INewsLetterSubscriptionApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerApiService>().As<IManufacturerApiService>().InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DTOHelper>().As<IDTOHelper>().InstancePerLifetimeScope();
            builder.RegisterType<NopConfigManagerHelper>().As<IConfigManagerHelper>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<NopWebHooksLogger>().As<Microsoft.AspNet.WebHooks.Diagnostics.ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();

            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<WebHookService>().As<IWebHookService>().SingleInstance();

            builder.RegisterType<ObjectConverter>().As<IObjectConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ApiTypeConverter>().As<IApiTypeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryFactory>().As<IFactory<Category>>().InstancePerLifetimeScope();
            builder.RegisterType<ProductFactory>().As<IFactory<Product>>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerFactory>().As<IFactory<Customer>>().InstancePerLifetimeScope();
            builder.RegisterType<AddressFactory>().As<IFactory<Address>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderFactory>().As<IFactory<Order>>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemFactory>().As<IFactory<ShoppingCartItem>>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerFactory>().As<IFactory<Manufacturer>>().InstancePerLifetimeScope();

            builder.RegisterType<Maps.JsonPropertyMapper>().As<Maps.IJsonPropertyMapper>().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<Dictionary<string, object>>().SingleInstance();
        }

        public virtual int Order
        {
            get { return Int16.MaxValue; }
        }
    }
}