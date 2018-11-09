using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Nop.Core.Infrastructure.Threading
{
    /// <summary>
    /// https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory HelperTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;

            return HelperTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;

                return func();
            })
            .Unwrap()
            .GetAwaiter()
            .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;

            HelperTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            })
            .Unwrap()
            .GetAwaiter()
            .GetResult();
        }
    }
}
