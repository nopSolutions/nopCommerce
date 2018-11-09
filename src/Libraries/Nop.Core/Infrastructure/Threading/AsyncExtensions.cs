using System;
using System.Threading.Tasks;

namespace Nop.Core.Infrastructure.Threading
{
    public static class AsyncExtensions
    {
        public static T AsSync<T>(this Task<T> asyncTask)
        {
            return asyncTask.ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
        }

        public static void AsSync(this Task asyncTask)
        {
            asyncTask.ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
        }
    }
}
