using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        public IServiceProvider ServiceProvider { get; set; }

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            //find startup tasks provided by other assemblies
            var startupTasks = Resolve<ITypeFinder>().FindClassesOfType<IStartupTask>();

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
        /// <param name="services">The contract for a collection of service descriptors</param>
        protected virtual void RegisterDependencies(NopConfig nopConfiguration, IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            //register engine
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            //register type finder
            var typeFinder = new WebAppTypeFinder();
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
            ServiceProvider = new AutofacServiceProvider(containerBuilder.Build());
        }

        /// <summary>
        /// Register and configure AutoMapper
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        protected virtual void AddAutoMapper(IServiceCollection services)
        {
            //find mapper configurations provided by other assemblies
            var typeFinder = new WebAppTypeFinder();
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
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configure components in the nop environment
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <param name="configuration">The root of an configuration hierarchy</param>
        public void Configure(IServiceCollection services, IConfigurationRoot configuration)
        {
            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //set base application path
            var hostingEnvironment = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();
            CommonHelper.BaseDirectory = hostingEnvironment.ContentRootPath;

            //initialize plugins
            PluginManager.Initialize();

            //add options feature
            services.AddOptions();

            //add NopConfig configuration parameters
            var nopConfig = services.ConfigureStartupConfig<NopConfig>(configuration.GetSection("Nop"));

            //add hosting configuration parameters
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));

            if (nopConfig.RedisCachingEnabled)
            {
                //add Redis distributed cache
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = nopConfig.RedisCachingConnectionString;
                });
            }
            else
                services.AddDistributedMemoryCache();

            //add memory cache
            services.AddMemoryCache();

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //register mapper configurations
            AddAutoMapper(services);

            //register dependencies
            RegisterDependencies(nopConfig, services);

            //startup tasks
            if (!nopConfig.IgnoreStartupTasks)
                RunStartupTasks();
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
		{
            return (T)ServiceProvider.GetRequiredService(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            return ServiceProvider.GetRequiredService(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)ServiceProvider.GetRequiredService(typeof(IEnumerable<T>));
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
                    return Activator.CreateInstance(type, parameters);
                }
                catch (NopException) { }
            }
            throw new NopException("No constructor was found that had all the dependencies satisfied.");
        }

        #endregion
    }
}
