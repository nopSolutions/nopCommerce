using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;

namespace Nop.Tests.Nop.Plugin.Misc.OmnichannelCore.Tests;

internal sealed class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    public List<TEntity> Entities { get; } = [];

    public IQueryable<TEntity> Table => Entities.AsQueryable();

    public Task<TEntity> GetByIdAsync(int? id, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true, bool useShortTermCache = false)
    {
        return Task.FromResult(Entities.FirstOrDefault(entity => entity.Id == id));
    }

    public Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        return Task.FromResult<IList<TEntity>>(Entities.Where(entity => ids.Contains(entity.Id)).ToList());
    }

    public Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null,
        bool includeDeleted = true)
    {
        var query = func is null ? Table : func(Table);
        return Task.FromResult<IList<TEntity>>(query.ToList());
    }

    public async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null,
        bool includeDeleted = true)
    {
        var query = func is null ? Table : await func(Table);
        return query.ToList();
    }

    public async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func,
        Func<ICacheKeyService, Task<CacheKey>> getCacheKey,
        bool includeDeleted = true)
    {
        var query = func is null ? Table : await func(Table);
        return query.ToList();
    }

    public Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false,
        bool includeDeleted = true)
    {
        throw new NotImplementedException();
    }

    public Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false,
        bool includeDeleted = true)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(TEntity entity, bool publishEvent = true)
    {
        if (entity.Id == 0)
            entity.Id = Entities.Count == 0 ? 1 : Entities.Max(existing => existing.Id) + 1;

        Entities.Add(entity);
        return Task.CompletedTask;
    }

    public Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        foreach (var entity in entities)
            InsertAsync(entity, publishEvent).GetAwaiter().GetResult();

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TEntity entity, bool publishEvent = true)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, bool publishEvent = true)
    {
        Entities.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        foreach (var entity in entities)
            Entities.Remove(entity);

        return Task.CompletedTask;
    }

    public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var removed = Entities.RemoveAll(entity => compiledPredicate(entity));
        return Task.FromResult(removed);
    }

    public Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
    {
        return Task.FromResult(entity);
    }

    public Task TruncateAsync(bool resetIdentity = false)
    {
        Entities.Clear();
        return Task.CompletedTask;
    }
}
