using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using AutofacContrib.Startable;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Core.Plugins;
using Nop.Core.Tasks;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    public class NopEngine : IEngine
    {
        private ContainerManager _container;

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

		#region Properties

		public IContainer Container
		{
			get { return _container.Container; }
		}

        public ContainerManager ContainerManager
        {
            get { return _container; }
        }

        public IWebContext RequestContext
        {
            get { return Container.Resolve<IWebContext>(); }
        }

		#endregion

		#region Methods

		public void Initialize()
		{
			try
			{
				var bootstrapper = _container.Resolve<IPluginBootstrapper>();
				var plugins = bootstrapper.GetPluginDefinitions();
				bootstrapper.InitializePlugins(this, plugins);
			}
			finally
			{
                //NOTE:Do we need a request life cycle handler?
				//container.Resolve<IRequestLifeCycleHandler>().Initialize();
			}

            var typeFinder = _container.Resolve<ITypeFinder>();

		    ContainerManager.UpdateContainer(x =>
		                                         {
		                                             
		                                             var drTypes =
                                                         typeFinder.FindClassesOfType
		                                                     <IDependencyRegistar>();
		                                             foreach (var t in drTypes)
		                                             {
		                                                 dynamic dependencyRegistar = Activator.CreateInstance(t);
		                                                 dependencyRegistar.Register(x, typeFinder);
		                                             }
		                                         });

			ContainerManager.StartComponents();

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

            _container = new ContainerManager(builder.Build());

            configurer.Configure(this, _container, broker, config);
        }

		#endregion

		#region Container Methods

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
        
    }
}
