using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins;

namespace Nop.Core.Infrastructure
{
    //TODO:Delete this
    ///// <summary>
    ///// Wraps an inversion of control container. The default container used by Nop is AutoFac.
    ///// </summary>
    //public abstract class ServiceContainerBase : IServiceContainer , IAutoStart
    //{
    //    public abstract void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponent(Type service, string key = "",
    //                             ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponent<TService, TImplementation>(string key = "",
    //                                                        ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponent(Type service, Type implementation, string key = "",
    //                             ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponentInstance<TService>(object instance, string key = "",
    //                                               ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponentInstance(Type service, object instance, string key = "",
    //                                     ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties,
    //                                                                      string key = "",
    //                                                                      ComponentLifeStyle lifeStyle =
    //                                                                          ComponentLifeStyle.Singleton);

    //    public abstract void AddComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties,
    //                                           string key = "",
    //                                           ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

    //    public abstract T Resolve<T>(string key = "");

    //    public abstract object Resolve(Type type);

    //    public abstract T[] ResolveAll<T>(string key = "");

    //    public abstract void StartComponents();

    //    public virtual void Start()
    //    {
    //    }

    //    public virtual void Stop()
    //    {
    //    }
    //}
}
