using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Http
{
    /// <summary>
    /// An <see cref="ISession"/> backed by an <see cref="IDistributedCache"/>.
    /// </summary>
    public class NopDistributedSession : DistributedSession, ISession
    {
        private bool _loaded = false;

        /// <summary>
        /// Initializes a new instance of <see cref="NopDistributedSession"/>.
        /// </summary>
        /// <param name="cache">The <see cref="IDistributedCache"/> used to store the session data.</param>
        /// <param name="sessionKey">A unique key used to lookup the session.</param>
        /// <param name="idleTimeout">How long the session can be inactive (e.g. not accessed) before it will expire.</param>
        /// <param name="ioTimeout">
        /// The maximum amount of time <see cref="LoadAsync(CancellationToken)"/> and <see cref="CommitAsync(CancellationToken)"/> are allowed take.
        /// </param>
        /// <param name="tryEstablishSession">
        /// A callback invoked during <see cref="Set(string, byte[])"/> to verify that modifying the session is currently valid.
        /// If the callback returns <see langword="false"/>, <see cref="Set(string, byte[])"/> throws an <see cref="InvalidOperationException"/>.
        /// <see cref="SessionMiddleware"/> provides a callback that returns <see langword="false"/> if the session was not established
        /// prior to sending the response.
        /// </param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="isNewSessionKey"><see langword="true"/> if establishing a new session; <see langword="false"/> if resuming a session.</param>
        public NopDistributedSession(
            IDistributedCache cache, string sessionKey, TimeSpan idleTimeout, TimeSpan ioTimeout,
            Func<bool> tryEstablishSession, ILoggerFactory loggerFactory, bool isNewSessionKey)
            : base(cache, sessionKey, idleTimeout, ioTimeout, tryEstablishSession, loggerFactory, isNewSessionKey)
        {
        }

        /// <inheritdoc />
        public new bool TryGetValue(string key, [NotNullWhen(true)] out byte[] value)
        {
            EnsureAsyncLoaded();
            return base.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public new void Set(string key, byte[] value)
        {
            EnsureAsyncLoaded();
            base.Set(key, value);
        }

        /// <inheritdoc />
        public new void Remove(string key)
        {
            EnsureAsyncLoaded();
            base.Remove(key);
        }

        /// <summary>
        /// Try to load session state asynchronously
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0#load-session-state-asynchronously"/>
        private void EnsureAsyncLoaded()
        {
            if (!_loaded)
            {
                try
                {
                    LoadAsync().Wait();
                }
                catch
                {
                    //session will be loaded synchronously.
                }

                _loaded = true;
            }
        }
    }
}
