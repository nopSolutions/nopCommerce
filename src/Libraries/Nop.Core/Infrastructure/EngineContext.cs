using System.Configuration;
using System.Runtime.CompilerServices;
using Nop.Core.Configuration;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// 封装Web请求，并提供一个IEngine的单例对象访问。
    /// </summary>
    public class EngineContext
    {
        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// 初始化创建一个IEngine的实例。
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        /// <remarks>
        /// [MethodImpl(MethodImplOptions.Synchronized)]声明，保证只能有一个线程访问，因为.Net的Web程序无论是WebForm还是mvc都在服务端都是多线程的。
        /// 这样标记只能有一个线程调用Initialize方法，也就保证了实例对象IEngine在内存中只有一份。然后把单例对象的存储到类Singleton中。
        /// Singleton就像一个对象容器，可以把许多单例对象存储在里面。
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = new NopEngine();

                var config = ConfigurationManager.GetSection("NopConfig") as NopConfig;
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. 
        /// Use this method to supply your own engine implementation.
        /// 替换单例中的实例对象。
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
        /// 获取IEngine单例实例对象。
        /// </summary>
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

        #endregion
    }
}
