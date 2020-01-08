using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Renci.SshNet.Security;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Channel message handler for the Distributed Cache Synchronization server.
    /// </summary>
    internal class CacheSynchronizationServerHandler : ChannelHandlerAdapter
    {
        private IDistributedCacheManager _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSynchronizationServerHandler"/> class.
        /// </summary>
        /// <param name="cacheManager">The cache manager to use.</param>
        /// <param name="logger">The logger to use.</param>
        public CacheSynchronizationServerHandler(IDistributedCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Processes a read message from the channel.
        /// </summary>
        /// <param name="context">The channel handler contract.</param>
        /// <param name="message">The read message.</param>
        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                if (message is IByteBuffer byteBuffer)
                {
                    // decode as UTF8 and deserialize.
                    var str = byteBuffer.ToString(Encoding.UTF8);

                    var list = JsonConvert.DeserializeObject<IList<CacheMessage>>(str, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

                    // process each message and pass on to the local cache manager.
                    foreach (var cacheMessage in list)
                    {
                        switch (cacheMessage.Operation)
                        {
                            case nameof(_cacheManager.Clear):
                                _cacheManager.Clear(false);
                                break;
                            case nameof(_cacheManager.Remove):
                                _cacheManager.Remove(cacheMessage.Key, cacheMessage.Entry.UtcDateTime);
                                break;
                            case nameof(_cacheManager.Set):
                                _cacheManager.Set(new CacheKey(cacheMessage.Key) { CacheTime = cacheMessage.Entry.CacheTime }, cacheMessage.Entry.Value, cacheMessage.Entry.UtcDateTime);
                                break;
                            case nameof(_cacheManager.RemoveByPrefix):
                                _cacheManager.RemoveByPrefix(cacheMessage.Key, cacheMessage.Entry.UtcDateTime);
                                break;
                        }
                    }

                    // send OK.
                    var buffer = ByteBufferUtil.EncodeString(ByteBufferUtil.DefaultAllocator, "OK", Encoding.UTF8);
                    await context.WriteAndFlushAsync(buffer);
                }
            }
            catch (Exception ex)
            {
                // send the exception to the client..
                var buffer = ByteBufferUtil.EncodeString(ByteBufferUtil.DefaultAllocator, ex.ToString(), Encoding.UTF8);
                await context.WriteAndFlushAsync(buffer);

                var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                    logger.Error($"Got an exception deserializing and processing cache entry. {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Runs when the read on the channel is complete.
        /// </summary>
        /// <param name="context">The channel handler context.</param>
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        /// <summary>
        /// Runs when an exception was caught.
        /// </summary>
        /// <param name="context">The channel handler context.</param>
        /// <param name="exception">The exception that occured.</param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}