using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Infrastructure.Mapper;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Represents Nop engine
    /// </summary>
    public class NopEngine : IEngine
    {
        /// <summary>
        /// Gets or sets function that provides access to service provider
        /// </summary>
        public Func<IServiceProvider> ContainerAccessor { get; set; }

        /// <summary>
        /// Get service provider
        /// </summary>
        private IServiceProvider _container => ContainerAccessor();

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies using Autofac
        /// </summary>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <param name="containerBuilder">Container builder used to build an Autofac.IContainer from component registrations</param>
        protected virtual void RegisterDependencies(NopConfig nopConfiguration, ContainerBuilder containerBuilder)
        {
            //register engine
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //find dependency registrars provided by other assemblies
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            //create and sort instances of dependency registrars
            var instances = dependencyRegistrars
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            //register all provided dependencies
            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder, nopConfiguration);
        }

        /// <summary>
        /// Register mapping
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterMapperConfiguration(NopConfig config)
        {
            //dependencies
            var typeFinder = new WebAppTypeFinder();

            //register mapper configurations provided by other assemblies
            var mcTypes = typeFinder.FindClassesOfType<IMapperConfiguration>();
            var mcInstances = new List<IMapperConfiguration>();
            foreach (var mcType in mcTypes)
                mcInstances.Add((IMapperConfiguration)Activator.CreateInstance(mcType));
            //sort
            mcInstances = mcInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //get configurations
            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
            foreach (var mc in mcInstances)
                configurationActions.Add(mc.GetConfiguration());
            //register
            AutoMapperConfiguration.Init(configurationActions);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <param name="containerBuilder">Container builder used to build an Autofac.IContainer from component registrations</param>
        public void Initialize(NopConfig nopConfiguration, ContainerBuilder containerBuilder)
        {
            //register dependencies
            RegisterDependencies(nopConfiguration, containerBuilder);

            //register mapper configurations
            RegisterMapperConfiguration(nopConfiguration);

            //startup tasks
            if (!nopConfiguration.IgnoreStartupTasks)
                RunStartupTasks();

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
		{
            return (T)_container.GetService(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            return _container.GetService(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)_container.GetService(typeof(IEnumerable<T>));
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
