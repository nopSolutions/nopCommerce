using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Nop.Core
{
    /// <summary>
    /// Represents a async helper
    /// </summary>
    public static class AsyncHelper
    {
        #region Methods

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }

        #endregion
    }
}
