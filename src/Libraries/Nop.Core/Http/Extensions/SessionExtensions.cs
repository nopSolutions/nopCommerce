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
        public static async Task SetAsync<T>(this ISession session, string key, T value)
        {
            await LoadAsync(session);
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Get value from session
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static async Task<T> GetAsync<T>(this ISession session, string key)
        {
            await LoadAsync(session);
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        private static async Task LoadAsync(ISession session)
        {
            if (!session.IsAvailable)
            {
                try
                {
                    await session.LoadAsync();
                }
                catch
                {
                    // fallback to synchronous handling
                }
            }
        }
    }
}
