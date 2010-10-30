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
using System.Configuration;
using System.Data.EntityClient;
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

namespace NopSolutions.NopCommerce.BusinessLogic.IoC
{
    /// <summary>
    /// Inversion of Control factory implementation.
    /// This is a simple factory built with Microsoft Unity    
    /// </summary>
    public static class IoCFactory
    {
        #region Fields

        private static IDictionary<string, IUnityContainer> _containersDictionary;

        #endregion

        #region Constructor

        /// <summary>
        /// Wires up the ioc containers
        /// </summary>        
        static IoCFactory()
        {
            InitContainers();
        }

        #endregion

        #region Utilities

        static void InitContainers()
        {
            /*
             * CREATE DICTIONARY
             */
            _containersDictionary = new Dictionary<string, IUnityContainer>();

            /*
             * CREATE CONTAINERS 
             */
            //Create root container
            IUnityContainer rootContainer = new UnityContainer();
            _containersDictionary.Add("RootContext", rootContainer);

            //Create container for real context, child of root container
            IUnityContainer realAppContainer = rootContainer.CreateChildContainer();
            _containersDictionary.Add("RealAppContext", realAppContainer);

            //Create container for testing, child of root container
            IUnityContainer fakeAppContainer = rootContainer.CreateChildContainer();
            _containersDictionary.Add("FakeAppContext", fakeAppContainer);

            /*
             * CONFIGURE CONTAINERS
             */
            ConfigureRootContainer(rootContainer);
            ConfigureRealContainer(realAppContainer);
            ConfigureFakeContainer(fakeAppContainer);
        }

        /// <summary>
        /// Configure root container.Register types and life time managers for unity builder process
        /// </summary>
        /// <param name="container">Container to configure</param>
        static void ConfigureRootContainer(IUnityContainer container)
        {
            // Take into account that Types and Mappings registration could be also done using the UNITY XML configuration
            //But we prefer doing it here (C# code) because we'll catch errors at compiling time instead execution time, if any type has been written wrong.

            //Register Repositories mappings
            //to be done

            //Register Managers(Services) mappings
            container.RegisterType<IOnlineUserManager, OnlineUserManager>(new TransientLifetimeManager());
            container.RegisterType<ISearchLogManager, SearchLogManager>(new TransientLifetimeManager());
            container.RegisterType<ICustomerActivityManager, CustomerActivityManager>(new TransientLifetimeManager());
            container.RegisterType<ILogManager, LogManager>(new TransientLifetimeManager());
            container.RegisterType<ICategoryManager, CategoryManager>(new TransientLifetimeManager());
            container.RegisterType<ISettingManager, SettingManager>(new TransientLifetimeManager());
            container.RegisterType<IBlogManager, BlogManager>(new TransientLifetimeManager());
            container.RegisterType<IForumManager, ForumManager>(new TransientLifetimeManager());
            container.RegisterType<INewsManager, NewsManager>(new TransientLifetimeManager());
            container.RegisterType<IPollManager, PollManager>(new TransientLifetimeManager());
            container.RegisterType<ITopicManager, TopicManager>(new TransientLifetimeManager());
            container.RegisterType<ICustomerManager, CustomerManager>(new TransientLifetimeManager());
            container.RegisterType<ICountryManager, CountryManager>(new TransientLifetimeManager());
            container.RegisterType<ICurrencyManager, CurrencyManager>(new TransientLifetimeManager());
            container.RegisterType<ILanguageManager, LanguageManager>(new TransientLifetimeManager());
            container.RegisterType<IStateProvinceManager, StateProvinceManager>(new TransientLifetimeManager());
            container.RegisterType<ILocaleStringResourceManager, LocaleStringResourceManager>(new TransientLifetimeManager());
            container.RegisterType<IMaintenanceManager, MaintenanceManager>(new TransientLifetimeManager());
            container.RegisterType<IManufacturerManager, ManufacturerManager>(new TransientLifetimeManager());
            container.RegisterType<IMeasureManager, MeasureManager>(new TransientLifetimeManager());
            container.RegisterType<IDownloadManager, DownloadManager>(new TransientLifetimeManager());
            container.RegisterType<IPictureManager, PictureManager>(new TransientLifetimeManager());
            container.RegisterType<ISMSManager, SMSManager>(new TransientLifetimeManager());
            container.RegisterType<IMessageManager, MessageManager>(new TransientLifetimeManager());
            container.RegisterType<IOrderManager, OrderManager>(new TransientLifetimeManager());
            container.RegisterType<IShoppingCartManager, ShoppingCartManager>(new TransientLifetimeManager());
            container.RegisterType<IPaymentManager, PaymentManager>(new TransientLifetimeManager());            
            container.RegisterType<ICheckoutAttributeManager, CheckoutAttributeManager>(new TransientLifetimeManager());
            container.RegisterType<IProductAttributeManager, ProductAttributeManager>(new TransientLifetimeManager());
            container.RegisterType<ISpecificationAttributeManager, SpecificationAttributeManager>(new TransientLifetimeManager());
            container.RegisterType<IProductManager, ProductManager>(new TransientLifetimeManager());
            container.RegisterType<IAffiliateManager, AffiliateManager>(new TransientLifetimeManager());
            container.RegisterType<ICampaignManager, CampaignManager>(new TransientLifetimeManager());
            container.RegisterType<IDiscountManager, DiscountManager>(new TransientLifetimeManager());
            container.RegisterType<IQBManager, QBManager>(new TransientLifetimeManager());
            container.RegisterType<IACLManager, ACLManager>(new TransientLifetimeManager());
            container.RegisterType<IBlacklistManager, BlacklistManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingByTotalManager, ShippingByTotalManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingByWeightAndCountryManager, ShippingByWeightAndCountryManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingByWeightManager, ShippingByWeightManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingManager, ShippingManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingMethodManager, ShippingMethodManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingRateComputationMethodManager, ShippingRateComputationMethodManager>(new TransientLifetimeManager());
            container.RegisterType<IShippingStatusManager, ShippingStatusManager>(new TransientLifetimeManager());
            container.RegisterType<ITaxCategoryManager, TaxCategoryManager>(new TransientLifetimeManager());
            container.RegisterType<ITaxManager, TaxManager>(new TransientLifetimeManager());
            container.RegisterType<ITaxProviderManager, TaxProviderManager>(new TransientLifetimeManager());
            container.RegisterType<ITaxRateManager, TaxRateManager>(new TransientLifetimeManager());
            container.RegisterType<ITemplateManager, TemplateManager>(new TransientLifetimeManager());
            container.RegisterType<IWarehouseManager, WarehouseManager>(new TransientLifetimeManager());
        }

        /// <summary>
        /// Configure real container. Register types and life time managers for unity builder process
        /// </summary>
        /// <param name="container">Container to configure</param>
        static void ConfigureRealContainer(IUnityContainer container)
        {
            //Object context
            //Connection string
            var ecsbuilder = new EntityConnectionStringBuilder();
            ecsbuilder.Provider = "System.Data.SqlClient";
            ecsbuilder.ProviderConnectionString = NopConfig.ConnectionString;
            ecsbuilder.Metadata = @"res://*/Data.NopModel.csdl|res://*/Data.NopModel.ssdl|res://*/Data.NopModel.msl";
            string connectionString = ecsbuilder.ToString();
            InjectionConstructor connectionStringParam = new InjectionConstructor(connectionString);
            //Registering object context
            container.RegisterType<NopObjectContext>(new PerExecutionContextLifetimeManager(), connectionStringParam);
        }

        /// <summary>
        /// Configure fake container.Register types and life time managers for unity builder process
        /// </summary>
        /// <param name="container">Container to configure</param>
        static void ConfigureFakeContainer(IUnityContainer container)
        {
            //to be done
            //container.RegisterType(typeof(INopObjectContext), typeof(NopObjectFakeContext), new PerExecutionContextLifetimeManager());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an injected object instance implementation for the requested interface
        /// IMPORTANT: It uses default IoC Container defined in AppSettings
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// defaultIocContainer AppSetting key not found
        /// </exception>
        public static T Resolve<T>()
        {
            //We use the default container specified in AppSettings
            string containerName = ConfigurationManager.AppSettings["defaultIoCContainer"];

            return Resolve<T>(containerName);
        }


        /// <summary>
        /// Returns an injected implementation for the requested interface
        /// It uses provided IoC Container passed as parameter
        /// </summary>
        public static T Resolve<T>(string containerName)
        {
            //check preconditions
            if (String.IsNullOrEmpty(containerName)
               ||
               String.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException("Default containter name is not set");
            }

            if (!_containersDictionary.ContainsKey(containerName))
                throw new InvalidOperationException("IoC container is not found");

            IUnityContainer container = _containersDictionary[containerName];

            return container.Resolve<T>();
        }

        #endregion
    }
}
