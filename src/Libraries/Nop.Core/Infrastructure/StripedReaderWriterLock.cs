using System;
using System.Threading;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A striped ReaderWriterLock wrapper
    /// </summary>
    public class StripedReaderWriterLock
    {
        private readonly ReaderWriterLockSlim[] _locks;
        private readonly int _mask;

        // defaults to 8 times the number of processor cores
        public StripedReaderWriterLock() : this((int)Math.Log2(Environment.ProcessorCount) + 3)
        {
        }

        public StripedReaderWriterLock(int nLocksExp)
        {
            var n = 1 << nLocksExp;
            _mask = n - 1;
            _locks = new ReaderWriterLockSlim[n];
            for (var i = 0; i < n; i++)
                _locks[i] = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public ReaderWriterLockSlim GetLock(object obj)
        {
            return _locks[obj.GetHashCode() & _mask];
        }

        public ReadOnlySpan<ReaderWriterLockSlim> GetAllLocks()
        {
            return _locks.AsSpan();
        }
    }
}
