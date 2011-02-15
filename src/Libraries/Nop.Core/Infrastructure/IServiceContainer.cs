using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Wraps an inversion of control container. The default container used by Nop is AutoFac.
    /// </summary>
    public interface IServiceContainer
    {
        void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);
        
        void AddComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentInstance(Type service, object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        T Resolve<T>(string key = "");

        object Resolve(Type type);

        T[] ResolveAll<T>(string key = "");

        /// <summary>Starts any <see cref="IAutoStart"/> components in the container.</summary>
        void StartComponents();
    }
}
