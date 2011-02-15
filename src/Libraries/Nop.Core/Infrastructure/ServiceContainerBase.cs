using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Wraps an inversion of control container. The default container used by N2 is Windsor.
    /// </summary>
    public abstract class ServiceContainerBase : IServiceContainer
    {
        public abstract void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponent(Type service, string key = "",
                                 ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponent<TService, TImplementation>(string key = "",
                                                            ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponent(Type service, Type implementation, string key = "",
                                 ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponentInstance<TService>(object instance, string key = "",
                                                   ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponentInstance(Type service, object instance, string key = "",
                                         ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties,
                                                                          string key = "",
                                                                          ComponentLifeStyle lifeStyle =
                                                                              ComponentLifeStyle.Singleton);

        public abstract void AddComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties,
                                               string key = "",
                                               ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        public abstract T Resolve<T>(string key = "");

        public abstract object Resolve(Type type);

        public abstract T[] ResolveAll<T>(string key = "");

        public abstract void StartComponents();
    }
}
