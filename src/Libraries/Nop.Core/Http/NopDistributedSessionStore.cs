using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Http
{
    /// <summary>
    /// An <see cref="ISessionStore"/> backed by an <see cref="IDistributedCache"/>.
    /// </summary>
    public class NopDistributedSessionStore: ISessionStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="NopDistributedSessionStore"/>.
        /// </summary>
        /// <param name="cache">The <see cref="IDistributedCache"/> used to store the session data.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public NopDistributedSessionStore(IDistributedCache cache, ILoggerFactory loggerFactory)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache;
            _loggerFactory = loggerFactory;
        }

        ///<inheritdoc/>
        public ISession Create(
            string sessionKey, TimeSpan idleTimeout, TimeSpan ioTimeout, 
            Func<bool> tryEstablishSession, bool isNewSessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException("The session key is invalid.", nameof(sessionKey));
            }

            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            return new NopDistributedSession(
                _cache, sessionKey, idleTimeout, ioTimeout, 
                tryEstablishSession, _loggerFactory, isNewSessionKey);
        }
    }
}
