using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Compression;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Client that synchronizes the Distributed caches.
    /// </summary>
    public class CacheSynchronizationClient : IDisposable
    {
        private readonly IPAddress _ipAddress;
        private readonly int _distributedCachePort;
        private readonly int _syncInterval;
        private readonly Bootstrap _bootstrap;


        private Timer _timer;
        private int _running = 0;
        private MultithreadEventLoopGroup _group;
        private IChannel _currentChannel;
        private string _lastReturnMessage = null;
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Initializes a new istance of the <see cref="CacheSynchronizationClient"/> class.
        /// </summary>
        /// <param name="ipAddress">The ip address peer to connect to.</param>
        /// <param name="distributedCachePort">The port to connect to.</param>
        /// <param name="syncInterval">The synchronization interval.</param>
        public CacheSynchronizationClient(IPAddress ipAddress, int distributedCachePort, int syncInterval)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            _distributedCachePort = distributedCachePort;
            _syncInterval = syncInterval;
            _group = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();
            _bootstrap
                .Group(_group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip));
                    pipeline.AddLast(ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip));

                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast("cache-messages", new CacheSynchronizationClientHandler(HandleReturnMessage));
                }));
            _timer = new Timer(ProcessQueue, null, TimeSpan.FromSeconds(new Random().Next(0, syncInterval)), TimeSpan.FromSeconds(syncInterval));
        }

        /// <summary>
        /// The queue of Cache messages to synchronize.
        /// </summary>
        public ConcurrentQueue<CacheMessage> DispatchQueue { get; } = new ConcurrentQueue<CacheMessage>();

        /// <summary>
        /// Disposes resources.
        /// TODO: Convert to Async Disposable. Requires IAsyncDisposable support in Autofac 5.0.
        /// </summary>
        public void Dispose()
        {
            if (_autoResetEvent != null)
            {
                _autoResetEvent.Dispose();
                _autoResetEvent = null;
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_group != null)
            {
                // async fire and forget. 
                _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                _group = null;
            }
        }

        /// <summary>
        /// Handle a return message from the server.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void HandleReturnMessage(string message)
        {
            _lastReturnMessage = message;
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Processes the queue for synchronization to the client.
        /// </summary>
        private async void ProcessQueue(object state)
        {
            // make sure that we are the only one running.
            if (0 == Interlocked.Exchange(ref _running, 1))
            {
                try
                {
                    if (DispatchQueue.IsEmpty)
                    {
                        return;
                    }

                    await Connect();

                    if (_currentChannel == null)
                    {
                        return;
                    }

                    var list = new List<CacheMessage>(DispatchQueue.Count);

                    while (DispatchQueue.TryDequeue(out var message))
                    {
                        list.Add(message);
                    }

                    // n log n resort of operations.
                    list.Sort(new Comparison<CacheMessage>((x, y) => x.OperationId.CompareTo(y.OperationId)));

                    try
                    {
                        _lastReturnMessage = null;

                        var message = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });
                        var buffer = ByteBufferUtil.EncodeString(ByteBufferUtil.DefaultAllocator, message, Encoding.UTF8);
                        await _currentChannel.WriteAndFlushAsync(buffer);

                        // wait half of the scheduled interval for an answer.
                        if (_autoResetEvent.WaitOne(_syncInterval * 500) && _lastReturnMessage == "OK")
                        {
                            return;
                        }

                        var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            // Resolve
                            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                            logger.Error($"Got an error processing the cache entries at peer: {_ipAddress}:{_distributedCachePort} {_lastReturnMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            // Resolve
                            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                            logger.Error($"Got an exception serializing and sending cache entry to peer: {_ipAddress}:{_distributedCachePort} {ex.Message}", ex);
                        }
                    }

                    // requeue items for retry.
                    foreach (var message in list)
                    {
                        DispatchQueue.Enqueue(message);
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _running, 0);
                }
            }
        }

        /// <summary>
        /// Builds the pipe line
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            if (_currentChannel == null || !_currentChannel.IsWritable)
            {
                try
                {
                    if (_currentChannel != null)
                    {
                        await _currentChannel.CloseAsync();
                    }
                    _currentChannel = await _bootstrap.ConnectAsync(new IPEndPoint(_ipAddress, _distributedCachePort));

                    var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();

                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                        logger.Information($"Distributed Connected to {_ipAddress} on port {_distributedCachePort}");
                    }
                }
                catch (Exception ex)
                {
                    _currentChannel = null;
                    var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        // Resolve
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                        logger.Error($"Got an exceptionconnecting to peer for distributed caching: {_ipAddress}:{_distributedCachePort} {ex.Message}", ex);
                    }
                }
            }
        }
    }
}
