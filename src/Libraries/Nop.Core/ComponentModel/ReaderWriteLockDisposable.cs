namespace Nop.Core.ComponentModel
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public partial class ReaderWriteLockDisposable : IDisposable
    {
        #region Fields

        protected bool _disposed;
        protected readonly ReaderWriterLockSlim _rwLock;
        protected readonly ReaderWriteLockType _readerWriteLockType;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriteLockDisposable"/> class.
        /// </summary>
        /// <param name="rwLock">The readers–writer lock</param>
        /// <param name="readerWriteLockType">Lock type</param>
        public ReaderWriteLockDisposable(ReaderWriterLockSlim rwLock, ReaderWriteLockType readerWriteLockType = ReaderWriteLockType.Write)
        {
            _rwLock = rwLock;
            _readerWriteLockType = readerWriteLockType;

            switch (_readerWriteLockType)
            {
                case ReaderWriteLockType.Read:
                    _rwLock.EnterReadLock();
                    break;
                case ReaderWriteLockType.Write:
                    _rwLock.EnterWriteLock();
                    break;
                case ReaderWriteLockType.UpgradeableRead:
                    _rwLock.EnterUpgradeableReadLock();
                    break;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Specifies whether to disposing resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                switch (_readerWriteLockType)
                {
                    case ReaderWriteLockType.Read:
                        _rwLock.ExitReadLock();
                        break;
                    case ReaderWriteLockType.Write:
                        _rwLock.ExitWriteLock();
                        break;
                    case ReaderWriteLockType.UpgradeableRead:
                        _rwLock.ExitUpgradeableReadLock();
                        break;
                }
            }

            _disposed = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
