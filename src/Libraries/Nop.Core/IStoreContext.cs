using System.Threading.Tasks;
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
        Task<Store> GetCurrentStore();

        /// <summary>
        /// Gets active store scope configuration
        /// </summary>
        Task<int> GetActiveStoreScopeConfiguration();
    }
}
