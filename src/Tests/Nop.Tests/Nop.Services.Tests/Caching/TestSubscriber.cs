using System.Net;
using StackExchange.Redis;

namespace Nop.Tests.Nop.Services.Tests.Caching;

public class TestSubscriber : ISubscriber
{
    protected static IList<Action<RedisChannel, RedisValue>> _handlers = new List<Action<RedisChannel, RedisValue>>();
    
    public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public bool TryWait(Task task)
    {
        throw new NotImplementedException();
    }

    public void Wait(Task task)
    {
        throw new NotImplementedException();
    }

    public T Wait<T>(Task<T> task)
    {
        throw new NotImplementedException();
    }

    public void WaitAll(params Task[] tasks)
    {
        throw new NotImplementedException();
    }

    public IConnectionMultiplexer Multiplexer => null;

    public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public EndPoint IdentifyEndpoint(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<EndPoint> IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public bool IsConnected(RedisChannel channel = new RedisChannel())
    {
        throw new NotImplementedException();
    }

    /// <summary>Posts a message to the given channel.</summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">The command flags to use.</param>
    /// <returns>
    /// The number of clients that received the message *on the destination server*,
    /// note that this doesn't mean much in a cluster as clients can get the message through other nodes.
    /// </returns>
    /// <remarks><seealso href="https://redis.io/commands/publish" /></remarks>
    public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
    {
        foreach (var handler in _handlers) 
            handler(channel, message);

        return _handlers.Count;
    }

    /// <inheritdoc cref="M:StackExchange.Redis.ISubscriber.Publish(StackExchange.Redis.RedisChannel,StackExchange.Redis.RedisValue,StackExchange.Redis.CommandFlags)" />
    public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
    {
        return Task.FromResult(Publish(channel, message, flags));
    }

    /// <summary>
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on <paramref name="channel" />.</param>
    /// <param name="flags">The command flags to use.</param>
    /// <remarks>
    /// <seealso href="https://redis.io/commands/subscribe" />,
    /// <seealso href="https://redis.io/commands/psubscribe" />
    /// </remarks>
    public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
    {
        _handlers.Add(handler);
    }

    /// <inheritdoc cref="M:StackExchange.Redis.ISubscriber.Subscribe(StackExchange.Redis.RedisChannel,System.Action{StackExchange.Redis.RedisChannel,StackExchange.Redis.RedisValue},StackExchange.Redis.CommandFlags)" />
    public Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
    {
        _handlers.Add(handler);

        return Task.CompletedTask;
    }

    public ChannelMessageQueue Subscribe(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<ChannelMessageQueue> SubscribeAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public EndPoint SubscribedEndpoint(RedisChannel channel)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }
}