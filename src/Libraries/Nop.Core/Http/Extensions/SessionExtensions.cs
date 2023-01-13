using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Core.Http.Extensions
{
    /// <summary>
    /// Represents extensions of ISession
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Set value to Session
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Get value from session
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// Remove value from Session
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        public static void Remove(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var _))
                session.Remove(key);
        }

        /// <summary>
        /// Set value to Session
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task SetAsync<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get value from session
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<T> GetAsync<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                return default;

            return Task.FromResult(JsonConvert.DeserializeObject<T>(value));
        }

        /// <summary>
        /// Remove value from Session
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task RemoveAsync(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var _))
                session.Remove(key);

            return Task.CompletedTask;
        }
    }
}
