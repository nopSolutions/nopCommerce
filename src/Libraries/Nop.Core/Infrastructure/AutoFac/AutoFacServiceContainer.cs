using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Integration.Mvc;
using AutofacContrib.Startable;
using Nop.Core.Plugins;

namespace Nop.Core.Infrastructure.AutoFac
{
    public class AutoFacServiceContainer : ServiceContainerBase
    {
        private IContainer _container;

        public IContainer Container
        {
            get { return _container; }
        }

        public AutoFacServiceContainer()
        {
            InitializeContainer();
        }

        public AutoFacServiceContainer(IContainer container)
        {
            _container = container;
            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new StartableModule<IAutoStart>(s => s.Start()));

            //type finder
            var typeFinder = new TypeFinder();
            builder.Register(c => typeFinder);

            //find IDependencyRegistar implementations
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistar>();
            foreach (var t in drTypes)
            {
                dynamic dependencyRegistar = Activator.CreateInstance(t);
                dependencyRegistar.Register(builder, typeFinder);
            }

            //event
            OnContainerBuilding(new ContainerBuilderEventArgs(builder));

            if (_container == null)
            {
                _container = builder.Build();
            }
            else
            {
                builder.Update(_container);
            }

            //event
            OnContainerBuildingComplete(new ContainerBuilderEventArgs(builder));
        }

        protected void OnContainerBuilding(ContainerBuilderEventArgs args)
        {
            if (ContainerBuilding != null)
            {
                ContainerBuilding(this, args);
            }
        }

        protected void OnContainerBuildingComplete(ContainerBuilderEventArgs args)
        {
            if (ContainerBuildingComplete != null)
            {
                ContainerBuildingComplete(this, args);
            }
        }

        public event EventHandler<ContainerBuilderEventArgs> ContainerBuilding;

        public event EventHandler<ContainerBuilderEventArgs> ContainerBuildingComplete;

        #region IServiceContainer



        //public override void AddComponent(string key, Type serviceType, Type classType)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            var serviceTypes = new List<Type> {serviceType};
        //                            if (typeof(IAutoStart).IsAssignableFrom(classType))
        //                                serviceTypes.Add(typeof (IAutoStart));
        //                            if (classType.IsGenericType)
        //                            {
        //                                x.RegisterGeneric(classType).Keyed(key, serviceType).As(serviceTypes.ToArray()).PerLifeStyle(ComponentLifeStyle.Singleton);
        //                            }
        //                            else
        //                            {
        //                                x.RegisterType(classType).Keyed(key, serviceType).As(serviceTypes.ToArray()).PerLifeStyle(ComponentLifeStyle.Singleton);
        //                            }
        //                        });
        //}

        //public override void AddComponentInstance(string key, Type serviceType, object instance)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            x.RegisterInstance(instance).Keyed(key, serviceType).As(serviceType);
        //                        });
        //}

        //public override void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            var serviceTypes = new List<Type> { serviceType };
        //                            if (typeof(IAutoStart).IsAssignableFrom(serviceType))
        //                                serviceTypes.Add(typeof(IAutoStart));
        //                            var registeredType = x.RegisterType(serviceType).Keyed(key, serviceType).As(serviceTypes.ToArray());
        //                        });
        //}

        //public override void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties)
        //{
        //    UpdateContainer(x => x.RegisterType(classType).Keyed(key, serviceType).As(serviceType).WithParameters(properties.Select(y => new NamedParameter(y.Key, y.Value))));
        //}

        //public override T Resolve<T>()
        //{
        //    return _container.Resolve<T>();
        //}

        //public override T Resolve<T>(string key)
        //{
        //    return _container.ResolveKeyed<T>(key);
        //}

        //public override object Resolve(Type type)
        //{
        //    return _container.Resolve(type);
        //}

        //public override void Release(object instance)
        //{
        //    throw new NotImplementedException();
        //}

        //public override T[] ResolveAll<T>()
        //{
        //    return _container.Resolve<IEnumerable<T>>().ToArray();
        //}

        //public override void StartComponents()
        //{
        //    Resolve<IStarter>().Start();
        //}

        //public bool IsAutoStart(Type type)
        //{
        //    return false;
        //}














        #endregion

        private void UpdateContainer(Action<ContainerBuilder> action)
        {
            var builder = new ContainerBuilder();
            action.Invoke(builder);
            builder.Update(_container);
        }

        //public override void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            var serviceTypes = new List<Type> { typeof(TService) };
        //                            if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
        //                                serviceTypes.Add(typeof(IAutoStart));

        //                            if (typeof(TService).IsGenericType)
        //                            {
        //                                x.RegisterGeneric(typeof(TService)).Keyed<TService>(key).As(serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
        //                            }
        //                            else
        //                            {
        //                                x.RegisterType<TService>().Keyed(key, serviceTypes[0]).As(serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
        //                            }
        //                        });
        //}

        //public override void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            var serviceTypes = new List<Type> {typeof (TService)};
        //                            if (typeof (IAutoStart).IsAssignableFrom(serviceTypes[0]))
        //                                serviceTypes.Add(typeof (IAutoStart));

        //                            if (typeof (TService).IsGenericType)
        //                            {
        //                                x.RegisterGeneric(typeof (TImplementation)).Keyed<TService>(key).As(
        //                                    serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
        //                            }
        //                            else
        //                            {
        //                                x.RegisterType<TImplementation>().Keyed(key, serviceTypes[0]).As(
        //                                    serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
        //                            }
        //                        });
        //}

        //public override void AddComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        //{
        //    UpdateContainer(x =>
        //    {
        //        var serviceTypes = new List<Type> { typeof(TService) };
        //        if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
        //            serviceTypes.Add(typeof(IAutoStart));

        //        x.RegisterInstance(instance).Keyed(key, serviceTypes[0]).As(serviceTypes.ToArray());
        //    });
        //}

        //public override void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        //{
        //    UpdateContainer(x =>
        //                        {
        //                            var serviceTypes = new List<Type> { typeof(TService) };
        //                            if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
        //                                serviceTypes.Add(typeof(IAutoStart));

        //                            x.RegisterType<TImplementation>().Keyed(key, typeof (TService)).As(serviceTypes.ToArray()).
        //                                WithParameters(
        //                                    properties.Select(y => new NamedParameter(y.Key, y.Value)));

        //                        });
        //}

        //public override T Resolve<T>(string key = "")
        //{
        //    if (string.IsNullOrEmpty(key))
        //    {
        //        return _container.Resolve<T>();
        //    }
        //    return _container.ResolveKeyed<T>(key);
        //}

        //public override object Resolve(Type type)
        //{
        //    return _container.Resolve(type);
        //}

        //public override T[] ResolveAll<T>(string key = "")
        //{
        //    if (string.IsNullOrEmpty(key))
        //    {
        //        return _container.Resolve<IEnumerable<T>>().ToArray();
        //    }
        //    return _container.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        //}

        //public override void StartComponents()
        //{
        //    Resolve<IStarter>().Start();
        //}

        public override void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent<TService, TService>(key, lifeStyle);
            //UpdateContainer(x =>
            //{
            //    //var registration = x.RegisterType<TService>().As<TService>().Keyed<TService>(key);

            //    //if (typeof(IAutoStart).IsAssignableFrom(typeof(TService)))
            //    //    registration.As<IAutoStart>().Keyed<IAutoStart>(key);

            //    var serviceTypes = new List<Type> { typeof(TService) };
            //    if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
            //        serviceTypes.Add(typeof(IAutoStart));

            //    if (serviceTypes[0].IsGenericType)
            //    {
            //        var temp = x.RegisterGeneric(serviceTypes[0]).As(serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
            //        if (!string.IsNullOrEmpty(key))
            //        {
            //            temp.Keyed(key, serviceTypes[0]);
            //        }
            //    }
            //    else
            //    {
            //        var temp = x.RegisterType<TService>().As<TService>().PerLifeStyle(lifeStyle);
            //        if (!string.IsNullOrEmpty(key))
            //        {
            //            temp.Keyed(key, serviceTypes[0]);
            //        }
            //    }
            //});
        }

        public override void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent(service, service, key, lifeStyle);
            //UpdateContainer(x =>
            //                    {
            //                        var serviceTypes = new List<Type> { service };
            //                        if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
            //                            serviceTypes.Add(typeof(IAutoStart));

            //                        if (service.IsGenericType)
            //                        {
            //                            var temp = x.RegisterGeneric(service).As(serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
            //                            if (!string.IsNullOrEmpty(key))
            //                            {
            //                                temp.Keyed(key, service);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            var temp = x.RegisterType(service).As(serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
            //                            if (!string.IsNullOrEmpty(key))
            //                            {
            //                                temp.Keyed(key, service);
            //                            }
            //                        }
        //});
        }

        public override void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent(typeof(TService), typeof(TImplementation), key, lifeStyle);
        }

        public override void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            UpdateContainer(x =>
                                {
                                    var serviceTypes = new List<Type> { service };
                                    if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
                                        serviceTypes.Add(typeof(IAutoStart));

                                    if (service.IsGenericType)
                                    {
                                        var temp = x.RegisterGeneric(implementation).As(
                                            serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            temp.Keyed(key, service);
                                        }
                                    }
                                    else
                                    {
                                        var temp = x.RegisterType(implementation).As(
                                            serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            temp.Keyed(key, service);
                                        }
                                    }
                                });
        }

        public override void AddComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponentInstance(typeof(TService), instance, key, lifeStyle);
        }

        public override void AddComponentInstance(Type service, object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            UpdateContainer(x =>
                                {
                                    var registration = x.RegisterInstance(instance).Keyed(key, service).As(service).PerLifeStyle(lifeStyle);

                                    if (typeof(IAutoStart).IsAssignableFrom(instance.GetType()))
                                        registration.As<IAutoStart>();

                                    //var t = instance.GetType();
                                    //var serviceTypes = new List<Type> { instance.GetType() };
                                    //if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
                                    //    serviceTypes.Add(typeof(IAutoStart));

                                    //var temp = x.RegisterInstance(instance).Keyed(key, serviceTypes[0]).As(serviceTypes.ToArray());
                                    //if (!string.IsNullOrEmpty(key))
                                    //{
                                    //    temp.Keyed(key, service);
                                    //}
                                });
        }

        public override void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponentWithParameters(typeof(TService), typeof(TImplementation), properties);
        }

        public override void AddComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            UpdateContainer(x =>
            {
                var serviceTypes = new List<Type> { service };
                if (typeof(IAutoStart).IsAssignableFrom(serviceTypes[0]))
                    serviceTypes.Add(typeof(IAutoStart));

                var temp = x.RegisterType(implementation).As(serviceTypes.ToArray()).
                    WithParameters(
                        properties.Select(y => new NamedParameter(y.Key, y.Value)));
                if (!string.IsNullOrEmpty(key))
                {
                    temp.Keyed(key, service);
                }
            });
        }

        public override T Resolve<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return _container.Resolve<T>();
            }
            return _container.ResolveKeyed<T>(key);
        }

        public override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public override T[] ResolveAll<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return _container.Resolve<IEnumerable<T>>().ToArray();
            }
            return _container.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        public override void StartComponents()
        {
            _container.Resolve<IStarter>().Start();
        }

        public override void Start()
        {
            UpdateContainer(x => x.RegisterControllers(Resolve<ITypeFinder>().GetAssemblies().ToArray()));
        }
    }

    public static class AutoFacServiceContainerExtensions
    {
        public static Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(this Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, ComponentLifeStyle lifeStyle)
        {
            switch (lifeStyle)
            {
                case ComponentLifeStyle.LifetimeScope:
                    return builder.InstancePerLifetimeScope();
                case ComponentLifeStyle.Transient:
                    return builder.InstancePerDependency();
                case ComponentLifeStyle.Singleton:
                    return builder.SingleInstance();
                default:
                    return builder.SingleInstance();
            }
        }
    }
}
