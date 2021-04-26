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
        private static readonly ConcurrentDictionary<string, AsyncLock> _lockerItems = new();

        public static AsyncLock GetLocker(string resource)
        {
            _lockerItems.TryAdd(resource, new AsyncLock());

            return _lockerItems[resource];
        }
    }
}
