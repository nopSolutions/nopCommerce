using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;

namespace Nop.Services
{
    public interface IEntityService<TEntity> where TEntity:BaseEntity
    {
        void Delete(TEntity entity);
        TEntity GetById(int id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
    }
}
