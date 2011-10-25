using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using AutofacContrib.Startable;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;

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
            var config = ConfigurationManager.GetSection("NopConfig") as NopConfig;
            InitializeContainer(configurer, broker, config);
		}
        
        #endregion

        #region Utilities

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }
        
        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker, NopConfig config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new StartableModule<IAutoStart>(s => s.Start()));

            _containerManager = new ContainerManager(builder.Build());

            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="databaseIsInstalled">A value indicating whether database is installed</param>
        public void Initialize(NopConfig config, bool databaseIsInstalled)
        {
            //start components
            this.ContainerManager.StartComponents();

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
