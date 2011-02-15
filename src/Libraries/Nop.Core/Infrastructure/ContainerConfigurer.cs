using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Configures the inversion of control container with services used by N2 CMS.
    /// </summary>
    public class ContainerConfigurer
    {
        /// <summary>
        /// Known configuration keys used to configure services.
        /// </summary>
        public static class ConfigurationKeys
        {
            /// <summary>Key used to configure services intended for medium trust.</summary>
            public const string MediumTrust = "MediumTrust";
            /// <summary>Key used to configure services intended for full trust.</summary>
            public const string FullTrust = "FullTrust";
        }

        public virtual void Configure(IEngine engine, EventBroker broker, ConfigurationManagerWrapper configuration)
        {
            configuration.Start();
            
            engine.Container.AddComponentInstance<ConfigurationManagerWrapper>(configuration, "nop.configuration");
            engine.Container.AddComponentInstance<IEngine>(engine, "nop.engine");
            engine.Container.AddComponentInstance<IServiceContainer>(engine.Container, "nop.container");
            engine.Container.AddComponentInstance<ContainerConfigurer>(this, "nop.containerConfigurer");

            AddComponentInstance(engine.Container, configuration.GetConnectionStringsSection());
            AddComponentInstance(engine.Container, configuration.Sections.Engine);
           
            if (configuration.Sections.Engine != null)
                RegisterConfiguredComponents(engine.Container, configuration.Sections.Engine);
                InitializeEnvironment(engine.Container, configuration.Sections.Host);

            AddComponentInstance(engine.Container, broker);

            engine.Container.AddComponent<ITypeFinder, WebAppTypeFinder>("nop.typeFinder");
            engine.Container.AddComponent<IWebContext, AdaptiveContext>("nop.webContext");
            engine.Container.AddComponent<ServiceRegistrator>("nop.typeFinder");
           
            var registrator = engine.Container.Resolve<ServiceRegistrator>();
            var services = registrator.FindServices();

            var configurations = GetComponentConfigurations(configuration);

            services = registrator.FilterServices(services, configurations);
            registrator.RegisterServices(services);
        }

        protected virtual string[] GetComponentConfigurations(ConfigurationManagerWrapper configuration)
        {
            List<string> configurations = new List<string>();
            string trustConfiguration = (CommonHelper.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium)
                ? ConfigurationKeys.FullTrust
                : ConfigurationKeys.MediumTrust;
            configurations.Add(trustConfiguration);
            return configurations.ToArray();
        }

        private void AddComponentInstance(IServiceContainer container, object instance)
        {
            container.AddComponentInstance(instance.GetType(), instance, instance.GetType().FullName);
        }

        protected virtual void InitializeEnvironment(IServiceContainer container, HostSection hostConfig)
        {
            if (hostConfig != null)
            {
                if (!hostConfig.IsWeb)
                    container.AddComponentInstance<IWebContext>(new ThreadContext(), "nop.webContext.notWeb");
            }
        }

        protected virtual void RegisterConfiguredComponents(IServiceContainer container, EngineSection engineConfig)
        {
            foreach (ComponentElement component in engineConfig.Components)
            {
                Type implementation = Type.GetType(component.Implementation);
                Type service = Type.GetType(component.Service);

                if (implementation == null)
                    throw new ComponentRegistrationException(component.Implementation);

                if (service == null && !String.IsNullOrEmpty(component.Service))
                    throw new ComponentRegistrationException(component.Service);

                if (service == null)
                    service = implementation;

                string name = component.Key;
                if (string.IsNullOrEmpty(name))
                    name = implementation.FullName;

                if (component.Parameters.Count == 0)
                {
                    container.AddComponent(service, implementation, name);
                }
                else
                {
                    container.AddComponentWithParameters(service, implementation,
                                                         component.Parameters.ToDictionary(), name);
                }
            }
        }
    }
}
