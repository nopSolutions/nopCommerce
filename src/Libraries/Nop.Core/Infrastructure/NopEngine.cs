using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Extensions;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Represents Nop engine
    /// </summary>
    public class NopEngine : IEngine
    {
        #region Properties

        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        private IServiceProvider _serviceProvider { get; set; }

        #endregion

        #region Utilities

        protected IServiceProvider GetServiceProvider()
        {
            var accessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var context = accessor.HttpContext;
            return context != null ? context.RequestServices : _serviceProvider;
        }

        /// <summary>
        /// Run startup tasks
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            //find startup tasks provided by other assemblies
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            //create and sort instances of startup tasks
            var instances = startupTasks
                .Where(startupTask => PluginManager.FindPlugin(startupTask).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //execute tasks
            foreach (var task in instances)
                task.Execute();
        }

        /// <summary>
        /// Register dependencies using Autofac
        /// </summary>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        protected virtual IServiceProvider RegisterDependencies(NopConfig nopConfiguration, IServiceCollection services, ITypeFinder typeFinder)
        {
            var containerBuilder = new ContainerBuilder();

            //register engine
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            //register type finder
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //find dependency registrars provided by other assemblies
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            //create and sort instances of dependency registrars
            var instances = dependencyRegistrars
                .Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            //register all provided dependencies
            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder, nopConfiguration);

            //populate Autofac container builder with the set of registered service descriptors
            containerBuilder.Populate(services);

            //create service provider
            _serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
            return _serviceProvider;
        }

        /// <summary>
        /// Register and configure AutoMapper
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
        {
            //find mapper configurations provided by other assemblies
            var mapperConfigurations = typeFinder.FindClassesOfType<IMapperConfiguration>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Where(mapperConfiguration => PluginManager.FindPlugin(mapperConfiguration).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(mapperConfiguration => (IMapperConfiguration)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //get all configuration actions
            var configurationActions = instances.Select(mapperConfiguration => mapperConfiguration.GetConfiguration());

            //register AutoMapper with provided configurations
            services.AddAutoMapper(mapperConfigurationExpression =>
            {
                foreach (var action in configurationActions)
                    action(mapperConfigurationExpression);
            });
            //register
            AutoMapperConfiguration.Init(configurationActions.ToList());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="hostingEnvironment">Web hosting environment an application is running in</param>
        public void Initialize(IHostingEnvironment hostingEnvironment)
        {
            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //set base application path
            CommonHelper.BaseDirectory = hostingEnvironment.ContentRootPath;

            //initialize plugins
            PluginManager.Initialize();
        }

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //find startup configurations provided by other assemblies
            var typeFinder = new WebAppTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Where(startupConfiguration => PluginManager.FindPlugin(startupConfiguration).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(startupConfiguration => (IStartup)Activator.CreateInstance(startupConfiguration))
                .OrderBy(startupConfiguration => startupConfiguration.Order);

            //configure services
            foreach (var instance in instances)
                instance.ConfigureServices(services,configuration);

            //register mapper configurations
            AddAutoMapper(services, typeFinder);

            //register dependencies
            var nopConfig = services.BuildServiceProvider().GetService<NopConfig>();
            RegisterDependencies(nopConfig, services, typeFinder);

            //run startup tasks
            if (!nopConfig.IgnoreStartupTasks)
                RunStartupTasks(typeFinder);

            return _serviceProvider;
        }

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            //find startup configurations provided by other assemblies
            var typeFinder = Resolve<ITypeFinder>();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Where(startupConfiguration => PluginManager.FindPlugin(startupConfiguration).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(startupConfiguration => (IStartup)Activator.CreateInstance(startupConfiguration))
                .OrderBy(startupConfiguration => startupConfiguration.Order);

            //configure request pipeline
            foreach (var instance in instances)
                instance.Configure(application);
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
		{
            return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            return GetServiceProvider().GetRequiredService(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new NopException("Unknown dependency");
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (NopException) { }
            }
            throw new NopException("No constructor was found that had all the dependencies satisfied.");
        }

        #endregion
    }
}
