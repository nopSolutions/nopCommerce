using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nop.Core.Configuration;
using Nop.Core.Events;
using Autofac;
using Autofac.Builder;

namespace Nop.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Configures the inversion of control container with services used by Nop.
    /// </summary>
    public class ContainerConfigurer
    {
        /// <summary>
        /// Known configuration keys used to configure services.
        /// </summary>
        public static class ConfigurationKeys
        {
            //TODO do we need it?
            /// <summary>Key used to configure services intended for medium trust.</summary>
            public const string MediumTrust = "MediumTrust";
            /// <summary>Key used to configure services intended for full trust.</summary>
            public const string FullTrust = "FullTrust";
        }

        public virtual void Configure(IEngine engine, ContainerManager containerManager, EventBroker broker, NopConfig configuration)
        {
            //register dependencies provided by other asemblies
            containerManager.AddComponent<IWebHelper, WebHelper>("nop.webHelper");
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("nop.typeFinder");
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            containerManager.UpdateContainer(x =>
            {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = new List<IDependencyRegistrar>();
                foreach (var drType in drTypes)
                    drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });

            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof (IConsumer<>)).ToList();
            containerManager.UpdateContainer(x =>
            {
                foreach(var consumer in consumers)
                {
                    x.RegisterType(consumer)
                        .As(consumer
                                .FindInterfaces((type, criteria) =>
                                                    {
                                                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                                                        return isMatch;
                                                    }, typeof (IConsumer<>)));
                }
            });

            containerManager.AddComponent<IEventPublisher, EventPublisher>("nop.eventPublisher");
            containerManager.AddComponent<ISubscriptionService, SubscriptionService>("nop.subscriptionService");

            //other dependencies
            containerManager.AddComponentInstance<NopConfig>(configuration, "nop.configuration");
            containerManager.AddComponentInstance<IEngine>(engine, "nop.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "nop.containerConfigurer");

            //if (configuration.Components != null)
            //    RegisterConfiguredComponents(containerManager, configuration.Components);

            //event broker
            containerManager.AddComponentInstance(broker);

            //service registration
            containerManager.AddComponent<DependencyAttributeRegistrator>("nop.serviceRegistrator");
            var registrator = containerManager.Resolve<DependencyAttributeRegistrator>();
            var services = registrator.FindServices();
            var configurations = GetComponentConfigurations(configuration);
            services = registrator.FilterServices(services, configurations);
            registrator.RegisterServices(services);
        }

        protected virtual string[] GetComponentConfigurations(NopConfig configuration)
        {
            List<string> configurations = new List<string>();
            string trustConfiguration = (CommonHelper.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium)
                ? ConfigurationKeys.FullTrust
                : ConfigurationKeys.MediumTrust;
            configurations.Add(trustConfiguration);
            return configurations.ToArray();
        }

        private void AddComponentInstance(IEngine engine, object instance)
        {
            engine.ContainerManager.AddComponentInstance(instance.GetType(), instance, instance.GetType().FullName);
        }

        //protected virtual void RegisterConfiguredComponents(ContainerManager container, NopConfig config)
        //{
        //    foreach (ComponentElement component in config.Components)
        //    {
        //        Type implementation = Type.GetType(component.Implementation);
        //        Type service = Type.GetType(component.Service);

        //        if (implementation == null)
        //            throw new ComponentRegistrationException(component.Implementation);

        //        if (service == null && !String.IsNullOrEmpty(component.Service))
        //            throw new ComponentRegistrationException(component.Service);

        //        if (service == null)
        //            service = implementation;

        //        string name = component.Key;
        //        if (string.IsNullOrEmpty(name))
        //            name = implementation.FullName;

        //        if (component.Parameters.Count == 0)
        //        {
        //            container.AddComponent(service, implementation, name);
        //        }
        //        else
        //        {
        //            container.AddComponentWithParameters(service, implementation,
        //                                                 component.Parameters.ToDictionary(), name);
        //        }
        //    }
        //}
    }
}
