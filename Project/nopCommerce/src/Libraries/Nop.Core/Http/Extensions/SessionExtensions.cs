using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Core.Http.Extensions;

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
    /// <returns> A task that represents the asynchronous operation</returns>
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    public static async Task<T> GetAsync<T>(this ISession session, string key)
    {
        await LoadAsync(session);
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// Remove the given key from session if present.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="key">Key</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task RemoveAsync(this ISession session, string key)
    {
        await LoadAsync(session);
        session.Remove(key);
    }

    /// <summary>
    /// Remove all entries from the current session, if any. The session cookie is not removed.
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task ClearAsync(this ISession session)
    {
        await LoadAsync(session);
        session.Clear();
    }

    /// <summary>
    /// Try to async load the session from the data store
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task LoadAsync(ISession session)
    {
        try
        {
            await session.LoadAsync();
        }
        catch
        {
            //fallback to synchronous handling
        }
    }
}