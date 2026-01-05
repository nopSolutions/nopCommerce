using Nop.Core.Infrastructure;

namespace Nop.Core.Middleware;

public class ObjectProcessingPublisher : IObjectProcessingPublisher
{
    
    public async Task<T> ProcessObjectAsync<T>(T @object)
    {
        
        var middlewares = EngineContext.Current.ResolveAll<IObjectProcessingMiddleware<T>>().ToList();
        
        var result = @object;
        
        foreach (var middleware in middlewares)
        {
            result = await middleware.ProcessObjectAsync(result);
            
            if (result == null)
                break;
        }
        
        return result;
    }
}