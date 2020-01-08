using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Codecs;
using DotNetty.Codecs.Compression;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Distributed cache server. Listens for cache update requests on
    /// a specified port.
    /// </summary>
    public sealed class CacheSynchronizationServer : IDisposable
    {
        private readonly IDistributedCacheManager _cacheManager;

        private readonly ServerBootstrap _bootstrap;
        private readonly int _distributedCachePort;

        private MultithreadEventLoopGroup _bossGroup;
        private MultithreadEventLoopGroup _workerGroup;
        private IChannel _bootstrapChannel;

        /// <summary>
        /// Initializes a new isntance of the <see cref="CacheSynchronizationServer"/> class.
        /// </summary>
        /// <param name="cacheManager">The cache manager to use.</param>
        /// <param name="nopConfig">The configuration objet.</param>
        public CacheSynchronizationServer(IDistributedCacheManager cacheManager, NopConfig nopConfig)
        {
            if (nopConfig is null)
                throw new ArgumentNullException(nameof(nopConfig));

            _distributedCachePort = nopConfig.DistributedCachePort;

            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();

            this._cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));

            _bootstrap = new ServerBootstrap();
            _bootstrap
                .Group(_bossGroup, _workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                //.Handler(new LoggingHandler(LogLevel.INFO))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip));
                    pipeline.AddLast(ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip));

                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast("cache-messages", new CacheSynchronizationServerHandler(cacheManager));
                }));

        }

        /// <summary>
        /// Initializes the server by starting to listen on the specified port.
        /// </summary>
        public void Initialize()
        {
            _bootstrapChannel = _bootstrap.BindAsync(_distributedCachePort).GetAwaiter().GetResult();

            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.Information($"Distributed Cache: Started listening on port {_distributedCachePort}");
            }
        }

        /// <summary>
        /// Disposes of all resources.
        /// </summary>
        public void Dispose()
        {
            if (_bootstrapChannel != null)
            {
                _bootstrapChannel.CloseAsync().GetAwaiter().GetResult();
                _bootstrapChannel = null;
            }
            if (_workerGroup != null)
            {
                // async fire and forget. 
                _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                _workerGroup = null;
            }
            if (_bossGroup != null)
            {
                // async fire and forget. 
                _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                _bossGroup = null;
            }
        }
    }
}
