using System;
using System.Collections.Generic;
using System.Text;
using Medallion.Threading.Sql;
using Nop.Core.Caching;
using Nop.Data;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Distributed locker using Sql Server.
    /// </summary>
    public class SQlServerLocker : ILocker
    {
        private string _connectionString = null;

        private string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    var dataSettings = DataSettingsManager.LoadSettings();
                    if (dataSettings == null || !dataSettings.IsValid)
                        return null;

                    if (dataSettings.DataProvider != DataProviderType.SqlServer)
                    {
                        return null;
                    }

                    _connectionString = dataSettings.ConnectionString;
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Perform some action with exclusive lock
        /// </summary>
        /// <param name="resource">The key we are locking on</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
        /// <param name="action">Action to be performed with locking</param>
        /// <returns>True if lock was acquired and action was performed; otherwise false</returns>
        public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
        {
            try
            {
                var semaphore = new SqlDistributedSemaphore(resource, 1, ConnectionString);
                using (semaphore.Acquire(expirationTime))
                {
                    action();
                    return true;
                }
            } catch
            {
                return false;
            }
        }
    }
}
