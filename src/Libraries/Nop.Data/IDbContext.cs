using System.Collections.Generic;
using System.Data.Entity;
using Nop.Core;

namespace Nop.Data
{
    public interface IDbContext 
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        int SaveChanges();

        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();
    }
}
