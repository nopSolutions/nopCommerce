using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Infrastructure.Mapper;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// 具体的Nop引擎
    /// </summary>
    public class NopEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
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

        /// <summary>
        /// Register dependencies
        /// 注册依赖
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies(NopConfig config)
        {
            //容器生成器
            var builder = new ContainerBuilder();
            
            //dependencies
            var typeFinder = new WebAppTypeFinder();
            builder.RegisterInstance(config).As<NopConfig>().SingleInstance(); //Nop配置
            builder.RegisterInstance(this).As<IEngine>().SingleInstance(); //引擎
            //ITypeFinder接口用WebAppTypeFinder的单例对象注册绑定。
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance(); //类型发现器

            //register dependencies provided by other assemblies
            //查找所有实现了接口IDependencyRegistrar的类。
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
            {
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }                

            //sort
            //排序
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
            {
                //依次调用实现IDependencyRegistrar接口的类的方法Register
                dependencyRegistrar.Register(builder, typeFinder, config);
            }

            var container = builder.Build();
            this._containerManager = new ContainerManager(container);

            //set dependency resolver
            // ???
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
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
        /// 在nop环境中初始化组件和插件。
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(NopConfig config)
        {
            //register dependencies
            //注册依赖
            RegisterDependencies(config);

            //register mapper configurations
            //注册映射配置
            RegisterMapperConfiguration(config);

            //startup tasks
            //启动任务
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
		{
            return ContainerManager.Resolve<T>();
		}

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }
        
        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public virtual ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
