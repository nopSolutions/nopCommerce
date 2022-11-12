<<<<<<< HEAD
﻿using System.Threading.Tasks;
using Nop.Core.Domain.Stores;

namespace Nop.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets the current store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Store> GetCurrentStoreAsync();

        /// <summary>
        /// Gets the current store
        /// </summary>
        Store GetCurrentStore();

        /// <summary>
        /// Gets active store scope configuration
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<int> GetActiveStoreScopeConfigurationAsync();
    }
}
=======
﻿using System.Threading.Tasks;
using Nop.Core.Domain.Stores;

namespace Nop.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets the current store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Store> GetCurrentStoreAsync();

        /// <summary>
        /// Gets the current store
        /// </summary>
        Store GetCurrentStore();

        /// <summary>
        /// Gets active store scope configuration
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<int> GetActiveStoreScopeConfigurationAsync();
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
