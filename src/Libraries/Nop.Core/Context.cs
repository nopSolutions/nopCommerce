using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Web.Configuration;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class Context
    {
        #region Initialization Methods
        /// <summary>Initializes a static instance of the Nop factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Debug.WriteLine("Constructing engine " + DateTime.Now);
                Singleton<IEngine>.Instance = CreateEngineInstance();
                Debug.WriteLine("Initializing engine " + DateTime.Now);
                Singleton<IEngine>.Instance.Initialize();
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.</summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        private static System.Configuration.Configuration GetConfiguration()
        {
            try
            {
                return System.Web.Hosting.HostingEnvironment.IsHosted
                    ? WebConfigurationManager.OpenWebConfiguration("~")
                    : ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Error reading configuration. This has happened when running a web site project in a virtual directory (reason unknown). " + ex);
                return null;
            }
        }

        /// <summary>Creates a factory instance and adds a http application injecting facility.</summary>
        /// <returns>A new factory.</returns>
        public static IEngine CreateEngineInstance()
        {
            try
            {
                var config = ConfigurationManager.GetSection("nop/engine") as EngineSection;
                if (config != null && !string.IsNullOrEmpty(config.EngineType))
                {
                    var engineType = Type.GetType(config.EngineType);
                    if (engineType == null)
                        throw new ConfigurationErrorsException("The type '" + engineType + "' could not be found. Please check the configuration at /configuration/nop/engine[@engineType] or check for missing assemblies.");
                    if (!typeof(IEngine).IsAssignableFrom(engineType))
                        throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'Nop.Core.Infrastructure.IEngine' and cannot be configured in /configuration/nop/engine[@engineType] for that purpose.");
                    return Activator.CreateInstance(engineType) as IEngine;
                }

                return new NopEngine();
            }
            catch (SecurityException ex)
            {
                Trace.TraceInformation("Caught SecurityException, reverting to MediumTrustEngine. " + ex);
                //TODO:Support medium trust?
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        /// <summary>Gets the singleton Nop engine used to access Nop services.</summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }
    }
}
