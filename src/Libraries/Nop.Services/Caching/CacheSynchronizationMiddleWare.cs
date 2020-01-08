using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Caching;
using Nop.Core.Configuration;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Cache synchronization middleware for the Distributed Cache.
    /// </summary>
    public sealed class CacheSynchronizationMiddleWare : IDisposable
    {
        private readonly ICacheMessageQueueProvider _cacheMessageQueueProvider;
        private readonly RequestDelegate _next;
        private readonly IList<CacheSynchronizationClient> _cacheSynchronizationClients = new List<CacheSynchronizationClient>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSynchronizationMiddleWare"/> class.
        /// </summary>
        /// <param name="cacheMessageQueueProvider">The cache message queue provider.</param>
        /// <param name="nopConfig">The nop configuration to use.</param>
        /// <param name="next">The next middleware to invoke.</param>
        public CacheSynchronizationMiddleWare(ICacheMessageQueueProvider cacheMessageQueueProvider, NopConfig nopConfig, RequestDelegate next)
        {
            _cacheMessageQueueProvider = cacheMessageQueueProvider ?? throw new ArgumentNullException(nameof(cacheMessageQueueProvider));
            _next = next ?? throw new ArgumentNullException(nameof(next));

            var localIPs = (from NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()
                            from UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses
                            select ip.Address).ToList();

            foreach (var peer in nopConfig.DistributedCachePeers.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var ipAddress = IPAddress.Parse(peer.Trim());
                if (localIPs.Contains(ipAddress))
                {
                    // skip own ip as peer.
                    continue;
                }

                _cacheSynchronizationClients.Add(new CacheSynchronizationClient(ipAddress, nopConfig.DistributedCachePort, nopConfig.DistributedCacheSyncInterval));
            }
        }

        /// <summary>
        /// Dispose of resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var client in _cacheSynchronizationClients)
            {
                client.Dispose();
            }
            _cacheSynchronizationClients.Clear();
        }

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context)
        {
            await _next(context);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(DispatchCacheUpdates);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Dispatches the Cache updates to all clients.
        /// </summary>
        /// <returns></returns>
        private void DispatchCacheUpdates()
        {
            // empty queue and transfer to client queues.
            while (_cacheMessageQueueProvider.CacheMessageQueue.TryDequeue(out var result))
            {
                foreach (var client in _cacheSynchronizationClients)
                {
                    client.DispatchQueue.Enqueue(result);
                }
            }
        }
    }
}
