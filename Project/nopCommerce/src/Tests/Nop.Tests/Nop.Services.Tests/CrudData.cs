using Nop.Core;

namespace Nop.Tests.Nop.Services.Tests;

public class CrudData<TEntity> where TEntity : BaseEntity
{
    public TEntity BaseEntity { get; set; }

    public Func<TEntity, Task> Insert { get; set; }

    public TEntity UpdatedEntity { get; set; }

    public Func<TEntity, Task> Update { get; set; }

    public Func<int, Task<TEntity>> GetById { get; set; }

    public Func<TEntity, TEntity, bool> IsEqual { get; set; }

    public Func<TEntity, Task> Delete { get; set; }
}