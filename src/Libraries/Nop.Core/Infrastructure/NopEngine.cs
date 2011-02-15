using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Core.Plugins;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    public class NopEngine : IEngine
    {
        private readonly IServiceContainer container;

		/// <summary>
		/// Creates an instance of the content engine using default settings and configuration.
		/// </summary>
		public NopEngine()
			: this(new AutoFacServiceContainer(), EventBroker.Instance, new ContainerConfigurer())
		{
		}

		/// <summary>Sets the current container to the given container.</summary>
		/// <param name="container">An previously prepared service container.</param>
		/// <param name="broker"></param>
		public NopEngine(IServiceContainer container, EventBroker broker, ContainerConfigurer configurer)
		{
			this.container = container;
			configurer.Configure(this, broker, new ConfigurationManagerWrapper());
		}

		/// <summary>Tries to determine runtime parameters from the given configuration.</summary>
		/// <param name="config">The configuration to use.</param>
		/// <param name="sectionGroup">The configuration section to retrieve configuration from</param>
		/// <param name="broker">Web ap event provider</param>
        public NopEngine(System.Configuration.Configuration config, string sectionGroup, IServiceContainer container, EventBroker broker, ContainerConfigurer configurer)
		{
			if (config == null) throw new ArgumentNullException("config");
			if (string.IsNullOrEmpty(sectionGroup)) throw new ArgumentException("Must be non-empty and match a section group in the configuration file.", "sectionGroup");

			this.container = container;
			configurer.Configure(this, broker, new ConfigurationReadingWrapper(config, sectionGroup));
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

		public IServiceContainer Container
		{
			get { return container; }
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
				var bootstrapper = container.Resolve<IPluginBootstrapper>();
				var plugins = bootstrapper.GetPluginDefinitions();
				bootstrapper.InitializePlugins(this, plugins);
			}
			finally
			{
                //NOTE:Do we need a request life cycle handler?
				//container.Resolve<IRequestLifeCycleHandler>().Initialize();
			}

			container.StartComponents();
		}

		#endregion

		#region Container Methods

		public T Resolve<T>()
		{
		    return Container.Resolve<T>();
		}

        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public Array ResolveAll(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

		#endregion



    }
}
