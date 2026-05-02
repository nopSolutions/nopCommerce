namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
/// </summary>
public partial class BaseSingleton
{
    static BaseSingleton()
    {
        AllSingletons = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Dictionary of type to singleton instances.
    /// </summary>
    public static IDictionary<Type, object> AllSingletons { get; }
}