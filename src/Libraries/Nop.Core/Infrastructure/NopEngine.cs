using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using AutofacContrib.Startable;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Core.Tasks;

namespace Nop.Core.Infrastructure
{
    public class NopEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
		/// Creates an instance of the content engine using default settings and configuration.
		/// </summary>
		public NopEngine()
            : this(EventBroker.Instance, new ContainerConfigurer())
		{
		}

		public NopEngine(EventBroker broker, ContainerConfigurer configurer)
		{
            InitializeContainer(configurer, broker, new ConfigurationManagerWrapper());
		}

        public NopEngine(System.Configuration.Configuration config, string sectionGroup, EventBroker broker, ContainerConfigurer configurer)
		{
			if (config == null) throw new ArgumentNullException("config");
			if (string.IsNullOrEmpty(sectionGroup)) throw new ArgumentException("Must be non-empty and match a section group in the configuration file.", "sectionGroup");

            InitializeContainer(configurer, broker, new ConfigurationReadingWrapper(config, sectionGroup));
		}

        #endregion

        #region Classes

        private class ConfigurationReadingWrapper : ConfigurationManagerWrapper
		{
			System.Configuration.Configuration config;

			public ConfigurationReadingWrapper(System.Configuration.Configuration config, string sectionGroup)
				: base(sectionGroup)
			{
				this.config = config;
			}

			public override T GetSection<T>(string sectionName)
			{
				return config.GetSection(sectionName) as T;
			}
		}

        #endregion

        #region Utilities

        private void InitPlugins()
        {
            var bootstrapper = _containerManager.Resolve<IPluginBootstrapper>();
            var plugins = bootstrapper.GetPluginDefinitions();
            bootstrapper.InitializePlugins(this, plugins);
        }

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
            {
                var startUpTask = ((IStartupTask)Activator.CreateInstance(startUpTaskType));
                startUpTask.Execute();
            }
        }

        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker, ConfigurationManagerWrapper config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new StartableModule<IAutoStart>(s => s.Start()));

            _containerManager = new ContainerManager(builder.Build());

            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            //plugins
            InitPlugins();

            //start components
            ContainerManager.StartComponents();

            //startup tasks
            RunStartupTasks();
        }

        public T Resolve<T>() where T : class
		{
            return ContainerManager.Resolve<T>();
		}

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public Array ResolveAll(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion

    }
}
