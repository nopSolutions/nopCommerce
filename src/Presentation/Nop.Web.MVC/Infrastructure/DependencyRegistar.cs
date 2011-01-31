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
using Autofac.Configuration;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
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


            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>();

            builder.RegisterType<WorkContext>().As<IWorkContext>();

            //services
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<CompareProductsService>().As<ICompareProductsService>();
            builder.RegisterType<ManufacturerService>().As<IManufacturerService>();
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>();
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>();
            builder.RegisterType<PriceFormatter>().As<IPriceFormatter>();
            builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>();
            builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>();
            builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>();
            builder.RegisterType<IProductService>().As<IProductService>();

            builder.RegisterType<AddressService>().As<IAddressService>();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfiguration<>));


            //TODO use more generic way to register ISettings implementations
            foreach (var setting in typeFinder.FindClassesOfType<ISettings>())
            {
                var settingType = setting.UnderlyingSystemType;
                builder.RegisterType(settingType).As(settingType);
            }
            builder.RegisterType<SettingService>().As<ISettingService>();
            
            builder.RegisterType<CustomerContentService>().As<ICustomerContentService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            
            builder.RegisterType<CountryService>().As<ICountryService>();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>();
            builder.RegisterType<MeasureService>().As<IMeasureService>();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>();

            builder.RegisterType<DiscountService>().As<IDiscountService>();
            
            builder.RegisterType<LanguageService>().As<ILanguageService>();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>();
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>();
            
            builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>();
            builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>();
            builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>();
            builder.RegisterType<GiftCardService>().As<IGiftCardService>();
            builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>();

            builder.RegisterType<PaymentService>().As<IPaymentService>();
            
            builder.RegisterType<EncryptionService>().As<IEncryptionService>();
            builder.RegisterType<IUserService>().As<IUserService>();
            
            builder.RegisterType<ShippingService>().As<IShippingService>();
                
            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>();
            builder.RegisterType<TaxService>().As<ITaxService>();
            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>();

            builder.RegisterType<DefaultLogger>().As<ILogger>();
        }
    }
}
