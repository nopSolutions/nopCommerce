using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class EngineContext
    {
        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <param name="services">The contract for a collection of service descriptors</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(NopConfig nopConfiguration, IServiceCollection services)
        {
            if (Singleton<IEngine>.Instance == null)
            {
                Singleton<IEngine>.Instance = new NopEngine();
                Singleton<IEngine>.Instance.Initialize(nopConfiguration, services);
            }

            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngine Current
        {
            get { return Singleton<IEngine>.Instance; }
        }

        #endregion
    }
}
