namespace Nop.Core.Middleware;

public interface IObjectProcessingPublisher
{
    public Task<T> ProcessObjectAsync<T>(T @object);
}