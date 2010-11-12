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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.EntityClient;
using System.Diagnostics;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Xml;
using Microsoft.Practices.Unity;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Maintenance;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Messages.SMS;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.BusinessLogic.Installation;

namespace NopSolutions.NopCommerce.BusinessLogic.Infrastructure
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        #region Fields

        private readonly IUnityContainer _container;

        #endregion

        #region Ctor

        public UnityDependencyResolver()
            : this(new UnityContainer())
        {
        }
        
        public UnityDependencyResolver(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this._container = container;
            //configure container
            ConfigureContainer(this._container);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Configure root container.Register types and life time managers for unity builder process
        /// </summary>
        /// <param name="container">Container to configure</param>
        protected virtual void ConfigureContainer(IUnityContainer container)
        {
            //Take into account that Types and Mappings registration could be also done using the UNITY XML configuration
            //But we prefer doing it here (C# code) because we'll catch errors at compiling time instead execution time, if any type has been written wrong.

            //Register repositories mappings
            //to be done

            //Register default cache manager            
            //container.RegisterType<ICacheManager, NopRequestCache>(new PerExecutionContextLifetimeManager());

            //Register managers(services) mappings
            container.RegisterType<IOnlineUserService, OnlineUserService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ISearchLogService, SearchLogService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICustomerActivityService, CustomerActivityService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ILogService, LogService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICategoryService, CategoryService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ISettingManager, SettingManager>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IBlogService, BlogService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IForumService, ForumService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<INewsService, NewsService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IPollService, PollService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITopicService, TopicService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICustomerService, CustomerService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICountryService, CountryService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICurrencyService, CurrencyService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ILanguageService, LanguageService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IStateProvinceService, StateProvinceService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IExportManager, ExportManager>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IImportManager, ImportManager>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ILocalizationManager, LocalizationManager>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IMaintenanceService, MaintenanceService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IManufacturerService, ManufacturerService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IMeasureService, MeasureService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IDownloadService, DownloadService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IPictureService, PictureService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ISMSService, SMSService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IMessageService, MessageService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IShoppingCartService, ShoppingCartService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IPaymentService, PaymentService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICheckoutAttributeService, CheckoutAttributeService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IProductAttributeService, ProductAttributeService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ISpecificationAttributeService, SpecificationAttributeService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IProductService, ProductService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IAffiliateService, AffiliateService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ICampaignService, CampaignService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IDiscountService, DiscountService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IQBService, QBService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IACLService, ACLService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IBlacklistService, BlacklistService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IShippingByTotalService, ShippingByTotalService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IShippingByWeightAndCountryService, ShippingByWeightAndCountryService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IShippingByWeightService, ShippingByWeightService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IShippingService, ShippingService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITaxCategoryService, TaxCategoryService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITaxService, TaxService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITaxProviderService, TaxProviderService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITaxRateService, TaxRateService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<ITemplateService, TemplateService>(new UnityPerExecutionContextLifetimeManager());
            container.RegisterType<IWarehouseService, WarehouseService>(new UnityPerExecutionContextLifetimeManager());

            //Object context

            //Connection string
            if (InstallerHelper.ConnectionStringIsSet())
            {
                var ecsbuilder = new EntityConnectionStringBuilder();
                ecsbuilder.Provider = "System.Data.SqlClient";
                ecsbuilder.ProviderConnectionString = NopConfig.ConnectionString;
                ecsbuilder.Metadata = @"res://*/Data.NopModel.csdl|res://*/Data.NopModel.ssdl|res://*/Data.NopModel.msl";
                string connectionString = ecsbuilder.ToString();
                InjectionConstructor connectionStringParam = new InjectionConstructor(connectionString);
                //Registering object context
                container.RegisterType<NopObjectContext>(new UnityPerExecutionContextLifetimeManager(), connectionStringParam);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register instance
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="instance">Instance</param>
        public void Register<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _container.RegisterInstance(instance);
        }

        /// <summary>
        /// Inject
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="existing">Type</param>
        public void Inject<T>(T existing)
        {
            if (existing == null)
                throw new ArgumentNullException("existing");

            _container.BuildUp(existing);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="type">Type</param>
        /// <returns>Result</returns>
        public T Resolve<T>(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return (T)_container.Resolve(type);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="type">Type</param>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public T Resolve<T>(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            return (T)_container.Resolve(type, name);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Result</returns>
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public T Resolve<T>(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return _container.Resolve<T>(name);
        }

        /// <summary>
        /// Resolve all
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Result</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            IEnumerable<T> namedInstances = _container.ResolveAll<T>();
            T unnamedInstance = default(T);

            try
            {
                unnamedInstance = _container.Resolve<T>();
            }
            catch (ResolutionFailedException)
            {
                //When default instance is missing
            }

            if (Equals(unnamedInstance, default(T)))
            {
                return namedInstances;
            }

            return new ReadOnlyCollection<T>(new List<T>(namedInstances) { unnamedInstance });
        }

        #endregion
    }
}
