namespace Nop.Core.Middleware;

public interface IObjectProcessingMiddleware<T>
{
    public Task<T> ProcessObjectAsync(T @object);
}