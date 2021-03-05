using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Nop.Core.ComponentModel
{
    public static class ResourcesLocker
    {
        private static readonly ConcurrentDictionary<string, Lazy<AsyncLock>> _lockerItems = new ConcurrentDictionary<string, Lazy<AsyncLock>>();

        public static AsyncLock GetLocker(string resource)
        {
            return _lockerItems.GetOrAdd(resource, key =>
            {
                return new Lazy<AsyncLock>(() =>
                {
                    return new AsyncLock();
                }, true);
            }).Value;
        }
    }
}
