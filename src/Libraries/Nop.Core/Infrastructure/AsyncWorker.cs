using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Performs work asynchronously.
    /// </summary>
    [Service(typeof(IWorker))]
    public class AsyncWorker : IWorker
    {
        int executingWorkItems = 0;

        /// <summary>Testability seam for the async worker.</summary>
        public Func<WaitCallback, bool> QueueUserWorkItem = ThreadPool.QueueUserWorkItem;

        #region IWorker Members

        /// <summary>Gets the number of executing work actions.</summary>
        public int ExecutingWorkItems
        {
            get { return executingWorkItems; }
        }

        /// <summary>Starts the execution of the specified work.</summary>
        /// <param name="action">The method to execute.</param>
        public void DoWork(Action action)
        {
            Interlocked.Increment(ref executingWorkItems);
            QueueUserWorkItem(delegate
            {
                try
                {
                    action();
                }
                finally
                {
                    Interlocked.Decrement(ref executingWorkItems);
                }
            });
        }

        #endregion
    }
}
