namespace Nop.Core.Infrastructure;

/// <summary>
/// A statically compiled "singleton" used to store objects throughout the 
/// lifetime of the app domain. Not so much singleton in the pattern's 
/// sense of the word as a standardized way to store single instances.
/// </summary>
/// <typeparam name="T">The type of object to store.</typeparam>
/// <remarks>Access to the instance is not synchronized.</remarks>
public partial class Singleton<T> : BaseSingleton
{
    private static T _instance;

    /// <summary>
    /// The singleton instance for the specified type T. Only one instance (at the time) of this object for each type of T.
    /// </summary>
    public static T Instance
    {
        get => _instance;
        set
        {
            _instance = value;
            AllSingletons[typeof(T)] = value;
        }
    }
}