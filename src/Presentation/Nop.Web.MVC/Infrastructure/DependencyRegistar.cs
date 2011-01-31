//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Web.MVC.Infrastructure
{
    public class DependencyRegistar : IDependencyRegistar
    {
        public virtual void Register(ContainerBuilder builder, TypeFinder typeFinder)
        {
            //put your DI here


            //TODO don't hardcode connection string name
            builder.Register<IDbContext>(c => new NopObjectContext("NopSqlConnection")).InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();


            //http context
            builder.Register(c => new HttpContextWrapper(HttpContext.Current))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().InstancePerHttpRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerHttpRequest();

            //services
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerHttpRequest();
            builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerHttpRequest();
            builder.RegisterType<ManufacturerService>().As<IManufacturerService>().InstancePerHttpRequest();
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>().InstancePerHttpRequest();
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>().InstancePerHttpRequest();
            builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerHttpRequest();
            builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerHttpRequest();
            builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerHttpRequest();
            builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().InstancePerHttpRequest();
            builder.RegisterType<IProductService>().As<IProductService>().InstancePerHttpRequest();

            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfiguration<>));


            //TODO use more generic way to register ISettings implementations
            foreach (var setting in typeFinder.FindClassesOfType<ISettings>())
            {
                var settingType = setting.UnderlyingSystemType;
                builder.RegisterType(settingType).As(settingType).InstancePerHttpRequest();
            }
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerHttpRequest();

            builder.RegisterType<CustomerContentService>().As<ICustomerContentService>().InstancePerHttpRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerHttpRequest();

            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerHttpRequest();
            builder.RegisterType<MeasureService>().As<IMeasureService>().InstancePerHttpRequest();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerHttpRequest();

            builder.RegisterType<DiscountService>().As<IDiscountService>().InstancePerHttpRequest();

            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerHttpRequest();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerHttpRequest();
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>().InstancePerHttpRequest();

            builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>().InstancePerHttpRequest();
            builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>().InstancePerHttpRequest();
            builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>().InstancePerHttpRequest();
            builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerHttpRequest();
            builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerHttpRequest();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerHttpRequest();

            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerHttpRequest();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerHttpRequest();
            builder.RegisterType<IUserService>().As<IUserService>().InstancePerHttpRequest();

            builder.RegisterType<ShippingService>().As<IShippingService>().InstancePerHttpRequest();

            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerHttpRequest();
            builder.RegisterType<TaxService>().As<ITaxService>().InstancePerHttpRequest();
            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerHttpRequest();

            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerHttpRequest();
        }
    }
}
