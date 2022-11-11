using System;
using System.Threading.Tasks;

namespace Nop.Core.Caching
{
    public interface ILocker
    {
        /// <summary>
        /// Perform asynchronous action with exclusive in-memory lock
        /// </summary>
        /// <param name="resource">The key we are locking on</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
        /// <param name="action">Action to be performed with locking</param>
        /// <returns>True if lock was acquired and action was performed; otherwise false</returns>
        Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action);
    }
}
