using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using ImageProcessor;
using Microsoft.Extensions.Hosting;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Core.Domain.Media;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.CustomProductReviews.Services
{
    public interface IBackgroundQueue
    {
        void QueueTask(Func<CancellationToken, Task> task);
        Task<Func<CancellationToken, Task>> PopQueue(CancellationToken cancellationToken);
    }

    public class BackgroundQueue : IBackgroundQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> Tasks;
        private SemaphoreSlim Signal;
        public BackgroundQueue()
        {
            Tasks =new ConcurrentQueue<Func<CancellationToken, Task>>();
            Signal = new SemaphoreSlim(0);
        }

        public void QueueTask(Func<CancellationToken, Task> task)
        {
            Tasks.Enqueue(task);
            Signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> PopQueue(CancellationToken cancellationToken)
        {
            await Signal.WaitAsync(cancellationToken);
            Tasks.TryDequeue(out var task);

            return task;
        }

    }
  
}
