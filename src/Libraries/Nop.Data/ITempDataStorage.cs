namespace Nop.Data;

/// <summary>
/// Represents temporary storage
/// </summary>
/// <typeparam name="T">Storage record mapping class</typeparam>
public partial interface ITempDataStorage<T> : IQueryable<T>, IDisposable, IAsyncDisposable where T : class
{
}