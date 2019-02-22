using System;
using System.Threading;

namespace Nop.Core.ComponentModel
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public class ReaderWriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;
        private readonly bool _writeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriteLockDisposable"/> class.
        /// </summary>
        /// <param name="rwLock">The readers–writer lock</param>
        /// <param name="writeLock">Use true if need to use write lock. If need to use read lock use false</param>
        public ReaderWriteLockDisposable(ReaderWriterLockSlim rwLock, bool writeLock)
        {
            _rwLock = rwLock;
            _writeLock = writeLock;

            if (_writeLock)
                _rwLock.EnterWriteLock();
            else
                _rwLock.EnterReadLock();
        }

        void IDisposable.Dispose()
        {
            if (_writeLock)
                _rwLock.ExitWriteLock();
            else
                _rwLock.ExitReadLock();
        }
    }
}
